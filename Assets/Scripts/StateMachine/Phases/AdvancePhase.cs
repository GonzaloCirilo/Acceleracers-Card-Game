using System.Collections.Generic;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.System;
using AcceleracersCCG.Core;
using AcceleracersCCG.Effects;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.StateMachine.Phases
{
    /// <summary>
    /// Automatic: advance all qualifying vehicles, strip temporaries, check win.
    /// </summary>
    public class AdvancePhase : IGamePhase
    {
        public GamePhaseId Id => GamePhaseId.Advance;
        public bool IsAutomatic => true;

        public void OnEnter(GameState state)
        {
        }

        public void OnExit(GameState state)
        {
        }

        public List<ICommand> GetAutoCommands(GameState state)
        {
            var commands = new List<ICommand>();
            var playerIdx = state.ActivePlayerIndex;
            var player = state.ActivePlayer;

            // Check each vehicle for advancement
            foreach (var stack in player.VehiclesInPlay)
            {
                if (stack.HasFinished) continue;

                // Mods that force an auto-advance this turn (e.g. Strato-Thruster) and
                // haven't fired yet. They advance the vehicle even if it can't escape.
                var autoAdvanceMods = new List<CardInstance>();
                foreach (var mod in stack.EquippedMods)
                {
                    if (mod.Data.HasEffect(EffectIds.AutoAdvanceNextTurn)
                        && !stack.Tokens.Has($"{EffectIds.AutoAdvanceFiredTokenPrefix}_{mod.UniqueId}"))
                    {
                        autoAdvanceMods.Add(mod);
                    }
                }

                bool canAdvance = AdvanceRules.CanAdvance(stack, state.RealmTrack);
                if (!canAdvance && autoAdvanceMods.Count == 0)
                    continue;

                int fromRealmIdx = stack.RealmIndex;

                // Strip temporaries (Shifts / AcceleCharger) before advancing, as normal.
                commands.Add(new StripTemporariesCommand(playerIdx, stack.Vehicle.UniqueId));

                // Advance to next realm.
                commands.Add(new AdvanceVehicleCommand(playerIdx, stack.Vehicle.UniqueId));

                // Reveal next realm if needed (advance hasn't executed yet, so use fromRealmIdx + 1).
                int nextRealmIdx = fromRealmIdx + 1;
                if (nextRealmIdx < Constants.RealmsPerRace
                    && !state.RealmTrack.IsRevealed(nextRealmIdx))
                {
                    commands.Add(new FlipRealmCommand(nextRealmIdx));
                }

                // Resolve auto-advance mods: their own text junks them, unless the vehicle
                // or the Realm being left retains Mods (Junk Realm), in which case they stay
                // but are flagged so they don't advance the vehicle again next turn.
                if (autoAdvanceMods.Count > 0)
                {
                    bool retained = AdvanceRules.RetainsMods(stack, state.RealmTrack, fromRealmIdx);
                    foreach (var mod in autoAdvanceMods)
                    {
                        if (retained)
                        {
                            commands.Add(new SetTokenCommand(playerIdx, stack.Vehicle.UniqueId,
                                $"{EffectIds.AutoAdvanceFiredTokenPrefix}_{mod.UniqueId}", 1));
                        }
                        else
                        {
                            commands.Add(new JunkCardCommand(playerIdx, stack.Vehicle.UniqueId, mod.UniqueId));
                        }
                    }
                }
            }

            // Check win condition after advances
            commands.Add(new CheckWinConditionCommand());

            return commands;
        }

        public List<ICommand> GetLegalPlayerCommands(GameState state)
        {
            return new List<ICommand>();
        }

        public GamePhaseId GetNextPhase(GameState state)
        {
            return GamePhaseId.PlayVehicle;
        }
    }
}

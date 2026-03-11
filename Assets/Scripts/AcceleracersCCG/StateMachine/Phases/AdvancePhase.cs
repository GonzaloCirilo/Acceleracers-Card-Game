using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.System;
using AcceleracersCCG.Core;
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

                if (AdvanceRules.CanAdvance(stack, state.RealmTrack))
                {
                    // Strip temporaries before advancing
                    commands.Add(new StripTemporariesCommand(playerIdx, stack.Vehicle.UniqueId));

                    // Advance to next realm
                    commands.Add(new AdvanceVehicleCommand(playerIdx, stack.Vehicle.UniqueId));

                    // Reveal next realm if needed
                    int nextRealmIdx = stack.RealmIndex + 1; // +1 because advance hasn't executed yet
                    if (nextRealmIdx < Constants.RealmsPerRace
                        && !state.RealmTrack.IsRevealed(nextRealmIdx))
                    {
                        commands.Add(new FlipRealmCommand(nextRealmIdx));
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

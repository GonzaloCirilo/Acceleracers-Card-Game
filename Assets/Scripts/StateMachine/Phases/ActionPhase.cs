using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.Player;
using AcceleracersCCG.Core;
using AcceleracersCCG.Effects;

namespace AcceleracersCCG.StateMachine.Phases
{
    /// <summary>
    /// Interactive: player spends AP on Mods, Shifts, AcceleChargers, Hazards, and extra draws.
    /// Also handles reaction window for 0-AP cards (playable any time, including opponent's turn).
    /// </summary>
    public class ActionPhase : IGamePhase
    {
        public GamePhaseId Id => GamePhaseId.Action;
        public bool IsAutomatic => false;

        public void OnEnter(GameState state)
        {
        }

        public void OnExit(GameState state)
        {
        }

        public List<ICommand> GetAutoCommands(GameState state)
        {
            return new List<ICommand>();
        }

        public List<ICommand> GetLegalPlayerCommands(GameState state)
        {
            var commands = new List<ICommand>();
            var player = state.ActivePlayer;
            var playerIdx = state.ActivePlayerIndex;

            // Equip Mods to vehicles
            var mods = player.Hand.GetByType(CardType.Mod);
            foreach (var mod in mods)
            {
                if (player.AP < ((IAPCostCard)mod.Data).APCost) continue;

                foreach (var stack in player.VehiclesInPlay)
                {
                    if (Rules.EquipRules.ValidateMod(mod.Data, stack, state.RealmTrack) == null)
                    {
                        commands.Add(new EquipModCommand(playerIdx, mod.UniqueId, stack.Vehicle.UniqueId));
                    }
                }
            }

            // Equip Shifts to vehicles
            var shifts = player.Hand.GetByType(CardType.Shift);
            foreach (var shift in shifts)
            {
                if (player.AP < ((IAPCostCard)shift.Data).APCost) continue;

                foreach (var stack in player.VehiclesInPlay)
                {
                    if (Rules.EquipRules.ValidateShift(shift.Data, stack, state.RealmTrack) == null)
                    {
                        commands.Add(new EquipShiftCommand(playerIdx, shift.UniqueId, stack.Vehicle.UniqueId));
                    }
                }
            }

            // Equip AcceleChargers
            var acceleChargers = player.Hand.GetByType(CardType.AcceleCharger);
            foreach (var ac in acceleChargers)
            {
                if (player.AP < ((IAPCostCard)ac.Data).APCost) continue;

                foreach (var stack in player.VehiclesInPlay)
                {
                    if (Rules.EquipRules.ValidateAcceleCharger(ac.Data, stack, state.RealmTrack) == null)
                    {
                        commands.Add(new EquipAcceleChargerCommand(playerIdx, ac.UniqueId, stack.Vehicle.UniqueId));
                    }
                }
            }

            // Play Hazards against opponent's equipment
            var hazards = player.Hand.GetByType(CardType.Hazard);
            var opponent = state.InactivePlayer;
            foreach (var hazard in hazards)
            {
                if (player.AP < ((IAPCostCard)hazard.Data).APCost) continue;

                foreach (var oppStack in opponent.VehiclesInPlay)
                {
                    // Target mods
                    foreach (var mod in oppStack.EquippedMods)
                    {
                        if (Rules.HazardTargetRules.CanTarget(hazard.Data, mod))
                        {
                            commands.Add(new PlayHazardCommand(playerIdx, hazard.UniqueId,
                                opponent.PlayerIndex, oppStack.Vehicle.UniqueId, mod.UniqueId));
                        }
                    }
                    // Target shifts
                    foreach (var shift in oppStack.EquippedShifts)
                    {
                        if (Rules.HazardTargetRules.CanTarget(hazard.Data, shift))
                        {
                            commands.Add(new PlayHazardCommand(playerIdx, hazard.UniqueId,
                                opponent.PlayerIndex, oppStack.Vehicle.UniqueId, shift.UniqueId));
                        }
                    }
                    // Target AcceleCharger if allowed
                    if (oppStack.AcceleCharger != null && Rules.HazardTargetRules.CanTarget(hazard.Data, oppStack.AcceleCharger))
                    {
                        commands.Add(new PlayHazardCommand(playerIdx, hazard.UniqueId,
                            opponent.PlayerIndex, oppStack.Vehicle.UniqueId, oppStack.AcceleCharger.UniqueId));
                    }
                }
            }

            // Activated vehicle effects (e.g. Rolling Thunder / Torqued Rolling Thunder transfer mod)
            foreach (var stack in player.VehiclesInPlay)
            {
                bool hasTransfer = stack.Vehicle.Data.HasEffect(EffectIds.TransferMod);
                bool hasTransferIgnore = stack.Vehicle.Data.HasEffect(EffectIds.TransferModIgnoreModability);
                if (!hasTransfer && !hasTransferIgnore) continue;
                if (!stack.EquippedMods.Any()) continue;

                bool ignoreModability = hasTransferIgnore;
                bool hasTarget = player.VehiclesInPlay.Any(t =>
                    t != stack &&
                    ((VehicleCardData)t.Vehicle.Data).Team == Team.MetalManiacs &&
                    t.RealmIndex == stack.RealmIndex &&
                    (ignoreModability || stack.EquippedMods.Any(m => Rules.ModabilityRules.CanEquipMod(m.Data, t.Vehicle.Data)))
                );
                if (hasTarget)
                    commands.Add(new ActivateVehicleEffectCommand(playerIdx, stack.Vehicle.UniqueId));
            }

            // Spend AP to draw
            if (player.AP >= Constants.APPerExtraDraw && !player.Deck.IsEmpty)
            {
                commands.Add(new SpendAPToDrawCommand(playerIdx));
            }

            // End phase
            commands.Add(new EndPhaseCommand(playerIdx));

            return commands;
        }

        public GamePhaseId GetNextPhase(GameState state)
        {
            return GamePhaseId.Discard;
        }
    }
}

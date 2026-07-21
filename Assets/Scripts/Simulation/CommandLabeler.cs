using System.Linq;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.Player;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Simulation
{
    /// <summary>
    /// Turns an <see cref="ICommand"/> into a human-readable button label,
    /// resolving card unique-ids to their names against the current game state.
    /// </summary>
    public static class CommandLabeler
    {
        public static string Describe(ICommand cmd, GameState state)
        {
            switch (cmd)
            {
                case EndPhaseCommand _:
                    return "▶ End Phase";
                case PlayVehicleCommand c:
                    return $"Play vehicle: {Name(state, c.CardUniqueId)}";
                case EquipModCommand c:
                    return $"Equip Mod {Name(state, c.CardUniqueId)} → {Name(state, c.TargetVehicleUniqueId)}";
                case EquipShiftCommand c:
                    return $"Equip Shift {Name(state, c.CardUniqueId)} → {Name(state, c.TargetVehicleUniqueId)}";
                case EquipAcceleChargerCommand c:
                    return $"Equip AcceleCharger {Name(state, c.CardUniqueId)} → {Name(state, c.TargetVehicleUniqueId)}";
                case PlayHazardCommand c:
                    return $"Hazard {Name(state, c.HazardCardUniqueId)} → {Name(state, c.TargetVehicleUniqueId)}";
                case DiscardCardCommand c:
                    return $"Discard {Name(state, c.CardUniqueId)}";
                case SpendAPToDrawCommand _:
                    return "Spend 1 AP: draw a card";
                case ActivateVehicleEffectCommand c:
                    return $"Activate effect: {Name(state, c.VehicleUniqueId)}";
                default:
                    return cmd.GetType().Name.Replace("Command", "");
            }
        }

        /// <summary>Resolve a card unique-id to its name by scanning both players' zones.</summary>
        public static string Name(GameState state, int uniqueId)
        {
            foreach (var player in state.Players)
            {
                var inHand = player.Hand.Cards.FirstOrDefault(c => c.UniqueId == uniqueId);
                if (inHand != null) return inHand.Data.Name;

                var inJunk = player.JunkPile.Cards.FirstOrDefault(c => c.UniqueId == uniqueId);
                if (inJunk != null) return inJunk.Data.Name;

                foreach (var stack in player.VehiclesInPlay)
                {
                    var inStack = stack.AllCards().FirstOrDefault(c => c.UniqueId == uniqueId);
                    if (inStack != null) return inStack.Data.Name;
                }
            }
            return $"#{uniqueId}";
        }
    }
}

using System.Collections.Generic;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;
using AcceleracersCCG.Effects;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Commands.Player
{
    /// <summary>
    /// Play a Hazard card with a mass effect that requires no single target.
    /// Currently handles: junk_all_race_mods (e.g. Ice Shrapnel).
    /// </summary>
    public class PlayMassHazardCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int HazardCardUniqueId { get; }

        public PlayMassHazardCommand(int playerIndex, int hazardCardUniqueId)
        {
            PlayerIndex = playerIndex;
            HazardCardUniqueId = hazardCardUniqueId;
        }

        public string Validate(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var hazardCard = player.Hand.Get(HazardCardUniqueId);
            if (hazardCard == null)
                return "Hazard card not in hand.";

            if (hazardCard.Data.CardType != CardType.Hazard)
                return "Card is not a Hazard.";

            var apError = ActionPointRules.ValidateCost(player, ((IAPCostCard)hazardCard.Data).APCost);
            if (apError != null) return apError;

            if (!hazardCard.Data.HasEffect(EffectIds.JunkAllRaceMods))
                return "Hazard has no supported mass effect.";

            return null;
        }

        public void Execute(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var hazardCard = player.Hand.Get(HazardCardUniqueId);

            player.Hand.Remove(HazardCardUniqueId);
            player.AP -= ((IAPCostCard)hazardCard.Data).APCost;
            player.JunkPile.Add(hazardCard);

            if (hazardCard.Data.HasEffect(EffectIds.JunkAllRaceMods))
                ExecuteJunkAllRaceMods(state);
        }

        private void ExecuteJunkAllRaceMods(GameState state)
        {
            var opponent = state.Players[1 - PlayerIndex];

            foreach (var stack in opponent.VehiclesInPlay)
            {
                    var toJunk = new List<CardInstance>();
                    foreach (var mod in stack.EquippedMods)
                    {
                        var modData = (ModCardData)mod.Data;
                        if ((modData.ModabilityIcons & ModabilityIcon.Race) != 0)
                            toJunk.Add(mod);
                    }

                    foreach (var mod in toJunk)
                    {
                        stack.RemoveEquipment(mod.UniqueId);
                        opponent.JunkPile.Add(mod);
                    }
            }
        }

        public void Undo(GameState state)
        {
            // Snapshot-based undo handles this via CommandProcessor
        }
    }
}

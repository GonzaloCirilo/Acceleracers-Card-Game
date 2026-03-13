using AcceleracersCCG.Cards;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;
using AcceleracersCCG.Rules;

namespace AcceleracersCCG.Commands.Player
{
    /// <summary>
    /// Play a Vehicle from hand to Realm 1 (index 0).
    /// </summary>
    public class PlayVehicleCommand : ICommand
    {
        public int PlayerIndex { get; }
        public int CardUniqueId { get; }

        public PlayVehicleCommand(int playerIndex, int cardUniqueId)
        {
            PlayerIndex = playerIndex;
            CardUniqueId = cardUniqueId;
        }

        public string Validate(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var card = player.Hand.Get(CardUniqueId);
            if (card == null)
                return "Card not in hand.";

            return PlayVehicleRules.Validate(state, PlayerIndex, card);
        }

        public void Execute(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var card = player.Hand.Get(CardUniqueId);
            player.Hand.Remove(CardUniqueId);

            var stack = new VehicleStack(card);
            stack.RealmIndex = 0;
            player.VehiclesInPlay.Add(stack);
            player.HasPlayedVehicleThisTurn = true;
        }

        public void Undo(GameState state)
        {
            var player = state.GetPlayer(PlayerIndex);
            var stackIdx = player.VehiclesInPlay.FindIndex(v => v.Vehicle.UniqueId == CardUniqueId);
            if (stackIdx >= 0)
            {
                var stack = player.VehiclesInPlay[stackIdx];
                player.VehiclesInPlay.RemoveAt(stackIdx);
                player.Hand.Add(stack.Vehicle);
                player.HasPlayedVehicleThisTurn = false;
            }
        }
    }
}

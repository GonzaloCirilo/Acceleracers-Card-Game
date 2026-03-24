using AcceleracersCCG.Cards;
using AcceleracersCCG.Components;
using AcceleracersCCG.Core;
using AcceleracersCCG.Effects;
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

        private readonly EffectRegistry _effectRegistry;

        public PlayVehicleCommand(int playerIndex, int cardUniqueId, EffectRegistry effectRegistry = null)
        {
            PlayerIndex = playerIndex;
            CardUniqueId = cardUniqueId;
            _effectRegistry = effectRegistry;
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

            if (_effectRegistry != null)
            {
                var context = new CardEffectContext
                {
                    OwnerPlayerIndex = PlayerIndex,
                    SourceCard = card,
                    SourceStack = stack,
                    Trigger = EffectTrigger.OnPlay
                };
                foreach (var effectId in card.Data.EffectIds)
                {
                    var effect = _effectRegistry.Get(effectId);
                    var commands = effect.Resolve(state, context);
                    foreach (var cmd in commands)
                        cmd.Execute(state);
                }
            }
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

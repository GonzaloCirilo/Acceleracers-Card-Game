using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Components;
using AcceleracersCCG.Effects;

namespace AcceleracersCCG.Core
{
    /// <summary>
    /// All state belonging to a single player.
    /// </summary>
    public class PlayerState
    {
        public int PlayerIndex { get; }
        public Deck Deck { get; set; }
        public Hand Hand { get; set; }
        public JunkPile JunkPile { get; set; }
        public List<VehicleStack> VehiclesInPlay { get; set; }
        public int VehiclesFinished { get; set; }
        public int AP { get; set; }
        public bool HasPlayedVehicleThisTurn { get; set; }

        public PlayerState(int playerIndex)
        {
            PlayerIndex = playerIndex;
            Deck = new Deck();
            Hand = new Hand();
            JunkPile = new JunkPile();
            VehiclesInPlay = new List<VehicleStack>();
            VehiclesFinished = 0;
            AP = 0;
            HasPlayedVehicleThisTurn = false;
        }

        private PlayerState(int playerIndex, Deck deck, Hand hand, JunkPile junkPile,
            List<VehicleStack> vehiclesInPlay, int vehiclesFinished, int ap, bool hasPlayedVehicle)
        {
            PlayerIndex = playerIndex;
            Deck = deck;
            Hand = hand;
            JunkPile = junkPile;
            VehiclesInPlay = vehiclesInPlay;
            VehiclesFinished = vehiclesFinished;
            AP = ap;
            HasPlayedVehicleThisTurn = hasPlayedVehicle;
        }

        /// <summary>
        /// Max hand size accounting for vehicle effects (e.g. Technetium's increase_hand_size:1).
        /// </summary>
        /// <summary>
        /// Max hand size accounting for vehicle effects (e.g. Technetium's max_hand_size:8).
        /// When multiple vehicles have the effect, the highest value wins.
        /// </summary>
        public int EffectiveMaxHandSize
        {
            get
            {
                int max = Constants.MaxHandSize;
                foreach (var stack in VehiclesInPlay)
                {
                    foreach (var effectId in stack.Vehicle.Data.EffectIds)
                    {
                        int val = EffectIds.ParseIntParam(effectId, EffectIds.MaxHandSizePrefix);
                        if (val > max) max = val;
                    }
                }
                return max;
            }
        }

        /// <summary>
        /// Find a vehicle stack by the vehicle card's unique ID.
        /// </summary>
        public VehicleStack GetVehicleStack(int vehicleUniqueId)
            => VehiclesInPlay.FirstOrDefault(v => v.Vehicle.UniqueId == vehicleUniqueId);

        /// <summary>
        /// Find which vehicle stack contains a given equipment card.
        /// </summary>
        public VehicleStack FindStackWithEquipment(int equipmentUniqueId)
            => VehiclesInPlay.FirstOrDefault(v => v.FindEquipment(equipmentUniqueId) != null);

        /// <summary>
        /// Count distinct teams among vehicles in play.
        /// </summary>
        public Dictionary<Team, int> GetTeamCounts()
        {
            var counts = new Dictionary<Team, int>();
            foreach (var stack in VehiclesInPlay)
            {
                if (stack.Vehicle.Data is not Cards.VehicleCardData vehicleData) continue;
                var team = vehicleData.Team;
                if (team == Team.None) continue;
                counts.TryGetValue(team, out int count);
                counts[team] = count + 1;
            }
            return counts;
        }

        public PlayerState Clone()
        {
            return new PlayerState(
                PlayerIndex,
                Deck.Clone(),
                Hand.Clone(),
                JunkPile.Clone(),
                VehiclesInPlay.Select(v => v.Clone()).ToList(),
                VehiclesFinished,
                AP,
                HasPlayedVehicleThisTurn
            );
        }
    }
}

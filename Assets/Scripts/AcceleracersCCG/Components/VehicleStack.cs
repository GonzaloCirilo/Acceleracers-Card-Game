using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Cards;

namespace AcceleracersCCG.Components
{
    /// <summary>
    /// A vehicle in play with all its attached equipment.
    /// Tracks realm progress and tokens.
    /// </summary>
    public class VehicleStack
    {
        /// <summary>The base vehicle card.</summary>
        public CardInstance Vehicle { get; }

        /// <summary>Permanently equipped mods (survive realm advance).</summary>
        public List<CardInstance> EquippedMods { get; }

        /// <summary>Temporarily equipped shifts (stripped on advance).</summary>
        public List<CardInstance> EquippedShifts { get; }

        /// <summary>Max 1 AcceleCharger (stripped on advance, immune to most hazards).</summary>
        public CardInstance AcceleCharger { get; set; }

        /// <summary>Counters and markers on this vehicle.</summary>
        public TokenSet Tokens { get; }

        /// <summary>
        /// 0-based realm index. 0 = first realm, 3 = fourth realm, 4 = finished.
        /// </summary>
        public int RealmIndex { get; set; }

        public bool HasFinished => RealmIndex >= Core.Constants.RealmsPerRace;

        public VehicleStack(CardInstance vehicle)
        {
            Vehicle = vehicle;
            EquippedMods = new List<CardInstance>();
            EquippedShifts = new List<CardInstance>();
            AcceleCharger = null;
            Tokens = new TokenSet();
            RealmIndex = 0;
        }

        private VehicleStack(CardInstance vehicle, List<CardInstance> mods, List<CardInstance> shifts,
            CardInstance acceleCharger, TokenSet tokens, int realmIndex)
        {
            Vehicle = vehicle;
            EquippedMods = mods;
            EquippedShifts = shifts;
            AcceleCharger = acceleCharger;
            Tokens = tokens;
            RealmIndex = realmIndex;
        }

        /// <summary>
        /// Gets all cards in the stack (vehicle + all equipment).
        /// </summary>
        public IEnumerable<CardInstance> AllCards()
        {
            yield return Vehicle;
            foreach (var mod in EquippedMods) yield return mod;
            foreach (var shift in EquippedShifts) yield return shift;
            if (AcceleCharger != null) yield return AcceleCharger;
        }

        /// <summary>
        /// Finds an equipped card by unique ID (in mods, shifts, or accelecharger).
        /// </summary>
        public CardInstance FindEquipment(int uniqueId)
        {
            return AllCards().FirstOrDefault(c => c.UniqueId == uniqueId);
        }

        /// <summary>
        /// Removes an equipped card by unique ID. Returns the removed card or null.
        /// </summary>
        public CardInstance RemoveEquipment(int uniqueId)
        {
            int idx = EquippedMods.FindIndex(c => c.UniqueId == uniqueId);
            if (idx >= 0)
            {
                var card = EquippedMods[idx];
                EquippedMods.RemoveAt(idx);
                return card;
            }

            idx = EquippedShifts.FindIndex(c => c.UniqueId == uniqueId);
            if (idx >= 0)
            {
                var card = EquippedShifts[idx];
                EquippedShifts.RemoveAt(idx);
                return card;
            }

            if (AcceleCharger != null && AcceleCharger.UniqueId == uniqueId)
            {
                var card = AcceleCharger;
                AcceleCharger = null;
                return card;
            }

            return null;
        }

        public VehicleStack Clone()
        {
            return new VehicleStack(
                Vehicle.Clone(),
                EquippedMods.Select(c => c.Clone()).ToList(),
                EquippedShifts.Select(c => c.Clone()).ToList(),
                AcceleCharger?.Clone(),
                Tokens.Clone(),
                RealmIndex
            );
        }
    }
}

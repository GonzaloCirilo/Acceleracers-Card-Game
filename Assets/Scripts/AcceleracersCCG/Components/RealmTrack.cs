using System;
using System.Linq;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Components
{
    /// <summary>
    /// The 4 Racing Realms that vehicles must pass through to win.
    /// </summary>
    public class RealmTrack
    {
        private readonly CardInstance[] _realms;
        private readonly bool[] _revealed;

        public RealmTrack()
        {
            _realms = new CardInstance[Constants.RealmsPerRace];
            _revealed = new bool[Constants.RealmsPerRace];
        }

        private RealmTrack(CardInstance[] realms, bool[] revealed)
        {
            _realms = realms;
            _revealed = revealed;
        }

        public void SetRealm(int index, CardInstance realm)
        {
            if (index < 0 || index >= Constants.RealmsPerRace)
                throw new ArgumentOutOfRangeException(nameof(index));
            _realms[index] = realm;
        }

        public void Reveal(int index)
        {
            if (index < 0 || index >= Constants.RealmsPerRace)
                throw new ArgumentOutOfRangeException(nameof(index));
            _revealed[index] = true;
        }

        public CardInstance GetRealm(int index)
        {
            if (index < 0 || index >= Constants.RealmsPerRace)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _realms[index];
        }

        public bool IsRevealed(int index)
        {
            if (index < 0 || index >= Constants.RealmsPerRace)
                throw new ArgumentOutOfRangeException(nameof(index));
            return _revealed[index];
        }

        /// <summary>
        /// Returns the terrain icons for a given realm index.
        /// Returns None if realm is not set or not revealed.
        /// </summary>
        public TerrainIcon GetTerrainAt(int realmIndex)
        {
            if (realmIndex < 0 || realmIndex >= Constants.RealmsPerRace)
                return TerrainIcon.None;
            var realm = _realms[realmIndex];
            if (realm == null || !_revealed[realmIndex])
                return TerrainIcon.None;
            return realm.Data.TerrainIcons;
        }

        public bool AllRealmsSet()
            => _realms.All(r => r != null);

        public RealmTrack Clone()
        {
            return new RealmTrack(
                _realms.Select(r => r?.Clone()).ToArray(),
                (bool[])_revealed.Clone()
            );
        }
    }
}

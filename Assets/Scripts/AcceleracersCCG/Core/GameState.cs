using AcceleracersCCG.Components;

namespace AcceleracersCCG.Core
{
    /// <summary>
    /// Root game state. All mutable game data lives here.
    /// Only Commands should mutate this.
    /// </summary>
    public class GameState
    {
        public PlayerState[] Players { get; }
        public RealmTrack RealmTrack { get; set; }
        public int ActivePlayerIndex { get; set; }
        public GamePhaseId CurrentPhase { get; set; }
        public int TurnNumber { get; set; }
        public GameResult Result { get; set; }
        public int Seed { get; set; }

        public PlayerState ActivePlayer => Players[ActivePlayerIndex];
        public PlayerState InactivePlayer => Players[1 - ActivePlayerIndex];

        public GameState(int seed = 0)
        {
            Players = new PlayerState[]
            {
                new PlayerState(0),
                new PlayerState(1)
            };
            RealmTrack = new RealmTrack();
            ActivePlayerIndex = 0;
            CurrentPhase = GamePhaseId.Setup;
            TurnNumber = 0;
            Result = GameResult.InProgress;
            Seed = seed;
        }

        private GameState(PlayerState[] players, RealmTrack realmTrack, int activePlayerIndex,
            GamePhaseId currentPhase, int turnNumber, GameResult result, int seed)
        {
            Players = players;
            RealmTrack = realmTrack;
            ActivePlayerIndex = activePlayerIndex;
            CurrentPhase = currentPhase;
            TurnNumber = turnNumber;
            Result = result;
            Seed = seed;
        }

        public PlayerState GetPlayer(int index) => Players[index];

        public GameState DeepClone()
        {
            return new GameState(
                new PlayerState[] { Players[0].Clone(), Players[1].Clone() },
                RealmTrack.Clone(),
                ActivePlayerIndex,
                CurrentPhase,
                TurnNumber,
                Result,
                Seed
            );
        }
    }
}

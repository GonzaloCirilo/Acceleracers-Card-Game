using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Reveal a face-down realm on the track.
    /// </summary>
    public class FlipRealmCommand : ICommand
    {
        public int RealmIndex { get; }

        public FlipRealmCommand(int realmIndex)
        {
            RealmIndex = realmIndex;
        }

        public string Validate(GameState state)
        {
            if (RealmIndex < 0 || RealmIndex >= Constants.RealmsPerRace)
                return $"Invalid realm index: {RealmIndex}.";
            if (state.RealmTrack.GetRealm(RealmIndex) == null)
                return "No realm card at this position.";
            if (state.RealmTrack.IsRevealed(RealmIndex))
                return "Realm is already revealed.";
            return null;
        }

        public void Execute(GameState state)
        {
            state.RealmTrack.Reveal(RealmIndex);
        }

        public void Undo(GameState state)
        {
            // Snapshot-based undo via CommandProcessor
        }
    }
}

using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands.System
{
    /// <summary>
    /// Reveal a face-down realm on the track.
    /// </summary>
    public class FlipRealmCommand : ICommand
    {
        public int RealmIndex { get; }

        private bool _wasRevealed;

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
            _wasRevealed = state.RealmTrack.IsRevealed(RealmIndex);
            state.RealmTrack.Reveal(RealmIndex);
        }

        public void Undo(GameState state)
        {
            // Can't un-reveal a realm in practice, but snapshot handles it
        }
    }
}

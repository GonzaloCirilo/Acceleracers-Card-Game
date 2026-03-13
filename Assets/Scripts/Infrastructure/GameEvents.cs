using AcceleracersCCG.Cards;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Infrastructure
{
    /// <summary>
    /// Game event types for UI notification and logging.
    /// </summary>
    public abstract class GameEvent { }

    public class CardDrawnEvent : GameEvent
    {
        public int PlayerIndex { get; }
        public CardInstance Card { get; }
        public CardDrawnEvent(int playerIndex, CardInstance card) { PlayerIndex = playerIndex; Card = card; }
    }

    public class VehicleAdvancedEvent : GameEvent
    {
        public int PlayerIndex { get; }
        public int VehicleUniqueId { get; }
        public int NewRealmIndex { get; }
        public VehicleAdvancedEvent(int playerIndex, int vehicleId, int newRealm)
        { PlayerIndex = playerIndex; VehicleUniqueId = vehicleId; NewRealmIndex = newRealm; }
    }

    public class CardEquippedEvent : GameEvent
    {
        public int PlayerIndex { get; }
        public CardInstance Card { get; }
        public int VehicleUniqueId { get; }
        public CardEquippedEvent(int playerIndex, CardInstance card, int vehicleId)
        { PlayerIndex = playerIndex; Card = card; VehicleUniqueId = vehicleId; }
    }

    public class CardJunkedEvent : GameEvent
    {
        public int PlayerIndex { get; }
        public CardInstance Card { get; }
        public string Reason { get; }
        public CardJunkedEvent(int playerIndex, CardInstance card, string reason = null)
        { PlayerIndex = playerIndex; Card = card; Reason = reason; }
    }

    public class PhaseChangedEvent : GameEvent
    {
        public GamePhaseId OldPhase { get; }
        public GamePhaseId NewPhase { get; }
        public PhaseChangedEvent(GamePhaseId oldPhase, GamePhaseId newPhase)
        { OldPhase = oldPhase; NewPhase = newPhase; }
    }

    public class GameOverEvent : GameEvent
    {
        public GameResult Result { get; }
        public GameOverEvent(GameResult result) { Result = result; }
    }

    public class VehiclePlayedEvent : GameEvent
    {
        public int PlayerIndex { get; }
        public CardInstance Vehicle { get; }
        public VehiclePlayedEvent(int playerIndex, CardInstance vehicle)
        { PlayerIndex = playerIndex; Vehicle = vehicle; }
    }

    public class HazardPlayedEvent : GameEvent
    {
        public int PlayerIndex { get; }
        public CardInstance Hazard { get; }
        public CardInstance Target { get; }
        public bool TargetJunked { get; }
        public HazardPlayedEvent(int playerIndex, CardInstance hazard, CardInstance target, bool junked)
        { PlayerIndex = playerIndex; Hazard = hazard; Target = target; TargetJunked = junked; }
    }

    public class TurnStartedEvent : GameEvent
    {
        public int PlayerIndex { get; }
        public int TurnNumber { get; }
        public TurnStartedEvent(int playerIndex, int turnNumber)
        { PlayerIndex = playerIndex; TurnNumber = turnNumber; }
    }
}

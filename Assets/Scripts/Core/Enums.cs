using System;

namespace AcceleracersCCG.Core
{
    public enum CardType
    {
        RacingRealm,
        Vehicle,
        Mod,
        Shift,
        AcceleCharger,
        Hazard
    }

    public enum Team
    {
        None,
        MetalManiacs,
        TekuRacers,
        Silencerz,
        RacingDrones
    }

    [Flags]
    public enum ModabilityIcon
    {
        None = 0,
        Street = 1 << 0,
        Race = 1 << 1,
        OffRoad = 1 << 2
    }

    [Flags]
    public enum TerrainIcon
    {
        None = 0,
        Sand = 1 << 0,
        Mud = 1 << 1,
        Water = 1 << 2,
        Rough = 1 << 3,
        Slick = 1 << 4,
        Paved = 1 << 5
    }

    public enum SPPCategory
    {
        Speed,
        Power,
        Performance
    }

    public enum GamePhaseId
    {
        Setup,
        Draw,
        Advance,
        PlayVehicle,
        TuneUp,
        Action,
        Discard,
        EndTurn
    }

    public enum GameResult
    {
        InProgress,
        Player0Wins,
        Player1Wins,
        Draw
    }

    public enum ChoiceType
    {
        TransferModSelectMod,
        TransferModSelectTarget,
        RecoverModsForAP,
        RecoverCardFromJunk
    }
}

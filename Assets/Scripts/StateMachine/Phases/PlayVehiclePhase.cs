using System.Collections.Generic;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.Player;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.StateMachine.Phases
{
    /// <summary>
    /// Interactive: player may optionally play 1 Vehicle from hand to Realm 1.
    /// </summary>
    public class PlayVehiclePhase : IGamePhase
    {
        public GamePhaseId Id => GamePhaseId.PlayVehicle;
        public bool IsAutomatic => false;

        public void OnEnter(GameState state)
        {
        }

        public void OnExit(GameState state)
        {
        }

        public List<ICommand> GetAutoCommands(GameState state)
        {
            return new List<ICommand>();
        }

        public List<ICommand> GetLegalPlayerCommands(GameState state)
        {
            var commands = new List<ICommand>();
            var player = state.ActivePlayer;
            var playerIdx = state.ActivePlayerIndex;

            if (!player.HasPlayedVehicleThisTurn)
            {
                var vehicles = player.Hand.GetByType(CardType.Vehicle);
                foreach (var v in vehicles)
                {
                    commands.Add(new PlayVehicleCommand(playerIdx, v.UniqueId));
                }
            }

            // Can always pass (end phase)
            commands.Add(new EndPhaseCommand(playerIdx));

            return commands;
        }

        public GamePhaseId GetNextPhase(GameState state)
        {
            return GamePhaseId.TuneUp;
        }
    }
}

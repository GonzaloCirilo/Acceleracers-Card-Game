using System;
using System.Collections.Generic;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Components;
using AcceleracersCCG.Infrastructure;
using AcceleracersCCG.StateMachine;
using AcceleracersCCG.StateMachine.Phases;

namespace AcceleracersCCG.Core
{
    /// <summary>
    /// Top-level orchestrator: owns state, phase machine, and command processor.
    /// This is the main entry point for game logic.
    /// </summary>
    public class GameController
    {
        public GameState State { get; }
        public CommandProcessor CommandProcessor { get; }
        public GamePhaseMachine PhaseMachine { get; }

        private readonly IRandomProvider _rng;

        public GameController(int seed = 0)
        {
            State = new GameState(seed);
            _rng = new SeededRandomProvider(seed);
            CommandProcessor = new CommandProcessor();
            PhaseMachine = new GamePhaseMachine(CommandProcessor);

            RegisterPhases();
        }

        public GameController(GameState state, IRandomProvider rng)
        {
            State = state;
            _rng = rng;
            CommandProcessor = new CommandProcessor();
            PhaseMachine = new GamePhaseMachine(CommandProcessor);

            RegisterPhases();
        }

        private void RegisterPhases()
        {
            PhaseMachine.RegisterPhase(new SetupPhase(_rng));
            PhaseMachine.RegisterPhase(new DrawPhase());
            PhaseMachine.RegisterPhase(new AdvancePhase());
            PhaseMachine.RegisterPhase(new PlayVehiclePhase());
            PhaseMachine.RegisterPhase(new TuneUpPhase());
            PhaseMachine.RegisterPhase(new ActionPhase());
            PhaseMachine.RegisterPhase(new DiscardPhase());
            PhaseMachine.RegisterPhase(new EndTurnPhase());
        }

        /// <summary>
        /// Initialize decks and realms, then run setup phase.
        /// </summary>
        public void StartGame(List<CardInstance> deck0, List<CardInstance> deck1,
            CardInstance[] realms)
        {
            // Set up decks
            State.Players[0].Deck = new Deck(deck0);
            State.Players[1].Deck = new Deck(deck1);

            // Set up realm track
            for (int i = 0; i < realms.Length && i < Constants.RealmsPerRace; i++)
            {
                State.RealmTrack.SetRealm(i, realms[i]);
            }

            // Run setup phase
            PhaseMachine.TransitionTo(GamePhaseId.Setup, State);
        }

        /// <summary>
        /// Submit a player command during an interactive phase.
        /// Returns null on success, or error string.
        /// </summary>
        public string SubmitCommand(ICommand command)
        {
            if (State.Result != GameResult.InProgress)
                return "Game is over.";

            var error = CommandProcessor.Execute(command, State);
            if (error != null) return error;

            // Check if this was an EndPhaseCommand — advance the phase machine
            if (command is Commands.Player.EndPhaseCommand)
            {
                PhaseMachine.AdvancePhase(State);
            }
            // For discard phase: check if we need to auto-advance after discarding
            else if (State.CurrentPhase == GamePhaseId.Discard
                     && !State.ActivePlayer.Hand.IsOverMaxSize)
            {
                PhaseMachine.AdvancePhase(State);
            }

            return null;
        }

        /// <summary>
        /// Get all legal commands the active player can issue right now.
        /// </summary>
        public List<ICommand> GetLegalCommands()
        {
            return PhaseMachine.GetLegalCommands(State);
        }

        /// <summary>
        /// Undo the last command. Also syncs the phase machine to the restored state.
        /// </summary>
        public ICommand Undo()
        {
            var command = CommandProcessor.Undo(State);
            if (command != null)
            {
                PhaseMachine.SyncToState(State);
            }
            return command;
        }

        public bool IsGameOver => State.Result != GameResult.InProgress;
    }
}

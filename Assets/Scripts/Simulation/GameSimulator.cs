using System;
using System.Collections.Generic;
using System.Linq;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Commands.Player;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Simulation
{
    /// <summary>
    /// Headless auto-driver that plays an already-started <see cref="GameController"/>
    /// to completion using a simple naive AI. Pure C# (no UnityEngine dependency) so it
    /// can run in edit-mode tests as well as inside a scene MonoBehaviour.
    ///
    /// The AI is deliberately dumb: on each interactive step it either takes a random
    /// legal action (equip a mod/shift, play a vehicle, etc.) or ends the phase. Combined
    /// with the deck-out rule this guarantees every game terminates.
    /// </summary>
    public static class GameSimulator
    {
        public class Options
        {
            /// <summary>Seed for the AI's action choices (independent of the game RNG).</summary>
            public int Seed = 1;

            /// <summary>Hard cap on turns before we bail out (safety net; games normally deck-out sooner).</summary>
            public int MaxTurns = 500;

            /// <summary>Max non-EndPhase actions the AI takes per interactive phase before forcing an end.</summary>
            public int MaxActionsPerPhase = 40;

            /// <summary>If set, receives a human-readable line for every command submitted.</summary>
            public Action<string> Log;
        }

        public class Result
        {
            public GameResult Outcome;
            public int Turns;
            public int Commands;
            public bool HitTurnCap;
        }

        public static Result Run(GameController controller, Options options = null)
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            options ??= new Options();

            var rng = new Random(options.Seed);
            int commandCount = 0;
            int actionsThisPhase = 0;

            void OnPhaseChanged(GamePhaseId from, GamePhaseId to) => actionsThisPhase = 0;
            controller.PhaseMachine.OnPhaseChanged += OnPhaseChanged;

            try
            {
                // Absolute safety net independent of TurnNumber, in case a phase can't advance.
                int hardCommandCap = options.MaxTurns * 500;

                while (!controller.IsGameOver
                       && controller.State.TurnNumber < options.MaxTurns
                       && commandCount < hardCommandCap)
                {
                    var legal = controller.GetLegalCommands();
                    if (legal == null || legal.Count == 0)
                        break; // Resting on an automatic phase with nothing to do — shouldn't happen.

                    var command = ChooseCommand(legal, rng, actionsThisPhase, options.MaxActionsPerPhase);
                    bool isAction = !(command is EndPhaseCommand);

                    var error = controller.SubmitCommand(command);
                    commandCount++;
                    if (isAction) actionsThisPhase++;

                    options.Log?.Invoke(
                        $"T{controller.State.TurnNumber,-3} {controller.State.CurrentPhase,-12} " +
                        $"{command.GetType().Name,-24} -> {(error ?? "ok")}");
                }
            }
            finally
            {
                controller.PhaseMachine.OnPhaseChanged -= OnPhaseChanged;
            }

            return new Result
            {
                Outcome = controller.State.Result,
                Turns = controller.State.TurnNumber,
                Commands = commandCount,
                HitTurnCap = controller.State.TurnNumber >= options.MaxTurns,
            };
        }

        private static ICommand ChooseCommand(List<ICommand> legal, Random rng,
            int actionsThisPhase, int maxActionsPerPhase)
        {
            var nonEnd = legal.Where(c => !(c is EndPhaseCommand)).ToList();

            // Take a real action while we're still under budget; otherwise wrap up the phase.
            if (nonEnd.Count > 0 && actionsThisPhase < maxActionsPerPhase)
                return nonEnd[rng.Next(nonEnd.Count)];

            // Prefer an explicit EndPhase; if none is offered (e.g. mandatory discard), fall
            // back to any legal command so we keep making forced progress.
            return legal.FirstOrDefault(c => c is EndPhaseCommand) ?? legal[0];
        }
    }
}

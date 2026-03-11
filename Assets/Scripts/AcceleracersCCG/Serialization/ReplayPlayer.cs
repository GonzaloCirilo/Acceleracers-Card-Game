using System.Collections.Generic;
using AcceleracersCCG.Cards;
using AcceleracersCCG.Commands;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Serialization
{
    /// <summary>
    /// Replays a game from a seed and command list.
    /// Used for testing determinism and debugging.
    /// </summary>
    public class ReplayPlayer
    {
        /// <summary>
        /// Replay a game with the same seed, decks, realms, and commands.
        /// Returns the final game state.
        /// </summary>
        public static GameState Replay(
            int seed,
            List<CardInstance> deck0,
            List<CardInstance> deck1,
            CardInstance[] realms,
            IReadOnlyList<ICommand> commands)
        {
            var controller = new GameController(seed);
            controller.StartGame(deck0, deck1, realms);

            foreach (var cmd in commands)
            {
                controller.SubmitCommand(cmd);
            }

            return controller.State;
        }
    }
}

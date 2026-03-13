using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands
{
    /// <summary>
    /// A command that mutates the game state. All state changes go through commands.
    /// </summary>
    public interface ICommand
    {
        /// <summary>Apply this command to the game state.</summary>
        void Execute(GameState state);

        /// <summary>Reverse this command's effects on the game state.</summary>
        void Undo(GameState state);

        /// <summary>
        /// Returns null if valid, or a reason string if this command cannot be executed.
        /// </summary>
        string Validate(GameState state);
    }
}

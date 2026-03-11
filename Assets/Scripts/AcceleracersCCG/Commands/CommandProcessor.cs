using System;
using System.Collections.Generic;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Commands
{
    /// <summary>
    /// Executes and tracks commands. Supports undo and triggered command chaining.
    /// </summary>
    public class CommandProcessor
    {
        private readonly CommandHistory _history = new CommandHistory();
        private readonly List<GameState> _snapshots = new List<GameState>();

        /// <summary>
        /// Maximum number of undo snapshots to retain. 0 = unlimited.
        /// When exceeded, the oldest snapshot is discarded (no longer undoable).
        /// </summary>
        public int MaxUndoDepth { get; set; } = 100;

        public CommandHistory History => _history;

        /// <summary>
        /// Event fired after a command executes successfully.
        /// </summary>
        public event Action<ICommand> OnCommandExecuted;

        /// <summary>
        /// Validates and executes a command. Returns null on success, or error reason.
        /// </summary>
        public string Execute(ICommand command, GameState state)
        {
            var error = command.Validate(state);
            if (error != null)
                return error;

            // Save snapshot for undo
            _snapshots.Add(state.DeepClone());
            TrimSnapshots();

            command.Execute(state);
            _history.Add(command);

            OnCommandExecuted?.Invoke(command);
            return null;
        }

        /// <summary>
        /// Executes a command without validation (for system/auto commands).
        /// </summary>
        public void ExecuteUnchecked(ICommand command, GameState state)
        {
            _snapshots.Add(state.DeepClone());
            TrimSnapshots();
            command.Execute(state);
            _history.Add(command);
            OnCommandExecuted?.Invoke(command);
        }

        /// <summary>
        /// Undoes the last command by restoring the snapshot.
        /// Returns the undone command, or null if nothing to undo.
        /// </summary>
        public ICommand Undo(GameState state)
        {
            var command = _history.RemoveLast();
            if (command == null) return null;

            if (_snapshots.Count > 0)
            {
                var snapshot = _snapshots[_snapshots.Count - 1];
                _snapshots.RemoveAt(_snapshots.Count - 1);

                // Restore state from snapshot
                RestoreState(state, snapshot);
            }
            else
            {
                // Fallback: use command's undo
                command.Undo(state);
            }

            return command;
        }

        /// <summary>
        /// Executes a batch of commands in order. Stops on first failure.
        /// </summary>
        public string ExecuteBatch(IReadOnlyList<ICommand> commands, GameState state)
        {
            foreach (var cmd in commands)
            {
                var error = Execute(cmd, state);
                if (error != null) return error;
            }
            return null;
        }

        private void TrimSnapshots()
        {
            if (MaxUndoDepth > 0 && _snapshots.Count > MaxUndoDepth)
            {
                _snapshots.RemoveAt(0);
            }
        }

        private void RestoreState(GameState target, GameState source)
        {
            // Copy all mutable fields from source to target
            target.Players[0] = source.Players[0];
            target.Players[1] = source.Players[1];
            target.RealmTrack = source.RealmTrack;
            target.ActivePlayerIndex = source.ActivePlayerIndex;
            target.CurrentPhase = source.CurrentPhase;
            target.TurnNumber = source.TurnNumber;
            target.Result = source.Result;
            target.Seed = source.Seed;
        }
    }
}

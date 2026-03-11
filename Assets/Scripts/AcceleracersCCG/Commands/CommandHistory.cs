using System.Collections.Generic;

namespace AcceleracersCCG.Commands
{
    /// <summary>
    /// Ordered list of executed commands for replay and undo.
    /// </summary>
    public class CommandHistory
    {
        private readonly List<ICommand> _commands = new List<ICommand>();

        public int Count => _commands.Count;
        public IReadOnlyList<ICommand> Commands => _commands;

        public void Add(ICommand command)
        {
            _commands.Add(command);
        }

        public ICommand RemoveLast()
        {
            if (_commands.Count == 0) return null;
            var last = _commands[_commands.Count - 1];
            _commands.RemoveAt(_commands.Count - 1);
            return last;
        }

        public ICommand GetLast()
        {
            return _commands.Count > 0 ? _commands[_commands.Count - 1] : null;
        }

        public void Clear()
        {
            _commands.Clear();
        }
    }
}

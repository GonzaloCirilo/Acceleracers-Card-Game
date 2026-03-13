using System;
using System.Collections.Generic;
using System.Text;
using AcceleracersCCG.Commands;

namespace AcceleracersCCG.Serialization
{
    /// <summary>
    /// Serialize command history for replay.
    /// Stores command type name and constructor parameters.
    /// </summary>
    public static class CommandSerializer
    {
        /// <summary>
        /// Serialize command history to a string representation.
        /// </summary>
        public static string Serialize(CommandHistory history)
        {
            var sb = new StringBuilder();
            foreach (var cmd in history.Commands)
            {
                sb.AppendLine(SerializeCommand(cmd));
            }
            return sb.ToString();
        }

        private static string SerializeCommand(ICommand command)
        {
            // Simple type name + reflection-free property serialization
            var type = command.GetType();
            var sb = new StringBuilder();
            sb.Append(type.Name);

            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (prop.CanRead && (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string)))
                {
                    sb.Append($"|{prop.Name}={prop.GetValue(command)}");
                }
            }

            return sb.ToString();
        }
    }
}

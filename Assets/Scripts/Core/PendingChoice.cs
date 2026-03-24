using System.Collections.Generic;

namespace AcceleracersCCG.Core
{
    /// <summary>
    /// Represents a player decision that must be resolved before the phase can advance.
    /// Set on GameState by effects; cleared by ResolveChoiceCommand.
    /// </summary>
    public class PendingChoice
    {
        /// <summary>What kind of choice the player must make.</summary>
        public ChoiceType Type { get; }

        /// <summary>The player who must make the choice.</summary>
        public int PlayerIndex { get; }

        /// <summary>The card whose effect triggered this choice.</summary>
        public int SourceCardUniqueId { get; }

        /// <summary>Unique IDs of cards the player can select from.</summary>
        public IReadOnlyList<int> Options { get; }

        public PendingChoice(ChoiceType type, int playerIndex, int sourceCardUniqueId, IReadOnlyList<int> options)
        {
            Type = type;
            PlayerIndex = playerIndex;
            SourceCardUniqueId = sourceCardUniqueId;
            Options = options;
        }

        public PendingChoice Clone()
        {
            return new PendingChoice(Type, PlayerIndex, SourceCardUniqueId, new List<int>(Options));
        }
    }
}

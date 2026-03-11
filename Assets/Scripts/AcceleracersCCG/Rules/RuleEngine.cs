using System.Collections.Generic;
using AcceleracersCCG.Core;

namespace AcceleracersCCG.Rules
{
    /// <summary>
    /// Aggregates rule validators. Used by commands to check legality before executing.
    /// </summary>
    public class RuleEngine
    {
        /// <summary>
        /// Validates a command by running all provided validation checks.
        /// Returns null if all valid, or the first failure reason.
        /// </summary>
        public static string Validate(params string[] validationResults)
        {
            foreach (var result in validationResults)
            {
                if (!string.IsNullOrEmpty(result))
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Validates a command by running all provided validation checks.
        /// Returns all failure reasons.
        /// </summary>
        public static List<string> ValidateAll(params string[] validationResults)
        {
            var errors = new List<string>();
            foreach (var result in validationResults)
            {
                if (!string.IsNullOrEmpty(result))
                    errors.Add(result);
            }
            return errors;
        }
    }
}

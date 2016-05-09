using System.Collections.Generic;

namespace Games.AI.AdversarialSearch
{
    public interface IProblem
    {
        /// <summary>
        /// Gets the maximum level that is searchable for this problem.
        /// </summary>
        int MaxLevel { get; }

        /// <summary>
        /// Gets the utility value for the state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>Utility value.</returns>
        double GetUtilityValue(IState state);

        /// <summary>
        /// Returns true if the node is a terminal state, otherwise false.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>True if terminal, otherwise false.</returns>
        bool IsTerminalState(IState state);

        /// <summary>
        /// Gets the successors for the given state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>Child successor.</returns>
        List<Successor> GetSuccessors(IState state);
    }
}

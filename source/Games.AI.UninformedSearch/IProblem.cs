using System.Collections.Generic;

namespace Games.AI.UninformedSearch
{
    /// <summary>
    /// Encapsulates the rules of the problem and what actions can be taken.
    /// </summary>
    public interface IProblem
    {
        /// <summary>
        /// Gets the initial state of the problem.
        /// </summary>
        /// <returns>The initial state.</returns>
        IState InitialState { get; }

        /// <summary>
        /// Gets the successors for the given state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>Child successors.</returns>
        List<Successor> GetSuccessors(IState state);
    }
}

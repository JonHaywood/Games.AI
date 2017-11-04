
using Games.AI.Search;

namespace Games.AI.AdversarialSearch
{
    /// <summary>
    /// Represents an algorithm which can take a particular problem, a starting
    /// state, and find a resulting solution.
    /// </summary>
    public interface IAlgorithm
    {
        AlgorithmResult SolveForBestAction(IProblem problem, IState state);
    }
}

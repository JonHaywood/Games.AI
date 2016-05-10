
namespace Games.AI.UninformedSearch
{
    /// <summary>
    /// Represents a class which can take a given problem and find all the resulting
    /// solutions.
    /// </summary>
    public interface IAlgorithm
    {
        AlgorithmResult Solve(IProblem problem);
    }
}


namespace Games.AI.AdversarialSearch
{
    public interface IAlgorithm
    {
        AlgorithmResult SolveForBestAction(IProblem problem, IState state);
    }
}

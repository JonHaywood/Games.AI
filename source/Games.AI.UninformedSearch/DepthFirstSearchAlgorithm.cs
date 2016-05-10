using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Games.AI.UninformedSearch
{
    public class DepthFirstSearchAlgorithm : IAlgorithm
    {
        private int recursionLevel = 0;
        private readonly List<Solution> solutions = new List<Solution>();
        private readonly HashSet<int> visitedNodes = new HashSet<int>();        

        public AlgorithmResult Solve(IProblem problem)
        {            
            recursionLevel = 0;
            visitedNodes.Clear();
            solutions.Clear();

            IState initialState = problem.InitialState;

            var watch = Stopwatch.StartNew();
            SolveRecursive(problem, initialState, initialState, new List<Successor>());
            watch.Stop();

            var statistics = new AlgorithmStatistics
            {
                ElapsedTimeInSeconds = watch.Elapsed.TotalSeconds,
                VisitedNodeCount = visitedNodes.Count
            };            

            return new AlgorithmResult(solutions, statistics);
        }

        private void SolveRecursive(IProblem problem, IState initial, IState state, List<Successor> currentSuccessors)
        {
            var successors = problem.GetSuccessors(state).ToList();

            // if there's no more successors then the game is done, save, then go back up the chain
            if (successors.Count == 0)
            {
                solutions.Add(new Solution(initial, currentSuccessors));
                return;
            }

            foreach (var successor in successors)
            {
                // keep a list of visited nodes. if we've seen this
                // before then skip it, no need to explore it again
                var uniqueCode = successor.ResultingState.GetHashCode();
                if (visitedNodes.Contains(uniqueCode))
                    continue;
                
                visitedNodes.Add(uniqueCode);

                // add assignment to previous lsit
                var newSuccessors = currentSuccessors.ToList(); // clone the successors so that it doesn't mess up recursion
                newSuccessors.Add(successor);

                // recurse down
                recursionLevel++;
                SolveRecursive(problem, initial, successor.ResultingState, newSuccessors);
                recursionLevel--;
            }
        }
    }
}

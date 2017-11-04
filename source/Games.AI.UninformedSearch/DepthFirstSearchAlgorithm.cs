using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Games.AI.Search;

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
                if (HasVisited(successor))
                    continue;
                
                visitedNodes.Add(successor.ResultingState.GetHashCode());

                // add assignment to previous lsit
                var newSuccessors = currentSuccessors.ToList(); // clone the successors so that it doesn't mess up recursion
                newSuccessors.Add(successor);

                // recurse down
                recursionLevel++;
                SolveRecursive(problem, initial, successor.ResultingState, newSuccessors);
                recursionLevel--;
            }
        }

        private bool HasVisited(Successor successor)
        {
            var uniqueCode = successor.ResultingState.GetHashCode();
            if (visitedNodes.Contains(uniqueCode))
                return true;
            
            var transformable = successor.ResultingState as ITransformable;
            if (transformable == null)
                return false;

            // check for transforms and see if those have been visited
            var transforms = transformable.GetTransforms();
            return transforms.Any(t => visitedNodes.Contains(t.GetHashCode()));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Games.AI.AdversarialSearch
{
    public class MinimaxWithAlphaBetaPruningAlgorithm : IAlgorithm
    {        
        private readonly Dictionary<IState, double> transpositionTable = new Dictionary<IState, double>();

        private IProblem Problem { get; set; }
        private AlgorithmStatistics Statistics { get; set; }

        public bool UseTranspositionTable { get; set; }

        public AlgorithmResult SolveForBestAction(IProblem problem, IState state)
        {
            Problem = problem;
            Statistics = new AlgorithmStatistics();
            transpositionTable.Clear();
            var stopwach = Stopwatch.StartNew();

            var v = double.NegativeInfinity;
            var alpha = double.NegativeInfinity;
            var beta = double.PositiveInfinity;
            IAction action = null;

            // perform the MAX-VALUE portion of minimax except we'll
            // record the action associated with the successor so we 
            // can return.
            var successors = Problem.GetSuccessors(state);
            foreach (var successor in successors)
            {
                Statistics.VisitedStateCount++;

                var vTemp = Math.Max(v, MinValue(successor.ResultingState, alpha, beta));
                if (vTemp > v)
                {
                    v = vTemp;
                    action = successor.Action;
                }
                if (v >= beta)
                    break;
                alpha = Math.Max(alpha, v);
            }

            stopwach.Stop();
            Statistics.ElapsedTimeInSeconds = new TimeSpan(stopwach.ElapsedTicks).TotalSeconds;

            return action == null 
                ? new AlgorithmResult(false, null, Statistics) 
                : new AlgorithmResult(true, action, Statistics);
        }

        /// <summary>
        /// Max portion of the minimax algorithm.
        /// </summary>
        /// <param name="state">The current state in gam.</param>
        /// <param name="alpha">The value of the best alternative for MAX along the path to state.</param>
        /// <param name="beta">The value of the best alternative for MIN along the path to state.</param>
        /// <returns>A utility value.</returns>
        private double MaxValue(IState state, double alpha, double beta)
        {
            if (Problem.IsTerminalState(state))
                return Problem.GetUtilityValue(state);

            if (UseTranspositionTable && transpositionTable.ContainsKey(state))
                return transpositionTable[state];

            var v = double.NegativeInfinity;
            var successors = Problem.GetSuccessors(state);
            foreach (var successor in successors)
            {
                Statistics.VisitedStateCount++;

                v = Math.Max(v, MinValue(successor.ResultingState, alpha, beta));
                if (v >= beta)
                    return v;
                alpha = Math.Max(alpha, v);
            }

            if (UseTranspositionTable && !transpositionTable.ContainsKey(state))
                transpositionTable[state] = v;

            return v;
        }

        /// <summary>
        /// Min portion of the minimax algorithm.
        /// </summary>
        /// <param name="state">The current state in gam.</param>
        /// <param name="alpha">The value of the best alternative for MAX along the path to state.</param>
        /// <param name="beta">The value of the best alternative for MIN along the path to state.</param>
        /// <returns>A utility value.</returns>
        private double MinValue(IState state, double alpha, double beta)
        {
            if (Problem.IsTerminalState(state))
                return Problem.GetUtilityValue(state);

            if (UseTranspositionTable && transpositionTable.ContainsKey(state))
                return transpositionTable[state];

            var v = double.PositiveInfinity;
            var successors = Problem.GetSuccessors(state);
            foreach (var successor in successors)
            {
                Statistics.VisitedStateCount++;

                v = Math.Min(v, MaxValue(successor.ResultingState, alpha, beta));
                if (v <= alpha)
                    return v;
                beta = Math.Min(beta, v);
            }

            if (UseTranspositionTable && !transpositionTable.ContainsKey(state))
                transpositionTable[state] = v;

            return v;
        }
    }
}

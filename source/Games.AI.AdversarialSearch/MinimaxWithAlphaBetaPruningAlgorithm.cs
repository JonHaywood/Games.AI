using System;
using System.Collections.Generic;

namespace Games.AI.AdversarialSearch
{
    public class MinimaxWithAlphaBetaPruningAlgorithm : IAlgorithm
    {        
        private readonly Dictionary<IState, double> transpositionTable = new Dictionary<IState, double>();

        private IProblem Problem { get; set; }

        public AlgorithmResult SolveForBestAction(IProblem problem, IState state)
        {
            Problem = problem;
            transpositionTable.Clear();

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

            if (action == null)
                return new AlgorithmResult(false, null, null);

            var stats = new AlgorithmStatistics();
            return new AlgorithmResult(true, action, stats);
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

            var v = double.NegativeInfinity;
            var successors = Problem.GetSuccessors(state);
            foreach (var successor in successors)
            {
                v = Math.Max(v, MinValue(successor.ResultingState, alpha, beta));
                if (v >= beta)
                    return v;
                alpha = Math.Max(alpha, v);
            }
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

            var v = double.PositiveInfinity;
            var successors = Problem.GetSuccessors(state);
            foreach (var successor in successors)
            {
                v = Math.Min(v, MaxValue(successor.ResultingState, alpha, beta));
                if (v <= alpha)
                    return v;
                beta = Math.Min(beta, v);
            }
            return v;
        }
    }
}

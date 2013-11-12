using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OptimizationAlgorithms.GeneticAlgorithm.Models;
using OptimizationAlgorithms.GeneticAlgorithm.Operations;

namespace OptimizationAlgorithms.GeneticAlgorithm.OperationAppliers
{
    // Attempt to parallelize operations duration application where possible
    // Operations better be thread-safe!
    public class ParallelOperationApplier<TCandidate> : IOperationApplier<TCandidate> where TCandidate:class
    {
        private readonly IDecisionMaker _decisionMaker;

        public ParallelOperationApplier()
        {
            _decisionMaker = new DecisionMaker();
        }

        public ParallelOperationApplier(IDecisionMaker decisionMaker)
        {
            _decisionMaker = decisionMaker;
        }

        public List<TCandidate> PerformNaturalSelection(List<EvaluatedCandidate<TCandidate>> evaluatedCandidates, INaturalSelectionOperation<TCandidate> naturalSelectionOperation, int numberOfSurvivors)
        {
            // Cant really be done in parallel
            return naturalSelectionOperation.Select(evaluatedCandidates, numberOfSurvivors);
        }

        public List<TCandidate> PerformMutation(List<TCandidate> candidates, IMutationOperation<TCandidate> mutationOperation, double mutationProbability)
        {
            if (mutationOperation == null) return candidates;

            var children = new ConcurrentBag<TCandidate>{candidates[0]};
            var toMutate = new List<TCandidate>();

            foreach (var c in candidates.Skip(1)) // never mutate first so we dont lose best solution
            {
                if (_decisionMaker.DecideBool(mutationProbability))
                {
                    toMutate.Add(c);
                }
                else
                {
                    children.Add(c);
                }
            }

            Parallel.ForEach(toMutate, candidate => children.Add(mutationOperation.Mutate(candidate)));

            return children.ToList();
        }

        public List<TCandidate> PerformCrossover(List<TCandidate> candidates, ICrossoverOperation<TCandidate> crossoverOperation, int numberToPerform)
        {            
            var children = new ConcurrentBag<TCandidate>();
            int potentialParents = candidates.Count;
            var toCrossover = new List<Tuple<TCandidate, TCandidate>>();

            for (var idx = 0; idx < numberToPerform; idx++)
            {
                // Find two parents to crossover, ensuring they are different
                var parent1Idx = _decisionMaker.DecideIntBetween(0, potentialParents - 1);
                var parent2Idx = parent1Idx;
                while (parent2Idx == parent1Idx)
                {
                    parent2Idx = _decisionMaker.DecideIntBetween(0, potentialParents - 1);
                }        
                toCrossover.Add(new Tuple<TCandidate, TCandidate>(candidates[parent1Idx],candidates[parent2Idx]));
            }
                
            Parallel.ForEach(toCrossover, x => children.Add(crossoverOperation.Crossover(x.Item1, x.Item2)));

            return children.ToList();
        }

        public List<EvaluatedCandidate<TCandidate>> EvaluateCandidates(List<TCandidate> candidates, ICandidateEvaluator<TCandidate> evaluator)
        {
            var evaluatedCandidates = new ConcurrentBag<EvaluatedCandidate<TCandidate>>();

            Parallel.ForEach(candidates, candidate => evaluatedCandidates.Add(evaluator.Evaluate(candidate)));

            return evaluatedCandidates.ToList();
        }
    }
}

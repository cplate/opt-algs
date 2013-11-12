using System.Collections.Generic;
using System.Linq;
using OptimizationAlgorithms.GeneticAlgorithm.Models;
using OptimizationAlgorithms.GeneticAlgorithm.Operations;

namespace OptimizationAlgorithms.GeneticAlgorithm.OperationAppliers
{
    // Serially apply operations (no parallelization)
    public class SerialOperationApplier<TCandidate> : IOperationApplier<TCandidate> where TCandidate:class
    {
        private readonly IDecisionMaker _decisionMaker;

        public SerialOperationApplier()
        {
            _decisionMaker = new DecisionMaker();
        }

        public SerialOperationApplier(IDecisionMaker decisionMaker)
        {
            _decisionMaker = decisionMaker;
        }

        public List<TCandidate> PerformNaturalSelection(List<EvaluatedCandidate<TCandidate>> evaluatedCandidates, INaturalSelectionOperation<TCandidate> naturalSelectionOperation, int numberOfSurvivors)
        {
            return naturalSelectionOperation.Select(evaluatedCandidates, numberOfSurvivors);
        }

        public List<TCandidate> PerformMutation(List<TCandidate> candidates, IMutationOperation<TCandidate> mutationOperation, double mutationProbability)
        {
            var first = candidates.First();
            var rest = candidates.Skip(1).ToList();

            if (mutationOperation == null) return candidates;

            var results = new List<TCandidate>(candidates.Count){first};
            
            foreach (var candidate in rest) // never mutate first so we dont lose best solution
            {
                if (_decisionMaker.DecideBool(mutationProbability))
                {
                    results.Add(mutationOperation.Mutate(candidate));
                }
                else
                {
                    results.Add(candidate);
                }
            }

            return results;
        }

        public List<TCandidate> PerformCrossover(List<TCandidate> candidates, ICrossoverOperation<TCandidate> crossoverOperation, int numberToPerform)
        {
            var children = new List<TCandidate>(numberToPerform);
            int potentialParents = candidates.Count;
            for (int i = 0; i < numberToPerform; i++)
            {
                // Find two parents to crossover, ensuring they are different
                int parent1Idx = _decisionMaker.DecideIntBetween(0, potentialParents - 1);
                int parent2Idx = parent1Idx;
                while (parent2Idx == parent1Idx)
                {
                    parent2Idx = _decisionMaker.DecideIntBetween(0, potentialParents - 1);
                }
                children.Add(crossoverOperation.Crossover(candidates[parent1Idx], candidates[parent2Idx]));
            }
            return children;
        }

        public List<EvaluatedCandidate<TCandidate>> EvaluateCandidates(List<TCandidate> candidates, ICandidateEvaluator<TCandidate> evaluator)
        {
            var evaluatedCandidates = new List<EvaluatedCandidate<TCandidate>>(candidates.Count);
            evaluatedCandidates.AddRange(candidates.Select(evaluator.Evaluate));
            return evaluatedCandidates;
        }
    }
}

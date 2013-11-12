using System;
using System.Collections.Generic;
using OptimizationAlgorithms.GeneticAlgorithm.Models;
using OptimizationAlgorithms.GeneticAlgorithm.Operations;

namespace OptimizationAlgorithms.GeneticAlgorithm.OperationAppliers
{
    // Applies operations defined by the algorithm, allowing for different structure around those operations
    public interface IOperationApplier<TCandidate> where TCandidate:class
    {        
        // Naturally select from the pool of evaluated candidates, returning those that should survive
        List<TCandidate> PerformNaturalSelection(List<EvaluatedCandidate<TCandidate>> evaluatedCandidates, INaturalSelectionOperation<TCandidate> naturalSelectionOperation, int numberOfSurvivors);

        // Perform mutation on a percentage of the specified candiates, returning the list of the new state of the population
        List<TCandidate> PerformMutation(List<TCandidate> candidates, IMutationOperation<TCandidate> mutationOperation, double mutationProbability);

        // Perform a specified number of crossovers on the list of candidates, returning only the generated children
        List<TCandidate> PerformCrossover(List<TCandidate> candidates, ICrossoverOperation<TCandidate> crossoverOperation, int numberToPerform);

        // Evaluate the specified candidates, returning the candidates and their evaluations
        List<EvaluatedCandidate<TCandidate>> EvaluateCandidates(List<TCandidate> candidates, ICandidateEvaluator<TCandidate> evaluator);
    }
}

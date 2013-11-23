using System.Threading;
using System.Threading.Tasks;
using OptimizationAlgorithms.GeneticAlgorithm.Models;
using OptimizationAlgorithms.GeneticAlgorithm.OperationAppliers;

namespace OptimizationAlgorithms.GeneticAlgorithm
{
    public class GeneticAlgorithm<TCandidate> where TCandidate : class
    {
        // Provide callback for when the solution is improved
        public delegate void SolutionImprovedEventHandler(object sender, SolutionImprovedArgs<TCandidate> args);
        public delegate void ProgressUpdateEventHandler(object sender, GeneticAlgorithmProgressUpdateArgs args);
        public event SolutionImprovedEventHandler SolutionImproved;
        public event ProgressUpdateEventHandler ProgressUpdated;

        public GeneticAlgorithmParameters Parameters { get; set; }
        public GeneticAlgorithmOperations<TCandidate> Operations { get; set; }
        public IOperationApplier<TCandidate> Applier { get; set; }
        
        public GeneticAlgorithm(GeneticAlgorithmParameters parameters, GeneticAlgorithmOperations<TCandidate> operations)
        {
            Parameters = parameters;
            Operations = operations;

            // Determine what applier to use based on parameters
            if (parameters.AllowParallelization)
            {
                Applier = new ParallelOperationApplier<TCandidate>();
            }
            else
            {
                Applier = new SerialOperationApplier<TCandidate>();
            }
        }

        public GeneticAlgorithm(GeneticAlgorithmParameters parameters, GeneticAlgorithmOperations<TCandidate> operations, IOperationApplier<TCandidate> applier)
        {
            Parameters = parameters;
            Operations = operations;
            Applier = applier;
        }

        public Task<TCandidate> GetBestCandidateAsync(TCandidate initialCandidate, CancellationToken token)
        {
            return Task<TCandidate>.Factory.StartNew(() => GetBestCandidate(initialCandidate, token), token);
        }

        public TCandidate GetBestCandidate(TCandidate initialCandidate)
        {
            return GetBestCandidate(initialCandidate, CancellationToken.None);
        }

        // Run the algorithm to find the best candidate solution, optionally
        // providing an initial candidate as a starting point
        public TCandidate GetBestCandidate(TCandidate initialCandidate, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return initialCandidate;

            // Kick things off by generating an initial pool of candidates and evaluating them
            var candidates = Operations.CandidateFactory.GeneratePool(Parameters.PopulationSize - 1);
            candidates.Add(initialCandidate ?? Operations.CandidateFactory.GenerateOne());
            var evaluatedCandidates = Applier.EvaluateCandidates(candidates, Operations.CandidateEvaluator);
            var sortedCandidates = Operations.CandidateEvaluator.Sort(evaluatedCandidates);
            EvaluatedCandidate<TCandidate> bestSolution = sortedCandidates[0];
            if (SolutionImproved != null)
            {
                SolutionImproved(this, new SolutionImprovedArgs<TCandidate> { Iteration = 0, Candidate = bestSolution.Candidate, Score = bestSolution.Score });
            }

            // Iterate to improve upon the current solution
            for (int i = 0; i < Parameters.MaxIterations; i++)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (i % 5 == 0)
                {
                    if (ProgressUpdated != null)
                        ProgressUpdated(this, new GeneticAlgorithmProgressUpdateArgs { IterationsBeingPerformed = Parameters.MaxIterations, IterationsProcessed = i });
                }

                // Do natural selection to trim down the population to the "best"
                candidates = Applier.PerformNaturalSelection(evaluatedCandidates, Operations.NaturalSelectionOperation, (int)(Parameters.SurvivalPercentage * evaluatedCandidates.Count));

                // If desired, add some new candidates to introduce some diversity into the population
                if (Parameters.NewBloodPercentage > 0.001)
                {
                    candidates.AddRange(Operations.CandidateFactory.GeneratePool((int)(Parameters.NewBloodPercentage * Parameters.PopulationSize)));
                }

                // Do crossover to generate more candidates based on the "best" remaining
                candidates.AddRange(Applier.PerformCrossover(candidates, Operations.CrossoverOperation, Parameters.PopulationSize - candidates.Count));

                // Do mutation to tweak some candidates to inject some new ones that might not be obtained via crossover
                candidates = Applier.PerformMutation(candidates, Operations.MutationOperation, Parameters.MutationPercentage);

                // Evaluate the population
                evaluatedCandidates = Applier.EvaluateCandidates(candidates, Operations.CandidateEvaluator);
                sortedCandidates = Operations.CandidateEvaluator.Sort(evaluatedCandidates);

                // See how we've done
                if (Operations.CandidateEvaluator.IsBetterThan(sortedCandidates[0],bestSolution))
                {
                    bestSolution = sortedCandidates[0];
                    if (SolutionImproved != null)
                    {
                        SolutionImproved(this, new SolutionImprovedArgs<TCandidate> { Iteration = i+1, Candidate = bestSolution.Candidate, Score = bestSolution.Score });
                    }
                }
            }

            // Return the best candidate
            return bestSolution.Candidate;
        }        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimizationAlgorithms.GeneticAlgorithm.Models;
using OptimizationAlgorithms.GeneticAlgorithm.OperationAppliers;
using OptimizationAlgorithms.GeneticAlgorithm.Operations;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace OptimizationAlgorithms.GeneticAlgorithm.Tests
{
    [TestClass]
    public class GeneticAlgorithmTests
    {
        private GeneticAlgorithm<Candidate> _target;
        private GeneticAlgorithmParameters _parms;
        private GeneticAlgorithmOperations<Candidate> _ops;
        private int _numCallbacksCalled = 0;

        [TestInitialize]
        public void Init()
        {
            _parms = new GeneticAlgorithmParameters();
            _ops = new GeneticAlgorithmOperations<Candidate>();
            _numCallbacksCalled = 0;
        }

        [TestMethod]
        public void Ctor_UsesSerialApplierWhenAllowParallizationIsFalse()
        {
            _parms.AllowParallelization = false;

            _target = new GeneticAlgorithm<Candidate>(_parms, _ops);

            Assert.IsInstanceOfType(_target.Applier, typeof(SerialOperationApplier<Candidate>));
        }

        [TestMethod]
        public void Ctor_UsesParallelApplierWhenAllowParallizationIsTrue()
        {
            _parms.AllowParallelization = true;
            
            _target = new GeneticAlgorithm<Candidate>(_parms, _ops);

            Assert.IsInstanceOfType(_target.Applier, typeof(ParallelOperationApplier<Candidate>));
        }

        [TestMethod]
        public void GetBestCandidate_FullPoolLessOneGeneratedWhenInitialSolutionProvided()
        {
            _parms.MaxIterations = 0; // shut off looping to focus on initial soln generation
            _parms.PopulationSize = 4;

            var candidateFactory = MockRepository.GenerateStub<ICandidateFactory<Candidate>>();
            candidateFactory.Expect(x => x.GeneratePool(3))
                            .Return(new List<Candidate> {new Candidate(), new Candidate(), new Candidate()});
            candidateFactory.Expect(x => x.GenerateOne()).Throw(new Exception("Shouldnt call this"));
            _ops.CandidateFactory = candidateFactory;

            var evaluator = MockRepository.GenerateStub<ICandidateEvaluator<Candidate>>();
            evaluator.Expect(x => x.Evaluate(null)).IgnoreArguments().Return(new EvaluatedCandidate<Candidate>()).Repeat.Times(4);            
            evaluator.Expect(x => x.Sort(null)).IgnoreArguments().Return(new List<EvaluatedCandidate<Candidate>> { new EvaluatedCandidate<Candidate>() });
            _ops.CandidateEvaluator = evaluator;

            _target = new GeneticAlgorithm<Candidate>(_parms, _ops);
            var result = _target.GetBestCandidate(new Candidate());

            candidateFactory.VerifyAllExpectations();
            evaluator.VerifyAllExpectations();
        }

        [TestMethod]
        public void GetBestCandidate_ExtraCandidateGeneratedWhenNoInitialSolutionProvided()
        {
            _parms.MaxIterations = 0; // shut off looping to focus on initial soln generation
            _parms.PopulationSize = 4;

            var candidateFactory = MockRepository.GenerateStub<ICandidateFactory<Candidate>>();
            candidateFactory.Expect(x => x.GeneratePool(3))
                            .Return(new List<Candidate> {new Candidate(), new Candidate(), new Candidate()});
            candidateFactory.Expect(x => x.GenerateOne()).Return(new Candidate());
            _ops.CandidateFactory = candidateFactory;

            var evaluator = MockRepository.GenerateStub<ICandidateEvaluator<Candidate>>();
            evaluator.Expect(x => x.Evaluate(null)).IgnoreArguments().Return(new EvaluatedCandidate<Candidate>()).Repeat.Times(4);
            evaluator.Expect(x => x.Sort(null)).IgnoreArguments().Return(new List<EvaluatedCandidate<Candidate>> { new EvaluatedCandidate<Candidate>() });
            _ops.CandidateEvaluator = evaluator;

            _target = new GeneticAlgorithm<Candidate>(_parms, _ops);
            var result = _target.GetBestCandidate(new Candidate());

            candidateFactory.VerifyAllExpectations();
            evaluator.VerifyAllExpectations();
        }

        [TestMethod]
        public void GetBestCandidate_NoNewBlood_PerformsOperationsProperly()
        {
            // Could, break this out into separate tests, just making sure things are wired up right
            _parms.MaxIterations = 1;
            _parms.PopulationSize = 5;
            _parms.SurvivalPercentage = 0.4;
            _parms.NewBloodPercentage = 0;

            var evaluatedCandidates = new List<EvaluatedCandidate<Candidate>>
                       {
                           new EvaluatedCandidate<Candidate>() { Score = 10, Candidate = new Candidate { Num1 = 10 }}, new EvaluatedCandidate<Candidate>(),
                           new EvaluatedCandidate<Candidate>(), new EvaluatedCandidate<Candidate>(),
                           new EvaluatedCandidate<Candidate>()
                       };
            var naturalSelectionResults = new List<Candidate>
                {
                    new Candidate(), new Candidate()
                };
            var crossoverResults = new List<Candidate>
                {
                    new Candidate(), new Candidate(), new Candidate()
                };
            var candidatesForEvaluation = new List<Candidate>
                {
                    new Candidate(), new Candidate(), new Candidate(), new Candidate(), new Candidate()
                };

            _ops.CandidateEvaluator = MockRepository.GenerateStub<ICandidateEvaluator<Candidate>>();
            _ops.CandidateEvaluator.Expect(x => x.Sort(evaluatedCandidates)).Return(evaluatedCandidates).Repeat.Times(2);
            _ops.CandidateEvaluator.Expect(x => x.IsBetterThan(null, null)).IgnoreArguments().Return(false);
            _ops.CandidateFactory = MockRepository.GenerateStub<ICandidateFactory<Candidate>>();
            _ops.CandidateFactory.Expect(x => x.GeneratePool(4)).Return(candidatesForEvaluation.Take(4).ToList());
            _ops.CrossoverOperation = MockRepository.GenerateStub<ICrossoverOperation<Candidate>>();
            _ops.MutationOperation = MockRepository.GenerateStub<IMutationOperation<Candidate>>();
            _ops.NaturalSelectionOperation = MockRepository.GenerateStub<INaturalSelectionOperation<Candidate>>();
            
            var applier = MockRepository.GenerateStub<IOperationApplier<Candidate>>();
            applier.Expect(x => x.EvaluateCandidates(null, null)).IgnoreArguments().Constraints(
                List.ContainsAll(candidatesForEvaluation), Is.Equal(_ops.CandidateEvaluator)).Return(evaluatedCandidates).Repeat.Times(2);
            applier.Expect(x => x.PerformNaturalSelection(evaluatedCandidates, _ops.NaturalSelectionOperation, 2))
                   .Return(naturalSelectionResults);
            applier.Expect(x => x.PerformCrossover(naturalSelectionResults, _ops.CrossoverOperation, 3)).Return(crossoverResults);
            applier.Expect(x => x.PerformMutation(null, null, 0)).IgnoreArguments().Constraints(
                List.ContainsAll(naturalSelectionResults) && List.ContainsAll(crossoverResults),
                Is.Equal(_ops.MutationOperation), Is.Equal(_parms.MutationPercentage))
                   .Return(candidatesForEvaluation);

            _target = new GeneticAlgorithm<Candidate>(_parms, _ops, applier);
            var result = _target.GetBestCandidate(candidatesForEvaluation.Skip(4).Take(1).First());

            applier.VerifyAllExpectations();
            _ops.CandidateFactory.VerifyAllExpectations();
            Assert.AreEqual(10, result.Num1);
        }

        [TestMethod]
        public void GetBestCandidate_NewBloodSpecified_PerformsOperationsProperly()
        {
            // Could, break this out into separate tests, just making sure things are wired up right
            _parms.MaxIterations = 1;
            _parms.PopulationSize = 5;
            _parms.SurvivalPercentage = 0.4;
            _parms.NewBloodPercentage = 0.2;

            var evaluatedCandidates = new List<EvaluatedCandidate<Candidate>>
                       {
                           new EvaluatedCandidate<Candidate>() { Score = 10, Candidate = new Candidate { Num1 = 10 }}, new EvaluatedCandidate<Candidate>(),
                           new EvaluatedCandidate<Candidate>(), new EvaluatedCandidate<Candidate>(),
                           new EvaluatedCandidate<Candidate>()
                       };
            var naturalSelectionResults = new List<Candidate>
                {
                    new Candidate(), new Candidate()
                };
            var crossoverResults = new List<Candidate>
                {
                    new Candidate(), new Candidate()
                };
            var candidatesForEvaluation = new List<Candidate>
                {
                    new Candidate(), new Candidate(), new Candidate(), new Candidate(), new Candidate()
                };
            var newBlood = new List<Candidate> { new Candidate() };

            _ops.CandidateEvaluator = MockRepository.GenerateStub<ICandidateEvaluator<Candidate>>();
            _ops.CandidateEvaluator.Expect(x => x.Sort(evaluatedCandidates)).Return(evaluatedCandidates).Repeat.Times(2);
            _ops.CandidateEvaluator.Expect(x => x.IsBetterThan(null, null)).IgnoreArguments().Return(false);
            _ops.CandidateFactory = MockRepository.GenerateStub<ICandidateFactory<Candidate>>();
            _ops.CandidateFactory.Expect(x => x.GeneratePool(4)).Return(candidatesForEvaluation.Take(4).ToList());
            _ops.CandidateFactory.Expect(x => x.GeneratePool(1)).Return(newBlood); // new blood
            _ops.CrossoverOperation = MockRepository.GenerateStub<ICrossoverOperation<Candidate>>();
            _ops.MutationOperation = MockRepository.GenerateStub<IMutationOperation<Candidate>>();
            _ops.NaturalSelectionOperation = MockRepository.GenerateStub<INaturalSelectionOperation<Candidate>>();
            
            var applier = MockRepository.GenerateStub<IOperationApplier<Candidate>>();
            applier.Expect(x => x.EvaluateCandidates(null, null)).IgnoreArguments().Constraints(
                List.ContainsAll(candidatesForEvaluation), Is.Equal(_ops.CandidateEvaluator)).Return(evaluatedCandidates).Repeat.Times(2);
            applier.Expect(x => x.PerformNaturalSelection(evaluatedCandidates, _ops.NaturalSelectionOperation, 2))
                   .Return(naturalSelectionResults);
            applier.Expect(x => x.PerformCrossover(null, null, 0)).IgnoreArguments().Constraints(
                List.ContainsAll(naturalSelectionResults) && List.ContainsAll(newBlood), Is.Equal(_ops.CrossoverOperation), Is.Equal(2)).Return(crossoverResults);
            applier.Expect(x => x.PerformMutation(null, null, 0)).IgnoreArguments().Constraints(
                List.ContainsAll(naturalSelectionResults) && List.ContainsAll(newBlood) && List.ContainsAll(crossoverResults),
                Is.Equal(_ops.MutationOperation), Is.Equal(_parms.MutationPercentage))
                   .Return(candidatesForEvaluation);

            _target = new GeneticAlgorithm<Candidate>(_parms, _ops, applier);
            var result = _target.GetBestCandidate(candidatesForEvaluation.Skip(4).Take(1).First());

            applier.VerifyAllExpectations();
            _ops.CandidateFactory.VerifyAllExpectations();
            Assert.AreEqual(10, result.Num1);
        }

        [TestMethod]
        public void GetBestCandidate_SolutionImproved_CallbackPerformed()
        {
            _parms.MaxIterations = 1;
            _parms.PopulationSize = 2;
            _parms.SurvivalPercentage = 0.5;
            _parms.NewBloodPercentage = 0;

            var initialEvaluatedCandidates = new List<EvaluatedCandidate<Candidate>>
                       {
                           new EvaluatedCandidate<Candidate>() { Score = 10, Candidate = new Candidate { Num1 = 10 }}, new EvaluatedCandidate<Candidate>(),                           
                       };
            var iterationEvaluatedCandidates = new List<EvaluatedCandidate<Candidate>>
                       {
                           new EvaluatedCandidate<Candidate>() { Score = 15, Candidate = new Candidate { Num1 = 10 }}, new EvaluatedCandidate<Candidate>(),                           
                       };
            var opResults = new List<Candidate>
                {
                    new Candidate(), new Candidate()
                };

            _ops.CandidateEvaluator = MockRepository.GenerateStub<ICandidateEvaluator<Candidate>>();
            _ops.CandidateEvaluator.Expect(x => x.Sort(initialEvaluatedCandidates)).Return(initialEvaluatedCandidates);
            _ops.CandidateEvaluator.Expect(x => x.Sort(iterationEvaluatedCandidates)).Return(iterationEvaluatedCandidates);
            _ops.CandidateEvaluator.Expect(x => x.IsBetterThan(null, null)).IgnoreArguments().Return(true);
            _ops.CandidateFactory = MockRepository.GenerateStub<ICandidateFactory<Candidate>>();
            _ops.CandidateFactory.Expect(x => x.GeneratePool(1)).Return(opResults.Take(1).ToList());
            _ops.CandidateFactory.Expect(x => x.GenerateOne()).Return(opResults.Skip(1).Take(1).First());
            _ops.CrossoverOperation = MockRepository.GenerateStub<ICrossoverOperation<Candidate>>();
            _ops.MutationOperation = MockRepository.GenerateStub<IMutationOperation<Candidate>>();
            _ops.NaturalSelectionOperation = MockRepository.GenerateStub<INaturalSelectionOperation<Candidate>>();
            
            var applier = MockRepository.GenerateStub<IOperationApplier<Candidate>>();
            applier.Expect(x => x.EvaluateCandidates(null, null)).IgnoreArguments().Return(initialEvaluatedCandidates);
            applier.Expect(x => x.EvaluateCandidates(null, null)).IgnoreArguments().Return(iterationEvaluatedCandidates);
            applier.Expect(x => x.PerformNaturalSelection(null, null, 0)).IgnoreArguments().Return(opResults);
            applier.Expect(x => x.PerformCrossover(null,null, 0)).IgnoreArguments().Return(opResults);
            applier.Expect(x => x.PerformMutation(null, null, 0)).IgnoreArguments().Return(opResults);

            _target = new GeneticAlgorithm<Candidate>(_parms, _ops, applier);
            _target.SolutionImproved += delegate(object sender, SolutionImprovedArgs<Candidate> args) { _numCallbacksCalled++; };
            var result = _target.GetBestCandidate(null);

            applier.VerifyAllExpectations();
            _ops.CandidateFactory.VerifyAllExpectations();
            Assert.AreEqual(2, _numCallbacksCalled);
        }

        [TestMethod]
        public void GetBestCandidateAsync_PerformsOperationsProperly()
        {
            _parms.MaxIterations = 1;
            _parms.PopulationSize = 5;
            _parms.SurvivalPercentage = 0.4;
            _parms.NewBloodPercentage = 0;

            var evaluatedCandidates = new List<EvaluatedCandidate<Candidate>>
                       {
                           new EvaluatedCandidate<Candidate>() { Score = 10, Candidate = new Candidate { Num1 = 10 }}, new EvaluatedCandidate<Candidate>(),
                           new EvaluatedCandidate<Candidate>(), new EvaluatedCandidate<Candidate>(),
                           new EvaluatedCandidate<Candidate>()
                       };
            var naturalSelectionResults = new List<Candidate>
                {
                    new Candidate(), new Candidate()
                };
            var crossoverResults = new List<Candidate>
                {
                    new Candidate(), new Candidate(), new Candidate()
                };
            var candidatesForEvaluation = new List<Candidate>
                {
                    new Candidate(), new Candidate(), new Candidate(), new Candidate(), new Candidate()
                };

            _ops.CandidateEvaluator = MockRepository.GenerateStub<ICandidateEvaluator<Candidate>>();
            _ops.CandidateEvaluator.Expect(x => x.Sort(evaluatedCandidates)).Return(evaluatedCandidates).Repeat.Times(2);
            _ops.CandidateEvaluator.Expect(x => x.IsBetterThan(null, null)).IgnoreArguments().Return(false);
            _ops.CandidateFactory = MockRepository.GenerateStub<ICandidateFactory<Candidate>>();
            _ops.CandidateFactory.Expect(x => x.GeneratePool(4)).Return(candidatesForEvaluation.Take(4).ToList());
            _ops.CrossoverOperation = MockRepository.GenerateStub<ICrossoverOperation<Candidate>>();
            _ops.MutationOperation = MockRepository.GenerateStub<IMutationOperation<Candidate>>();
            _ops.NaturalSelectionOperation = MockRepository.GenerateStub<INaturalSelectionOperation<Candidate>>();
            
            var applier = MockRepository.GenerateStub<IOperationApplier<Candidate>>();
            applier.Expect(x => x.EvaluateCandidates(null, null)).IgnoreArguments().Constraints(
                List.ContainsAll(candidatesForEvaluation), Is.Equal(_ops.CandidateEvaluator)).Return(evaluatedCandidates).Repeat.Times(2);
            applier.Expect(x => x.PerformNaturalSelection(evaluatedCandidates, _ops.NaturalSelectionOperation, 2))
                   .Return(naturalSelectionResults);
            applier.Expect(x => x.PerformCrossover(naturalSelectionResults, _ops.CrossoverOperation, 3)).Return(crossoverResults);
            applier.Expect(x => x.PerformMutation(null, null, 0)).IgnoreArguments().Constraints(
                List.ContainsAll(naturalSelectionResults) && List.ContainsAll(crossoverResults),
                Is.Equal(_ops.MutationOperation), Is.Equal(_parms.MutationPercentage))
                   .Return(candidatesForEvaluation);

            _target = new GeneticAlgorithm<Candidate>(_parms, _ops, applier);
            var cancelSource = new CancellationTokenSource();
            var resultTask = _target.GetBestCandidateAsync(candidatesForEvaluation.Skip(4).Take(1).First(), cancelSource.Token);
            resultTask.Wait();

            applier.VerifyAllExpectations();
            _ops.CandidateFactory.VerifyAllExpectations();
            Assert.AreEqual(10, resultTask.Result.Num1);
        }

    }
}

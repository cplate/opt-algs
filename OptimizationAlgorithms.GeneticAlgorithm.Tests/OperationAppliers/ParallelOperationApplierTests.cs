using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimizationAlgorithms.GeneticAlgorithm.Models;
using OptimizationAlgorithms.GeneticAlgorithm.OperationAppliers;
using OptimizationAlgorithms.GeneticAlgorithm.Operations;
using Rhino.Mocks;

namespace OptimizationAlgorithms.GeneticAlgorithm.Tests.OperationAppliers
{
    // Not really testing the parallel aspects of the implementation here...just making sure 
    // the methods execute without error
    [TestClass]
    public class ParallelOperationApplierTests
    {
        private ParallelOperationApplier<Candidate> _target;
        private IDecisionMaker _decisionMaker;
        private List<Candidate> _candidates;

        [TestInitialize]
        public void Init()
        {
            _decisionMaker = MockRepository.GenerateStub<IDecisionMaker>();
            _target = new ParallelOperationApplier<Candidate>(_decisionMaker);
            _candidates = new List<Candidate>
                {
                    new Candidate { Num1 = 1, Num2 = 2},
                    new Candidate { Num1 = 2, Num2 = 3},
                    new Candidate { Num1 = 3, Num2 = 4},
                    new Candidate { Num1 = 4, Num2 = 5},
                    new Candidate { Num1 = 5, Num2 = 6},
                };           
        }

        [TestMethod]
        public void PerformNaturalSelection_AppliesOperationProperly()
        {
            var survivors = 3;
            var evaluatedCandidates = _candidates.Select(x => new EvaluatedCandidate<Candidate> { Candidate = x, Score = x.Num1 + 1 })
                .OrderByDescending(x => x.Score).ToList();
            var op = MockRepository.GenerateStub<INaturalSelectionOperation<Candidate>>();
            op.Expect(x => x.Select(evaluatedCandidates, survivors)).Return(_candidates.Take(3).ToList());

            var result = _target.PerformNaturalSelection(evaluatedCandidates, op, survivors);

            Assert.AreEqual(3, result.Count());
            op.VerifyAllExpectations();
        }

        [TestMethod]
        public void PerformCrossover_AppliesOperationProperly()
        {
            var expectedResult1 = new Candidate();
            var expectedResult2 = new Candidate();
            var op = MockRepository.GenerateStub<ICrossoverOperation<Candidate>>();
            op.Expect(x => x.Crossover(null, null)).IgnoreArguments().Return(expectedResult1).Repeat.Times(1);
            op.Expect(x => x.Crossover(null, null)).IgnoreArguments().Return(expectedResult2).Repeat.Times(1);
            _decisionMaker.Expect(x => x.DecideIntBetween(0, 4)).Return(2).Repeat.Times(1);
            _decisionMaker.Expect(x => x.DecideIntBetween(0, 4)).Return(4).Repeat.Times(1);
            _decisionMaker.Expect(x => x.DecideIntBetween(0, 4)).Return(1).Repeat.Times(1);
            _decisionMaker.Expect(x => x.DecideIntBetween(0, 4)).Return(3).Repeat.Times(1);

            var result = _target.PerformCrossover(_candidates, op, 2).ToList();

            op.VerifyAllExpectations();
            _decisionMaker.VerifyAllExpectations();
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(expectedResult1));
            Assert.IsTrue(result.Contains(expectedResult2));
        }

        [TestMethod]
        public void PerformMutation_AppliesOperationProperlyWhenNeverMutating()
        {
            var op = MockRepository.GenerateStub<IMutationOperation<Candidate>>();
            op.Expect(x => x.Mutate(null)).IgnoreArguments().Return(new Candidate { Num1 = 99 }).Throw(new Exception("Shouldnt call this"));
            _decisionMaker.Expect(x => x.DecideBool(0)).Return(false).Repeat.Times(4);

            var result = _target.PerformMutation(_candidates, op, 0.0).ToList();

            op.VerifyAllExpectations();
            _decisionMaker.VerifyAllExpectations();
            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.All(x => x.Num1 != 99));
        }

        [TestMethod]
        public void PerformMutation_AppliesOperationProperlyWhenAlwaysMutating()
        {
            var op = MockRepository.GenerateStub<IMutationOperation<Candidate>>();
            op.Expect(x => x.Mutate(null)).IgnoreArguments().Return(new Candidate { Num1 = 99 }).Repeat.Times(4);
            _decisionMaker.Expect(x => x.DecideBool(1)).Return(true).Repeat.Times(4);

            var result = _target.PerformMutation(_candidates, op, 1.0).ToList();

            op.VerifyAllExpectations();
            _decisionMaker.VerifyAllExpectations();            
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(4, result.Count(x => x.Num1 == 99));
        }

        [TestMethod]
        public void EvaluateCandidates_AppliesOperationProperly()
        {
            var op = MockRepository.GenerateStub<ICandidateEvaluator<Candidate>>();
            op.Expect(x => x.Evaluate(_candidates.ElementAt(0))).Return(new EvaluatedCandidate<Candidate> { Candidate = _candidates.ElementAt(0), Score = _candidates.ElementAt(0).Num1*10 });
            op.Expect(x => x.Evaluate(_candidates.ElementAt(1))).Return(new EvaluatedCandidate<Candidate> { Candidate = _candidates.ElementAt(1), Score = _candidates.ElementAt(1).Num1*10 });
            op.Expect(x => x.Evaluate(_candidates.ElementAt(2))).Return(new EvaluatedCandidate<Candidate> { Candidate = _candidates.ElementAt(2), Score = _candidates.ElementAt(2).Num1*10 });
            op.Expect(x => x.Evaluate(_candidates.ElementAt(3))).Return(new EvaluatedCandidate<Candidate> { Candidate = _candidates.ElementAt(3), Score = _candidates.ElementAt(3).Num1*10 });
            op.Expect(x => x.Evaluate(_candidates.ElementAt(4))).Return(new EvaluatedCandidate<Candidate> { Candidate = _candidates.ElementAt(4), Score = _candidates.ElementAt(4).Num1*10 });

            var result = _target.EvaluateCandidates(_candidates, op).ToList();

            Assert.AreEqual(5, result.Count);
            op.VerifyAllExpectations();
        }
    }
}

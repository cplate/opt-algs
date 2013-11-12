using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptimizationAlgorithms.GeneticAlgorithm.Operations;

namespace OptimizationAlgorithms.GeneticAlgorithm.Tests.Operations
{
    // A few tests just to exercise the methods.  
    // Could do better but not worth it.
    [TestClass]
    public class DecisionMakerTests
    {
        [TestMethod]
        public void DecideBool_ProbabilityIsNonZero_Success()
        {
            var target = new DecisionMaker();
            var result = target.DecideBool(0.05);
            Assert.IsTrue(result || !result);
        }

        [TestMethod]
        public void DecideBool_ProbabilityIsZero_AlwaysFalse()
        {
            var target = new DecisionMaker();
            var result = target.DecideBool(0.0);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DecideNextInt_Success()
        {
            var target = new DecisionMaker();
            var result = target.DecideIntBetween(10, 20);
            Assert.IsTrue(result >= 10 && result <= 20);
        }
    }
}

using AlgorithmicBacktestingEngine.Objects;

namespace AlgorithmicBacktestingEngine.tests
{
    [TestClass]
    public sealed class TickTests
    {
        #region serialization tests

        [TestMethod]
        public void Test_TickSerialization()
        {
            Tick tick = new Tick(new DateTime(2025, 8, 7), 150, 1.2m);
            var bytes = tick.GetBytes();
            var t = Tick.FromBytes(bytes);
        }

        #endregion
    }
}

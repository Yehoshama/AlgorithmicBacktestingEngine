using AlgorithmicBacktestingEngine.Objects;

namespace AlgorithmicBacktestingEngine.tests
{
    internal static class Utility
    {
        internal static Random random = new Random();
        internal static decimal RandomDecimal(Random random)
        {
            return (decimal)(random.NextDouble() * (double)(1000));
        }
        internal static Tick GenerateRandomTick(Random random, decimal minPriceDiff = 0.01m)
        {
            long dateTime = (long)(random.NextDouble() * DateTime.MaxValue.Ticks);
            decimal price = RoundDownToNearest(RandomDecimal(random), minPriceDiff);
            decimal volume = RoundDownToNearest(RandomDecimal(random), minPriceDiff);

            return new Tick(new DateTime(dateTime), price, volume);
        }
        internal static IEnumerable<Tick> GenerateRandomTicks(Random random, int count = 100, decimal minPriceDiff = 0.01m)
        {
            for (int i = 0; i < count; i++)
            {
                Tick t = GenerateRandomTick(random, minPriceDiff);
                yield return t;
            }
        }
        private static decimal RoundDownToNearest(decimal val, decimal margin)
        {
            var times = (int)(val / margin);
            if(times <= 0)
            {
                times = 1;
            }
            return times * margin;
        }
    }
    [TestClass]
    public sealed class TickTests
    {

        #region serialization tests
        /// <summary>
        /// tests correct serialization length
        /// </summary>
        [TestMethod]
        public void Test_TickSerialization()
        {
            Tick tick = Utility.GenerateRandomTick(Utility.random);
            var bytes = tick.GetBytes();
            Assert.IsTrue(bytes.Length == Tick.SerialisationLength);
        }
        /// <summary>
        /// tests correct serialization and desialization
        /// </summary>
        [TestMethod]
        public void Test_SerializeDeserialize()
        {
            Tick tick = Utility.GenerateRandomTick(Utility.random);
            var bytes = tick.GetBytes();
            var clone = Tick.FromBytes(bytes);
            Assert.AreEqual(tick, clone);
        }
        /// <summary>
        /// tests handeling incorrect input
        /// </summary>
        [TestMethod]
        public void Test_WrongInput()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Tick tick = new Tick(DateTime.Now, -100, 100);
            }, "price cannot be <= 0");
            Assert.Throws<ArgumentException>(() =>
            {
                Tick tick = new Tick(DateTime.Now, 0, 100);
            }, "price cannot be <= 0");
            Assert.Throws<ArgumentException>(() =>
            {
                Tick tick = new Tick(DateTime.Now, 100, -100);
            }, "volume cannot be <= 0");
            Assert.Throws<ArgumentException>(() =>
            {
                Tick tick = new Tick(DateTime.Now, 100, 0);
            }, "volume cannot be <= 0");
        }

        #endregion

    }

    [TestClass]
    public sealed class TickSequenceTests
    {
        /// <summary>
        /// tests serialization and deserialization of tick sequence.
        /// checks for metadata corectness, tick collection corectness tick by tick and compares metadata length to expected.
        /// </summary>
        [TestMethod]
        public void Test_SerializationDeserialization()
        {
            TicksSequence sequence = new TicksSequence(Utility.GenerateRandomTicks(Utility.random, 100, 0.1m), 0.1m);
            var metadataBytes = sequence.WriteMetadata();
            Assert.AreEqual(metadataBytes.Length, TicksSequence.MetadataSerialisationLength, "metadata length doesnt match expected serialization length");
            var bytes = sequence.GetBytes();
            var clone = TicksSequence.FromBytes(bytes);
            bool equals = true;
            Assert.AreEqual(sequence.Count, clone.Count, "length of sequence is incorrect");
            for (int i = 0; i < sequence.Ticks.Count; i++)
            {
                Tick tick = sequence.Ticks[i];
                Tick cloneTick = clone.Ticks[i];

                if (!tick.Equals(cloneTick))
                {
                    equals = false;
                }
            }
            Assert.IsTrue(equals, "sequence doesnt match");
            Assert.AreEqual(sequence.First, clone.First, "first doesnt match in metadata");
            Assert.AreEqual(sequence.TimeSpan, clone.TimeSpan, "timespan doesnt match in metadata");
            Assert.AreEqual(sequence.Beginning, clone.Beginning, "beginning doesnt match in metadata");
            Assert.AreEqual(sequence.Ending, clone.Ending, "ending doesnt match in metadata");
            Assert.AreEqual(sequence.MinPriceChange, clone.MinPriceChange, "minPriceChange doesnt match in metadata");
        }
        /// <summary>
        /// tests for incorrect input management
        /// </summary>
        [TestMethod]
        public void Test_TickSequenceIncorrectInput()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                TicksSequence sequence = new TicksSequence(Utility.GenerateRandomTicks(Utility.random, 100, 0.1m), -0.1m);
            }, "minPriceChange should be >= 0");

            Assert.Throws<ArgumentException>(() =>
            {
                TicksSequence sequence = new TicksSequence(Utility.GenerateRandomTicks(Utility.random, 100, 0.1m), 0);
            }, "minPriceChange should be >= 0");
        }
    }
}

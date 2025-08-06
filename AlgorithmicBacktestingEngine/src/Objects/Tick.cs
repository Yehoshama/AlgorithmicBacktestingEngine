using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmicBacktestingEngine.Objects
{
    /// <summary>
    /// represents a single price tick
    /// </summary>
    /// <param name="Time">time of tick occurance</param>
    /// <param name="Price">the price</param>
    /// <param name="Volume">the amount of the trade as base asset</param>
    public record Tick(
        DateTime Time,
        decimal Price,
        decimal Volume
        )
    {
        public const int SerialisationLength = 8 + 16 + 16;
        /// <summary>
        /// serialises the Tick Record
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer))
            {
                writer.Write(Time.Ticks);
                writer.Write(Price);
                writer.Write(Volume);

                return buffer.ToArray();
            }
        }

        /// <summary>
        /// DeSerialises the Tick Record
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Tick FromBytes(byte[] bytes)
        {
            using (var buffer = new MemoryStream(bytes))
            using (var reader = new BinaryReader(buffer))
            {
                var timeDiff = new DateTime(reader.ReadInt64());
                var price = reader.ReadDecimal();
                var vol = reader.ReadDecimal();

                return new Tick(timeDiff, price, vol);
            }
        }

    }
    /// <summary>
    /// Represents a collection of ticks with metadata
    /// </summary>
    /// <param name="ticks">the Tick collection</param>
    /// <param name="minimumPriceDiff">minimum alowed price change</param>
    public class TicksSequence(IEnumerable<Tick> ticks, decimal minimumPriceDiff)
    {
        public IReadOnlyList<Tick> Ticks { get; } = ticks.OrderBy(x => x.Time).ToList();
        public decimal MinPriceChange { get; } = minimumPriceDiff;
        public int Count => Ticks.Count;
        public DateTime Beginning => Ticks[0].Time;
        public DateTime Ending => Ticks.Last().Time;
        public TimeSpan TimeSpan => Ending - Beginning;
        public Tick First => Ticks[0];
        public const int MetadataSerialisationLength = Tick.SerialisationLength + 8 + 16 + 4; 
        /// <summary>
        /// serialises the TickSequence
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            var prev = Ticks.First();
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer))
            {
                writer.Write(WriteMetadata());
                bool skip = false;
                foreach (var t in Ticks)
                {
                    if (!skip)
                    {
                        skip = true;
                        continue;
                    }

                    var diff = TickDifference(prev, t);
                    writer.Write(diff.GetBytes());
                    prev = t;
                }

                return buffer.ToArray();
            }
        }
        /// <summary>
        /// Deserialises the TickSequence
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static TicksSequence FromBytes(byte[] bytes)
        {
            using(var buffer = new MemoryStream(bytes))
            using(var reader =  new BinaryReader(buffer))
            {
                var metadata = ReadMetadata(reader.ReadBytes(MetadataSerialisationLength));
                var minPriceChange = metadata.MinimumPriceChange;
                Tick prev = metadata.FirstTick;
                List<Tick> ticks = new List<Tick>() { prev };
                for (int i = 0; i < metadata.TicksCount; i++)
                {
                    var data = reader.ReadBytes(TickDiff.SerializationLength);
                    var tickDiff = TickDiff.FromBytes(data);
                    var tick = GetTick(prev, tickDiff, minPriceChange);
                    ticks.Add(tick);
                    prev = tick;
                }

                TicksSequence ticksSequence = new TicksSequence(ticks, minPriceChange);
                return ticksSequence;
            }
        }
        /// <summary>
        /// calculates the Tick from TickDiff and previous Tick
        /// </summary>
        /// <param name="prev">previous Tick</param>
        /// <param name="diff">TickDiff</param>
        /// <param name="minPriceChange">the minimum alowed price difference value</param>
        /// <returns></returns>
        private static Tick GetTick(Tick prev, TickDiff diff, decimal minPriceChange)
        {
            DateTime time = prev.Time + new TimeSpan(diff.TimeDifferenceTicks);
            decimal price = prev.Price + diff.PriceDifference * minPriceChange;
            decimal volume = diff.CurrentVolume;

            return new Tick(time, price, volume);
        }
        /// <summary>
        /// calculates the TickDiff from two Ticks
        /// </summary>
        /// <param name="prev">previous Tick</param>
        /// <param name="Current">current Tick</param>
        /// <returns></returns>
        private TickDiff TickDifference(Tick prev, Tick Current)
        {
            var timeDiff = (Current.Time - prev.Time).Ticks;
            var diff = (short)((Current.Price - prev.Price) / MinPriceChange);
            var currentVolume = Current.Volume;

            return new TickDiff(timeDiff, diff, currentVolume);
        }
        /// <summary>
        /// deserialises the TickSequence metadata
        /// </summary>
        /// <param name="bytes">metadata byte array</param>
        /// <returns></returns>
        internal static (Tick FirstTick, DateTime LastTime, decimal MinimumPriceChange, int TicksCount) ReadMetadata(byte[] bytes)
        {
            using (var buffer = new MemoryStream(bytes))
            using (var reader = new BinaryReader(buffer))
            {
                Tick first = Tick.FromBytes(reader.ReadBytes(Tick.SerialisationLength));
                DateTime ending = new DateTime(reader.ReadInt64());
                decimal minPriceChange = reader.ReadDecimal();
                int count = reader.ReadInt32();

                return (first, ending, minPriceChange, count);
            }
        }
        /// <summary>
        /// serialises TickSequence metadata
        /// </summary>
        /// <returns></returns>
        internal byte[] WriteMetadata()
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer))
            {
                writer.Write(First.GetBytes());
                writer.Write(Ending.Ticks);
                writer.Write(MinPriceChange);
                writer.Write(Count);

                return buffer.ToArray();
            }
        }
    }
    /// <summary>
    /// represents the difference between two Ticks
    /// </summary>
    /// <param name="TimeDifferenceTicks">the time difference in ticks</param>
    /// <param name="PriceDifference">price difference as amount of "minimum alowed change" units</param>
    /// <param name="CurrentVolume">the exact volume of the latest tick</param>
    internal record TickDiff(
        long TimeDifferenceTicks,
        short PriceDifference,
        decimal CurrentVolume
        )
    {
        internal const int SerializationLength = 8 + 2 + 16;
        /// <summary>
        /// serialises the TickDiff
        /// </summary>
        /// <returns></returns>
        internal byte[] GetBytes()
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer))
            {
                writer.Write(TimeDifferenceTicks);
                writer.Write(PriceDifference);
                writer.Write(CurrentVolume);

                return buffer.ToArray();
            }
        }
        /// <summary>
        /// deserialises the TickDiff
        /// </summary>
        /// <param name="bytes">TickDiff byte array</param>
        /// <returns></returns>
        internal static TickDiff FromBytes(byte[] bytes)
        {
            using(var buffer = new MemoryStream(bytes))
            using(var reader =  new BinaryReader(buffer))
            {
                var timeDiff = reader.ReadInt64();
                var priceDiff = reader.ReadInt16();
                var currVol = reader.ReadDecimal();

                return new TickDiff(timeDiff, priceDiff, currVol);
            }
        }
    }
}

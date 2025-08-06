using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmicBacktestingEngine.Objects
{
    /// <summary>
    /// Represents a financial candle
    /// </summary>
    /// <param name="StartTime">start time of candle formation</param>
    /// <param name="Open">open price</param>
    /// <param name="Close">close price</param>
    /// <param name="High">high price</param>
    /// <param name="Low">low price</param>
    /// <param name="Volume">total volume during candle formation</param>
    public record Candle(
        DateTime StartTime,
        decimal Open,
        decimal Close,
        decimal High,
        decimal Low,
        decimal Volume
        )
    {
        public const int SerialisationLength = 8 + 16 * 5;
        /// <summary>
        /// Serialises the Candle
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer))
            {
                writer.Write(StartTime.Ticks);
                writer.Write(Open);
                writer.Write(Close);
                writer.Write(High);
                writer.Write(Low);
                writer.Write(Volume);

                return buffer.ToArray();
            }
        }
        /// <summary>
        /// deserialises The Candle
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Candle FromBytes(byte[] bytes)
        {
            //todo check for errors
            using (var buffer = new MemoryStream(bytes))
            using (var reader = new BinaryReader(buffer))
            {
                var startTime = new DateTime(reader.ReadInt64());
                var open = reader.ReadDecimal();
                var close = reader.ReadDecimal();
                var high = reader.ReadDecimal();
                var low = reader.ReadDecimal();
                var vol = reader.ReadDecimal();

                return new Candle(startTime, open, close, high, low, vol);
            }
        }
    }
    /// <summary>
    /// represents the difference between two Candles
    /// </summary>
    /// <param name="TimeDifferenceTicks">the time difference in ticks</param>
    /// <param name="OpenDifference">open difference as amount of "minimum alowed change" units</param>
    /// <param name="CloseDifference">close difference as amount of "minimum alowed change" units</param>
    /// <param name="HighDifference">high difference as amount of "minimum alowed change" units</param>
    /// <param name="LowDifference">low difference as amount of "minimum alowed change" units</param>
    /// <param name="VolumeCurrent">the exact volume of the latest Candle</param>
    internal record CandleDiff(
        long TimeDifferenceTicks,
        short OpenDifference,
        short CloseDifference,
        short HighDifference,
        short LowDifference,
        decimal VolumeCurrent
        )
    {
        internal const int SerializationLength = 8 + 2 * 4 + 16;
        /// <summary>
        /// serialises the CandleDiff
        /// </summary>
        /// <returns></returns>
        internal byte[] GetBytes()
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer))
            {
                writer.Write(TimeDifferenceTicks);
                writer.Write(OpenDifference);
                writer.Write(CloseDifference);
                writer.Write(HighDifference);
                writer.Write(LowDifference);
                writer.Write(VolumeCurrent);

                return buffer.ToArray();
            }
        }
        /// <summary>
        /// deserialises the CandleDiff
        /// </summary>
        /// <param name="bytes">CandleDiff byte array</param>
        /// <returns></returns>
        internal static CandleDiff FromBytes(byte[] bytes)
        {
            using (var buffer = new MemoryStream(bytes))
            using (var reader = new BinaryReader(buffer))
            {
                var timeDiff = reader.ReadInt64();
                var openDiff = reader.ReadInt16();
                var closeDiff = reader.ReadInt16();
                var highDiff = reader.ReadInt16();
                var lowDiff = reader.ReadInt16();
                var currVol = reader.ReadDecimal();

                return new CandleDiff(timeDiff, openDiff, closeDiff, highDiff, lowDiff, currVol);
            }
        }
    }
    /// <summary>
    /// Represents a collection of candles with metadata
    /// </summary>
    /// <param name="candles">the Candle collection</param>
    /// <param name="minimumPriceDiff">minimum alowed price change</param>
    public class CandleSequence(IEnumerable<Candle> candles, decimal minimumPriceDiff)
    {
        public IReadOnlyList<Candle> candles { get; } = candles.OrderBy(x => x.StartTime).ToList();
        public decimal MinPriceChange { get; } = minimumPriceDiff;
        public int Count => candles.Count;
        public DateTime FirstCandleTime => candles[0].StartTime;
        public DateTime LastCandleTime => candles.Last().StartTime;
        public TimeSpan TimeSpan => LastCandleTime - FirstCandleTime;
        public TimeSpan Interval => candles[1].StartTime - First.StartTime;
        public Candle First => candles[0];
        public const int MetadataSerialisationLength = Candle.SerialisationLength + 8 + 16 + 8 + 4;
        /// <summary>
        /// serialises the CandleSequence
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            var prev = candles.First();
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer))
            {
                writer.Write(WriteMetadata());
                bool skip = false;
                foreach (var c in candles)
                {
                    if (!skip)
                    {
                        skip = true;
                        continue;
                    }

                    var diff = CandleDifference(prev, c);
                    writer.Write(diff.GetBytes());
                    prev = c;
                }

                return buffer.ToArray();
            }
        }
        /// <summary>
        /// deserialises the CandleSequence
        /// </summary>
        /// <param name="bytes">CandleSequence byte array</param>
        /// <returns></returns>
        public static CandleSequence FromBytes(byte[] bytes)
        {
            using (var buffer = new MemoryStream(bytes))
            using (var reader = new BinaryReader(buffer))
            {
                var metadata = ReadMetadata(reader.ReadBytes(MetadataSerialisationLength));
                var minPriceChange = metadata.MinimumPriceChange;
                Candle prev = metadata.FirstCandle;
                List<Candle> candles = new List<Candle>() { prev };
                for (int i = 0; i < metadata.TicksCount; i++)
                {
                    var data = reader.ReadBytes(CandleDiff.SerializationLength);
                    var candleDiff = CandleDiff.FromBytes(data);
                    var candle = GetCandle(prev, candleDiff, minPriceChange);
                    candles.Add(candle);
                    prev = candle;
                }

                CandleSequence candleSequence = new CandleSequence(candles, minPriceChange);
                return candleSequence;
            }
        }
        /// <summary>
        /// calculates a candle from Candle and CandleDiff
        /// </summary>
        /// <param name="prev">previous Candle</param>
        /// <param name="diff">current CandleDiff</param>
        /// <param name="minPriceChange">minimum alowed price change</param>
        /// <returns></returns>
        private static Candle GetCandle(Candle prev, CandleDiff diff, decimal minPriceChange)
        {
            DateTime time = prev.StartTime + new TimeSpan(diff.TimeDifferenceTicks);//todo change to use interval for more efficient memory management - e48dbae6-8621-415c-9d98-c0fc5ee4c700
            decimal open = prev.Open + diff.OpenDifference * minPriceChange;
            decimal close = prev.Close + diff.CloseDifference * minPriceChange;
            decimal high = prev.High + diff.HighDifference * minPriceChange;
            decimal low = prev.Low + diff.LowDifference * minPriceChange;
            decimal volume = diff.VolumeCurrent;

            return new Candle(time, open, close, high, low, volume);
        }
        /// <summary>
        /// calculates CandleDiff from two Candles
        /// </summary>
        /// <param name="prev">previous Candle</param>
        /// <param name="Current">current Candle</param>
        /// <returns></returns>
        private CandleDiff CandleDifference(Candle prev, Candle Current)
        {
            var timeDiff = (Current.StartTime - prev.StartTime).Ticks;
            var openDiff = (short)((Current.Open - prev.Open) / MinPriceChange);
            var closeDiff = (short)((Current.Close - prev.Close) / MinPriceChange);
            var highDiff = (short)((Current.High - prev.High) / MinPriceChange);
            var lowDiff = (short)((Current.Low - prev.Low) / MinPriceChange);
            var currentVolume = Current.Volume;

            return new CandleDiff(timeDiff, openDiff, closeDiff, highDiff, lowDiff, currentVolume);
        }
        /// <summary>
        /// deserialises CandleSequence metadata from bytes
        /// </summary>
        /// <param name="bytes">metadata byte array</param>
        /// <returns></returns>
        internal static (Candle FirstCandle, DateTime LastTime, decimal MinimumPriceChange, TimeSpan Interval, int TicksCount) ReadMetadata(byte[] bytes)
        {
            using (var buffer = new MemoryStream(bytes))
            using (var reader = new BinaryReader(buffer))
            {
                Candle first = Candle.FromBytes(reader.ReadBytes(Candle.SerialisationLength));
                DateTime ending = new DateTime(reader.ReadInt64());
                decimal minPriceChange = reader.ReadDecimal();
                TimeSpan interval = new TimeSpan(reader.ReadInt64());
                int count = reader.ReadInt32();

                return (first, ending, minPriceChange, interval, count);
            }
        }
        /// <summary>
        /// serialises CandleSequence metadata
        /// </summary>
        /// <returns></returns>
        internal byte[] WriteMetadata()
        {
            using (var buffer = new MemoryStream())
            using (var writer = new BinaryWriter(buffer))
            {
                writer.Write(First.GetBytes());
                writer.Write(LastCandleTime.Ticks);
                writer.Write(MinPriceChange);
                writer.Write(Interval.Ticks);
                writer.Write(Count);

                return buffer.ToArray();
            }
        }
    }
}

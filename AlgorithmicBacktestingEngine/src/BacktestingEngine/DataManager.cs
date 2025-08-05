using AlgorithmicBacktestingEngine.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmicBacktestingEngine.src.BacktestingEngine
{
    public static class DataManager
    {
        public static List<Candle> ToCandles(ReadOnlySpan<byte> bytes)
        {

        }
        public static List<Tick> ToTicks(ReadOnlySpan<byte> bytes)
        {

        }

        public static bool SaveCandles(IEnumerable<Candle> candles, string fileName)
        {

        }

        public static bool SaveTicks(IEnumerable<Tick> ticks, string fileName)
        {

        }

        public static List<Candle> LoadCandles(string fileName)
        {

        }

        public static List<Tick> LoadTicks(string fileName)
        {

        }
    }
}

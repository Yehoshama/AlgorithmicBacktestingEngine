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
        );
}

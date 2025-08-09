using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmicBacktestingEngineBridge.Objects
{
    /// <summary>
    /// Represents a price update at a specific point in time.
    /// </summary>
    /// <param name="DateTime">The timestamp of the price update.</param>
    /// <param name="Price">The price value at the given time.</param>
    /// <param name="Volume">The trading volume associated with the price update.</param>
    public record PriceUpdate(DateTime DateTime, decimal Price, decimal? Volume);

    /// <summary>
    /// Represents a candlestick update containing OHLC data and volume.
    /// </summary>
    /// <param name="DateTime">The timestamp of the candle.</param>
    /// <param name="Open">The opening price of the candle.</param>
    /// <param name="High">The highest price during the candle period.</param>
    /// <param name="Low">The lowest price during the candle period.</param>
    /// <param name="Close">The closing price of the candle.</param>
    /// <param name="Volume">The trading volume during the candle period.</param>
    public record CandleUpdate(DateTime DateTime, decimal Open, decimal High, decimal Low, decimal Close, decimal Volume);

}

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
        );
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmicBacktestingEngineBridge.Objects
{
    /// <summary>
    /// Specifies the type of order being placed.
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// A market order executes immediately at the current market price.
        /// </summary>
        Market,

        /// <summary>
        /// A limit order executes only at a specified price or better.
        /// </summary>
        Limit,
    }

    /// <summary>
    /// Represents the current status of an order.
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// The order has been created but not yet processed.
        /// </summary>
        New,

        /// <summary>
        /// The order is currently active and open.
        /// </summary>
        Open,

        /// <summary>
        /// The order has been fully executed.
        /// </summary>
        Filled,

        /// <summary>
        /// The order is awaiting execution or confirmation.
        /// </summary>
        Pending,

        /// <summary>
        /// The order has been canceled before execution.
        /// </summary>
        Canceled,

        /// <summary>
        /// The order was rejected and will not be executed.
        /// </summary>
        Rejected,
    }

    /// <summary>
    /// Indicates the direction of the order.
    /// </summary>
    public enum OrderSide
    {
        /// <summary>
        /// A buy order.
        /// </summary>
        Buy,

        /// <summary>
        /// A sell order.
        /// </summary>
        Sell,
    }

    /// <summary>
    /// Represents a trading order within the algorithmic backtesting engine.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// The amount of the asset being traded.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The price at which the order is placed.
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// A unique identifier for the order.
        /// </summary>
        public string? OrderId { get; set; }
        /// <summary>
        /// The timestamp when the order was created.
        /// </summary>
        public DateTime? CreationTime { get; set; }
        /// <summary>
        /// The timestamp when the order was opened.
        /// </summary>
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// The timestamp when the order was closed.
        /// </summary>
        public DateTime CloseTime { get; set; }

        /// <summary>
        /// The profit or loss resulting from the order.
        /// </summary>
        public decimal PnL { get; set; }

        /// <summary>
        /// The price at which the order was entered.
        /// </summary>
        public decimal EntryPrice { get; set; }

        /// <summary>
        /// The price at which the order was exited.
        /// </summary>
        public decimal ExitPrice { get; set; }

        /// <summary>
        /// The type of the order (e.g., Market or Limit).
        /// </summary>
        public OrderType Type { get; set; }

        /// <summary>
        /// The current status of the order.
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// The side of the order (Buy or Sell).
        /// </summary>
        public OrderSide Side { get; set; }

        /// <summary>
        /// The commission fee associated with the order.
        /// </summary>
        public decimal Commision { get; set; }
    }

}

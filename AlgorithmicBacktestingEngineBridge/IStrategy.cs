using AlgorithmicBacktestingEngineBridge.Objects;

namespace AlgorithmicBacktestingEngineBridge
{
    /// <summary>
    /// Delegate for placing an order.
    /// </summary>
    /// <param name="order">The order to be placed.</param>
    /// <returns>The unique identifier of the placed order.</returns>
    public delegate string SendOrderDelegate(Order order);

    /// <summary>
    /// Delegate for retrieving the current system time.
    /// </summary>
    /// <returns>The current <see cref="DateTime"/>.</returns>
    public delegate DateTime GetCurrentTimeDelegate();

    /// <summary>
    /// Delegate for closing an existing order.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order to close.</param>
    /// <returns><c>true</c> if the order was successfully closed; otherwise, <c>false</c>.</returns>
    public delegate bool CloseOrderDelegate(string orderId);

    /// <summary>
    /// Delegate for retrieving all current orders.
    /// </summary>
    /// <returns>An enumerable collection of <see cref="Order"/> objects.</returns>
    public delegate IEnumerable<Order> GetOrdersDelegate();

    /// <summary>
    /// Abstract base class for implementing trading strategies.
    /// </summary>
    public abstract class IStrategy
    {
        /// <summary>
        /// Delegate used to place new orders.
        /// </summary>
        public SendOrderDelegate? _PlaceOrder { get; set; }

        /// <summary>
        /// Delegate used to retrieve the current time.
        /// </summary>
        public GetCurrentTimeDelegate? _GetCurrentTime { get; set; }

        /// <summary>
        /// Delegate used to close existing orders.
        /// </summary>
        public CloseOrderDelegate? _CloseOrder { get; set; }

        /// <summary>
        /// Delegate used to retrieve all current orders.
        /// </summary>
        public GetOrdersDelegate? _GetOrders { get; set; }

        /// <summary>
        /// Called once when the strategy is initialized.
        /// </summary>
        public abstract void OnInit();

        /// <summary>
        /// Called once when the strategy is de-initialized.
        /// </summary>
        public abstract void OnDeInit();

        /// <summary>
        /// Called whenever a new price update is received.
        /// </summary>
        /// <param name="priceUpdate">The latest <see cref="PriceUpdate"/> data.</param>
        public abstract void OnNewPrice(PriceUpdate priceUpdate);

        /// <summary>
        /// Called whenever a new candlestick update is received.
        /// </summary>
        /// <param name="candleUpdate">The latest <see cref="CandleUpdate"/> data.</param>
        public abstract void OnNewCandle(CandleUpdate candleUpdate);

        /// <summary>
        /// Places a new order using the configured delegate.
        /// </summary>
        /// <param name="amount">The amount to trade.</param>
        /// <param name="orderSide">The side of the order (Buy or Sell).</param>
        /// <param name="orderType">The type of the order (Market or Limit).</param>
        /// <param name="price">Optional price for limit orders.</param>
        /// <returns>The unique identifier of the placed order.</returns>
        /// <exception cref="InvalidOperationException">Thrown if required delegates are not set.</exception>
        public string PlaceOrder(decimal amount, OrderSide orderSide, OrderType orderType = OrderType.Market, decimal? price = null)
        {
            Order order = new Order()
            {
                Amount = amount,
                Side = orderSide,
                Type = orderType,
                Price = price,
                CreationTime = _GetCurrentTime?.Invoke() ?? throw new InvalidOperationException("GetCurrentTime delegate is not set."),
                Status = OrderStatus.New,
                PnL = 0,
                Commision = 0
            };

            return _PlaceOrder?.Invoke(order) ?? throw new InvalidOperationException("PlaceOrder delegate is not set.");
        }

        /// <summary>
        /// Closes an existing order using the configured delegate.
        /// </summary>
        /// <param name="orderId">The unique identifier of the order to close.</param>
        /// <returns><c>true</c> if the order was successfully closed; otherwise, <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the delegate is not set.</exception>
        public bool CloseOrder(string orderId)
        {
            return _CloseOrder?.Invoke(orderId) ?? throw new InvalidOperationException("CloseOrder delegate is not set.");
        }

        /// <summary>
        /// Retrieves all current orders using the configured delegate.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="Order"/> objects.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the delegate is not set.</exception>
        public IEnumerable<Order> GetOrders()
        {
            return _GetOrders?.Invoke() ?? throw new InvalidOperationException("GetOrders delegate is not set.");
        }
    }
}

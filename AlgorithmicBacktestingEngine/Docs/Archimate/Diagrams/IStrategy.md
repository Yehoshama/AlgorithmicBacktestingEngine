# IStrategy Interface

The `IStrategy` interface defines the primary way to interact with the Algorithmic Backtesting Engine.  
It supports opening, closing, and monitoring orders, and will eventually integrate wallet functionality.

## 📐 Interface Diagram

![IStrategy UML Diagram](img_638903030358559354.png)

### Methods
- `OpenOrder`
- `CloseOrder`
- `GetOrders`
- `OnInit`
- `OnNewPrice`
- `OnNewCandle`
- `OnDelInit`

## 🧾 Order Object

The `Order` type encapsulates all necessary data to interact with the engine.

![Order Object Structure](img_638903031416548316.png)

### Fields
- **Type**: Market | Limit
- **Status**: Open | Closed | Pending
- **Direction**: Buy | Sell
- **Amount**, **OpenTime**, **EntryPrice**
- **Price**, **CloseTime**, **ExitPrice**
- **OrderId**, **Bruto PnL**, **CommissionAmount**

## 🧩 Architecture Note

This interface resides in a separate project to maintain modularity and allow independent development of strategy logic.

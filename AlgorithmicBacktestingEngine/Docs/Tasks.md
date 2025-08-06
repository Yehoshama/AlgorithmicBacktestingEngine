# RoadMap
---
## Must Have Features
- [ ] custom financial data serialization for space efficiency
- [ ] create unit tests
- [ ] order management
- [ ] modular strategies based on a common interface
- [ ] Reporting
- [ ] documentation with examples
- [ ] The ability to convert lower-frequency data to higher-frequency data
- [ ] loading with date range filters
- [ ] back testing engine
- [ ] error handeling, try/catch, etc...

## Versions and progress
### V0.0.0
- [x] make tick serialisation - 0.0.0.4
- [x] make candles serialisation - 0.0.0.5
- [x] add xml documentation to Tick and Candle and asociated types - 0.0.1

### V0.0.1 - current
- [ ] test data serialisation with unit tests - in progress

## Todo's
- [ ] change to use interval for more efficient memory management - e48dbae6-8621-415c-9d98-c0fc5ee4c700

## Future Improvements and Ideas
- [ ] Change the serialization proccess to use IEnumerable<> instead of List<>
- [ ] Move interfaces to a seperate project for the ability to create modular strategies with little to no dependancies
- [ ] add indicator interface
- [ ] add common indicators
- [ ] synthesising ticks from candles
- [ ] Live Data Connection
- [ ] different reporting outputs
- [ ] Functions to check for gaps in the data, fill in missing values, or remove outliers
- [ ] add different types of strategy files, like: .cs .dll .wasm


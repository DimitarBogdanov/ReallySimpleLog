# Please don't use this - it's terrible! I recommend [Serilog](https://github.com/serilog/serilog)


---

## ReallySimpleLog

ReallySimpleLog is a 10-minute logging library.

## Usage
```csharp
using ReallySimpleLog;

Logger logger = new(null, true);
logger.Error("Hi!");
```

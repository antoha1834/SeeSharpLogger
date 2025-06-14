## SeeSharpLogger

![Static Badge](https://img.shields.io/badge/language-C%23-%2305a630?style=for-the-badge) ![Static Badge](https://img.shields.io/badge/.NET-6.0%2C%207.0%2C%208.0-%23c62ce8?style=for-the-badge) ![Static Badge](https://img.shields.io/badge/thread%20safe-%2358bbfc?style=for-the-badge)


A simple and flexible console logging system with color output, file logging, and customizable settings.

#### 📄 Read this in [Russian](https://github.com/antoha1834/SeeSharpLogger/blob/main/README_ru.md)

## 🚀 Getting Started

Explore the [examples](https://github.com/antoha1834/SeeSharpLogger/tree/main/examples) and [documentation](https://github.com/antoha1834/SeeSharpLogger/tree/main/docs) to get started.

## 📦 Installing

**Via NuGet:**
```
dotnet add package SeeSharpLogger
```

## 💡 Capabilities

- Multiple log levels: `Unimportant`, `Log`, `Info`, `Success`, `Error`, `Warning`
- Colored output in the console
- Multiple file logging support
- Customizable prefixes and colors per log level
- Thread safety

## 🧪 Example

```csharp
// Named source
Log mainLog = new Log("Main");

mainLog.WriteLine("App started successfully", LogState.Success);
mainLog.WriteLine("This is a warning", LogState.Warning);
mainLog.WriteLine("Something went wrong", LogState.Error);

// Static calls
Log.WriteLine("Hi from static call!", "Someone", LogState.Info);

// Change the color of LogState (globally)
Log.SetStateColor(LogState.Info, ConsoleColor.Blue);
Log.WriteLine("Now the color for LogState.Info is blue!", "Update", LogState.Info);
// Change the prefix of LogState (globally)
Log.SetStatePrefix(LogState.Error, "[ERROR] ");
mainLog.WriteLine("Now the errors have become even more scary", LogState.Error);
```

### 🖨️ Example Output

```
[Timestamp] [+] [Main] App started successfully  
[Timestamp] [!] [Main] This is a warning  
[Timestamp] [X] [Main] Something went wrong  
[Timestamp] [i] [Someone] Hi from static call!  
[Timestamp] [i] [Update] Now the color for LogState.Info is blue!  
[Timestamp] [ERROR] [Main] Now the errors have become even more scary
```

## 💬 Feedback
Got any thoughts, ideas, or found a bug?
You can freely open an [issue](https://github.com/antoha1834/SeeSharpLogger/issues) or start a [discussion](https://github.com/antoha1834/SeeSharpLogger/discussions) — I'd love to hear from you!

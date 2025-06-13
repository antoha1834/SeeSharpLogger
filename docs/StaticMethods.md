# ⛔ Static methods of Log

This file describes all static methods of the Log class.
- [Basic](#-basic)
  - [WriteLine](#-writeline)
  - [RawLine](#-rawline)
  - [SkipLine](#-skipline)
  - [GetTimeStamp](#-gettimestamp)
  - [SetStateColor](#-setstatecolor)
  - [SetStatePrefix](#-setstateprefix)
- [File Logging](-#filelogging)
  - [EnableLogToFile](#-enablelogtofile)
  - [DisableLogToFile](#-disablelogtofile)
- [Advanced](-#advanced)
  - [Raw](-#raw)

# 📃 Basic

## 🖊 WriteLine

This file describes all static methods of the `Log` class.

| Argument                        | Assignment                                                 |
|---------------------------------|------------------------------------------------------------|
| `object? message = null`        | The message for log                                        |
| `string? from = null`           | The name of the source (e.g. class or method name)         |
| `LogState state = LogState.Log` | The log level (defines both the message prefix and text color) |

 ### 📄 Example
 
 ```csharp
Log.WriteLine("Hello!", "Main", LogState.Info);
 ```
 
| Part of the message | Explanation                                        |
|---------------------|----------------------------------------------------|
| `[Timestamp]`       | Current time (if `Log.AddTimeStamp` is `true`)     |
| `[i]`               | Prefix for `LogState.Info` (based on the `state` argument) |
| `[Main]`            | Source provided via the `from` argument                 |
| `Hello!`            | Message passed via the `message` argument               |

## 🖊 RawLine

Writes a raw message to the console without any formatting or prefixes.

| Argument                                  | Assignment          |
|-------------------------------------------|---------------------|
| `object? message`                         | Message to display |
| `ConsoleColor color = ConsoleColor.White` | Console text color |

 ### 📄 Example
 
  ```csharp
Log.RawLine("Just normal string without any prefixes", ConsoleColor.DarkYellow);
 ```
 
 ```
 Output:
Just normal string without any prefixes
```

## ⏭ SkipLine

Writes an empty line to the console (a line break with no message).

 ### 📄 Example
 
```csharp
Log.WriteLine("String 1...");
Log.SkipLine();
Log.WriteLine("String 2...");
```

 ```
 Output:
[Timestamp] [*] [Anonymous] String 1...

[Timestamp] [*] [Anonymous] String 2...
```


## 📅 GetTimeStamp

Returns the current date and time.

 ### 📄 Example
 ```csharp
Log.WriteLine(Log.GetTimeStamp());
```

```
Output:
[Timestamp] [*] [Anonymous] <timestamp at call time>
```


## 🎨 SetStateColor

Globally changes the color associated with a specific `LogState`.


| Argument                | Assignment                               |
|-------------------------|------------------------------------------|
| `LogState state`        | The state for which to change color |
| `ConsoleColor newColor` | New color to assign                               |

 ### 📄 Example
 ```csharp
Log.SetStateColor(LogState.Info, ConsoleColor.Blue); // Future messages with LogState.Info will now be blue
```

## 🏷 SetStatePrefix

Глобально меняет префикс для одного LogState

| Argument                | Assignment                                  |
|-------------------------|---------------------------------------------|
| `LogState state`        | The state for which to change prefix |
| `string newPrefix`      | The new prefix                               |

 ### 📄 Example
 ```csharp
Log.SetStatePrefix(LogState.Info, "[Information] ");
Log.WriteLine("Something", "Main", LogState.Info);
```

```
Output:
[Timestamp] [Information] [Main] Something
```

> [!NOTE]  
> Prefixes are not followed by a space by default. Add a space at the end of your custom prefix if needed.

# 📁 File Logging

## ✔ EnableLogToFile

Enables logging to a file.

| Argument                    | Assignment                               |
|-----------------------------|------------------------------------------|
| `string path`               | Directory where logs should be stored               |
| `string format`             | Format of the log filename             |
| `bool addTimeStamp = true`  | Whether to add a timestamp to the filename      |
| `string timeSplitter = "-"` | Character to use instead of `:` in timestamps (e.g. `-` for Windows safety) |

> [!TIP]  
> Both absolute and relative paths are supported.

> [!TIP]
> You can use the following placeholders in the filename:
> - `<Date>` - current date
> - `<Time>` - current time

 ### 📄 Example
 ```csharp
Log.EnableLogToFile("LogsFolder", "<Date> <Time> main.log");
```

## ❌ DisableLogToFile

Disables file logging and flushes all queued log entries to the current log file.

 ### 📄 Example
 ```csharp
Log.EnableLogToFile("LogsFolder", "<Date> <Time> main.log");
Log.WriteLine("Hello there!");
Log.DisableLogToFile();
// File content:
[Timestamp] [*] [Anonymous] Hello there!
```

# 💎 Advanced

## ✏ Raw

Writes a raw message to the console with no formatting and **without** a line break.

> [!TIP]
> Useful when building complex console output manually.

| Argument                                  | Assignment          |
|-------------------------------------------|---------------------|
| `object? message`                         | Message to display |
| `ConsoleColor color = ConsoleColor.White` | Console text color       |

 ### 📄 Example
 
  ```csharp
Log.Raw("Some complex ", ConsoleColor.DarkYellow);
Log.Raw("output", ConsoleColor.DarkYellow);
 ```
 
 ```
 Output:
Some complex output
```

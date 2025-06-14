# ⛔ Static methods of Log

This file describes all static methods of the Log class.
- [Basic](#-basic)
  - [WriteLine](#-writeline)
  - [RawLine](#-rawline)
  - [SkipLine](#-skipline)
  - [GetTimeStamp](#-gettimestamp)
- [LogManager](#-manager)
  - [SetStateColor](#-setstatecolor)
  - [SetStatePrefix](#-setstateprefix)
     - [Exceptions](#-exceptions)
- [File Logging](#-filelogging)
  - [EnableLogToFile](#-enablelogtofile)
  - [DisableLogToFile](#-disablelogtofile)
- [Advanced](#-advanced)
  - [Raw](#-raw)

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

# 💼 LogManager


## 🎨 SetStateColor

Globally changes the color associated with a specific `LogState`.


| Argument                | Assignment                               |
|-------------------------|------------------------------------------|
| `LogState state`        | The state for which to change color |
| `ConsoleColor newColor` | New color to assign                               |

 ### 📄 Example
 ```csharp
LogManager.SetStateColor(LogState.Info, ConsoleColor.Blue); // Future messages with LogState.Info will now be blue
```

## 🏷 SetStatePrefix

Globally changes the prefix associated with a specific `LogState`.

| Argument                | Assignment                                  |
|-------------------------|---------------------------------------------|
| `LogState state`        | The state for which to change prefix |
| `string newPrefix`      | The new prefix                               |

 ### 📄 Example
 ```csharp
LogManager.SetStatePrefix(LogState.Info, "[Information] ");
Log.WriteLine("Something", "Main", LogState.Info);
```

```
Output:
[Timestamp] [Information] [Main] Something
```

> [!NOTE]  
> Prefixes are not followed by a space by default. Add a space at the end of your custom prefix if needed.

## ⚠ Exceptions

All values ​​below are in the LogManager.Exceptions class (`LogManager.Exceptions`)

### ❓ ThrowOnNonExistingChannel (`bool`)

A boolean value that determines whether a NonExistingLogChannelException should be thrown when there is no channel with the specified name

# 📁 File Logging

## ❓ What is a logging channel (`LogFileChannel`)?

A logging channel defines **where** your log file will be stored and **what** its name format will be.

> [!TIP]
> Starting from version **v2** (this documentation covers it), logging to multiple files is supported.

## 🏗 Constructor: creating a new channel

### `Log(string name, string channel, string directory, string format, bool addTimeStamp = true, char safeReplacer = '-')`

| Argument                    | Assignment                                                |
|-----------------------------|-----------------------------------------------------------|
| `string name`               | The log source (who is writing)                                                   |
| `string channel`            | The name of the log file channel                                   |
| `string directory`          | Directory where logs should be stored                     |
| `string format`             | Format of the log filename                                |
| `bool addTimeStamp = true`  | Whether to add a timestamp to the filename                |
| `string safeReplacer = '-'` | Replacement for forbidden characters in Windows filenames and folders |

> [!TIP]  
> Both absolute and relative paths are supported.

> [!TIP]
> You can use the following placeholders in the filename (`format`):
> - `<Date>` - current date
> - `<Time>` - current time

 ### 📄 Example
 ```csharp
Log  log = new  Log("Something", "main", "Logs", "<Date> <Time>.log");
```

## 🔗 Constructor: using an existing channel

### `Log(string name, string channel)`

| Argument                    | Assignment                    |
|-----------------------------|-------------------------------|
| `string name`               | The log source                |
| `string channel`            | The name of the log channel   |

> [!NOTE]
> If `LogManager.Exceptions.ThrowOnNonExistingChannel` is set to `true`, an exception will be thrown if the channel doesn't exist.

## ❌ StopLoggingToFile

Stops logging to the file and flushes all queued log entries into the currently active file.

 ### 📄 Example
 ```csharp
Log log = new Log("Something", "main", "Logs", "<Date> <Time>.log");
log.WriteLine("Hello!");
log.StopLoggingToFile();
log.WriteLine("Bye bye!");
// File content:
[Timestamp] [*] [Something] Hello!
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
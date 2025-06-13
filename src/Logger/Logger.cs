using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SeeSharpLogger
{
    /// <summary>
    /// LogState specifies a pair for prefix and color
    /// </summary>
    public enum LogState
    {
        Log,
        Unimportant,
        Error,
        Warning,
        Success,
        Info
    }

    internal static class Maps
    {
        public static readonly Dictionary<LogState, string> prefixMap = new()
    {
        {LogState.Log, "[*] "},
        {LogState.Error, "[X] "},
        {LogState.Warning, "[!] "},
        {LogState.Success, "[+] "},
        {LogState.Info, "[i] "},
        {LogState.Unimportant, "[-] "}
    };

        public static readonly Dictionary<LogState, ConsoleColor> colorMap = new()
    {
        {LogState.Log, ConsoleColor.White},
        {LogState.Error, ConsoleColor.Red},
        {LogState.Warning, ConsoleColor.Yellow},
        {LogState.Success, ConsoleColor.Green},
        {LogState.Info, ConsoleColor.Cyan},
        {LogState.Unimportant, ConsoleColor.DarkGray}
    };
    }

#nullable enable
    public class Log
    {
        public string Name { get; set; }

        /// <summary>
        /// Default source for static calls of Write() / WriteLine()
        /// </summary>
        public static string DefaultSource { get; set; } = "Anonymous";

        /// <summary>
        /// Prefix for unknown LogState (you will never see it... at least i hope)
        /// </summary>
        public static string DefaultPrefix { get; set; } = "[?] ";

        /// <summary>
        /// Should the log be written to a file?
        /// </summary>
        public static bool LogToFile { get => _logToFile; }
        private static bool _logToFile = false;

        public static bool AddTimeStamp { get; set; } = true;

        private static FileStream? _logFile;

        /// <summary>
        /// Create new Log instance with only name specified (without logging to the file)
        /// </summary>
        /// <param name="name">source</param>
        public Log(string name) => Name = name;

        /// <summary>
        /// Create new Log instance with only name specified (with logging to the file)
        /// </summary>
        /// <param name="name">source that would be added to formatted log</param>
        /// <param name="path">directory for the .log files</param>
        /// <param name="format">format for the .log file name</param>
        /// <param name="timeSplitter">time splitter (replace of ":")</param>
        /// <param name="addTimeStamp">should a timestamp be added to each message</param>
        public static void EnableLogToFile(string path, string format, bool addTimeStamp = true, string timeSplitter = "-")
        {
            _logToFile = true;
            AddTimeStamp = addTimeStamp;

            SetPath(path, format, timeSplitter);
        }

        public static void DisableLogToFile()
        {
            _logToFile = false;
            AddTimeStamp = false;

            _logFile?.Flush();
            _logFile?.Dispose();
        }

        /// <summary>
        /// Sets the .log file path
        /// </summary>
        /// <param name="path">directory for the .log files</param>
        /// <param name="format">format for the .log file name</param>
        /// <param name="timeSplitter">time splitter (replace of ":")</param>
        private static void SetPath(string path, string format, string timeSplitter = "-")
        {
            Directory.CreateDirectory(path);

            if (!path.EndsWith('\\') && !path.EndsWith('/'))
            {
                path += "\\";
            }

            string fileName = ParseFormat(format, timeSplitter);
            string fullPath = Path.Combine(path, fileName);

            _logFile = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            AppDomain.CurrentDomain.ProcessExit += EndLoggerWork;
        }

        private static void EndLoggerWork(object? sender, EventArgs e)
        {
            if (_logToFile)
                DisableLogToFile();
        }

        private static string ParseFormat(string input, string timeSplitter)
        {
            return input.Replace("<Date>", DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern))
                        .Replace("<Time>", DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern).Replace(":", timeSplitter));
        }

        private static void AddText(FileStream? fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs?.Write(info, 0, info.Length);
        }

        private static readonly object _rawLock = new();

        // base for colored + prefixed writing
        private static void RawWrite(string prefix = "", string from = "", string? message = null, ConsoleColor color = ConsoleColor.White)
        {
            lock (_rawLock)
            {
                Console.ForegroundColor = color;
                Console.Write($"{prefix}{from}{message}");

                if (LogToFile)
                    AddText(_logFile, $"{prefix}{from}{message}");

                Console.ResetColor();
            }
        }

        // base for correct writing
        private static void CoreWrite(object? message, string from = "Anonymous", LogState state = LogState.Log)
        {
            if (Maps.prefixMap.TryGetValue(state, out var prefix) && Maps.colorMap.TryGetValue(state, out var color))
            {
                RawWrite(AddTimeStamp ? $"[{GetTimeStamp()}] {prefix}" : prefix, $"[{from}] ", message?.ToString(), color);
            }
            else
            {
                RawWrite(DefaultPrefix, DefaultSource, message?.ToString(), ConsoleColor.DarkGray);
            }
        }

        public static string GetTimeStamp()
        {
            return DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.FullDateTimePattern);
        }

        /// <summary>
        /// Writes a new line with the specified LogState
        /// </summary>
        public void WriteLine(object? message = null, LogState state = LogState.Log)
        {
            CoreWrite(message + Environment.NewLine, Name, state);
        }

        /// <summary>
        /// Writes a new line with the specified LogState and source
        /// </summary>

        public static void WriteLine(object? message = null, string? from = null, LogState state = LogState.Log)
        {
            from ??= DefaultSource;
            CoreWrite(message + Environment.NewLine, from, state);
        }

        /// <summary>
        /// Writes a string without prefixes and line break
        /// </summary>
        /// <param name="message">string to write</param>
        /// <param name="color">color of stringto write</param>
        public static void Raw(object? message = null, ConsoleColor color = ConsoleColor.White)
        {
            RawWrite("", "", message?.ToString(), color);
        }

        /// <summary>
        /// Writes new line with specified color and without prefixes
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void RawLine(object? message, ConsoleColor color = ConsoleColor.White)
        {
            RawWrite("", "", message?.ToString() + Environment.NewLine, color);
        }

        /// <summary>
        /// Go to next line without message
        /// </summary>
        public static void SkipLine()
        {
            RawWrite("", "", Environment.NewLine);
        }

        /// <summary>
        /// Globally changes the prefix for one LogState
        /// </summary>
        /// <param name="state">LogState to change prefix</param>
        /// <param name="newPrefix">new prefix</param>
        public static void SetStatePrefix(LogState state, string newPrefix)
        {
            Maps.prefixMap[state] = newPrefix;
        }

        /// <summary>
        /// Globally changes the color for one LogState
        /// </summary>
        /// <param name="state">LogState to change color</param>
        /// <param name="newColor">new color</param>
        public static void SetStateColor(LogState state, ConsoleColor newColor)
        {
            Maps.colorMap[state] = newColor;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

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

    public static class LogFileChannel
    {
        private static Dictionary<string, FileStream> _channels = new();

        /// <summary>
        /// Sets the .log file path
        /// </summary>
        /// <param name="directory">directory for the .log files</param>
        /// <param name="format">format for the .log file name</param>
        /// <param name="safeReplacer">replacement for prohibited characters in Windows file/folder names</param>
        public static bool BeginChannel(string name, string directory, string format, char safeReplacer = '-')
        {
            if (_channels.ContainsKey(name)) {
                return false;
            }

            Directory.CreateDirectory(directory);

            if (!directory.EndsWith('\\') && !directory.EndsWith('/'))
            {
                directory += "\\";
            }

            string fileName = ParseFormat(format, safeReplacer);
            string fullPath = Path.Combine(directory, fileName);

            FileStream _fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            _channels.Add(name, _fileStream);

            AppDomain.CurrentDomain.ProcessExit += StopAllChannels;

            return true;
        }

        public static bool IsChannelExists(string channel)
        {
            return _channels.ContainsKey(channel);
        }

        public static void Write(string channel, string value)
        {
            byte[] text = new UTF8Encoding(true).GetBytes(value);

            if (_channels.TryGetValue(channel, out FileStream stream))
            {
                stream.Write(text, 0, text.Length);
                stream.Flush();
            }
        }

        public static bool StopChannel(string channel)
        {
            if (_channels.ContainsKey(channel) && _channels.TryGetValue(channel, out FileStream fileStream))
            {
                fileStream.Flush();
                fileStream.Dispose();
                _channels.Remove(channel);
                return true;
            }

            return false;
        }

        private static void StopAllChannels(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, FileStream> channel in _channels)
            {
                channel.Value.Flush();
                channel.Value.Dispose();
            }
        }

        private static string ParseFormat(string input, char safeReplacer)
        {
            return WindowsSafeName(
                input.Replace("<Date>", DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern))
                     .Replace("<Time>", DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern)),
                safeReplacer);
        }

        public static string WindowsSafeName(string input, char safeReplacer)
        {
            return input.Replace('/', safeReplacer)
                        .Replace('\\', safeReplacer)
                        .Replace(':', safeReplacer)
                        .Replace('?', safeReplacer)
                        .Replace('*', safeReplacer)
                        .Replace('\"', safeReplacer)
                        .Replace('<', safeReplacer)
                        .Replace('>', safeReplacer)
                        .Replace('|', safeReplacer);
        }
    }

    public class NonExistingLogChannelException : Exception
    {
        public NonExistingLogChannelException(string name) : base($"Log file channel {name} does not exist") { }
    }

    /// <summary>
    /// Configuration of the Log and its static methods
    /// </summary>
    public static class LogManager
    {
        public static class Exceptions
        {
            /// <summary>
            /// Occurs with the Log(string name, string channel) constructor when there is no channel with the specified name
            /// </summary>
            public static bool ThrowOnNonExistingChannel { get; set; } = false;
        }

        /// <summary>
        /// Default source for static calls of Write() / WriteLine()
        /// </summary>
        public static string DefaultSource { get; set; } = "Anonymous";

        /// <summary>
        /// Prefix for unknown LogState (you will never see it... at least i hope)
        /// </summary>
        public static string DefaultPrefix { get; set; } = "[?] ";

        /// <summary>
        /// Should timestamp be added by default?
        /// </summary>
        public static bool AddTimeStamp { get; set; } = true;

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

#nullable enable
    /// <summary>
    /// Create new Log instance with only name specified (without logging to the file)
    /// </summary>
    /// <param name="name">source</param>
    public class Log
    {
        public string Name { get; set; }

        public bool AddTimeStamp { get; set; } = true;

        /// <summary>
        /// Log file channel name for current logger
        /// </summary>
        public string? ChannelName { get => _logFileChannel; }

        private string? _logFileChannel;

        /// <summary>
        /// Instance with only logging to the console (no file logging)
        /// </summary>
        /// <param name="name">source</param>
        public Log(string name)
        {
            Name = name;
            _logFileChannel = null;
        }

        /// <summary>
        /// Instance with console and file logging (new LogFileChannel will be created)
        /// </summary>
        /// <param name="name">source</param>
        /// <param name="channel">channel name for file logging</param>
        /// <param name="directory">directory for the .log files</param>
        /// <param name="format">format for the .log file name</param>
        /// <param name="addTimeStamp">should a timestamp be added to each message</param>
        /// <param name="safeReplacer">replacement for prohibited characters in Windows file/folder names</param>
        public Log(string name, string channel, string directory, string format, bool addTimeStamp = true, char safeReplacer = '-')
        {
            AddTimeStamp = addTimeStamp;
            Name = name;
            LogFileChannel.BeginChannel(channel, directory, format, safeReplacer);
            _logFileChannel = channel;
        }

        /// <summary>
        /// Intance with console and file logging (will use existing LogFileChannel)
        /// </summary>
        /// <param name="name">source</param>
        /// <param name="channel">existing channel name</param>
        public Log(string name, string channel)
        {
            Name = name;
            if (LogFileChannel.IsChannelExists(channel))
                _logFileChannel = channel;
            else if (LogManager.Exceptions.ThrowOnNonExistingChannel) throw new NonExistingLogChannelException(channel);
        }

        public void StopLoggingToFile()
        {
            if (_logFileChannel != null)
                LogFileChannel.StopChannel(_logFileChannel);
        }

        private void EndLoggerWork(object? sender, EventArgs e)
        {
            StopLoggingToFile();
        }

        /// <summary>
        /// Writes a new line with the specified LogState
        /// </summary>
        public void WriteLine(object? message = null, LogState state = LogState.Log)
        {
            string result = CoreWrite(message + Environment.NewLine, Name, AddTimeStamp, state);
            if (_logFileChannel != null)
                LogFileChannel.Write(_logFileChannel, result);
        }

        private static readonly object _rawLock = new();

        // base for colored + prefixed writing
        private static string RawWrite(string prefix = "", string from = "", string? message = null, ConsoleColor color = ConsoleColor.White)
        {
            lock (_rawLock)
            {
                Console.ForegroundColor = color;
                Console.Write($"{prefix}{from}{message}");
                Console.ResetColor();

                return $"{prefix}{from}{message}";
            }
        }

        // base for correct writing
        private static string CoreWrite(object? message, string from = "Anonymous", bool addTimeStamp = true, LogState state = LogState.Log)
        {
            if (Maps.prefixMap.TryGetValue(state, out var prefix) && Maps.colorMap.TryGetValue(state, out var color))
            {
                return RawWrite(addTimeStamp ? $"[{GetTimeStamp()}] {prefix}" : prefix, $"[{from}] ", message?.ToString(), color);
            }
            else
            {
                return RawWrite(LogManager.DefaultPrefix, LogManager.DefaultSource, message?.ToString(), ConsoleColor.DarkGray);
            }
        }

        /// <summary>
        /// Writes a new line with the specified LogState and source
        /// </summary>
        public static void WriteLine(object? message = null, string? from = null, LogState state = LogState.Log)
        {
            from ??= LogManager.DefaultSource;
            CoreWrite(message + Environment.NewLine, from, LogManager.AddTimeStamp, state);
        }

        /// <summary>
        /// Writes a string to the console without prefixes or line breaks.
        /// [!] Intended for advanced use. For regular output, prefer <see cref="RawLine(object, ConsoleColor)"/>.
        /// </summary>
        /// <remarks>
        /// Useful when building complex console output manually.
        /// </remarks>
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

        public static string GetTimeStamp()
        {
            return DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.FullDateTimePattern);
        }
    }
}
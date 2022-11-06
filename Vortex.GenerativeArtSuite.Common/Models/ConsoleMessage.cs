using System;

namespace Vortex.GenerativeArtSuite.Common.Models
{
    public struct ConsoleMessage
    {
        public ConsoleMessageType Type { get; private set; }

        public string Message { get; private set; }

        public static ConsoleMessage Info(string message) => new() { Type = ConsoleMessageType.Information, Message = AddPrefixes(message) };

        public static ConsoleMessage Warning(string message) => new() { Type = ConsoleMessageType.Warning, Message = AddPrefixes(message) };

        public static ConsoleMessage Error(string message) => new() { Type = ConsoleMessageType.Error, Message = AddPrefixes(message) };

        private static string AddPrefixes(string message) => $"[{DateTime.Now.ToShortDateString()}] : {message}";
    }
}

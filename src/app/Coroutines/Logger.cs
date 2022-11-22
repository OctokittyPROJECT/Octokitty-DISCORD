using System.Diagnostics;

namespace OctokittyBOT.Coroutines
{
    /// <summary>
    /// Logging module for bot
    /// </summary>
    public static class Logger
    {
        #region Preferences for logger's output
        /// <summary>
        /// Writes an red colored custom message with customized code's sector
        /// </summary>
        /// <param name="message">
        /// Custom message to your log's output
        /// </param>
        /// <param name="type">
        /// Custom type of program's sector where log appeared
        /// </param>
        /// <param name="isInFile">
        /// Write an output in file logs with help of IO or not
        /// </param>
        public static void Error(string message, string? type, bool isInFile = false)
        {
            var stackFrame = new StackFrame(1);

            string logsMessage = ConstructMessage(message, "ERROR", type, stackFrame);

            if (isInFile)
            {
                string auditPath = $"logs/audit-{DateTime.Now.ToString("dd/MM/yyyy")}.logs";

                Utils.LogsHandler(auditPath, logsMessage);
            }

            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(logsMessage);

            Console.ResetColor();
        }

        /// <summary>
        /// Writes an red error message of exception
        /// </summary>
        /// <param name="exception">
        /// Exception which you want to be sent into
        /// </param>
        /// <param name="isInFile">
        /// Write an output in file logs with help of IO or not
        /// </param>
        public static void Error(Exception exception, bool isInFile = false)
        {
            var stackFrame = new StackFrame(1);

            string methodName = stackFrame.GetMethod()!.Name,
                   methodType = stackFrame.GetMethod()!.DeclaringType!.ToString();

            string warnMessage = $"An exception {exception.StackTrace} occured at ${methodName}::{methodType}...";

            if (isInFile)
            {
                string auditPath = $"logs/audit-{DateTime.Now.ToString("dd/MM/yyyy")}.logs";

                Utils.LogsHandler(auditPath, warnMessage);
                Utils.LogsHandler(auditPath, exception.ToString());
            }

            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(warnMessage);
            Console.WriteLine(exception.ToString());

            Console.ResetColor();
        }

        /// <summary>
        /// Writes an yellow colored custom message with customized code's sector
        /// </summary>
        /// <param name="message">
        /// Custom message to your log's output
        /// </param>
        /// <param name="type">
        /// Custom type of program's sector where log appeared
        /// </param>
        /// <param name="isInFile">
        /// Write an output in file logs with help of IO or not. By default is <c>false</c>
        /// </param>
        public static void Warn(string message, string? type, bool isInFile = false)
        {
            var stackFrame = new StackFrame(1);

            string logsMessage = ConstructMessage(message, "WARN", type, stackFrame);

            if (isInFile)
            {
                string auditPath = $"logs/audit-{DateTime.Now.ToString("dd/MM/yyyy")}.logs";

                Utils.LogsHandler(auditPath, logsMessage);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(logsMessage);

            Console.ResetColor();
        }

        /// <summary>
        /// Writes an green colored custom message with customized code's sector
        /// </summary>
        /// <param name="message">
        /// Custom message to your log's output
        /// </param>
        /// <param name="type">
        /// Custom type of program's sector where log appeared
        /// </param>
        /// <param name="isInFile">
        /// Write an output in file logs with help of IO or not. By default is <c>false</c>
        /// </param>
        public static void Success(string message, string? type, bool isInFile = false)
        {
            var stackFrame = new StackFrame(1);

            string logsMessage = ConstructMessage(message, "SUCCESS", type, stackFrame);

            if (isInFile)
            {
                string auditPath = $"logs/audit-{DateTime.Now.ToString("dd/MM/yyyy")}.logs";

                Utils.LogsHandler(auditPath, logsMessage);
            }

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(logsMessage);

            Console.ResetColor();
        }

        /// <summary>
        /// Writes an white colored custom message with customized code's sector
        /// </summary>
        /// <param name="message">
        /// Custom message to your log's output
        /// </param>
        /// <param name="type">
        /// Custom type of program's sector where log appeared
        /// </param>
        /// <param name="isInFile">
        /// Write an output in file logs with help of IO or not. By default is <c>false</c>
        /// </param>
        public static void Info(string message, string? type, bool isInFile = false)
        {
            var stackFrame = new StackFrame(1);

            string logsMessage = ConstructMessage(message, "INFO", type, stackFrame);

            if (isInFile)
            {
                string auditPath = $"logs/audit-{DateTime.Now.ToString("dd/MM/yyyy")}.logs";

                Utils.LogsHandler(auditPath, logsMessage);
            }

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(logsMessage);

            Console.ResetColor();
        }
        #endregion

        /// <summary>
        /// Writes a custom colored message with customized code's sector and result type
        /// </summary>
        /// <param name="message">
        /// Custom message to your log's output
        /// </param>
        /// <param name="logsType">
        /// A custom string which defines a result type (like error, success and etc)
        /// </param>
        /// <param name="type">
        /// Custom type of program's sector where log appeared
        /// </param>
        /// <param name="isInFile">
        /// Write an output in file logs with help of IO or not. By default is <c>false</c>
        /// </param>
        /// <param name="color">
        /// A foreground color in which message will be styled
        /// </param>
        public static void CustomLog(string message, string logsType, string? type, bool isInFile = false, ConsoleColor color = ConsoleColor.Gray)
        {
            var stackFrame = new StackFrame(1);

            string logsMessage = ConstructMessage(message, logsType, type, stackFrame);

            if (isInFile)
            {
                string auditPath = $"logs/audit-{DateTime.Now.ToString("dd/MM/yyyy")}.logs";

                Utils.LogsHandler(auditPath, logsMessage);
            }

            Console.ForegroundColor = color;

            Console.WriteLine(logsMessage);

            Console.ResetColor();
        }

        /// <summary>
        /// Constructs future logs message with stackframe and custom params
        /// </summary>
        /// <param name="message">
        /// Non-formatted message
        /// </param>
        /// <param name="logType">
        /// A custom string which defines a result type (like error, success and etc)
        /// </param>
        /// <param name="type">
        /// Custom type of program's sector where log appeared
        /// </param>
        /// <param name="frames">
        /// Defined stackframe of function, which required this method (we need to assigne previous non-logging method)</param>
        /// <returns>
        /// A string representing formatted log's message
        /// </returns>
        private static string ConstructMessage(string message, string logType, string? type, StackFrame frames)
        {
            if (type == null || type == "")
                type = "DEFAULT";

            string methodName = frames.GetMethod()!.Name,
                   methodType = frames.GetMethod()!.DeclaringType!.ToString();

            string timeFormatted = DateTime.Now.ToString("R");

            return $"[{logType}/{type}] STACK: ({methodName} - {methodType}) {timeFormatted} | {message}";
        }
    }
}
using SeeSharpLogger;

namespace SeeSharpLogger.Examples
{
    internal class MultiSources
    {
        static void Main(string[] args)
        {
            // Multiply Log instances
            Log mainLog = new("Main");
            mainLog.WriteLine("Initialized successfully", LogState.Success);

            mainLog.WriteLine("Launching Work()...", LogState.Unimportant);
            Work();
        }

        static void Work()
        {
            Log workLog = new("Work");
            workLog.WriteLine("Work is working", LogState.Success);
        }
    }
}
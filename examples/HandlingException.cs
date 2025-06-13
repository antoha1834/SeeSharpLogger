namespace SeeSharpLogger.Examples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Multiply Log instances
            Log mainLog = new("Main");

            try
            {
                Dangerous();
            }
            catch (Exception ex) {
                mainLog.WriteLine(ex.ToString(), LogState.Error);
            }
        }

        static void Dangerous()
        {
            Log workLog = new("DangerousMethod");
            workLog.WriteLine("Dangerous method is doing his job!", LogState.Warning);
            throw new Exception("Something went wrong");
        }
    }
}
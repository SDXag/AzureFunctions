using System.Threading;

namespace SDX.FunctionsDemo.ImageProcessing
{
    static class Simulation
    {
        public static int SleepFactor = 2;

        public static void SimulateProcessing(int factor)
        {
            // Verarbeitungsdauer simulieren
            var f = factor.ToString()[0] - '0';
            Thread.Sleep(f * SleepFactor * 1_000);
        }
    }
}

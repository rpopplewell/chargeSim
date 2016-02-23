namespace DebugConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            ChargeSim.ChargeSystem sim = new ChargeSim.ChargeSystem();

            for (int i = -5; i <= 5; i++) {
                sim.NewCharge(i, 0, 1, 1);
            }

            sim.NewBoundary(-50, 50, 0, 0);

            sim.RunSim();
        }
    }
}

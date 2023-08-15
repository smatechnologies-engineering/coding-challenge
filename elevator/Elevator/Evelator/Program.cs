using System.Threading.Tasks;

namespace Elevator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        private static async Task MainAsync()
        {
            var run = new RunProgram();
            await run.Run();
        }
    }
}

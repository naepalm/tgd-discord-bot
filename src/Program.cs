using System.Threading.Tasks;

namespace Gloaming.DiscordBot
{
    class Program
    {
        public static Task Main(string[] args)
            => Startup.RunAsync(args);
    }
}

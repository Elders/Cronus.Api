using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Elders.Cronus.Dashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CronusDashboard.GetHost().Run();
        }
    }
}

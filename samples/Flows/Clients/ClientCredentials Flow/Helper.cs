using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Extensions;

namespace Thinktecture.Samples
{
    public static class Helper
    {
        public static void Timer(Action a)
        {
            var sw = new Stopwatch();

            sw.Start();
            a();

            string.Format("\n\nElapsed Time: {0}\n", sw.ElapsedMilliseconds).ConsoleRed();
        }

        public static void ShowConsole(IEnumerable<ViewClaim> claims)
        {
            "\nClaims\n".ConsoleYellow();

            claims.ToList().ForEach(c =>
            {
                Console.WriteLine(" " + c.Type);
                string.Format("  {0}\n", c.Value).ConsoleGreen();
            });
        }    
    }
    
    public class ViewClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}

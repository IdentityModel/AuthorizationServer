using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
}

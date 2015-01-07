using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProxySeeker
{
    public static class StaticRandomHandler
    {
        static int seed = Environment.TickCount;

        static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public static int Rand()
        {
            return random.Value.Next();
        }

        public static int RandMax(int max)
        {
            return random.Value.Next(max);
        }

        public static int RandRange(int min, int max)
        {
            return random.Value.Next(min, max);
        }
    }
}

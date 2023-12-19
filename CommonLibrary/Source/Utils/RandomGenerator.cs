using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Utils
{
    public class RandomGenerator
    {
        private readonly Random _rand = null;

        public RandomGenerator(int seed)
        {
            _rand = new Random(seed);
        }

        public RandomGenerator()
        {
            _rand = new Random();
        }

        public int GetNumber(int min, int max)
        {
            if (max < min)
            {
                throw new ArgumentException($"GetNumber max ({max}) was less than min ({min})!");
            }

            return (int)((max - min + 1) * _rand.NextDouble()) + min;
        }

        public List<int> GetNumberList(int numEntries, int min, int max)
        {
            var list = new List<int>();

            for (int i = 0; i < numEntries; ++i)
            {
                list.Add(GetNumber(min, max));
            }

            return list;
        }

    }
}

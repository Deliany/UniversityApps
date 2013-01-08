using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace by_Deliany
{

    public class RandomMonteCarlo
    {
        private double randomValue = 1.0;

        public RandomMonteCarlo()
        {
            Next();
            Next();
            Next();
        }

        public double Next()
        {
            double m = 8192;
            double d = 67101323;
            randomValue = m * randomValue - d * (int)(m * randomValue / d);
            return randomValue / d;
        }
    }

}

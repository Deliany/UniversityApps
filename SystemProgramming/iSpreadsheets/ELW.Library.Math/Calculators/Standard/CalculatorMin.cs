using System;

namespace ELW.Library.Math.Calculators.Standard
{
    internal sealed class CalculatorMin : IOperationCalculator
    {
        #region IOperationCalculator Members

        public double Calculate(params double[] parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");
            if (parameters.Length != 2)
                throw new ArgumentException("It is function with 2 parameter. Parameters count should be equal to 2.", "parameters");
            //
            return System.Math.Min(parameters[0], parameters[1]);
        }

        #endregion
    }
}

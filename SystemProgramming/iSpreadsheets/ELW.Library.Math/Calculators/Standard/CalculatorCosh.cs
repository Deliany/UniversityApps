using System;

namespace ELW.Library.Math.Calculators.Standard
{
    internal sealed class CalculatorCosh : IOperationCalculator
    {
        #region IOperationCalculator Members

        public double Calculate(params double[] parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");
            if (parameters.Length != 1)
                throw new ArgumentException("It is function with 1 parameter. Parameters count should be equal to 1.", "parameters");
            //
            return System.Math.Cosh(parameters[0]);
        }

        #endregion
    }
}

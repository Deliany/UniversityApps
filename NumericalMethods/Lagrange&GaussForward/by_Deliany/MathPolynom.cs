using System;

namespace MathPolynom
{
    public class Polynomial
    {
        private readonly double[] _coefficients;

        public Polynomial(params double[] coefficients)
        {
            _coefficients = coefficients;
        }

        public double this[int n]
        {
            get { return _coefficients[n]; }
            set { _coefficients[n] = value; }
        }

        public int Order
        {
            get { return _coefficients.Length; }
        }

        public override string ToString()
        {
            return string.Format("Coefficients: " + string.Join("; ", _coefficients));
        }

        public double Calculate(double x)
        {
            int n = Order - 1;
            double result = _coefficients[n];
            for (int i = n - 1; i >= 0; i--)
                result += Math.Pow(x, n-i) * _coefficients[i];
            return result;
        }


        public static Polynomial operator +(Polynomial pFirst, Polynomial pSecond)
        {
            int itemsCount = Math.Max(pFirst.Order, pSecond.Order);
            var result = new double[itemsCount];
            for (int i = 0; i < itemsCount; i++)
            {
                double a = 0;
                double b = 0;
                if (i < pFirst.Order)
                    a = pFirst[i];
                if (i < pSecond.Order)
                    b = pSecond[i];
                result[i] = a + b;
            }
            return new Polynomial(result);
        }

        public static Polynomial operator -(Polynomial pFirst, Polynomial pSecond)
        {
            int itemsCount = Math.Max(pFirst.Order, pSecond.Order);
            var result = new double[itemsCount];
            for (int i = 0; i < itemsCount; i++)
            {
                double a = 0;
                double b = 0;
                if (i < pFirst.Order)
                    a = pFirst[i];
                if (i < pSecond.Order)
                    b = pSecond[i];
                result[i] = a - b;
            }
            return new Polynomial(result);
        }

        public static Polynomial operator *(Polynomial pFirst, Polynomial pSecond)
        {
            int itemsCount = pFirst.Order + pSecond.Order - 1;
            var result = new double[itemsCount];
            for (int i = 0; i < pFirst.Order; i++)
            {
                for (int j = 0; j < pSecond.Order; j++)
                    result[i + j] += pFirst[i] * pSecond[j];
            }

            return new Polynomial(result);
        }

        public static Polynomial operator *(Polynomial pFirst, double number)
        {
            Polynomial result = new Polynomial(pFirst._coefficients);
            for (int i = 0; i < result.Order; ++i )
            {
                result[i] *= number;
            }
            return result;
        }
        public static Polynomial operator *(double number, Polynomial pFirst)
        {
            Polynomial result = new Polynomial(pFirst._coefficients);
            for (int i = 0; i < result.Order; ++i)
            {
                result[i] *= number;
            }
            return result;
        }

        public static Polynomial operator /(Polynomial pFirst, double number)
        {
            if(number == 0)
            {
                throw new DivideByZeroException();
            }
            Polynomial result = new Polynomial(pFirst._coefficients);
            for (int i = 0; i < result.Order; ++i)
            {
                result[i] /= number;
            }
            return result;
        }
    }
}
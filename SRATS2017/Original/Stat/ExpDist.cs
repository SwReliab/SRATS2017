using SRATS.Utils;

namespace SRATS.Stat
{
    public class ExpDist : SDist
    {
		GetParam lambdaFunc;

		protected double Lambda
		{
			get { return lambdaFunc(); }
		}

		public ExpDist(GetParam lambdaFunc)
        {
			this.lambdaFunc = lambdaFunc;
        }

        public override double Pdf(double x)
        {
            return Lambda * NMath.Exp(-Lambda * x);
        }

        public override double Cdf(double x)
        {
            return -NMath.Expm1(-Lambda * x);
        }

        public override double Ccdf(double x)
        {
            return NMath.Exp(-Lambda * x);
        }

        public override double Mean()
        {
            return 1.0 / Lambda;
        }

        public override double Variance()
        {
            return 1.0 / (Lambda * Lambda);
        }

        public override double Quantile(double p, double eps)
        {
            return -1.0 / Lambda * NMath.Log(1.0 - p);
        }
    }
}

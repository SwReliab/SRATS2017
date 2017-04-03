using SRATS.Utils;

namespace SRATS.Stat
{
    public class NormalDist : SDist
    {
        const double PI = 3.14159265358979324;
        const double LOG_PI = 1.14472988584940017;

		GetParam muFunc;
		GetParam sigFunc;

		protected double Mu
		{
			get { return muFunc(); }
		}

		protected double Sig
		{
			get { return sigFunc(); }
		}

		public NormalDist(GetParam mu, GetParam sigma)
        {
            muFunc = mu;
            sigFunc = sigma;
        }

        public override double Pdf(double x)
        {
            double y = (x - Mu) / Sig;
            return 1.0 / NMath.Sqrt(2.0 * PI) * NMath.Exp(-y * y / 2.0) / Sig;
        }

        public override double Cdf(double x)
        {
            double y = (x - Mu) / Sig;
            return P_normal(y);
        }

        public override double Ccdf(double x)
        {
            double y = (x - Mu) / Sig;
            return Q_normal(y);
        }

        public override double Mean()
        {
            return Mu;
        }

        public override double Variance()
        {
            return Sig * Sig;
        }

        public override double Quantile(double p, double eps)
        {
            return Mu - Sig * NMath.Sqrt(2.0) * NMath.Dierfc(2.0 * p);
        }

        // private for normal
        private double P_normal(double x)
        {
            if (x >= 0.0)
                return 0.5 * (1 + NMath.P_gamma(0.5, 0.5 * x * x, LOG_PI / 2));
            else
                return 0.5 * NMath.Q_gamma(0.5, 0.5 * x * x, LOG_PI / 2);
            //    return 0.5 * (1.0 + erf(x/sqrt(2.0)));
        }

        private double Q_normal(double x)
        {
            if (x >= 0.0)
                return 0.5 * NMath.Q_gamma(0.5, 0.5 * x * x, LOG_PI / 2);
            else
                return 0.5 * (1 + NMath.P_gamma(0.5, 0.5 * x * x, LOG_PI / 2));
            //    return 0.5 * (1.0 + erf(-x/sqrt(2.0)));
        }
    }

    public class TruncNormalDist : NormalDist
	{
		protected double P0
		{
			get { return base.Ccdf(0.0); }
		}

		public TruncNormalDist(GetParam mu, GetParam sigma) : base(mu, sigma) { }

		public override double Pdf(double x)
		{
			return base.Pdf(x) / P0;
		}

		public override double Cdf(double x)
		{
			return 1.0 - base.Ccdf(x) / P0;
		}

		public override double Ccdf(double x)
		{
			return base.Ccdf(x) / P0;
		}

		public override double Mean()
		{
			double R = P0 / base.Pdf(0.0);
			return Mu + Sig / R;
		}

		public override double Variance()
		{
			double s = -Mu / Sig;
			double R = P0 / base.Pdf(0.0);
			return Sig * Sig * (1.0 + s / R - 1.0 / R / R);
		}

		public override double Quantile(double p, double eps)
		{
			return base.Quantile(1.0 - (1.0 - p) * P0, eps);
		}
	}

    public class LogNormalDist : NormalDist
	{
		public LogNormalDist(GetParam mu, GetParam sigma) : base(mu, sigma) { }

		public override double Pdf(double x)
		{
			return base.Pdf(NMath.Log(x)) / x;
		}

		public override double Cdf(double x)
		{
			if (x == 0.0)
			{
				return 0.0;
			}
			return base.Cdf(NMath.Log(x));
		}

		public override double Ccdf(double x)
		{
			if (x == 0.0)
			{
				return 1.0;
			}
			return base.Ccdf(NMath.Log(x));
		}

		public override double Mean()
		{
			return NMath.Exp(Mu + Sig * Sig / 2.0);
		}

		public override double Variance()
		{
			return NMath.Exp(2.0 * Mu + Sig * Sig)
				* (NMath.Exp(Sig * Sig) - 1.0);
		}

		public override double Quantile(double p, double eps)
		{
			return NMath.Exp(base.Quantile(p, eps));
		}
	}
}

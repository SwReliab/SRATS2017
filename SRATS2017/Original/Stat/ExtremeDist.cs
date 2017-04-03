using SRATS.Utils;

namespace SRATS.Stat
{
    public class ExtremeDist : SDist
    {
        const double EGAM = 0.57721566490153286060651209008240;
        const double PI = 3.14159265358979324;

		GetParam muFunc;
		GetParam thetaFunc;

		protected double Mu
		{
			get { return muFunc(); }
		}

		protected double Theta
		{
			get { return thetaFunc(); }
		}

        public ExtremeDist(GetParam mu, GetParam theta)
        {
            muFunc = mu;
            thetaFunc = theta;
        }

        public override double Pdf(double x)
        {
            double y = NMath.Exp(-(x - Mu) / Theta);
            return y * NMath.Exp(-y) / Theta;
        }

        public override double Cdf(double x)
        {
            double y = NMath.Exp(-(x - Mu) / Theta);
            return NMath.Exp(-y);
        }

        public override double Ccdf(double x)
        {
            double y = NMath.Exp(-(x - Mu) / Theta);
            return -NMath.Expm1(-y);
        }

        public override double Mean()
        {
            return Mu + EGAM * Theta;
        }

        public override double Variance()
        {
            return Theta * Theta * PI * PI / 6.0;
        }

        public override double Quantile(double p, double eps)
        {
            return Mu - Theta * NMath.Log(-NMath.Log(p));
        }
    }

    public class TruncMaxDist : ExtremeDist
	{
		protected double P0
		{
			get { return base.Ccdf(0.0); }
		}

		public TruncMaxDist(GetParam mu, GetParam theta)
			: base(mu, theta)
		{
		}

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
			//TODO: this is the mean of TruncNormal. It should be modified.
			double R = P0 / base.Pdf(0.0);
			return Mu + Theta / R;
		}

		public override double Variance()
		{
			//TODO: this is the variance of TruncNormal. It should be modified.
			double s = -Mu / Theta;
			double R = P0 / base.Pdf(0.0);
			return Theta * Theta * (1.0 + s / R - 1.0 / R / R);
		}

		public override double Quantile(double p, double eps)
		{
			return base.Quantile(1.0 - (1.0 - p) * P0, eps);
		}
	}

    public class TruncMinDist : ExtremeDist
	{
		protected double P0
		{
			get { return base.Cdf(0.0); }
		}

		public TruncMinDist(GetParam mu, GetParam theta)
			: base(mu, theta)
		{
		}

		public override double Pdf(double x)
		{
			return base.Pdf(-x) / P0;
		}

		public override double Cdf(double x)
		{
			return 1.0 - base.Cdf(-x) / P0;
		}

		public override double Ccdf(double x)
		{
			return base.Cdf(-x) / P0;
		}

		public override double Mean()
		{
			//TODO: this is the mean of TruncNormal. It should be modified.
			double R = P0 / base.Pdf(0.0);
			return Mu + Theta / R;
		}

		public override double Variance()
		{
			//TODO: this is the variance of TruncNormal. It should be modified.
			double s = -Mu / Theta;
			double R = P0 / base.Pdf(0.0);
			return Theta * Theta * (1.0 + s / R - 1.0 / R / R);
		}

		public override double Quantile(double p, double eps)
		{
			return (-1.0) * base.Quantile((1.0 - p) * P0, eps);
		}
	}

    public class LogMaxDist : ExtremeDist
	{
		public LogMaxDist(GetParam mu, GetParam sigma)
			: base(mu, sigma)
		{
		}

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
			//TODO: this is the mean of TruncNormal. It should be modified.
			return NMath.Exp(Mu + Theta * Theta / 2.0);
		}

		public override double Variance()
		{
			//TODO: this is the mean of TruncNormal. It should be modified.
			return NMath.Exp(2.0 * Mu + Theta * Theta)
				* (NMath.Exp(Theta * Theta) - 1.0);
		}

		public override double Quantile(double p, double eps)
		{
			return NMath.Exp(base.Quantile(p, eps));
		}
	}

    public class LogMinDist : ExtremeDist
	{
		public LogMinDist(GetParam mu, GetParam sigma)
			: base(mu, sigma)
		{
		}

		public override double Pdf(double x)
		{
			return base.Pdf(-NMath.Log(x)) / x;
		}

		public override double Cdf(double x)
		{
			if (x == 0.0)
			{
				return 0.0;
			}
			return base.Ccdf(-NMath.Log(x));
		}

		public override double Ccdf(double x)
		{
			if (x == 0.0)
			{
				return 1.0;
			}
			return base.Cdf(-NMath.Log(x));
		}

		public override double Mean()
		{
			//TODO: this is the mean of TruncNormal. It should be modified.
			return NMath.Exp(Mu + Theta * Theta / 2.0);
		}

		public override double Variance()
		{
			//TODO: this is the mean of TruncNormal. It should be modified.
			return NMath.Exp(2.0 * Mu + Theta * Theta)
				* (NMath.Exp(Theta * Theta) - 1.0);
		}

		public override double Quantile(double p, double eps)
		{
			return NMath.Exp(-base.Quantile(1.0 - p, eps));
		}
	}
}

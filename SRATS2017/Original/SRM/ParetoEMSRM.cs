using SRATS.Utils;

namespace SRATS.MODELS
{
	public class ParetoParam : OriginalParam
	{
		public double Omega
		{
			get { return param[0]; }
			set { param[0] = value; }
		}

		public double A
		{
			get { return param[1]; }
			set { param[1] = value; }
		}

		public double C
		{
			get { return param[2]; }
			set { param[2] = value; }
		}

		public ParetoParam(double omega_ = 1.0, double a_ = 1.0, double c_ = 1.0) : base(3)
		{
			Omega = omega_;
			A = a_;
			C = c_;
		}

		public double GetOmega()
		{
			return Omega;
		}

		public double GetA()
		{
			return A;
		}

		public double GetC()
		{
			return C;
		}

		public override IModelParam Create()
		{
			return new ParetoParam();
		}
	}

	public class ParetoEMSRM : SRMBase, IEMbase
    {
		ParetoParam param;
		readonly double newton_eps;
		double en1, en2, en3;

		public ParetoEMSRM(ParetoParam param) : base("Pareto SRGM", 3, param.GetOmega, new Stat.ParetoDist(param.GetA, param.GetC))
        {
			this.param = param;
            newton_eps = 1.0e-8;
        }

        public void Initialize(SRMData data)
        {
            param.Omega = 1.0;
            param.A = 1.0;
            param.C = 1.0;
        }

        private double Estep(SRMData data)
        {
			int dsize = data.Size;
			double[] time = data.Time;
			int[] num = data.Fault;
			int[] type = data.Type;

            int j;
            int x;
            double t;
            double tmp1, tmp2, tmp3;
            double llf;
            double g00, g01, g02;
            double g10, g11, g12;

            // E-step
            t = time[0];
            x = num[0];

            en1 = 0.0;
            en2 = 0.0;
            en3 = 0.0;
            llf = 0.0;
            g00 = 1.0;
            g01 = param.A / param.C;
            g02 = NMath.Psi(param.A) - NMath.Log(param.C);
            g10 = dist.Ccdf(t);
            g11 = param.A / (param.C + t) * g10;
            g12 = (NMath.Psi(param.A) - NMath.Log(param.C + t)) * g10;
            if (x != 0)
            {
                tmp1 = g00 - g10;
                tmp2 = g01 - g11;
                tmp3 = g02 - g12;
                en1 += x;
                en2 += x * tmp2 / tmp1;
                en3 += x * tmp3 / tmp1;
                llf += x * NMath.Log(tmp1) - NMath.Lgamma(x + 1.0);
            }
            if (type[0] != 0)
            {
                en1 += 1.0;
                en2 += (param.A + 1.0) / (param.C + t);
                en3 += NMath.Psi(param.A + 1.0) - NMath.Log(param.C + t);
                llf += NMath.Log(dist.Pdf(t));
            }
            for (j = 1; j < dsize; j++)
            {
                x = num[j];
                if (time[j] != 0.0)
                {
                    t += time[j];
                    g00 = g10;
                    g01 = g11;
                    g02 = g12;
                    g10 = dist.Ccdf(t);
                    g11 = param.A / (param.C + t) * g10;
                    g12 = (NMath.Psi(param.A) - NMath.Log(param.C + t)) * g10;
                }
                if (x != 0)
                {
                    tmp1 = g00 - g10;
                    tmp2 = g01 - g11;
                    tmp3 = g02 - g12;
                    en1 += x;
                    en2 += x * tmp2 / tmp1;
                    en3 += x * tmp3 / tmp1;
                    llf += x * NMath.Log(tmp1) - NMath.Lgamma(x + 1.0);
                }
                if (type[j] != 0)
                {
                    en1 += 1.0;
                    en2 += (param.A + 1.0) / (param.C + t);
                    en3 += NMath.Psi(param.A + 1.0) - NMath.Log(param.C + t);
                    llf += NMath.Log(dist.Pdf(t));
                }
            }
            llf += NMath.Log(param.Omega) * en1;
            en1 += param.Omega * g10;
            en2 += param.Omega * g11;
            en3 += param.Omega * g12;
            llf += -param.Omega * (1.0 - g10);

            return llf;
        }

        private void Mstep()
        {
            double x, xn, f, df, step;
            double v = NMath.Log(en2 / en1) - en3 / en1;
            x = param.A;
            do
            {
                f = NMath.Log(x) - NMath.Psi(x) - v;
                df = 1.0 / x - NMath.Polygamma(1, x);
                step = 1.0;
                do
                {
                    xn = x - step * f / df;
                    if (xn > 0.0) break;
                    step *= 0.5;
                } while (true);
                if (double.IsNaN(x) || double.IsInfinity(x))
                {
                    throw new System.NotFiniteNumberException();
                }
                if (NMath.Abs(xn - x) < newton_eps * NMath.Abs(x))
                {
                    break;
                }
                x = xn;
            } while (true);
            param.A = xn;
            param.Omega = en1;
            param.C = param.A * en1 / en2;
        }

		public override IModelParam GetParam()
		{
			return param;
		}

		public double Emstep(SRMData data)
		{
			double llf = Estep(data);
			Mstep();
			return llf;
		}

		public void Pre_em(SRMData data)
		{
		}

		public void Post_em(SRMData data)
		{
		}
	}
}

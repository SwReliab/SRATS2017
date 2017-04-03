using SRATS.Utils;

namespace SRATS.MODELS
{
	public class NormalParam : OriginalParam
	{
		public double Omega
		{
			get { return param[0]; }
			set { param[0] = value; }
		}

		public double Mu
		{
			get { return param[1]; }
			set { param[1] = value; }
		}

		public double Sig
		{
			get { return param[2]; }
			set { param[2] = value; }
		}

		public NormalParam(double omega_ = 1.0, double mu_ = 1.0, double sig_ = 1.0) : base(3)
		{
			Omega = omega_;
			Mu = mu_;
			Sig = sig_;
		}

		public double GetOmega()
		{
			return Omega;
		}

		public double GetMu()
		{
			return Mu;
		}

		public double GetSig()
		{
			return Sig;
		}

		public override IModelParam Create()
		{
			return new NormalParam();
		}
	}

	public class TNormEMSRM : SRMBase, IEMbase
    {
		NormalParam param;
        Stat.NormalDist dist0;
        double en1, en2, en3;

		private double Mu0()
		{
			return 0.0;
		}

		private double Sig1()
		{
			return 1.0;
		}

		public TNormEMSRM(NormalParam param) : base("TruncNormal SRGM", 3, param.GetOmega, new Stat.TruncNormalDist(param.GetMu, param.GetSig))
        {
			this.param = param;
            dist0 = new Stat.NormalDist(Mu0, Sig1);
        }

        public void Initialize(SRMData data)
        {
            param.Omega = 1.0;
            param.Mu = 0.0;
			param.Sig = data.TotalTime / 2.0;
        }

        private double Estep(SRMData data)
        {
			int dsize = data.Size;
			double[] time = data.Time;
			int[] num = data.Fault;
			int[] type = data.Type;

            double tomega = param.Omega / dist0.Ccdf(-param.Mu / param.Sig);

            int j;
			int x;
			double t, y;
            double tmp1, tmp2, tmp3;
            double tmp, llf;
            double g00, g01, g02;
            double g10, g11, g12;
            double g20, g21, g22;

            en1 = 0.0;
            en2 = 0.0;
            en3 = 0.0;
            llf = 0.0;
            tmp = dist0.Pdf(-param.Mu / param.Sig);
            g00 = dist0.Ccdf(-param.Mu / param.Sig);
            g01 = param.Sig * tmp + param.Mu * g00;
            g02 = (param.Mu * param.Sig) * tmp + (param.Sig * param.Sig + param.Mu * param.Mu) * g00;
            t = time[0];
            x = num[0];
            y = (t - param.Mu) / param.Sig;
            tmp = dist0.Pdf(y);
            g10 = g00;
            g11 = g01;
            g12 = g02;
            g20 = dist0.Ccdf(y);
            g21 = param.Sig * tmp + param.Mu * g20;
            g22 = (param.Sig * t + param.Mu * param.Sig) * tmp + (param.Sig * param.Sig + param.Mu * param.Mu) * g20;
            if (x != 0)
            {
                tmp1 = g10 - g20;
                tmp2 = g11 - g21;
                tmp3 = g12 - g22;
                en1 += x;
                en2 += x * tmp2 / tmp1;
                en3 += x * tmp3 / tmp1;
                llf += x * (NMath.Log(tmp1) - NMath.Log(g00)) - NMath.Lgamma(x + 1.0);
            }
            if (type[0] != 0)
            {
                en1 += 1.0;
                en2 += t;
                en3 += t * t;
                llf += NMath.Log(dist.Pdf(t));
            }
            for (j = 1; j < dsize; j++)
            {
                x = num[j];
                if (time[j] != 0.0)
                {
                    t += time[j];
                    y = (t - param.Mu) / param.Sig;
                    tmp = dist0.Pdf(y);
                    g10 = g20;
                    g11 = g21;
                    g12 = g22;
                    g20 = dist0.Ccdf(y);
                    g21 = param.Sig * tmp + param.Mu * g20;
                    g22 = (param.Sig * t + param.Mu * param.Sig) * tmp + (param.Sig * param.Sig + param.Mu * param.Mu) * g20;
                }
                if (x != 0)
                {
                    tmp1 = g10 - g20;
                    tmp2 = g11 - g21;
                    tmp3 = g12 - g22;
                    en1 += x;
                    en2 += x * tmp2 / tmp1;
                    en3 += x * tmp3 / tmp1;
                    llf += x * (NMath.Log(tmp1) - NMath.Log(g00)) - NMath.Lgamma(x + 1.0);
                }
                if (type[j] != 0)
                {
                    en1 += 1.0;
                    en2 += t;
                    en3 += t * t;
                    llf += NMath.Log(dist.Pdf(t));
                }
            }
            llf += (NMath.Log(tomega) + NMath.Log(g00)) * en1;
            en1 += tomega * (1.0 - g00 + g20);
            en2 += tomega * (param.Mu - g01 + g21);
            en3 += tomega * (param.Sig * param.Sig + param.Mu * param.Mu - g02 + g22);
            llf += -tomega * (g00 - g20);

            return llf;
        }

        private void Mstep()
        {
            param.Mu = en2 / en1;
            param.Sig = NMath.Sqrt(en3 / en1 - param.Mu * param.Mu);
            //::pnorm(0.0, mu, param.sig, true);
            param.Omega = en1 * dist0.Ccdf(-param.Mu / param.Sig);
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

	public class LNormEMSRM : SRMBase, IEMbase
	{
		NormalParam param;
		Stat.NormalDist dist0;
		double en1, en2, en3;

		private double Mu0()
		{
			return 0.0;
		}

		private double Sig1()
		{
			return 1.0;
		}

		public LNormEMSRM(NormalParam param) : base("LogNormal SRGM", 3, param.GetOmega, new Stat.LogNormalDist(param.GetMu, param.GetSig))
		{
			this.param = param;
			dist0 = new Stat.NormalDist(Mu0, Sig1);
		}

		public void Initialize(SRMData data)
		{
			param.Omega = 1.0;
			param.Mu = 1.0;
			param.Sig = NMath.Log(data.TotalTime / 2.0);
		}

		private double Estep(SRMData data)
		{
			int dsize = data.Size;
			double[] time = data.Time;
			int[] num = data.Fault;
			int[] type = data.Type;

			int j;
			int x;
			double t, y;
			double tmp1, tmp2, tmp3;
			double tmp, llf;
			double g00, g01, g02;
			double g10, g11, g12;

			// E-step
			t = time[0];
			x = num[0];
			y = (NMath.Log(t) - param.Mu) / param.Sig;
			en1 = 0.0;
			en2 = 0.0;
			en3 = 0.0;
			llf = 0.0;
			tmp = dist0.Pdf(y);
			g00 = 1.0;
			g01 = param.Mu;
			g02 = param.Sig * param.Sig + param.Mu * param.Mu;
			g10 = dist0.Ccdf(y);
			g11 = param.Sig * tmp + param.Mu * g10;
			g12 = (param.Sig * NMath.Log(t) + param.Mu * param.Sig) * tmp
			+ (param.Sig * param.Sig + param.Mu * param.Mu) * g10;
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
				en2 += NMath.Log(t);
				en3 += NMath.Log(t) * NMath.Log(t);
				llf += NMath.Log(dist.Pdf(t));
			}
			for (j = 1; j < dsize; j++)
			{
				x = num[j];
				if (time[j] != 0.0)
				{
					t += time[j];
					y = (NMath.Log(t) - param.Mu) / param.Sig;
					tmp = dist0.Pdf(y);
					g00 = g10;
					g01 = g11;
					g02 = g12;
					g10 = dist0.Ccdf(y);
					g11 = param.Sig * tmp + param.Mu * g10;
					g12 = (param.Sig * NMath.Log(t) + param.Mu * param.Sig) * tmp
					+ (param.Sig * param.Sig + param.Mu * param.Mu) * g10;
				}
				if (x != 0)
				{
					tmp1 = g00 - g10;
					tmp2 = g01 - g11;
					tmp3 = g02 - g12;
					en1 += x;
					en2 += x * tmp2 / tmp1;
					en3 += x * tmp3 / tmp1;
					llf += x * NMath.Log(tmp1) - NMath.Lgamma(x + 1);
				}
				if (type[j] != 0)
				{
					en1 += 1.0;
					en2 += NMath.Log(t);
					en3 += NMath.Log(t) * NMath.Log(t);
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
			param.Mu = en2 / en1;
			param.Sig = NMath.Sqrt(en3 / en1 - param.Mu * param.Mu);
			//::pnorm(0.0, mu, sigma, true);
			param.Omega = en1;
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

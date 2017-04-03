using SRATS.Utils;

namespace SRATS.MODELS
{
	public class LogisParam : OriginalParam
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

		public double Phi
		{
			get { return param[2]; }
			set { param[2] = value; }
		}

		public LogisParam(double omega_ = 1.0, double mu_ = 1.0, double phi_ = 1.0) : base(3)
		{
			Omega = omega_;
			Mu = mu_;
			Phi = phi_;
		}

		public double GetOmega()
		{
			return Omega;
		}

		public double GetMu()
		{
			return Mu;
		}

		public double GetPhi()
		{
			return Phi;
		}

		public override IModelParam Create()
		{
			return new LogisParam();
		}
	}

	public class TLogisEMSRM : SRMBase, IEMbase
    {
		LogisParam param;
        double en1, en2, en3;

		public TLogisEMSRM(LogisParam param) : base("TruncLogistic SRGM", 3, param.GetOmega, new Stat.TruncLogisDist(param.GetMu, param.GetPhi))
        {
			this.param = param;
        }

        public void Initialize(SRMData data)
        {
            param.Omega = 1.0;
            param.Mu = 0.0;
			param.Phi = data.TotalTime / 2.0;
        }

        private double Estep(SRMData data)
        {
			int dsize = data.Size;
			double[] time = data.Time;
			int[] num = data.Fault;
			int[] type = data.Type;

            int j;
			int x;
			double t, y, yp2, y0, y0p2;
            double tmp1, tmp2, tmp3;
            double llf;
            double g00, g01, g02;
            double g10, g11, g12;
            double g20, g21, g22;

            // E-step
            y0 = NMath.Exp(-param.Mu / param.Phi);
            y0p2 = 1.0 + 2.0 * y0 + y0 * y0;
            double tomega = param.Omega * (1.0 + y0);

            en1 = 0.0;
            en2 = 0.0;
            en3 = 0.0;
            llf = 0.0;
            g00 = 1.0 / (1.0 + y0);
            g01 = 1.0 / (2.0 * y0p2);
            g02 = (1.0 + (1.0 + NMath.Log(y0)) * y0) / y0p2;
            t = time[0];
            x = num[0];
            y = NMath.Exp((t - param.Mu) / param.Phi);
            yp2 = 1.0 + 2.0 * y + y * y;
            g10 = g00;
            g11 = g01;
            g12 = g02;
            g20 = 1.0 / (1.0 + y);
            g21 = 1.0 / (2.0 * yp2);
            g22 = (1.0 + (1.0 + NMath.Log(y)) * y) / yp2;
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
                en2 += 1.0 / (1.0 + y);
                en3 += (y - 1.0) * NMath.Log(y) / (1.0 + y);
                llf += NMath.Log(dist.Pdf(t));
            }
            for (j = 1; j < dsize; j++)
            {
                x = num[j];
                if (time[j] != 0.0)
                {
                    t += time[j];
                    y = NMath.Exp((t - param.Mu) / param.Phi);
                    yp2 = 1.0 + 2.0 * y + y * y;
                    g10 = g20;
                    g11 = g21;
                    g12 = g22;
                    g20 = 1.0 / (1.0 + y);
                    g21 = 1.0 / (2.0 * yp2);
                    g22 = (1.0 + (1.0 + NMath.Log(y)) * y) / yp2;
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
                    en2 += 1.0 / (1.0 + y);
                    en3 += (y - 1.0) * NMath.Log(y) / (1.0 + y);
                    llf += NMath.Log(dist.Pdf(t));
                }
            }
            llf += (NMath.Log(tomega) + NMath.Log(g00)) * en1;
            en1 += tomega * (1.0 - g00 + g20);
            en2 += tomega * (0.5 - g01 + g21);
            en3 += tomega * (1.0 - g02 + g22);
            llf += -tomega * (g00 - g20);

            return llf;
        }

        private void Mstep()
        {
            param.Phi = param.Phi * en3 / en1;
            param.Mu = param.Mu + param.Phi * (NMath.Log(en1 / 2.0) - NMath.Log(en2));
            param.Omega = en1 / (1.0 + NMath.Exp(-param.Mu / param.Phi));
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

	public class LLogisEMSRM : SRMBase, IEMbase
	{
		LogisParam param;
		double en1, en2, en3;

		public LLogisEMSRM(LogisParam param) : base("LogLogistic SRGM", 3, param.GetOmega, new Stat.LogLogisDist(param.GetMu, param.GetPhi))
        {
			this.param = param;
		}

		public void Initialize(SRMData data)
		{
			param.Omega = 1.0;
			param.Mu = 1.0;
			param.Phi = NMath.Log(data.TotalTime / 2.0);
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
			double llf;
			double g00, g01, g02;
			double g10, g11, g12;

			// E-step
			t = time[0];
			x = num[0];
			y = NMath.Exp((NMath.Log(t) - param.Mu) / param.Phi);
			en1 = 0.0;
			en2 = 0.0;
			en3 = 0.0;
			llf = 0.0;
			g00 = 1.0;
			g01 = 0.5;
			g02 = 1.0;
			g10 = 1.0 / (1.0 + y);
			g11 = 1.0 / (2.0 * (1.0 + y) * (1.0 + y));
			g12 = (1.0 + (1.0 + NMath.Log(y)) * y) / ((1.0 + y) * (1.0 + y));
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
				en2 += 1.0 / (1.0 + y);
				en3 += (y - 1.0) * NMath.Log(y) / (1.0 + y);
				llf += NMath.Log(dist.Pdf(t));
			}
			for (j = 1; j < dsize; j++)
			{
				x = num[j];
				if (time[j] != 0.0)
				{
					t += time[j];
					y = NMath.Exp((NMath.Log(t) - param.Mu) / param.Phi);
					g00 = g10;
					g01 = g11;
					g02 = g12;
					g10 = 1.0 / (1.0 + y);
					g11 = 1.0 / (2.0 * (1.0 + y) * (1.0 + y));
					g12 = (1.0 + (1.0 + NMath.Log(y)) * y) / ((1.0 + y) * (1.0 + y));
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
					en2 += 1.0 / (1.0 + y);
					en3 += (y - 1.0) * NMath.Log(y) / (1.0 + y);
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
			double mu = param.Mu;
			double phi = param.Phi;
			param.Phi = phi * en3 / en1;
			param.Mu = mu + param.Phi * (NMath.Log(en1 / 2.0) - NMath.Log(en2));
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

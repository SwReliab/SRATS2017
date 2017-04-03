using SRATS.Utils;

namespace SRATS.MODELS
{
	public class ExtremeParam : OriginalParam
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

		public double Theta
		{
			get { return param[2]; }
			set { param[2] = value; }
		}

		public ExtremeParam(double omega_ = 1.0, double mu_ = 1.0, double theta_ = 1.0) : base(3)
		{
			Omega = omega_;
			Mu = mu_;
			Theta = theta_;
		}

		public double GetOmega()
		{
			return Omega;
		}

		public double GetMu()
		{
			return Mu;
		}

		public double GetTheta()
		{
			return Theta;
		}

		public override IModelParam Create()
		{
			return new ExtremeParam();
		}
	}

	public class TXvmaxEMSRM : SRMBase, IEMbase
    {
		ExtremeParam param;
        double en1, en2, en3;

		public TXvmaxEMSRM(ExtremeParam param) : base("TruncExtremeMax SRGM", 3, param.GetOmega, new Stat.TruncMaxDist(param.GetMu, param.GetTheta))
        {
			this.param = param;
        }

        public void Initialize(SRMData data)
        {
            param.Omega = 1.0;
            param.Mu = 0.0;
			param.Theta = data.TotalTime / 3.0;
        }

        private double Estep(SRMData data)
        {
			int dsize = data.Size;
			double[] time = data.Time;
			int[] num = data.Fault;
			int[] type = data.Type;

            int j;
			int x;
			double t, y, y0;
            double tmp1, tmp2, tmp3;
            double llf;
            double g00, g01, g02;
            double g10, g11, g12;
            double g20, g21, g22;

            // E-step
            y0 = NMath.Exp(param.Mu / param.Theta);
            //    double tomega = omega / (1.0 - exp(-y0));
            double tomega = -param.Omega / NMath.Expm1(-y0);

            en1 = 0.0;
            en2 = 0.0;
            en3 = 0.0;
            llf = 0.0;
            //    g00 = 1.0 - exp(-y0);
            //    g01 = exp(-mu/theta)*(1.0 - (1.0 + y0)*exp(-y0));
            //    g02 = 1.0 - exp(-y0) * (1.0 + y0 * log(y0));
            g00 = -NMath.Expm1(-y0);
            g01 = -NMath.Exp(-param.Mu / param.Theta) * (NMath.Expm1(-y0) + y0 * NMath.Exp(-y0));
            g02 = -(NMath.Expm1(-y0) + NMath.Exp(-y0) * y0 * param.Mu / param.Theta);
            t = time[0];
            x = num[0];
            y = NMath.Exp(-(t - param.Mu) / param.Theta);
            g10 = g00;
            g11 = g01;
            g12 = g02;
            //    g20 = 1.0 - exp(-y);
            //    g21 = exp(-mu/theta)*(1.0 - (1.0 + y)*exp(-y));
            //    g22 = 1.0 - exp(-y) * (1.0 + y * log(y));
            g20 = -NMath.Expm1(-y);
            g21 = -NMath.Exp(-param.Mu / param.Theta) * (NMath.Expm1(-y) + y * NMath.Exp(-y));
            g22 = -NMath.Expm1(-y) + NMath.Exp(-y) * y * (t - param.Mu) / param.Theta;
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
                en2 += NMath.Exp(-t / param.Theta);
                en3 += (t - param.Mu) / param.Theta * (1.0 - y);
                llf += NMath.Log(dist.Pdf(t));
            }
            for (j = 1; j < dsize; j++)
            {
                x = num[j];
                if (time[j] != 0.0)
                {
                    t += time[j];
                    y = NMath.Exp(-(t - param.Mu) / param.Theta);
                    g10 = g20;
                    g11 = g21;
                    g12 = g22;
                    //        g20 = 1.0 - exp(-y);
                    //        g21 = exp(-mu/theta)*(1.0 - (1.0 + y)*exp(-y));
                    //        g22 = 1.0 - exp(-y) * (1.0 + y * log(y));
                    g20 = -NMath.Expm1(-y);
                    g21 = -NMath.Exp(-param.Mu / param.Theta) * (NMath.Expm1(-y) + y * NMath.Exp(-y));
                    g22 = -NMath.Expm1(-y) + NMath.Exp(-y) * y * (t - param.Mu) / param.Theta;
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
                    en2 += NMath.Exp(-t / param.Theta);
                    en3 += (t - param.Mu) / param.Theta * (1.0 - y);
                    llf += NMath.Log(dist.Pdf(t));
                }
            }
            llf += (NMath.Log(tomega) + NMath.Log(g00)) * en1;
            en1 += tomega * (1.0 - g00 + g20);
            en2 += tomega * (NMath.Exp(-param.Mu / param.Theta) - g01 + g21);
            en3 += tomega * (1.0 - g02 + g22);
            llf += -tomega * (g00 - g20);

            return llf;
        }

        private void Mstep()
        {
            double mu = param.Mu;
			double theta = param.Theta;
            param.Mu = -theta * NMath.Log(en2 / en1);
            param.Theta = theta * (en3 / en1);
			param.Omega = -en1 * NMath.Expm1(-NMath.Exp(param.Mu / param.Theta));
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

    public class TXvminEMSRM : SRMBase, IEMbase
	{
		ExtremeParam param;
		double en1, en2, en3;

		public TXvminEMSRM(ExtremeParam param) : base("TruncExtremeMin SRGM", 3, param.GetOmega, new Stat.TruncMinDist(param.GetMu, param.GetTheta))
		{
			this.param = param;
		}

		public void Initialize(SRMData data)
		{
			param.Omega = 1.0;
			param.Mu = 0.0;
			param.Theta = data.TotalTime / 3.0;
		}

		private double Estep(SRMData data)
		{
			int dsize = data.Size;
			double[] time = data.Time;
			int[] num = data.Fault;
			int[] type = data.Type;

			int j;
			int x;
			double t, y, y0;
			double tmp1, tmp2, tmp3;
			double llf;
			double g00, g01, g02;
			double g10, g11, g12;
			double g20, g21, g22;

			// E-step
			y0 = NMath.Exp(param.Mu / param.Theta);
			double tomega = param.Omega / NMath.Exp(-y0);

			en1 = 0.0;
			en2 = 0.0;
			en3 = 0.0;
			llf = 0.0;
			g00 = NMath.Exp(-y0);
			g01 = NMath.Exp(-param.Mu / param.Theta) * ((1.0 + y0) * NMath.Exp(-y0));
			g02 = NMath.Exp(-y0) * (1.0 + y0 * param.Mu / param.Theta);
			t = -time[0];
			x = num[0];
			y = NMath.Exp(-(t - param.Mu) / param.Theta);
			g10 = g00;
			g11 = g01;
			g12 = g02;
			g20 = NMath.Exp(-y);
			g21 = NMath.Exp(-param.Mu / param.Theta) * ((1.0 + y) * NMath.Exp(-y));
			g22 = NMath.Exp(-y) * (1.0 - y * (t - param.Mu) / param.Theta);
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
				en2 += NMath.Exp(-t / param.Theta);
				en3 += (t - param.Mu) / param.Theta * (1.0 - y);
				llf += NMath.Log(dist.Pdf(-t));
			}
			for (j = 1; j < dsize; j++)
			{
				x = num[j];
				if (time[j] != 0.0)
				{
					t -= time[j];
					y = NMath.Exp(-(t - param.Mu) / param.Theta);
					g10 = g20;
					g11 = g21;
					g12 = g22;
					g20 = NMath.Exp(-y);
					g21 = NMath.Exp(-param.Mu / param.Theta) * ((1.0 + y) * NMath.Exp(-y));
					g22 = NMath.Exp(-y) * (1.0 - y * (t - param.Mu) / param.Theta);
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
					en2 += NMath.Exp(-t / param.Theta);
					en3 += (t - param.Mu) / param.Theta * (1.0 - y);
					llf += NMath.Log(dist.Pdf(-t));
				}
			}
			llf += (NMath.Log(tomega) + NMath.Log(g00)) * en1;
			en1 += tomega * (1.0 - g00 + g20);
			en2 += tomega * (NMath.Exp(-param.Mu / param.Theta) - g01 + g21);
			en3 += tomega * (1.0 - g02 + g22);
			llf += -tomega * (g00 - g20);

			return llf;
		}

		private void Mstep()
		{
			double mu = param.Mu;
			double theta = param.Theta;
			param.Mu = -theta * NMath.Log(en2 / en1);
			param.Theta = theta * (en3 / en1);
			param.Omega = en1 * NMath.Exp(-NMath.Exp(param.Mu / param.Theta));
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

	public class LXvmaxEMSRM : SRMBase, IEMbase
	{
		ExtremeParam param;
		double en1, en2, en3;

		public LXvmaxEMSRM(ExtremeParam param) : base("LogExtremeMax SRGM", 3, param.GetOmega, new Stat.LogMaxDist(param.GetMu, param.GetTheta))
		{
			this.param = param;
		}

		public void Initialize(SRMData data)
		{
			param.Omega = 1.0;
			param.Mu = 1.0;
			param.Theta = NMath.Log(data.TotalTime);
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
			y = NMath.Exp(-(NMath.Log(t) - param.Mu) / param.Theta);
			en1 = 0.0;
			en2 = 0.0;
			en3 = 0.0;
			llf = 0.0;
			g00 = 1.0;
			g01 = NMath.Exp(-param.Mu / param.Theta);
			g02 = 1.0;
			// g10 = 1.0 - NMath.exp(-y);
			// g11 = NMath.exp(-mu/theta)*(1.0 - (1.0 + y)*NMath.exp(-y));
			// g12 = 1.0 - NMath.exp(-y) * (1.0 + y * NMath.log(y));
			g10 = -NMath.Expm1(-y);
			g11 = NMath.Exp(-param.Mu / param.Theta) * (-NMath.Expm1(-y) - y * NMath.Exp(-y));
			g12 = -NMath.Expm1(-y) + NMath.Exp(-y) * y * (NMath.Log(t) - param.Mu) / param.Theta;
			if (x != 0)
			{
				//    tmp1 = 1.0 - g10;
				tmp1 = NMath.Exp(-y);
				tmp2 = NMath.Exp(-param.Mu / param.Theta) - g11;
				//    tmp3 = 1.0 - g12;
				tmp3 = NMath.Exp(-y) * (1.0 + y * NMath.Log(y));
				en1 += x;
				en2 += x * tmp2 / tmp1;
				en3 += x * tmp3 / tmp1;
				llf += x * NMath.Log(tmp1) - NMath.Lgamma(x + 1.0);
			}
			if (type[0] != 0)
			{
				en1 += 1.0;
				en2 += NMath.Exp(-NMath.Log(t) / param.Theta);
				en3 += (NMath.Log(t) - param.Mu) / param.Theta * (1.0 - y);
				llf += NMath.Log(dist.Pdf(t));
			}
			for (j = 1; j < dsize; j++)
			{
				x = num[j];
				if (time[j] != 0.0)
				{
					t += time[j];
					y = NMath.Exp(-(NMath.Log(t) - param.Mu) / param.Theta);
					g00 = g10;
					g01 = g11;
					g02 = g12;
					// g10 = 1.0 - NMath.exp(-y);
					// g11 = NMath.exp(-mu/theta)*(1.0 - (1.0 + y)*NMath.exp(-y));
					// g12 = 1.0 - NMath.exp(-y) * (1.0 + y * NMath.log(y));
					g10 = -NMath.Expm1(-y);
					g11 = NMath.Exp(-param.Mu / param.Theta) * (-NMath.Expm1(-y) - y * NMath.Exp(-y));
					g12 = -NMath.Expm1(-y) + NMath.Exp(-y) * y * (NMath.Log(t) - param.Mu) / param.Theta;
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
					en2 += NMath.Exp(-NMath.Log(t) / param.Theta);
					en3 += (NMath.Log(t) - param.Mu) / param.Theta * (1.0 - y);
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
			double theta = param.Theta;
			param.Mu = -theta * NMath.Log(en2 / en1);
			param.Theta = theta * (en3 / en1);
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

	public class LXvminEMSRM : SRMBase, IEMbase
	{
		ExtremeParam param;
		double en1, en2, en3;

		public LXvminEMSRM(ExtremeParam param) : base("LogExtremeMin SRGM", 3, param.GetOmega, new Stat.LogMinDist(param.GetMu, param.GetTheta))
		{
			this.param = param;
		}

		public void Initialize(SRMData data)
		{
			param.Omega = 1.0;
			param.Mu = 1.0;
			param.Theta = NMath.Log(data.TotalTime);
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
			y = NMath.Exp((NMath.Log(t) + param.Mu) / param.Theta);
			en1 = 0.0;
			en2 = 0.0;
			en3 = 0.0;
			llf = 0.0;
			g00 = 1.0;
			g01 = NMath.Exp(-param.Mu / param.Theta);
			g02 = 1.0;
			g10 = NMath.Exp(-y);
			g11 = NMath.Exp(-param.Mu / param.Theta) * (1.0 + y) * NMath.Exp(-y);
			//  g12 = NMath.exp(-y) * (1.0 + y * NMath.log(y));
			g12 = NMath.Exp(-y) * (1.0 + y * (NMath.Log(t) + param.Mu) / param.Theta);
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
				en2 += NMath.Exp(NMath.Log(t) / param.Theta);
				en3 += -(NMath.Log(t) + param.Mu) / param.Theta * (1.0 - y);
				llf += NMath.Log(dist.Pdf(t));
			}
			for (j = 1; j < dsize; j++)
			{
				x = num[j];
				if (time[j] != 0.0)
				{
					t += time[j];
					y = NMath.Exp((NMath.Log(t) + param.Mu) / param.Theta);
					g00 = g10;
					g01 = g11;
					g02 = g12;
					g10 = NMath.Exp(-y);
					g11 = NMath.Exp(-param.Mu / param.Theta) * (1.0 + y) * NMath.Exp(-y);
					//      g12 = NMath.exp(-y) * (1.0 + y * NMath.log(y));
					g12 = NMath.Exp(-y) * (1.0 + y * (NMath.Log(t) + param.Mu) / param.Theta);
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
					en2 += NMath.Exp(NMath.Log(t) / param.Theta);
					en3 += -(NMath.Log(t) + param.Mu) / param.Theta * (1.0 - y);
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
			double theta = param.Theta;
			param.Mu = -theta * NMath.Log(en2 / en1);
			param.Theta = theta * (en3 / en1);
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

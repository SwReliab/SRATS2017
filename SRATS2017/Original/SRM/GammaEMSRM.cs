using System;
using SRATS.Utils;

namespace SRATS.MODELS
{
	public class GammaParam : OriginalParam
	{
		public double Omega
		{
			get { return param[0]; }
			set { param[0] = value; }
		}

		public double Alpha
		{
			get { return param[1]; }
			set { param[1] = value; }
		}

		public double Beta
		{
			get { return param[2]; }
			set { param[2] = value; }
		}

		public GammaParam(double omega_ = 1.0, double alpha_ = 1.0, double beta_ = 1.0) : base(3)
		{
			Omega = omega_;
			Alpha = alpha_;
			Beta = beta_;
		}

		public double GetOmega()
		{
			return Omega;
		}

		public double GetAlpha()
		{
			return Alpha;
		}

		public double GetBeta()
		{
			return Beta;
		}

		public override IModelParam Create()
		{
			return new GammaParam();
		}
	}

	public class GammaEMSRM : SRMBase, IEMbase
	{
		private GammaParam param;

		double en1;
		double en2;
		double en3;

        public double Log_integrand(double x)
        {
            return NMath.Log(x);
        }

		double newton_eps;
        Stat.GammaDist dist1;
        ExpectedValue elog;
//        LogIntegrand flog;

		private double ShapePlusOne()
		{
			return param.Alpha + 1;
		}

		public GammaEMSRM(GammaParam param) : base("Gamma SRGM", 3, param.GetOmega, new Stat.GammaDist(param.GetAlpha, param.GetBeta))
		{
			this.param = param;
			dist1 = new Stat.GammaDist(ShapePlusOne, param.GetBeta);
			elog = new ExpectedValue(dist, 15, 1.0e-8);
//			flog = new LogIntegrand();
			newton_eps = 1.0e-8;
		}

        public void Initialize(SRMData data)
        {
			param.Omega = data.TotalFaults;
			param.Alpha = 1.0;
			param.Beta = 2.0 / data.TotalTime;
        }

        private double Estep(SRMData data)
        {
			int dsize = data.Size;
			double[] time = data.Time;
			int[] num = data.Fault;
			int[] type = data.Type;

            int j;
            double t0;
            int x1;
            double t1;
			double tmp1, tmp2, tmp3, tmp4, llf;
            double gam10, gam11, gam20, gam21;

            // E-step
            en1 = 0.0;
            en2 = 0.0;
            en3 = 0.0;
            llf = 0.0;

            t0 = 0.0;
            t1 = time[0];
            x1 = num[0];
            gam10 = 1.0;
            gam11 = 1.0;
            gam20 = dist.Ccdf(t1);
            gam21 = dist1.Ccdf(t1);
            tmp3 = elog.Interval(Log_integrand, 1.0e-10, t1);
            tmp4 = tmp3;
            if (x1 != 0)
            {
                tmp1 = gam10 - gam20;
                tmp2 = (param.Alpha / param.Beta) * (gam11 - gam21);
                en1 += x1;
                en2 += x1 * tmp2 / tmp1;
                en3 += x1 * tmp3 / tmp1;
                llf += x1 * NMath.Log(tmp1) - NMath.Lgamma(x1 + 1.0);
            }
            if (type[0] != 0)
            {
                en1 += 1.0;
                en2 += t1;
                en3 += NMath.Log(t1);
                llf += param.Alpha * NMath.Log(param.Beta) + (param.Alpha - 1.0) * NMath.Log(t1)
                - param.Beta * t1 - NMath.Lgamma(param.Alpha);
            }
            for (j = 1; j < dsize; j++)
            {
                x1 = num[j];
                if (time[j] != 0.0)
                {
                    t0 = t1;
                    t1 = t0 + time[j];
                    gam10 = gam20;
                    gam11 = gam21;
                    gam20 = dist.Ccdf(t1);
                    gam21 = dist1.Ccdf(t1);
                    tmp3 = elog.Interval(Log_integrand, t0, t1);
                    tmp4 += tmp3;
                }
                if (x1 != 0)
                {
                    tmp1 = gam10 - gam20;
                    tmp2 = (param.Alpha / param.Beta) * (gam11 - gam21);
                    en1 += x1;
                    en2 += x1 * tmp2 / tmp1;
                    en3 += x1 * tmp3 / tmp1;
                    llf += x1 * NMath.Log(tmp1) - NMath.Lgamma(x1 + 1.0);
                }
                if (type[j] != 0)
                {
                    en1 += 1.0;
                    en2 += t1;
                    en3 += NMath.Log(t1);
                    llf += param.Alpha * NMath.Log(param.Beta) + (param.Alpha - 1.0) * NMath.Log(t1)
                    - param.Beta * t1 - NMath.Lgamma(param.Alpha);
                }
            }
            llf += NMath.Log(param.Omega) * en1;
            en1 += param.Omega * gam20;
            en2 += param.Omega * (param.Alpha / param.Beta) * gam21;
            en3 += param.Omega * (NMath.Psi(param.Alpha) - NMath.Log(param.Beta) - tmp4);
            llf += -param.Omega * (1.0 - gam20);

            return llf;
        }

        private void Mstep()
        {
            int cnt1, cnt2;
            double x, xn, f, df, step;
            double v = NMath.Log(en2 / en1) - en3 / en1;
            x = param.Alpha;
            cnt1 = 0;
            do
            {
                f = NMath.Log(x) - NMath.Psi(x) - v;
                df = 1.0 / x - NMath.Polygamma(1, x);
                step = 1.0;
                cnt2 = 0;
                do
                {
                    xn = x - step * f / df;
                    if (xn > 0.0) break;
                    step *= 0.5;
                    if (cnt2 >= 1000)
                    {
                        xn = double.NaN;
                        goto FINAL;
                    }
                    cnt2++;
                } while (true);
                if (double.IsNaN(x) || double.IsInfinity(x))
                {
                    throw new NotFiniteNumberException();
                }
                if (cnt1 >= 1000)
                {
                    xn = double.NaN;
                    goto FINAL;
                }
                cnt1++;
                if (NMath.Abs(xn - x) < newton_eps * NMath.Abs(x))
                {
                    break;
                }
                x = xn;
            } while (true);
        FINAL:
			param.Alpha = xn;
            param.Omega = en1;
			param.Beta = param.Alpha * en1 / en2;
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

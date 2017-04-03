using System;
using SRATS.Utils;

namespace SRATS.MODELS
{
	public class ExpParam : OriginalParam
	{
		public double Omega
		{
			get
			{
				return param[0];
			}

			set
			{
				param[0] = value;
			}
		}

		public double Lambda
		{
			get
			{
				return param[1];
			}

			set
			{
				param[1] = value;
			}
		}

		public ExpParam(double a = 1.0, double b = 1.0) : base(2)
		{
			Omega = a;
			Lambda = b;
		}

		public double GetOmega()
		{
			return Omega;
		}

		public double GetLambda()
		{
			return Lambda;
		}

		public override IModelParam Create()
		{
			return new ExpParam();
		}
	}

	public class ExpEMSRM : SRMBase, IEMbase
    {
		private ExpParam param;
		double en1;
		double en2;

		public ExpEMSRM(ExpParam param) : base("Exponential SRGM", 2, param.GetOmega, new Stat.ExpDist(param.GetLambda))
		{
			this.param = param;
		}

		public override IModelParam GetParam()
		{
			return param;
		}

		public void Initialize(SRMData data)
		{
			param.Omega = data.TotalFaults;
			param.Lambda = 2.0 / data.TotalTime;
		}

		public double Emstep(SRMData data)
		{
			double llf = Estep(data);
			Mstep();
			return llf;
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
            double tmp1, tmp2, llf;

            // E-step
            t0 = 0.0;
            t1 = time[0];
            x1 = num[0];
            en1 = 0.0;
            en2 = 0.0;
            llf = 0.0;
            if (x1 != 0)
            {
                tmp1 = 1 - NMath.Exp(-param.Lambda * t1);
                tmp2 = 1.0 / param.Lambda - (t1 + 1 / param.Lambda) * NMath.Exp(-param.Lambda * t1);
                en1 = x1;
                en2 = x1 * tmp2 / tmp1;
                llf = x1 * NMath.Log(tmp1) - NMath.Lgamma(x1 + 1.0);
            }
            if (type[0] != 0)
            {
                en1 += 1.0;
                en2 += t1;
                llf += NMath.Log(param.Lambda) - param.Lambda * t1;
            }
            for (j = 1; j < dsize; j++)
            {
                if (time[j] != 0.0)
                {
                    t0 = t1;
                    t1 = t0 + time[j];
                }
                x1 = num[j];
                if (x1 != 0)
                {
                    tmp1 = NMath.Exp(-param.Lambda * t0) - NMath.Exp(-param.Lambda * t1);
                    tmp2 = (t0 + 1.0 / param.Lambda) * NMath.Exp(-param.Lambda * t0)
                    - (t1 + 1.0 / param.Lambda) * NMath.Exp(-param.Lambda * t1);
                    en1 += x1;
                    en2 += x1 * tmp2 / tmp1;
                    llf += x1 * NMath.Log(tmp1) - NMath.Lgamma(x1 + 1.0);
                }
                if (type[j] != 0)
                {
                    en1 += 1.0;
                    en2 += t1;
                    llf += NMath.Log(param.Lambda) - param.Lambda * t1;
                }
            }
            llf += NMath.Log(param.Omega) * en1;  // en1 is total number of faults
            en1 += param.Omega * NMath.Exp(-param.Lambda * t1);  // t1 is the last time
            en2 += param.Omega * (t1 + 1.0 / param.Lambda) * NMath.Exp(-param.Lambda * t1); // t1 is the last time
            llf += -param.Omega * (1.0 - NMath.Exp(-param.Lambda * t1));

            return llf;
        }

		private void Mstep()
        {
            param.Omega = en1;
            param.Lambda = en1 / en2;
        }

		public void Pre_em(SRMData data)
		{
		}

		public void Post_em(SRMData data)
		{
		}
	}
}

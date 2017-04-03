using System;
using SRATS.Utils;

namespace SRATS.HErlang
{
	public class HErlangEMSRM : SRMBase, IEMbase
	{
		HErlangParam param;

		private double[] tmp1;
		private double[] tmp2;
		private double[] a0;
		private double[] a1;
		private double[] gam10;
		private double[] gam11;
		private double[] gam20;
		private double[] gam21;
		private double en1;
		private double[] en2;
		private double[] en3;

		public HErlangEMSRM(HErlangParam param) : base("(" + Blas.ArrayToString(param.Shape, "G", ",") + ")-HyperErlang SRGM", param.M, param.GetOmega, new HErlangDist(param.M, param.GetAlpha, param.GetShape, param.GetRate))
		{
			this.param = param;
			tmp1 = new double[param.M];
			tmp2 = new double[param.M];
			a0 = new double[param.M];
			a1 = new double[param.M];
			gam10 = new double[param.M];
			gam11 = new double[param.M];
			gam20 = new double[param.M];
			gam21 = new double[param.M];
			en2 = new double[param.M];
			en3 = new double[param.M];
		}

		public void Initialize(SRMData data)
		{
			Random rnd = new Random();
			param.Omega = data.TotalFaults;
			double[] tmp = new double[param.M];
			double sum = 0.0;
			for (int i = 0; i < param.M; i++)
			{
				sum += tmp[i] = rnd.NextDouble();
				param.Rate[i] = param.Shape[i] / data.TotalTime;
			}
			for (int i = 0; i < param.M; i++)
			{
				param.Alpha[i] = tmp[i] / sum;
			}
		}

		private double Ipdf(int i, double x)
		{
			double y = param.Rate[i] * x;
			return param.Rate[i] * NMath.Pow(y, param.Shape[i] - 1) * NMath.Exp(-y - NMath.Lfact(param.Shape[i] - 1));
		}

		public double Emstep(SRMData data)
		{
			int dsize = data.Size;
			double[] time = data.Time;
			int[] num = data.Fault;
			int[] type = data.Type;

			int x1;
			double t1;
			double llf, tmp1sum;

			// E-step
			// initialize
			for (int i = 0; i < param.M; i++)
			{
				a0[i] = NMath.Lgamma(param.Shape[i]);
				a1[i] = NMath.Lgamma(param.Shape[i] + 1.0);
			}
			en1 = 0.0;
			for (int i = 0; i < param.M; i++)
			{
				en2[i] = 0.0;
				en3[i] = 0.0;
			}
			llf = 0.0;

			t1 = time[0]; //dat.getTime(1);
			x1 = num[0]; //dat.getNumber(1);
			for (int i = 0; i < param.M; i++)
			{
				gam10[i] = NMath.Q_gamma(param.Shape[i], param.Rate[i] * t1, a0[i]);
				gam11[i] = NMath.Q_gamma(param.Shape[i] + 1, param.Rate[i] * t1, a1[i]);
			}
			if (x1 != 0)
			{
				tmp1sum = 0.0;
				for (int i = 0; i < param.M; i++)
				{
					tmp1[i] = 1.0 - gam10[i];
					tmp1sum += param.Alpha[i] * tmp1[i];
					tmp2[i] = param.Shape[i] / param.Rate[i] * (1.0 - gam11[i]);
				}
				en1 += x1;
				for (int i = 0; i < param.M; i++)
				{
					en2[i] += x1 * param.Alpha[i] * tmp1[i] / tmp1sum;
					en3[i] += x1 * param.Alpha[i] * tmp2[i] / tmp1sum;
				}
				llf = x1 * NMath.Log(param.Omega * tmp1sum) - NMath.Lgamma(x1 + 1.0);
			}
			if (type[0] == 1) // dat.getType(1) == 1)
			{
				tmp1sum = 0.0;
				for (int i = 0; i < param.M; i++)
				{
					tmp1[i] = Ipdf(i, t1);
					tmp1sum += param.Alpha[i] * tmp1[i];
				}
				en1 += 1.0;
				for (int i = 0; i < param.M; i++)
				{
					en2[i] += param.Alpha[i] * tmp1[i] / tmp1sum;
					en3[i] += t1 * param.Alpha[i] * tmp1[i] / tmp1sum;
				}
				llf += NMath.Log(param.Omega * tmp1sum);
			}
			for (int j = 2; j <= dsize; j++)
			{
				t1 += time[j-1]; //dat.getTime(j);
				x1 = num[j-1]; //dat.getNumber(j);
				for (int i = 0; i < param.M; i++)
				{
					gam20[i] = NMath.Q_gamma(param.Shape[i], param.Rate[i] * t1, a0[i]);
					gam21[i] = NMath.Q_gamma(param.Shape[i] + 1, param.Rate[i] * t1, a1[i]);
				}
				if (x1 != 0)
				{
					tmp1sum = 0.0;
					for (int i = 0; i < param.M; i++)
					{
						tmp1[i] = gam10[i] - gam20[i];
						tmp1sum += param.Alpha[i] * tmp1[i];
						tmp2[i] = (param.Shape[i] / param.Rate[i]) * (gam11[i] - gam21[i]);
					}
					en1 += x1;
					for (int i = 0; i < param.M; i++)
					{
						en2[i] += x1 * param.Alpha[i] * tmp1[i] / tmp1sum;
						en3[i] += x1 * param.Alpha[i] * tmp2[i] / tmp1sum;
					}
					llf += x1 * NMath.Log(param.Omega * tmp1sum) - NMath.Lgamma(x1 + 1);
				}
				if (type[j-1] == 1) //dat.getType(j) == 1)
				{
					tmp1sum = 0.0;
					for (int i = 0; i < param.M; i++)
					{
						tmp1[i] = Ipdf(i, t1);
						tmp1sum += param.Alpha[i] * tmp1[i];
					}
					en1 += 1.0;
					for (int i = 0; i < param.M; i++)
					{
						en2[i] += param.Alpha[i] * tmp1[i] / tmp1sum;
						en3[i] += t1 * param.Alpha[i] * tmp1[i] / tmp1sum;
					}
					llf += NMath.Log(param.Omega * tmp1sum);
				}
				for (int i = 0; i < param.M; i++)
				{
					gam10[i] = gam20[i];
					gam11[i] = gam21[i];
				}
			}
			tmp1sum = 0.0;
			for (int i = 0; i < param.M; i++)
			{
				tmp1sum += param.Alpha[i] * gam10[i];      // gam10 is the last time
				en2[i] += param.Omega * param.Alpha[i] * gam10[i];  // gam10 is the last time
				en3[i] += param.Omega * param.Alpha[i] * (param.Shape[i] / param.Rate[i]) * gam11[i];  // gam11 is the last time
			}
			en1 += param.Omega * tmp1sum;
			llf += -param.Omega * (1.0 - tmp1sum);

			// M-step
			param.Omega = en1;
			for (int i = 0; i < param.M; i++)
			{
				param.Alpha[i] = en2[i] / en1;
				param.Rate[i] = param.Shape[i] * en2[i] / en3[i];
			}
			return llf;
		}

		public override IModelParam GetParam()
		{
			return param;
		}

		public void Pre_em(SRMData data)
		{
		}

		public void Post_em(SRMData data)
		{
		}
	}
}

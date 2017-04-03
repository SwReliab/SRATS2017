using SRATS.Utils;

namespace SRATS
{
	abstract public class SRM
	{
		private readonly string name;
		protected int npara;

		protected SRM(string name, int n)
		{
			this.name = name;
			npara = n;
		}

		public int NParam()
		{
			return npara;
		}

		public virtual string GetModelName()
		{
			return name;
		}

		//		public abstract void fit(Data.SRMFData data);
		public abstract Result Calc(SRMData data);
		public abstract GraphData GraphData(double[] mvftime, double[] relitime);
		public abstract MVFData MvfData(double[] mvftime);
		public abstract IModelParam GetParam();
	}

	abstract public class SRMBase : SRM
	{
		GetParam omegaFunc;

		private double Omega
		{
			get { return omegaFunc(); }
		}

		protected Stat.SDist dist;

		protected SRMBase(string name, int n, GetParam omega, Stat.SDist dist) : base(name, n)
		{
			omegaFunc = omega;
			this.dist = dist;
		}

		protected virtual void CalcMVF(double[] t, double[] mvf)
		{
			for (int i = 0; i < t.Length; i++)
			{
				mvf[i] = Omega * dist.Cdf(t[i]);
			}
		}

		protected virtual void CalcIntensity(double[] t, double[] intensity)
		{
			for (int i = 0; i < t.Length; i++)
			{
				intensity[i] = Omega * dist.Pdf(t[i]);
			}
		}

		private double Reliab(double t, double s, double mvf)
		{
			return NMath.Exp(-(Omega * dist.Cdf(t + s) - mvf));
		}

		private double Bexlife(double te, double cdf, double ffp, double p)
		{
			if (ffp > p)
			{
				return double.NaN;
			}
			return dist.Quantile(cdf - NMath.Log(p) / Omega, NMath.ZERO) - te;
		}

		private double ConBexlife(double te, double cdf, double ffp, double p)
		{
			return Bexlife(te, cdf, ffp, p + ffp * (1.0 - p));
		}

		public override Result Calc(SRMData data)
		{
			Result result = new Result();
			double te = data.TotalTime;
			double me = data.TotalFaults;

			int dsize = 0;
			double[] t = new double[data.Size];
			double[] x = new double[data.Size];
			double[] w = new double[data.Size];
			for (int i = 0; i < data.Size; i++)
			{
				if (data.Time[i] > 0)
				{
					t[dsize] = data.Time[i];
					x[dsize] = data.Fault[i];
					w[dsize] = data.Type[i];
					dsize++;
				}
				else
				{
					w[dsize] += data.Fault[i];
					w[dsize] += data.Type[i];
				}
			}

			double ctime = 0.0;
			double prevmvf = 0.0;
			double cfault = 0.0;
			double csum = 0.0;
			double dsum = 0.0;
			double llf = 0.0;
			double cmvf = 0.0;
			double pdf = 0.0;
			double dmvf = 0.0;
			for (int i = 0; i < dsize; i++)
			{
				ctime += t[i];
				cfault += x[i];
				cfault += w[i];

				cmvf = Omega * dist.Cdf(ctime);
				pdf = Omega * dist.Pdf(ctime);
				dmvf = cmvf - prevmvf;
				prevmvf = cmvf;

				if (x[i] > 0)
				{
					llf += x[i] * NMath.Log(dmvf) - NMath.Lgamma(x[i] + 1);
				}
				if (w[i] > 0)
				{
					llf += w[i] * NMath.Log(pdf);
				}
				csum += (cmvf - cfault) * (cmvf - cfault);
				dsum += (dmvf - (x[i] + w[i])) * (dmvf - (x[i] + w[i]));
			}
			llf += -cmvf;

			result.Llf = llf;
			result.Aic = -2 * result.Llf + 2 * NParam();
			result.Bic = -2 * result.Llf + NParam() * NMath.Log(dsize);
			result.CMse = csum / dsize;
			result.CMsee = csum / (dsize - NParam());
			result.DMse = dsum / dsize;
			result.DMsee = dsum / (dsize - NParam());

			result.Residual = Omega - cmvf;
			result.Total = me + result.Residual;
			result.Ffp = NMath.Exp(-result.Residual);
			result.CMttf = te / cmvf;
			result.IMttf = 1.0 / pdf;

			result.Bexlife = Bexlife(te, cmvf / Omega, result.Ffp, 0.1);
			result.Median = Bexlife(te, cmvf / Omega, result.Ffp, 0.5);
			result.ConBexlife = ConBexlife(te, cmvf / Omega, result.Ffp, 0.1);
			result.ConMedian = ConBexlife(te, cmvf / Omega, result.Ffp, 0.5);

			MTTFValue mttf = new MTTFValue(Reliab, 1.0e-8);
			result.ConMttf = mttf.Calc(te, cmvf, result.Ffp, ConBexlife(te, cmvf / Omega, result.Ffp, 1.0e-6));

			return result;
		}

		public override GraphData GraphData(double[] mvftime, double[] relitime)
		{
			double[] mvf = new double[mvftime.Length];
			double[] intensity = new double[mvftime.Length];
			double[] reli = new double[relitime.Length];

			for (int i = 0; i < mvftime.Length; i++)
			{
				mvf[i] = Omega * dist.Cdf(mvftime[i]);
				intensity[i] = Omega * dist.Pdf(mvftime[i]);
			}

			double te = relitime[0];
			double temvf = Omega * dist.Cdf(te);
			for (int i = 0; i < relitime.Length; i++)
			{
				reli[i] = Reliab(relitime[i], te, temvf);
			}

			return new GraphData(mvftime, mvf, intensity, relitime, reli);
		}

		public override MVFData MvfData(double[] mvftime)
		{
			double[] mvf = new double[mvftime.Length];

			for (int i = 0; i < mvftime.Length; i++)
			{
				mvf[i] = Omega * dist.Cdf(mvftime[i]);
			}

			return new MVFData(mvftime, mvf);
		}
	}
}

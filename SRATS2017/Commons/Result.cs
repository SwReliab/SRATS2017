using System;

namespace SRATS
{
	public class Result
	{
		public double te;
		public int me;

		public double Llf { get; set; } // maxmum log-likelihood
		public double Aic { get; set; } // AIC
		public double Bic { get; set; } // BIC
		public double CMse { get; set; } // mean squared errors
		public double CMsee { get; set; } // mean squared errors (divided by the degrees of freedom)
		public double DMse { get; set; } // mean squared errors
		public double DMsee { get; set; } // mean squared errors (divided by the degrees of freedom)

		public double Ffp { get; set; } // Fault free probability
		public double Total { get; set; } // expected number of faults
		public double Residual { get; set; }
		public double CMttf { get; set; }
		public double IMttf { get; set; }
		public double ConMttf { get; set; }
		public double Bexlife { get; set; }
		public double Median { get; set; }
		public double ConBexlife { get; set; }
		public double ConMedian { get; set; }

		public double[] Time { get; set; }
		public double[] Mvf { get; set; }
		public double[] Dmvf { get; set; }

		public Result()
		{
			//this.srm = srm;
			//this.data = data;

		}

  //      public double calcMVF(double[] time)
		//{
		//	this.time = new double[time.Length];
		//	Array.Copy(time, this.time, time.Length);
		//	srm.calcMVF(time, mvf, dmvf);
		//}

		//public void calc(double[] time, double[] num)
		//{
			

		//public double residual(double t)
		//{
		//	return omega * dist.ccdf(t);
		//}

		//public double reliab(double s, double t)
		//{
		//	return NMath.exp(-(mvf(t + s) - mvf(s)));
		//}

		//public double ffp(double t)
		//{
		//	return NMath.exp(-omega * dist.ccdf(t));
		//}

		//public double cmttf(double t)
		//{
		//	return t / mvf(t);
		//}

		//public double imttf(double t)
		//{
		//	return 1.0 / dmvf(t);
		//}

		//public double conmttf(double t)
		//{
		//	return mttf.calc(t);
		//}

		//public double bexlife(double t, double p)
		//{
		//	if (ffp(t) > p)
		//	{
		//		return double.NaN;
		//	}
		//	return dist.quantile(dist.cdf(t) - NMath.log(p) / omega) - t;
		//}

		//public double conbexlife(double t, double p)
		//{
		//	return bexlife(t, p + ffp(t) * (1.0 - p));
		//}

		//public double total()
		//{
		//	return data.TotalFaults + residual(data.TotalTime);
		//}

		//public double calcllf()
		//{
		//	double t0 = 0.0, t1 = 0.0;
		//	double resllf = 0.0;
		//	double d;

		//	int dsize = data.Size;
		//	double[] time = data.Time.ptr();
		//	double[] num = data.Fault.ptr();
		//	double[] type = data.Type.ptr();

		//	for (int k = 0; k < dsize; k++)
		//	{
		//		if (time[k] > 0.0)
		//		{
		//			t1 = t0 + time[k];
		//		}
		//		if ((int)num[k] != 0)
		//		{
		//			if (t0 == 0.0)
		//			{
		//				d = dist.cdf(t1);
		//			}
		//			else
		//			{
		//				d = dist.cdf(t1) - dist.cdf(t0);
		//			}
		//			resllf += num[k] * NMath.log(omega) + num[k] * NMath.log(d)
		//			  - NMath.lgamma(num[k] + 1.0);
		//		}
		//		if ((int)type[k] != 0)
		//		{
		//			d = dist.pdf(t1);
		//			resllf += type[k] * NMath.log(omega) + type[k] * NMath.log(d);
		//		}
		//		t0 = t1;
		//	}
		//	resllf += -omega * dist.cdf(t0);
		//	return resllf;
		//}
	}
}

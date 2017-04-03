using System;

namespace SRATS
{
	public class MVFData
	{
		protected double[] mvftime;
		protected double[] mvf;

		public MVFData(double[] mvftime, double[] mvf)
		{
			this.mvftime = mvftime;
			this.mvf = mvf;
		}

		public double[] MVFTime
		{
			get { return mvftime; }
		}

		public double[] MVF
		{
			get { return mvf; }
		}
	}
}

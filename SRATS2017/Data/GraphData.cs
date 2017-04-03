using System;

namespace SRATS
{
	public class GraphData
	{
		protected double[] mvftime;
		protected double[] mvf;

		protected double[] intensity;

		protected double[] relitime;
		protected double[] reli;

		public GraphData(double[] mvftime, double[] mvf, double[] intensity, double[] relitime, double[] reli)
		{
			this.mvftime = mvftime;
			this.mvf = mvf;
			this.intensity = intensity;
			this.relitime = relitime;
			this.reli = reli;
		}

		public double[] MVFTime
		{
			get { return mvftime; }
		}

		public double[] MVF
		{
			get { return mvf; }
		}

		public double[] Intensity
		{
			get { return intensity; }
		}

		public double[] ReliTime
		{
			get { return relitime; }
		}

		public double[] Reli
		{
			get { return reli; }
		}
	}
}

using System;
using System.Collections.Generic;

namespace SRATS
{
    public class SRMData
    {
        protected int dsize;
		//protected List<double> time;
		//protected List<int> fault;
		//protected List<int> type;

		protected double[] time;
		protected int[] fault;
		protected int[] type;

		// statistics
		protected double totalTime;
        protected double totalFaults;
        protected double maxTime;
        protected double minTime;
        protected double meanTime;

		public bool CheckData()
        {
			if (!(dsize >= 1 && time.Length == dsize && fault.Length == dsize && type.Length == dsize))
			{
				return false;
			}
			for (int i = 0; i < dsize; i++)
			{
				if (!(time[i] >= 0 && fault[i] >= 0 && (type[i] == 0 || type[i] == 1)))
				{
					return false;
				}
			}
			return true;
        }

        public void SetData(double[] time, int[] fault, int[] type)
        {
			dsize = time.Length;
			this.time = time;
			this.fault = fault;
			this.type = type;
			CalcStatistics();
        }

		private double Dsum(int n, double[] x)
		{
			double sum = 0;
			for (int i = 0; i < n; i++)
			{
				sum += x[i];
			}
			return sum;
		}

		private int Isum(int n, int[] x)
		{
			int sum = 0;
			for (int i = 0; i < n; i++)
			{
				sum += x[i];
			}
			return sum;
		}

        private void CalcStatistics()
        {
			totalTime = Dsum(dsize, time);
			totalFaults = Isum(dsize, fault) + Isum(dsize, type);
            maxTime = Dsum(dsize, time);
            minTime = time[0];
            double ct = 0.0;
            double s = 0.0;
            for (int i = 0; i < dsize; i++)
            {
                ct += time[i];
                s += (fault[i] + type[i]) * ct;
            }
            meanTime = s / totalFaults;
        }

		public int Size
		{
			get
			{
				return dsize;
			}
		}

		public double[] Time
		{
			get
			{
				return time;
			}
		}

        public int[] Fault
		{
			get
			{
				return fault;
			}
		}

        public int[] Type
		{
			get
			{
				return type;
			}
		}

        public double TotalTime
        {
			get { return totalTime; }
        }

        public double TotalFaults
        {
			get { return totalFaults; }
        }

        public double MaxTime
        {
			get { return maxTime; }
        }

        public double MinTime
        {
			get { return minTime; }
        }

        public double MeanTime
        {
			get { return meanTime; }
        }
    }
}

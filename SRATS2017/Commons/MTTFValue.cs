using SRATS.Utils;

namespace SRATS
{
	public delegate double ReliabFunc(double t, double te, double mvf);

	class MTTFValue
    {
		ReliabFunc reliab;
        double te;
        double ffp;
		double mvf;

        //int inte_divide;
		readonly double inte_eps;
		//double[] gx;
  //      double[] gw;

		public MTTFValue(ReliabFunc reliab, double eps)
        {
			this.reliab = reliab;
            //inte_divide = divide;
            inte_eps = eps;
            //gx = new double[divide];
            //gw = new double[divide];
            //NMath.GaussWeights(gx, gw, inte_eps);
        }

        public double ConReliab(double t)
        {
            return (reliab(t, te, mvf) - ffp) / (1.0 - ffp);
        }

        public double Calc(double te, double mvf, double ffp, double ub)
        {
            double err;
            this.te = te;
			this.ffp = ffp;
			this.mvf = mvf;
            return NMath.Deintde(ConReliab, 0.0, ub, out err, inte_eps);
        }
    }
}

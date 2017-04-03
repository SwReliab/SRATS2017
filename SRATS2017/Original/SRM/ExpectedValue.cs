using SRATS.Utils;

namespace SRATS.MODELS
{
    class ExpectedValue
    {
        NMath.Integrand m_func;
        Stat.SDist dist;

//        int inte_divide;
		readonly double inte_eps;
		double[] gx;
        double[] gw;

        public ExpectedValue(Stat.SDist dist,
            int divide, double eps)
        {
            this.dist = dist;
//            inte_divide = divide;
            inte_eps = eps;
            gx = new double[divide];
            gw = new double[divide];
            NMath.GaussWeights(gx, gw, inte_eps);
        }

        public double Value(double x)
        {
            return m_func(x) * dist.Pdf(x);
        }

        public double Interval(NMath.Integrand f, double a, double b)
        {
			m_func = f;
            return NMath.Gauss(Value, a, b, gx, gw);
        }

        public double Infinity(NMath.Integrand f, double a)
        {
            double err;
            m_func = f;
            return NMath.Deintdei(Value, a, out err, inte_eps);
        }
    }
}

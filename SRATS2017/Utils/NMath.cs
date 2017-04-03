using System;

namespace SRATS.Utils
{
    public static class NMath
    {
        static int FACTMAX;
        static double PI;
        static double LOG_2PI;
		//        static double LOG_PI;

		static public double ZERO = 1.0e-14;

        static int N;
        static double B2;
        static double B4;
        static double B6;
        static double B8;
        static double B10;
        static double B12;
        static double B14;
        static double B16;

        static double[] nfact;
        static double[] lognfact;

        static NMath()
        {
            FACTMAX = 20;
            PI = 3.14159265358979324; // pi
            LOG_2PI = 1.83787706640934548;
            //            LOG_PI = 1.14472988584940017; // log(pi)

            N = 8;
            //static double B0 = 1;
            //static double B1 = (-1.0 / 2.0);
            B2 = (1.0 / 6.0);
            B4 = (-1.0 / 30.0);
            B6 = (1.0 / 42.0);
            B8 = (-1.0 / 30.0);
            B10 = (5.0 / 66.0);
            B12 = (-691.0 / 2730.0);
            B14 = (7.0 / 6.0);
            B16 = (-3617.0 / 510.0);

            nfact = new double[FACTMAX + 1];

            nfact[0] = 1.0;                        // 0
            nfact[1] = 1.0;                        // 1
            nfact[2] = 2.0;                        // 2
            nfact[3] = 6.0;                        // 3
            nfact[4] = 24.0;                       // 4
            nfact[5] = 120.0;                      // 5
            nfact[6] = 720.0;                      // 6
            nfact[7] = 5040.0;                     // 7
            nfact[8] = 40320.0;                    // 8
            nfact[9] = 362880.0;                   // 9
            nfact[10] = 3628800.0;                  // 10
            nfact[11] = 39916800.0;                 // 11
            nfact[12] = 479001600.0;                // 12
            nfact[13] = 6227020800.0;               // 13
            nfact[14] = 87178291200.0;              // 14
            nfact[15] = 1307674368000.0;            // 15
            nfact[16] = 20922789888000.0;           // 16
            nfact[17] = 355687428096000.0;          // 17
            nfact[18] = 6402373705728000.0;         // 18
            nfact[19] = 121645100408832000.0;       // 19
            nfact[20] = 2432902008176640000.0;       // 20

            lognfact = new double[FACTMAX + 1];
            lognfact[0] = 0.0;
            lognfact[1] = 0.0;
            lognfact[2] = 0.6931471805599453;
            lognfact[3] = 1.791759469228055;
            lognfact[4] = 3.1780538303479458;
            lognfact[5] = 4.787491742782046;
            lognfact[6] = 6.579251212010101;
            lognfact[7] = 8.525161361065415;
            lognfact[8] = 10.60460290274525;
            lognfact[9] = 12.801827480081469;
            lognfact[10] = 15.104412573075516;
            lognfact[11] = 17.502307845873887;
            lognfact[12] = 19.987214495661885;
            lognfact[13] = 22.552163853123425;
            lognfact[14] = 25.19122118273868;
            lognfact[15] = 27.89927138384089;
            lognfact[16] = 30.671860106080672;
            lognfact[17] = 33.50507345013689;
            lognfact[18] = 36.39544520803305;
            lognfact[19] = 39.339884187199495;
            lognfact[20] = 42.335616460753485;
        }

        static public double Exp(double x)
        {
            return Math.Exp(x);
        }

		static public double Abs(double x)
		{
			return Math.Abs(x);
		}

        static public double Log(double x)
        {
            return Math.Log(x);
        }

        static public double Sqrt(double x)
        {
            return Math.Sqrt(x);
        }

        static public double Pow(double x, double y)
		{
			return Math.Pow(x, y);
		}

        static public double Max(params double[] x)
		{
			double maxx = x[0];
			for (int i = 1; i < x.Length; i++)
			{
				if (maxx < x[i])
				{
					maxx = x[i];
				}
			}
			return maxx;
		}

        static public double Min(params double[] x)
		{
			double minx = x[0];
			for (int i = 1; i < x.Length; i++)
			{
				if (minx > x[i])
				{
					minx = x[i];
				}
			}
			return minx;
		}

		static public double Expm1(double x)
        {
            if (Math.Abs(x) < 1.0e-5)
            {
                return x + 0.5 * x * x;
            }
            else
            {
                return Exp(x) - 1.0;
            }
        }

        static public double Lgamma(double x)
        {
            double v, w;
            v = 1;
            while (x < N) { v *= x; x++; }
            w = 1 / (x * x);
            return ((((((((B16 / (16 * 15)) * w + (B14 / (14 * 13))) * w
            + (B12 / (12 * 11))) * w + (B10 / (10 * 9))) * w
            + (B8 / (8 * 7))) * w + (B6 / (6 * 5))) * w
            + (B4 / (4 * 3))) * w + (B2 / (2 * 1))) / x
            + 0.5 * LOG_2PI - Log(v) - x + (x - 0.5) * Log(x);
        }

        static public double Tgamma(double x)
        {
            if (x < 0)
            {
                return PI / (Math.Sin(PI * x) * Exp(Lgamma(1 - x)));
            }
            return Exp(Lgamma(x));
        }

        static public double Psi(double x)
        {
            double v, w;
            v = 0;
            while (x < N) { v += 1 / x; x++; }
            w = 1 / (x * x);
            v += ((((((((B16 / 16) * w + (B14 / 14)) * w
            + (B12 / 12)) * w + (B10 / 10)) * w
            + (B8 / 8)) * w + (B6 / 6)) * w
            + (B4 / 4)) * w + (B2 / 2)) * w + 0.5 / x;
            return Log(x) - v;
        }

        static public double Polygamma(int n, double x)
        {
            int k;
            double t, u, v, w;
            u = 1;
            for (k = 1 - n; k < 0; k++) u *= k;
            v = 0;
            while (x < N) { v += 1 / Math.Pow(x, n + 1); x++; }
            w = x * x;
            t = (((((((B16
            * (n + 15.0) * (n + 14) / (16 * 15 * w) + B14)
            * (n + 13.0) * (n + 12) / (14 * 13 * w) + B12)
            * (n + 11.0) * (n + 10) / (12 * 11 * w) + B10)
            * (n + 9.0) * (n + 8) / (10 * 9 * w) + B8)
            * (n + 7.0) * (n + 6) / (8 * 7 * w) + B6)
            * (n + 5.0) * (n + 4) / (6 * 5 * w) + B4)
            * (n + 3.0) * (n + 2) / (4 * 3 * w) + B2)
            * (n + 1.0) * n / (2 * 1 * w)
            + 0.5 * n / x + 1;
            return u * (t / Math.Pow(x, n) + n * v);
        }

        static public double Tfact(int s)
        {
            if (s <= FACTMAX)
            {
                return nfact[s];
            }
            else
            {
                return Exp(Lgamma(1.0 + s));
            }
        }

        static public double Lfact(int s)
        {
            if (s <= FACTMAX)
            {
                return lognfact[s];
            }
            else
            {
                return Lgamma(1.0 + s);
            }
        }

        /// error functions
        static public double Derf(double x)
        {
            int k;
            double w, t, y;
            double[] a = {
                5.958930743e-11, -1.13739022964e-9, 
                1.466005199839e-8, -1.635035446196e-7, 
                1.6461004480962e-6, -1.492559551950604e-5, 
                1.2055331122299265e-4, -8.548326981129666e-4, 
                0.00522397762482322257, -0.0268661706450773342, 
                0.11283791670954881569, -0.37612638903183748117, 
                1.12837916709551257377, 
                2.372510631e-11, -4.5493253732e-10, 
                5.90362766598e-9, -6.642090827576e-8, 
                6.7595634268133e-7, -6.21188515924e-6, 
                5.10388300970969e-5, -3.7015410692956173e-4, 
                0.00233307631218880978, -0.0125498847718219221, 
                0.05657061146827041994, -0.2137966477645600658, 
                0.84270079294971486929, 
                9.49905026e-12, -1.8310229805e-10, 
                2.39463074e-9, -2.721444369609e-8, 
                2.8045522331686e-7, -2.61830022482897e-6, 
                2.195455056768781e-5, -1.6358986921372656e-4, 
                0.00107052153564110318, -0.00608284718113590151, 
                0.02986978465246258244, -0.13055593046562267625, 
                0.67493323603965504676, 
                3.82722073e-12, -7.421598602e-11, 
                9.793057408e-10, -1.126008898854e-8, 
                1.1775134830784e-7, -1.1199275838265e-6, 
                9.62023443095201e-6, -7.404402135070773e-5, 
                5.0689993654144881e-4, -0.00307553051439272889, 
                0.01668977892553165586, -0.08548534594781312114, 
                0.56909076642393639985, 
                1.55296588e-12, -3.032205868e-11, 
                4.0424830707e-10, -4.71135111493e-9, 
                5.011915876293e-8, -4.8722516178974e-7, 
                4.30683284629395e-6, -3.445026145385764e-5, 
                2.4879276133931664e-4, -0.00162940941748079288, 
                0.00988786373932350462, -0.05962426839442303805, 
                0.49766113250947636708
            };

            double[] b = {
                -2.9734388465e-10, 2.69776334046e-9, 
                -6.40788827665e-9, -1.6678201321e-8, 
                -2.1854388148686e-7, 2.66246030457984e-6, 
                1.612722157047886e-5, -2.5616361025506629e-4, 
                1.5380842432375365e-4, 0.00815533022524927908, 
                -0.01402283663896319337, -0.19746892495383021487, 
                0.71511720328842845913, 
                -1.951073787e-11, -3.2302692214e-10, 
                5.22461866919e-9, 3.42940918551e-9, 
                -3.5772874310272e-7, 1.9999935792654e-7, 
                2.687044575042908e-5, -1.1843240273775776e-4, 
                -8.0991728956032271e-4, 0.00661062970502241174, 
                0.00909530922354827295, -0.2016007277849101314, 
                0.51169696718727644908, 
                3.147682272e-11, -4.8465972408e-10, 
                6.3675740242e-10, 3.377623323271e-8, 
                -1.5451139637086e-7, -2.03340624738438e-6, 
                1.947204525295057e-5, 2.854147231653228e-5, 
                -0.00101565063152200272, 0.00271187003520095655, 
                0.02328095035422810727, -0.16725021123116877197, 
                0.32490054966649436974, 
                2.31936337e-11, -6.303206648e-11, 
                -2.64888267434e-9, 2.050708040581e-8, 
                1.1371857327578e-7, -2.11211337219663e-6, 
                3.68797328322935e-6, 9.823686253424796e-5, 
                -6.5860243990455368e-4, -7.5285814895230877e-4, 
                0.02585434424202960464, -0.11637092784486193258, 
                0.18267336775296612024, 
                -3.67789363e-12, 2.0876046746e-10, 
                -1.93319027226e-9, -4.35953392472e-9, 
                1.8006992266137e-7, -7.8441223763969e-7, 
                -6.75407647949153e-6, 8.428418334440096e-5, 
                -1.7604388937031815e-4, -0.0023972961143507161, 
                0.0206412902387602297, -0.06905562880005864105, 
                0.09084526782065478489
            };

            w = x < 0 ? -x : x;
            if (w < 2.2)
            {
                t = w * w;
                k = (int)t;
                t -= k;
                k *= 13;
                y = ((((((((((((a[k] * t + a[k + 1]) * t +
                a[k + 2]) * t + a[k + 3]) * t + a[k + 4]) * t +
                a[k + 5]) * t + a[k + 6]) * t + a[k + 7]) * t +
                a[k + 8]) * t + a[k + 9]) * t + a[k + 10]) * t +
                a[k + 11]) * t + a[k + 12]) * w;
            }
            else if (w < 6.9)
            {
                k = (int)w;
                t = w - k;
                k = 13 * (k - 2);
                y = (((((((((((b[k] * t + b[k + 1]) * t +
                b[k + 2]) * t + b[k + 3]) * t + b[k + 4]) * t +
                b[k + 5]) * t + b[k + 6]) * t + b[k + 7]) * t +
                b[k + 8]) * t + b[k + 9]) * t + b[k + 10]) * t +
                b[k + 11]) * t + b[k + 12];
                y *= y;
                y *= y;
                y *= y;
                y = 1 - y * y;
            }
            else
            {
                y = 1;
            }
            return x < 0 ? -y : y;
        }

        static public double Derfc(double x)
        {
            double t, u, y;

            t = 3.97886080735226 / (Math.Abs(x) + 3.97886080735226);
            u = t - 0.5;
            y = (((((((((0.00127109764952614092 * u + 1.19314022838340944e-4) * u -
            0.003963850973605135) * u - 8.70779635317295828e-4) * u +
            0.00773672528313526668) * u + 0.00383335126264887303) * u -
            0.0127223813782122755) * u - 0.0133823644533460069) * u +
            0.0161315329733252248) * u + 0.0390976845588484035) * u +
            0.00249367200053503304;
            y = ((((((((((((y * u - 0.0838864557023001992) * u -
            0.119463959964325415) * u + 0.0166207924969367356) * u +
            0.357524274449531043) * u + 0.805276408752910567) * u +
            1.18902982909273333) * u + 1.37040217682338167) * u +
            1.31314653831023098) * u + 1.07925515155856677) * u +
            0.774368199119538609) * u + 0.490165080585318424) * u +
            0.275374741597376782) * t * Exp(-x * x);
            return x < 0 ? 2 - y : y;
        }

        static public double Dierfc(double y)
        {
            double s, t, u, w, x, z;

            z = y;
            if (y > 1)
            {
                z = 2 - y;
            }
            w = 0.916461398268964 - Log(z);
            u = Sqrt(w);
            s = (Log(u) + 0.488826640273108) / w;
            t = 1 / (u + 0.231729200323405);
            x = u * (1 - s * (s * 0.124610454613712 + 0.5)) -
            ((((-0.0728846765585675 * t + 0.269999308670029) * t +
            0.150689047360223) * t + 0.116065025341614) * t +
            0.499999303439796) * t;
            t = 3.97886080735226 / (x + 3.97886080735226);
            u = t - 0.5;
            s = (((((((((0.00112648096188977922 * u +
            1.05739299623423047e-4) * u - 0.00351287146129100025) * u -
            7.71708358954120939e-4) * u + 0.00685649426074558612) * u +
            0.00339721910367775861) * u - 0.011274916933250487) * u -
            0.0118598117047771104) * u + 0.0142961988697898018) * u +
            0.0346494207789099922) * u + 0.00220995927012179067;
            s = ((((((((((((s * u - 0.0743424357241784861) * u -
            0.105872177941595488) * u + 0.0147297938331485121) * u +
            0.316847638520135944) * u + 0.713657635868730364) * u +
            1.05375024970847138) * u + 1.21448730779995237) * u +
            1.16374581931560831) * u + 0.956464974744799006) * u +
            0.686265948274097816) * u + 0.434397492331430115) * u +
            0.244044510593190935) * t -
            z * Exp(x * x - 0.120782237635245222);
            x += s * (x * s + 1);
            if (y > 1)
            {
                x = -x;
            }
            return x;
        }

        // private for gamma
        static public double P_gamma(double a, double x, double loggamma_a)
		{
			int k;
			double result, term, previous;
			if (x >= 1 + a) return 1 - Q_gamma(a, x, loggamma_a);
			if (x == 0) return 0;
			result = term = NMath.Exp(a * NMath.Log(x) - x - loggamma_a) / a;
			for (k = 1; k < 1000; k++)
			{
				term *= x / (a + k);
				previous = result;
				result += term;
				if (result == previous) return result;
			}
			return result;
		}

		static public double Q_gamma(double a, double x, double loggamma_a)
		{
			int k;
			double result, w, temp, previous;
			double la, lb;
			la = 1; lb = 1 + x - a;
			if (x < 1 + a) return 1 - P_gamma(a, x, loggamma_a);
			w = NMath.Exp(a * NMath.Log(x) - x - loggamma_a);
			result = w / lb;
			for (k = 2; k < 1000; k++)
			{
				temp = ((k - 1 - a) * (lb - la) + (k + x) * lb) / k;
				la = lb;
				lb = temp;
				w *= (k - 1 - a) / k;
				temp = w / (la * lb);
				previous = result;
				result += temp;
				if (result == previous) return result;
			}
			return result;
		}

		/////// integral

		public delegate double Integrand(double x);

		public static void GaussWeights(double[] x, double[] w, double eps)
        {

            int n = x.Length;
            int i, l, m;
            double p0, p1, p2;
            double q0, q1, q2;
            double tmp, dt;

            switch (n)
            {
                case 1:
                    x[0] = 0.0;
                    w[0] = 2.0;
                    return;
                case 2:
                    x[0] = Sqrt(1.0 / 3.0);
                    w[0] = 1.0;
                    x[1] = -x[0];
                    w[1] = w[0];
                    return;
                case 3:
                    x[0] = Sqrt(0.6);
                    w[0] = 5.0 / 9.0;
                    x[1] = 0.0;
                    w[1] = 8.0 / 9.0;
                    x[2] = -x[0];
                    w[2] = w[0];
                    return;
            }

            m = n / 2;
            for (i = 0; i < m; i++)
            {
                tmp = Math.Cos((i + 1.0 - 1.0 / 4.0) / (n + 1.0 / 2.0) * PI);
                do
                {
                    p1 = tmp;
                    p2 = (3.0 * tmp * tmp - 1.0) / 2.0;
                    q1 = 1.0;
                    q2 = 3.0 * tmp;
                    for (l = 3; l <= n; l++)
                    {
                        p0 = p1;
                        p1 = p2;
                        p2 = ((2.0 * l - 1) * tmp * p1 - (l - 1) * p0) / l;
                        q0 = q1;
                        q1 = q2;
                        q2 = ((2.0 * l - 1) * (tmp * q1 + p1) - (l - 1) * q0) / l;
                    }
                    dt = p2 / q2;
                    tmp = tmp - dt;
                } while (Math.Abs(dt) > Math.Abs(tmp) * eps);
                x[i] = tmp;
                w[i] = 2.0 / (n * p1 * q2);
            }
            if (n % 2 != 0)
            {
                x[n / 2] = 0.0;
                tmp = (double) n;
                for (i = 1; i <= m; i++)
                    tmp = tmp * (0.5 - i) / i;
                w[n / 2] = 2.0 / (tmp * tmp);
            }
            for (i = 0; i < m; i++)
            {
                x[n - 1 - i] = -x[i];
                w[n - 1 - i] = w[i];
            }
            return;
        }

        public static double Tapezoid(Integrand func, double a, double b, double eps, int maxiter = 200)
        {
            int i, k;
            int n = 1;
            double s, h, t, tn = 0.0;

            h = b - a;
            t = h * (func(a) + func(b)) / 2.0;

            for (k = 1; k <= maxiter; k++)
            {
                n = 2 * n;
                h = h / 2.0;
                s = 0.0;
                for (i = 1; i <= n - 1; i += 2)
                {
                    s += func(a + i * h);
                }
                tn = t / 2.0 + h * s;
                if (Math.Abs(tn - t) / Math.Abs(t) < eps)
                {
                    break;
                }
                t = tn;
            }
            return tn;
        }

        public static double Deintde(Integrand f, double a, double b, out double err, double eps)
        {
            /* ---- adjustable parameter ---- */
            const int mmax = 256;
            const double efs = 0.1, hoff = 8.5;
            double pi2 = 2 * Math.Atan(1.0);
            /* ------------------------------ */

            int m;
            double res, epsln, epsh, h0, ehp, ehm, epst, ba, ir, h, iback,
            irback, t, ep, em, xw, xa, wg, fa, fb, errt, errh = 0.0, errd;

            epsln = 1 - Log(efs * eps);
            epsh = Sqrt(efs * eps);
            h0 = hoff / epsln;
            ehp = Exp(h0);
            ehm = 1 / ehp;
            epst = Exp(-ehm * epsln);
            ba = b - a;
            ir = f((a + b) * 0.5) * (ba * 0.25);
            res = ir * (2 * pi2);
            err = Math.Abs(res) * epst;
            h = 2 * h0;
            m = 1;
            do
            {
                iback = res;
                irback = ir;
                t = h * 0.5;
                do
                {
                    em = Exp(t);
                    ep = pi2 * em;
                    em = pi2 / em;
                    do
                    {
                        xw = 1 / (1 + Exp(ep - em));
                        xa = ba * xw;
                        wg = xa * (1 - xw);
                        fa = f(a + xa) * wg;
                        fb = f(b - xa) * wg;
                        ir += fa + fb;
                        res += (fa + fb) * (ep + em);
                        errt = (Math.Abs(fa) + Math.Abs(fb)) * (ep + em);
                        if (m == 1) err += errt * epst;
                        ep *= ehp;
                        em *= ehm;
                    } while (errt > err || xw > epsh);
                    t += h;
                } while (t < h0);
                if (m == 1)
                {
                    errh = (err / epst) * epsh * h0;
                    errd = 1 + 2 * errh;
                }
                else
                {
                    errd = h * (Math.Abs(res - 2 * iback) + 4 * Math.Abs(ir - 2 * irback));
                }
                h *= 0.5;
                m *= 2;
            } while (errd > errh && m < mmax);
            res *= h;
            if (errd > errh)
            {
                err = -errd * m;
            }
            else
            {
                err = errh * epsh * m / (2 * efs);
            }
            return res;
        }

        //public static double deintde(Integrand f, double a, double b, Sci.DVector param,
        //           out double err, double eps = Double.Epsilon)
        //{
        //    f.setParam(param);
        //    return deintde(f, a, b, out err, eps);
        //}


        public static double Deintdei(Integrand f, double a, out double err, double eps)
        {
            /* ---- adjustable parameter ---- */
            const int mmax = 256;
            const double efs = 0.1, hoff = 11.0;
            double pi4 = Math.Atan(1.0);
            /* ------------------------------ */
            int m;
            double res, epsln, epsh, h0, ehp, ehm, epst, ir, h, iback, irback,
              t, ep, em, xp, xm, fp, fm, errt, errh = 0.0, errd;

            epsln = 1 - Log(efs * eps);
            epsh = Sqrt(efs * eps);
            h0 = hoff / epsln;
            ehp = Exp(h0);
            ehm = 1 / ehp;
            epst = Exp(-ehm * epsln);
            ir = f(a + 1);
            res = ir * (2 * pi4);
            err = Math.Abs(res) * epst;
            h = 2 * h0;
            m = 1;
            do
            {
                iback = res;
                irback = ir;
                t = h * 0.5;
                do
                {
                    em = Exp(t);
                    ep = pi4 * em;
                    em = pi4 / em;
                    do
                    {
                        xp = Exp(ep - em);
                        xm = 1 / xp;
                        fp = f(a + xp) * xp;
                        fm = f(a + xm) * xm;
                        ir += fp + fm;
                        res += (fp + fm) * (ep + em);
                        errt = (Math.Abs(fp) + Math.Abs(fm)) * (ep + em);
                        if (m == 1) err += errt * epst;
                        ep *= ehp;
                        em *= ehm;
                    } while (errt > err || xm > epsh);
                    t += h;
                } while (t < h0);
                if (m == 1)
                {
                    errh = (err / epst) * epsh * h0;
                    errd = 1 + 2 * errh;
                }
                else
                {
                    errd = h * (Math.Abs(res - 2 * iback) + 4 * Math.Abs(ir - 2 * irback));
                }
                h *= 0.5;
                m *= 2;
            } while (errd > errh && m < mmax);
            res *= h;
            if (errd > errh)
            {
                err = -errd * m;
            }
            else
            {
                err = errh * epsh * m / (2 * efs);
            }
            return res;
        }

        //public static double deintdei(Integrand f, double a, Sci.DVector param,
        //    out double err, double eps = Double.Epsilon)
        //{
        //    f.setParam(param);
        //    return deintdei(f, a, out err, eps);
        //}

        public static double Gauss(Integrand f, double a, double b,
            double[] x, double[] w)
        {

            int n = x.Length;
            double t1 = (b - a) / 2.0;
            double t2 = (b + a) / 2.0;
            double sum = 0.0;
            for (int i = 0; i < n; i++)
            {
                sum += w[i] * f(t1 * x[i] + t2);
            }
            sum *= t1;
            return sum;
        }

        //public static double gauss(Integrand f, double a, double b,
        // Sci.DVector param, double[] x, double[] w)
        //{

        //    f.setParam(param);
        //    return gauss(f, a, b, x, w);
        //}

    }
}

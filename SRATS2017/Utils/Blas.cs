using System;
using System.Globalization;

namespace SRATS.Utils
{
	public static class Blas
	{
		public static double Ddot(int n, double[] x, double[] y)
		{
			double sum = 0.0;
			for (int i = 0; i < n; i++)
			{
				sum += x[i] * y[i];
			}
			return sum;
		}

		public static double Asum(int n, double[] x)
		{
			double sum = 0.0;
			for (int i = 0; i < n; i++)
			{
				sum += NMath.Abs(x[i]);
			}
			return sum;
		}

		public static double Amax(int n, double[] x)
		{
			double m = 0.0;
			for (int i = 0; i < n; i++)
			{
				if (NMath.Abs(x[i]) > m)
				{
					m = NMath.Abs(x[i]);
				}
			}
			return m;
		}

		public static double Adiff(int n, double[] x, double[] y)
		{
			double m = 0.0;
			for (int i = 0; i < n; i++)
			{
				double tmp = NMath.Abs(x[i] - y[i]);
				if (tmp > m)
				{
					m = tmp;
				}
			}
			return m;
		}

		public static double Rdiff(int n, double[] x, double[] y)
		{
			return Adiff(n, x, y) / Amax(n, y);
		}

		public static void Daxpy(int n, double alpha, double[] x, double[] y)
		{
			for (int i = 0; i < n; i++)
			{
				y[i] += alpha * x[i];
			}
		}

		public static void Dcopy(int n, double[] x, double[] y)
		{
			Array.Copy(x, y, n);
		}

		public static void Fill(int n, double[] x, double v)
		{
			for (int i = 0; i < n; i++)
			{
				x[i] = v;
			}
		}

		public static void Dscal(int n, double alpha, double[] x)
		{
			for (int i = 0; i < n; i++)
			{
				x[i] *= alpha;
			}
		}

		public static string ArrayToString<T>(T[] x, string format, string separator)
			where T : IFormattable
		{
			string[] s = new string[x.Length];
			for (int i = 0; i < x.Length; i++)
			{
				s[i] = x[i].ToString(format, CultureInfo.CurrentCulture);
			}
			return string.Join(separator, s);
		}

		public static string ArrayToString<T>(T[] x, string[] format, string separator)
			where T : IFormattable
		{
			string[] s = new string[x.Length];
			for (int i = 0; i < x.Length; i++)
			{
				s[i] = x[i].ToString(format[i], CultureInfo.CurrentCulture);
			}
			return string.Join(separator, s);
		}

		/*
		public class Blas
		{
			unsafe public static int dggglm(Sci.DMatrix x, Sci.DMatrix w, Sci.DVector z, Sci.DVector beta,
				Sci.DVector residual, int lwork, double[] work)
			{
				int info = 0;
				int xnrow = x.nrow();
				int xncol = x.ncol();
				int wncol = w.ncol();
				int wnrow = w.nrow();
				Sci.DMatrix tmpx = new Sci.DMatrix(x.nrow(), x.ncol());
				tmpx.copyFrom(x.ptr());
				fixed (
					double* xptr = x.ptr(),
						wptr = w.ptr(),
						zptr = z.ptr(),
						betaptr = beta.ptr(),
						residualptr = residual.ptr(),
						workptr = work)
				{
					dggglm_(&xnrow, &xncol, &wncol,
					   xptr, &xnrow, wptr, &wnrow,
					   zptr, betaptr, residualptr,
					   workptr, &lwork, &info);
				}
				x.copyFrom(tmpx.ptr());
				return info;
			}

			unsafe public static double ddot(Sci.DVector x, Sci.DVector y)
			{
				int n = x.size();
				int incx = 1;
				int incy = 1;
				fixed (double*
					xptr = x.ptr(),
					yptr = y.ptr())
				{
					return ddot_(&n, xptr, &incx, yptr, &incy);
				}
			}

			unsafe public static void daxpy(double alpha, Sci.DVector x, Sci.DVector y)
			{
				int n = x.size();
				int incx = 1;
				int incy = 1;
				x.print();
				System.Console.WriteLine();
				y.print();
				System.Console.WriteLine();
				fixed (
					double*
					xptr = x.ptr(),
					yptr = y.ptr())
				{
					daxpy_(&n, &alpha, xptr, &incx, yptr, &incy);
				}
			}

			unsafe public static void dgemv(char tr, double alpha, Sci.DMatrix A, Sci.DVector x, double beta, Sci.DVector y)
			{
				int m = A.nrow();
				int n = A.ncol();
				int incx = 1;
				int incy = 1;
				fixed (
					double* Aptr = A.ptr(),
					xptr = x.ptr(),
					yptr = y.ptr())
				{
					dgemv_(&tr, &m, &n, &alpha, Aptr, &m, xptr, &incx, &beta, yptr, &incy);
				}
			}

			unsafe public static void dger(double alpha, Sci.DVector x, Sci.DVector y, Sci.DMatrix A)
			{
				int m = A.nrow();
				int n = A.ncol();
				int lda = m;
				int incx = 1;
				int incy = 1;
				fixed (
					double* Aptr = A.ptr(),
					xptr = x.ptr(),
					yptr = y.ptr())
				{
					dger_(&m, &n, &alpha, xptr, &incx, yptr, &incy, Aptr, &lda);
				}
			}

			unsafe public static void dgemm(char trA, char trB, double alpha, Sci.DMatrix A, Sci.DMatrix B, double beta, Sci.DMatrix C)
			{
				int m = C.nrow();
				int n = C.ncol();
				int k;
				if (trA == 'N' || trA == 'n')
				{
					k = A.ncol();
				}
				else
				{
					k = A.nrow();
				}
				int lda = A.nrow();
				int ldb = B.nrow();
				int ldc = C.nrow();
				fixed (
					double* Aptr = A.ptr(),
						Bptr = B.ptr(),
						Cptr = C.ptr())
				{
					dgemm_(&trA, &trB, &m, &n, &k, &alpha, Aptr, &lda, Bptr, &ldb, &beta, Cptr, &ldc);
				}
			}

			[DllImport("Rlapack.dll", CallingConvention = CallingConvention.Cdecl)]
			unsafe private static extern void dggglm_(int* n, int* m, int* p,
						double* A, int* lda,
						double* B, int* ldb,
						double* d, double* x, double* y,
						double* work, int* lwork, int* info);

			[DllImport("Rblas.dll", CallingConvention = CallingConvention.Cdecl)]
			unsafe private static extern double ddot_(int* n,
				double* x, int* incx, double* y, int* incy);

			[DllImport("Rblas.dll", CallingConvention = CallingConvention.Cdecl)]
			unsafe private static extern void dgemv_(char* trans, int* m, int* n,
			double* alpha, double* a, int* lda,
			double* x, int* incx, double* beta,
			double* y, int* incy);

			[DllImport("Rblas.dll", CallingConvention = CallingConvention.Cdecl)]
			unsafe private static extern void daxpy_(int* n,
					  double* alpha, double* x, int* incx,
					  double* y, int* incy);

			[DllImport("Rblas.dll", CallingConvention = CallingConvention.Cdecl)]
			unsafe private static extern void dger_(int* m, int* n,
			 double* alpha, double* x, int* incx, double* y, int* incy,
			 double* A, int* lda);

			[DllImport("Rblas.dll", CallingConvention = CallingConvention.Cdecl)]
			unsafe private static extern void dgemm_(char* transa,
			  char* transb, int* m, int* n, int* k,
			  double* alpha, double* A, int* lda,
			  double* B, int* ldb, double* beta, double* C, int* ldc);
		}
		*/
	}
}

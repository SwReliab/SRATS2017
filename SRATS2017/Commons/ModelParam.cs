namespace SRATS
{
	public interface IModelParam
	{
		//int size();
		IModelParam Create();
		void CopyTo(IModelParam p);
		double Adiff(IModelParam param);
		double Rdiff(IModelParam param);
		//void print();
		string ToString(params string[] formats);
		double[] ToArray();
	}
}

using System;

namespace SRATS.Utils
{
    public interface IMessage
    {
        void Show();
        void Error();
        void Warning();
        void Final();
    }

    public class NullMessage : IMessage
    {
        public void Show() { }
        public void Error() { }
        public void Warning() { }
        public void Final() { }
    }

	public class DefaultMessage : IMessage
	{
		public DefaultMessage()
		{
		}

		public virtual void Show()
		{
			Console.Write("#");
		}

		public virtual void Warning()
		{
			Console.WriteLine();
			Console.WriteLine("Warning");
		}

		public virtual void Error()
		{
			Console.WriteLine();
			Console.WriteLine("Error");
			throw new NotFiniteNumberException();
		}

		public virtual void Final()
		{
			Console.WriteLine("Done");
		}
	}
}

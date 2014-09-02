using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleModule
{
	public class Engine
	{
		private bool enabled = false;

		public bool Enabled
		{
			get { return this.enabled; }
		}

		public double Factorial(int n)
		{
			double result = 1;
			for (int i = 2; i <= n; ++i)
				result *= i;
			return result;
		}

		public void Start()
		{
			enabled = true;
		}

		public void Stop()
		{
			enabled = false;
		}

	}
}

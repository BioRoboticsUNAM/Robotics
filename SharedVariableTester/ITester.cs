using System;
using System.Net;

namespace SharedVariableTester
{
	public interface ITester
	{
		void Setup(string moduleName, int port);
		void Run();
	}
}

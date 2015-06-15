using System;
using System.Collections.Generic;
using System.Text;
using Robotics.Sockets;

namespace TestTcp
{
	public static class Extensions
	{
		public static bool IsAnsi(this TcpPacket p)
		{
			for (int i = 0; i < p.Data.Length; ++i )
			{
				if (p[i] >= 128)
					return false;
			}
			return true;
		}

		public static string[] DataStrings(this TcpPacket p)
		{
			StringBuilder sb = new StringBuilder();
			List<string> dataStrings = new List<string>();
			int cc = 0;
			while (cc < p.Data.Length)
			{
				if ((p[cc] == 0) || (p[cc] > 127))
				{
					if (sb.Length != 0)
					{
						dataStrings.Add(sb.ToString());
						sb.Length = 0;
					}
					++cc;
					continue;
				}
				sb.Append((char)p[cc++]);
			}
			return dataStrings.ToArray();
		}
	}
}

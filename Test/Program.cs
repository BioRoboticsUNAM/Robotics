using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using Robotics;
using Robotics.API;
using Robotics.Mathematics;
using Robotics.HAL.Sensors.Telemetric;

namespace Test
{
	public class SuperType
	{
		public int X { get; set; }
		public int Y { get; set; }
		public double[] Z { get; set; }
		public SubType ST { get; set; }
		public SubType[] STA { get; set; }
	}
	public class SubType{ public int I { get; set; } public int[][] J { get; set; } }

	class Program
	{
		public static int i = 0;
		public static Random rnd = new Random();
		public delegate void DoubleArrayDelegate(double[] array);
		private static  Stopwatch stopwatch = new Stopwatch();

		public static void Main(string[] args)
		{
			string variableType;
			bool isArray;
			int arrayLength;
			string variableName;
			string variableData;
			string data;
			Response rsp;

			Robotics.API.PrimitiveSharedVariables.SharedVariableBuilder.BuildClass(typeof(SuperType), "Z:\\SuperTypeSharedVar.cs");
			Robotics.API.PrimitiveSharedVariables.SharedVariableBuilder.BuildClass(typeof(SuperType[]), "Z:\\SuperTypeArraySharedVar.cs");
			return;


			data = "{ RecognizedSpeech recognizedSpeech { 2 \"robot no\" 0.92161322 \"robot hello\" 0.91587275 } }";
			data = "read_var \"{ string connected \\\"BLK OBJ-FNDT\\\" }\" 1";
			rsp = Response.Parse(data);
			data = rsp.Parameters;

			Parser.ParseSharedVariable(data, out variableType, out isArray, out arrayLength, out variableName, out variableData);
			Robotics.HAL.Sensors.RecognizedSpeech speech;
			Robotics.API.MiscSharedVariables.RecognizedSpeechSharedVariable.SDeserialize(
			"{ 2 \"robot no\" 0.92161322 \"robot hello\" 0.91587275", out speech);

			Vector2 v2 = null;
			Vector3 v3 = Vector3.UnitZ;
			foo(new Vector3(1, 2, 3));
			Vector v = new Vector(v2);
			SharedVars();
			
			//Console.Read();
			//BenchmarkCommand();
			//MainMath2();
			//MainTypes();
			//MainSignature();
			//MainTypes();
			//MainFill();
			//MathTest1();
			//Console.Read();
		}

		private static void foo(Vector4 v)
		{
			v.W = 0;
			Vector3 v3 = new Vector3(1, 2, 3);
			Vector4 v4 = new Vector4(v3);
			v4.W.ToString();
		}

		private static void SharedVars()
		{
			bool exit = false;
			int count = 0;
			int elapsed = 0;
			CommandManager cmdMan = new CommandManager();
			ConnectionManager cnnMan = new ConnectionManager(2011, cmdMan);
			cnnMan.Start();
			cnnMan.DataReceived += new ConnectionManagerDataReceivedEH(delegate(ConnectionManager connectionManager, System.Net.Sockets.TcpPacket packet)
			{
				Console.WriteLine("Received: " + packet.DataString);
				++count;
			});
			cnnMan.Disconnected += new System.Net.Sockets.TcpClientDisconnectedEventHandler(delegate(System.Net.EndPoint ep)
			{
				exit = true; ;
			});

			cnnMan.ClientDisconnected += new System.Net.Sockets.TcpClientDisconnectedEventHandler(delegate(System.Net.EndPoint ep)
			{
				exit = true; ;
			});
			cmdMan.Start();

			LaserReading[] readings = new LaserReading[768];
			double step = Math.PI / 512;
			double angle = -step * readings.Length / 2;
			for (int i = 0; i < readings.Length; ++i)
			{
				readings[i] = new LaserReading(null, angle, 4);
				angle += step;
			}

			while (cnnMan.ConnectedClientsCount < 1)
				Thread.Sleep(10);
			cnnMan.Send(new Command("create_var", "laserReadings|0"));
			cnnMan.Send(new Command("suscribe_var", "laserReadings suscribe=writeany report=content"));

			while (!exit)
			{
				//cnnMan.Send(new Command("write_var", "laserReadings|" + Serialize(readings)));
				cnnMan.Send(new Command("write_var", "laserReadings|0x0000"));
				Thread.Sleep(100);
				elapsed += 100;
				if (elapsed >= 1000)
				{
					Console.WriteLine("RPS: " + count.ToString());
					elapsed = 0;
					count = 0;
				}
				continue;

				//if (!Console.KeyAvailable)
				//{
				//	Thread.Sleep(100);
				//	continue;
				//}
				//Console.ReadLine();
			}

		}

		public static string Serialize(Robotics.HAL.Sensors.Telemetric.LaserReading[] readings)
		{
			if ((readings == null) || (readings.Length < 2))
				return null;

			int count = readings.Length;
			double min = readings[0].Angle;
			double step = readings[1].Angle - readings[0].Angle;
			ushort distanceMM;
			int i;
			byte[] bytes;

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			bytes = BitConverter.GetBytes(count);
			for(i =0; i < bytes.Length; ++i)
				sb.AppendFormat("{0:x2}", bytes[i]);

			bytes = BitConverter.GetBytes(min);
			for (i = 0; i < bytes.Length; ++i)
				sb.AppendFormat("{0:x2}", bytes[i]);

			bytes = BitConverter.GetBytes(step);
			for (i = 0; i < bytes.Length; ++i)
				sb.AppendFormat("{0:x2}", bytes[i]);

			for(i = 0; i < count; ++i)
			{
				distanceMM = (ushort)readings[i].DistanceMillimeters;
				if (readings[i].Mistaken)
					distanceMM |= 0x8000;
				bytes = BitConverter.GetBytes(distanceMM);
				sb.AppendFormat("{0:x2}", bytes[0]);
				sb.AppendFormat("{0:x2}", bytes[1]);
			}
			return sb.ToString();
		}

		public static Robotics.HAL.Sensors.Telemetric.LaserReading[] Deserialize(string serializedString)
		{
			if ((serializedString == null) || (serializedString.Length < 20) || ((serializedString.Length % 2) != 0))
				return null;

			int count;
			bool mistaken;
			double min;
			double step;
			double angle;
			double distance;
			ushort distanceMM;
			Robotics.HAL.Sensors.Telemetric.LaserReading[] readings;
			int ix;
			byte[] bytes;

			ix = serializedString.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase) ? 2 : 0;
			bytes = new byte[(serializedString.Length / 2) - ix];
			for (int i = 0; i < bytes.Length; ++i, ix+=2)
				bytes[i] = Byte.Parse(serializedString.Substring(ix), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);

			ix = 0;
			count = BitConverter.ToInt32(bytes, ix);
			ix += 4;
			if ((count < 0) || (((serializedString.Length - 40) / 4) != count))
				return null;

			min = BitConverter.ToDouble(bytes, ix);
			ix += 8;

			step = BitConverter.ToDouble(bytes, ix);
			ix += 8;

			angle = min;
			readings = new Robotics.HAL.Sensors.Telemetric.LaserReading[count];
			for (int i = 0; i < count; ++i, angle += step)
			{
				distanceMM = BitConverter.ToUInt16(bytes, ix);
				if (mistaken = ((distanceMM & 0x8000) != 0))
					distanceMM &= 0x7FFF;
				ix += 2;
				distance = distanceMM / 1000.0;
				readings[i] = new Robotics.HAL.Sensors.Telemetric.LaserReading(null, angle, distance, mistaken);
			}
			return readings;
		}

		private static void MathTest1()
		{
			Vector v;
			Matrix m1 = new Matrix(4, 4,
				1, 0, 0, 0,
				0, 1, 0, 0,
				0, 0, 1, 0,
				0, 0, 0, 1);
			Matrix m2 = new Matrix(2, 4,
				1, 0, 2, 0,
				0, 1, 0, 2);

			Matrix m3 = new Matrix(3, 3,
				1, 1, 1,
				1, 0, 0,
				0, 0, 1);

			Matrix m4 = new Matrix(5, 5,
				0, 0, 1, 0, 0,
				1, 0, 0, 0, 0,
				0, 1, 0, 0, 0,
				0, 0, 0, 1, 0,
				0, 0, 0, 0, 1);

			Vector v1 = new Vector(1, 2, 3, 4);
			
			v = 3*v1/v1.Length;

			m3 = m3.Inverse;
			m4 = m4.Inverse;
		}

		public static void BenchmarkCommand()
		{
			Command cmd;
			Response rsp;
			Console.WriteLine("Benchmark command succeed");
			stopwatch.Start();
			Command.TryParse("sp \"hola \" @1", out cmd);
			stopwatch.Stop();
			Console.WriteLine("Elapsed: " + stopwatch.Elapsed);
			stopwatch.Reset();
			stopwatch.Start();
			for (int i = 0; i < 1000; ++i)
				Command.TryParse("sp \"hola \" @1", out cmd);
			stopwatch.Stop();
			Console.WriteLine("1000 commands parsed.");
			Console.WriteLine("Elapsed: " + stopwatch.Elapsed);

			Console.WriteLine();
			Console.WriteLine("Benchmark response succeed");
			stopwatch.Reset();
			stopwatch.Start();
			Response.TryParse("sp \"hola \" 1 @1", out rsp);
			stopwatch.Stop();
			Console.WriteLine("Elapsed: " + stopwatch.Elapsed);
			stopwatch.Reset();
			stopwatch.Start();
			for (int i = 0; i < 1000; ++i)
				Response.TryParse("sp \"hola \" 1 @1", out rsp);
			stopwatch.Stop();
			Console.WriteLine("1000 responses parsed.");
			Console.WriteLine("Elapsed: " + stopwatch.Elapsed);

			Console.WriteLine();
			Console.WriteLine("Benchmark command fail");
			stopwatch.Start();
			Command.TryParse("sp \"hola ", out cmd);
			stopwatch.Stop();
			Console.WriteLine("Elapsed: " + stopwatch.Elapsed);
			stopwatch.Reset();
			stopwatch.Start();
			for (int i = 0; i < 1000; ++i)
				Command.TryParse("sp \"hola ", out cmd);
			stopwatch.Stop();
			Console.WriteLine("1000 commands parsed.");
			Console.WriteLine("Elapsed: " + stopwatch.Elapsed);

			Console.WriteLine();
			Console.WriteLine("Benchmark response fail");
			stopwatch.Reset();
			stopwatch.Start();
			Response.TryParse("sp \"hola \" @1", out rsp);
			stopwatch.Stop();
			Console.WriteLine("Elapsed: " + stopwatch.Elapsed);
			stopwatch.Reset();
			stopwatch.Start();
			for (int i = 0; i < 1000; ++i)
				Response.TryParse("sp \"hola \" @1", out rsp);
			stopwatch.Stop();
			Console.WriteLine("1000 responses parsed.");
			Console.WriteLine("Elapsed: " + stopwatch.Elapsed);
		}

		public static void MainMath2()
		{
			Matrix m;

			m = new Matrix(2, 2);
			m[0, 0] = 0;
			m[0, 1] = -1;
			m[1, 0] = 1;
			m[1, 1] = 0;
			Console.WriteLine(m.ToString());
			Console.WriteLine("Determinant");
			Console.WriteLine(m.Determinant);

			m = new Matrix(4, 4);
			m[0, 0]= 0;
			m[0, 1]= -1;
			m[0, 2]= 0;
			m[0, 3]= 10;
			m[1, 0]= 1;
			m[1, 1]= 0;
			m[1, 2]= 0;
			m[1, 3]= 5;
			m[2, 0]= 0;
			m[2, 1]= 0;
			m[2, 2]= 1;
			m[2, 3]= 0;
			m[3, 0]= 0;
			m[3, 1]= 0;
			m[3, 2]= 0;
			m[3, 3]= 1;

			Console.WriteLine(m.ToString());
			Console.WriteLine("Invert");
			Console.WriteLine(m.Inverse.ToString());

			
			
			m = Matrix.Identity(4);
			Vector4 v4 = new Vector4(1, 2, 3, 4);
			Vector4 result = (Vector4) (v4 * m);
			Console.WriteLine(result.ToString());
			result = (Vector4)(m * v4);
			Console.WriteLine(result.ToString());
		}

		public static void MainFill()
		{
			string[] s = new string[3];
			s[0] = "Hola";
			s[1] = "Mundo";
			s[2] = "Cruel!";

			Console.WriteLine(String.Join(" ", s));
			Console.WriteLine("Fill()");
			Fill(s);
			Console.WriteLine(String.Join(" ", s));
		}

		public static void Fill(object[] p)
		{
			for (int i = 0; i < p.Length; ++i)
				p[i] = i.ToString();
		}

		public static void MainTypes()
		{
			Type[] types = {

					typeof(byte),
					typeof(byte[]),
					typeof(char),
					typeof(char[]),
					typeof(string[]),

					typeof(int), 
					typeof(long), 
					typeof(Int16), 
					typeof(Int32), 
					typeof(Int64), 

					typeof(uint), 
					typeof(ulong), 
					typeof(UInt16), 
					typeof(UInt32), 
					typeof(UInt64), 

					typeof(float), 
					typeof(double), 
					typeof(Single), 
					typeof(Double), 

					typeof(string), 
					typeof(String)
			
			};

			String s = "";
			foreach (Type t in types)
			{
				s+=("case \"" + t.Name + "\":\r\n");
				Console.WriteLine(t.Name);
			}
		}

		public static void DoubleArray(double[] dobles)
		{
			Console.WriteLine("Ejecutando llamada a array of double");
		}

		public static void Nothing()
		{
			
		}
		
		public static void Nothing(double d1, double d2)
		{
			
		}

		public static void Vacio()
		{
			Console.WriteLine("Ejecutando llamada de comando vacio");
		}

		public delegate void DosDobles(double d1, double d2);

		public static void Suma(double d1, double d2)
		{
			Console.WriteLine("Ejecutando llamada de comando Suma. " + d1.ToString() + " + " + d2.ToString() + " = " + (d1+d2).ToString());
			++i;
		}

		public static void Doble(double d)
		{
			Console.WriteLine("Ejecutando llamada de comando double. El parametro es: " + d.ToString());
		}

		public static void StringArray(string[] s)
		{
			Console.WriteLine(String.Join(" ", s));
		}

		public static void MainSignature()
		{
			Signature s;
			bool result;
			SignatureBuilder sb = new SignatureBuilder();
			System.Diagnostics.Stopwatch sw;
			sw = new System.Diagnostics.Stopwatch();
			
			sb.AddNewFromDelegate(new DoubleEventHandler(Doble));
			sb.AddNewFromDelegate(new VoidEventHandler(Vacio));
			sb.AddNewFromDelegate(new DosDobles(Suma));
			sb.AddNewFromDelegate(new DoubleArrayDelegate(DoubleArray));
			sb.AddNewFromDelegate(new StringArrayEventHandler(StringArray));
			
			//Console.WriteLine(sb.RegexPattern);
			
			s = sb.GenerateSignature("command");

			Command cmd = new Command("command", "Hola Mundo!");
			sw.Start();
			result = s.CallIfMatch(cmd);
			sw.Stop();
			Console.WriteLine("Elapsed: " + sw.ElapsedMilliseconds);
			cmd.Parameters = "";
			result = s.CallIfMatch(cmd);
			cmd.Parameters = "3";
			result = s.CallIfMatch(cmd);
			cmd.Parameters = "3.141592 2.71";
			result = s.CallIfMatch(cmd);
			cmd.Parameters = "3.141592 2.71 3.141592 2.71 3.141592 2.71 3.141592 2.71 3.141592 2.71";
			result = s.CallIfMatch(cmd);

			Response rsp = Response.CreateFromCommand(cmd, false);
			rsp.Parameters = "3.1416 1";
			SignatureAnalysisResult sar = s.Analyze(rsp);
			double a = 0;
			double b = 0;
			sar.Update("d1", ref a);
			Console.WriteLine("a: " + a.ToString());
			sar.GetParameter("d2", out b);
			Console.WriteLine("b: " + b.ToString());
			sar.Execute();

			Console.WriteLine();
			Console.WriteLine("Ejecutando Benchmark");

			//TextWriter cout = Console.Out;
			//Console.SetOut(TextWriter.Null);
			//Thread thread = new Thread(new ThreadStart(new VoidEventHandler(delegate()
			//{
			//	while (true) { sar = s.Analyze(rsp); sar.Execute(); }
			//})));
			//i = 0;
			//thread.Start();
			//Thread.Sleep(1000);
			//thread.Abort();

			sb.Clear();
			sb.AddNewFromDelegate(new DosDobles(Nothing));
			sb.AddNewFromDelegate(new VoidEventHandler(Nothing));
			s = sb.GenerateSignature("command");
			rsp = new Response("command", "1 2", true, -1);
			TimeSpan total = new TimeSpan(0);
			sw = new Stopwatch();
			sar = s.Analyze(rsp);
			for (int count = 0; count < 100; ++count)
			{
				Console.Write("\r");
				Console.Write(count);
				Console.Write("%\t");
				sw.Reset();
				sw.Start();
				for (i = 0; i < 100000; ++i)
				{
					sar.Execute();
				}
				sw.Stop();
				total +=sw.Elapsed;
			}
			Console.WriteLine();

			total = new TimeSpan(total.Ticks / 100);
			//Console.SetOut(cout);
			Console.WriteLine("Ejecutado 100 veces 100k ciclos. Promedio de rafaga: " + total.TotalMilliseconds + "ms");
			Console.WriteLine("Promedio de ejecucion: " + (total.TotalMilliseconds/(100.0)) + "us");
			
			//s.Parse(null, a, b, c);
		}

		public static void MainMath()
		{
			Console.WriteLine(Math.Cos(-Math.PI));
			Console.WriteLine(MathUtil.Cos(-Math.PI));
			Console.WriteLine(MathUtil.Cos(-Math.PI / 2));
			Console.WriteLine(MathUtil.Cos(0));
			Console.WriteLine(MathUtil.Sin(Math.PI / 2));
			Console.WriteLine(MathUtil.Sin(Math.PI));
		}

		/*
		public static void Tree()
		{
			BinarySearchTree<int> tree = new BinarySearchTree<int>();

			tree.Add(5);
			tree.Add(6);
			tree.Add(1);
			tree.Add(17);
			tree.Add(8);
			tree.Add(3);
			tree.Add(11);
			tree.Add(6);

			foreach (BinarySearchTreeNode<int> node in tree)
			{
				Console.Write(node.Value);
				Console.Write(", ");
			}
		}
		*/

	}
}


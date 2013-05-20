using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SharedVariableTester
{
	public class SocketServerTester : ITester
	{
		/// <summary>
		/// Manejador de conexiones
		/// </summary>
		SocketTcpServer server;

		/// <summary>
		/// Reloj de alta presicion para medir tiempos.
		/// </summary>
		System.Diagnostics.Stopwatch sw;

		/// <summary>
		/// Constructor. Inicializa instancias de la clase ConnectionManagerTester
		/// </summary>
		public SocketServerTester()
		{

			/*
			 * Se inicializa el SocketTcpServer. Bajo el esquema actual
			 * todas las aplicaciones son servicores y es el blackboard el que
			 * se conecta a ellas (asi solo es necesario configurar una
			 * aplicacion). Se le indica puerto de conexio. El modulo y puerto
			 * deben ser configurados en el blackboard
			*/
			server = new SocketTcpServer(2000);
			server.ClientConnected += new TcpClientConnectedEventHandler(server_ClientConnected);
			server.ClientDisconnected += new TcpClientDisconnectedEventHandler(server_ClientDisconnected);
			server.DataReceived += new TcpDataReceivedEventHandler(server_DataReceived);

			// Configuro el reloj
			sw = new System.Diagnostics.Stopwatch();
		}

		#region ITester Members

		/// <summary>
		/// Reconfigura de ser necesario el puerto de conexion del modulo
		/// </summary>
		/// <param name="port">Puerto de conexion</param>
		public void Setup(string moduleName, int port)
		{
			/*
			 * El puerto de comunicaciones puede ser cambiado
			 * mientras el servidor no se inicie. El nombre del
			 * modulo se ignora
			*/
			server.Port = port;
		}

		public void Run()
		{
			/*
			 * Se inicia el SocketTcpServer.
			*/
			server.Start();

			// Espero a que conecte
			while (server.ClientsConnected == 0)
				Thread.Sleep(10);

			//Inicia timer
			sw.Start();

			/*
			 * Se crea la variable mediante
			 * create_var "variable|0"
			*/
			server.SendToAll("create_var \"odometryPos|0\"");

			/*
			 * Realizo la suscripcion. Para el ejemplo no me interesa verificar lo
			 * que responde el Blackboard, pero todo llega a
			 *	server_DataReceived(...)
			 * incluso la basura, y se imprime en pantalla.
			 * 
			 * Nota que hay un error y dice "suscribe" en lugar de "subscribe".
			 * Esto cambiara en la siguiente version, por lo que puedes programar
			 * con "subscribe".
			*/
			server.SendToAll("suscribe_var \"localizerPos suscribe=writeany report=content\"");

			/*
			 * La suscripcion es para la variable 'variable'
			 * reportando cuando cualquier modulo escriba y reportando todo
			 * el contenido de la variable.
			 * 
			 * Ahora realizo 100 escrituras a intervalo de 100ms aprox.
			 * Se reporta el tiempo de demora.
			*/
			for (int i = 0; i < 100000; ++i)
			{
				// Se obtiene el valor a escribir en la variable en hex string
				byte[] iData = BitConverter.GetBytes(i);
				string sData = String.Empty;
				for (int j = 0; j < iData.Length; ++j)
					sData += iData[j].ToString("X2");

				server.SendToAll("write_var \"odometryPos|0x" + sData + "\"");
				Thread.Sleep(100);
			}

			server.Stop();
			sw.Stop();
		}

		#endregion

		#region Event Managers

		private void server_DataReceived(TcpPacket p)
		{
			long time = sw.ElapsedMilliseconds;

			// Hay que analizar cada una de las cadenas recibidas
			foreach (string s in p.DataStrings)
			{
				Console.WriteLine("Received: " + s);
				/*
				 * Se manejan los comandos alive y ready.
				 * Esto parece muy simplificado, pero un control mas
				 * robusto de las comunicaciones requiere de varias lineas de
				 * codigo extra> colas, semaforos, parsers, etc.
				*/
				switch (s)
				{
					case "alive":
						server.SendToAll("alive 1");
						break;

					case "ready":
						server.SendToAll("ready 1");
						break;
				}
			}
			Console.WriteLine("\ttime: " + time.ToString() + "ms");
		}

		private void server_ClientDisconnected(EndPoint ep)
		{
			Console.WriteLine("Disconnected from blackboard");
		}

		private void server_ClientConnected(Socket s)
		{
			Console.WriteLine("Connected to blackboard");
		}

		#endregion
	}
}

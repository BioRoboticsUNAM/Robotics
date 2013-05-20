using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Robotics.API;

namespace SharedVariableTester
{
	public class VolumeTester :ITester
	{
		/// <summary>
		/// Manejador de conexiones
		/// </summary>
		ConnectionManager connectionManager;

		/// <summary>
		/// Manejador de comandos
		/// </summary>
		CommandManager commandManager;

		/// <summary>
		/// Reloj de alta presicion para medir tiempos.
		/// </summary>
		System.Diagnostics.Stopwatch sw;

		/// <summary>
		/// Constructor. Inicializa instancias de la clase ConnectionManagerTester
		/// </summary>
		public VolumeTester()
		{
			/*
			 * Primero se crea el CommandManager. Este gestiona los comandos de
			 * sistema como alive, busy, ready, etc. Ahorra trabajo.
			 * Adicionalmente se suscriben los eventos (apuntador a funcion)
			 * para manejar las notificaciones de la clase
			*/
			commandManager = new CommandManager();
			commandManager.Started += new CommandManagerStatusChangedEventHandler(commandManager_Started);
			commandManager.Stopped += new CommandManagerStatusChangedEventHandler(commandManager_Stopped);
			commandManager.CommandReceived += new CommandReceivedEventHandler(commandManager_CommandReceived);
			commandManager.ResponseReceived += new ResponseReceivedEventHandler(commandManager_ResponseReceived);

			/*
			 * Ahora se inicializa el ConnectionManager. Bajo el esquema actual
			 * todas las aplicaciones son servidores y es el blackboard el que
			 * se conecta a ellas (asi solo es necesario configurar una
			 * aplicacion). Se le indica nombre del modulo, puerto de conexion
			 * y el gestor de comandos. El modulo y puerto deben ser
			 * configurados en el blackboard
			*/
			connectionManager = new ConnectionManager("TESTER", 2000, 2000, IPAddress.Loopback, commandManager);
			connectionManager.Started += new ConnectionManagerStatusChangedEventHandler(connectionManager_Started);
			connectionManager.Stopped += new ConnectionManagerStatusChangedEventHandler(connectionManager_Stopped);
			connectionManager.ClientDisconnected += new TcpClientDisconnectedEventHandler(connectionManager_ClientDisconnected);
			connectionManager.ClientConnected += new TcpClientConnectedEventHandler(connectionManager_ClientConnected);

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
			 * Una vez inicializado el connectionManager el puerto de
			 * comunicaciones y el nombre del modulo pueden ser cambiado
			 * mientras el servidor no se inicie. Se establecen los
			 * PortIn, PortOut al mismo valor para que trabaje en
			 * modo bidireccional.
			*/
			connectionManager.ModuleName = moduleName;
			connectionManager.PortIn = port;
			connectionManager.PortOut = port;
		}

		public void Run()
		{
			/*
			 * Se inicia el CommandManager. Si el ConnectionManager no ha 
			 * iniciado el CommandManager lo arranca de manera automatica.
			*/
			commandManager.Start();

			// Espero a que conecte
			while (connectionManager.ConnectedClientsCount == 0)
				Thread.Sleep(10);

			commandManager.Ready = true;
			connectionManager.Send(new Command("create_var", "variable|0"));
			connectionManager.Send(new Command("suscribe_var", "variable suscribe=writeany report=content"));

			string sData = null;
			StringBuilder sb;
			byte[] iData;

			sw.Start();
			for (int k = 0; k < 1000; ++k)
			{
				sData = String.Empty;
				for (int i = 0; i < 1023; ++i)
				{
					// Se obtiene el valor a escribir en la variable en hex string
					iData = BitConverter.GetBytes(i);
					for (int j = 0; j < iData.Length; ++j)
						sData += iData[j].ToString("X2");
				}
			}

			sw.Stop();
			Console.WriteLine("Data serialization append average: " + (sw.ElapsedMilliseconds / 1000) + " ms");
			sw.Reset();

			sw.Start();

			for (int k = 0; k < 1000; ++k)
			{
				sb = new StringBuilder(4096);
				for (int i = 0; i < 1023; ++i)
				{
					// Se obtiene el valor a escribir en la variable en hex string
					iData = BitConverter.GetBytes(i);
					for (int j = 0; j < iData.Length; ++j)
						sb.Append(iData[j].ToString("X2"));
				}
			}

			sw.Stop();
			Console.WriteLine("Data serialization SB average: " + (sw.ElapsedMilliseconds / 1000) + " ms");
			
			sw.Reset();
			sw.Start();
			for (int i = 0; i < 10000; ++i)
			{
				connectionManager.Send(new Command("write_var", "variable|0x" + sData));
				Thread.Sleep(100);
			}

			sw.Stop();
		}

		#endregion

		#region Event Managers

		void commandManager_ResponseReceived(Response response)
		{
			long time = sw.ElapsedMilliseconds;
			Console.WriteLine("Received response: " + response.ToString());
			Console.WriteLine("\ttime: " + time.ToString() + "ms");
		}

		void commandManager_Stopped(CommandManager commandManager)
		{
			Console.WriteLine("CommandManager stopped");
		}

		void commandManager_Started(CommandManager commandManager)
		{
			Console.WriteLine("CommandManager started");
		}

		private void commandManager_CommandReceived(Command command)
		{
			Console.WriteLine("Received command: " + command.ToString());
		}

		private void connectionManager_ClientConnected(Socket s)
		{
			Console.WriteLine("Connected to blackboard");
		}

		private void connectionManager_ClientDisconnected(EndPoint ep)
		{
			Console.WriteLine("Disconnected from blackboard");
		}

		private void connectionManager_Stopped(ConnectionManager connectionManager)
		{
			Console.WriteLine("ConnectionManager stopped");
		}

		private void connectionManager_Started(ConnectionManager connectionManager)
		{
			Console.WriteLine("ConnectionManager started");
		}

		#endregion
	}
}

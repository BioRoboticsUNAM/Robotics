using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Robotics.API;

namespace SharedVariableTester
{
	public class ConnectionManagerTester : ITester
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
		public ConnectionManagerTester()
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

			/*
			 * Se envia un comando ready para informar al BB que el modulo esta
			 * listo
			*/
			commandManager.Ready = true;

			//Inicia timer
			sw.Start();

			/*
			 * Se crea la variable mediante
			 * create_var "variable|0"
			*/
			connectionManager.Send(new Command("create_var", "variable|0"));

			/*
			 * Realizo la suscripcion. Para el ejemplo no me interesa verificar lo
			 * que responde el Blackboard, pero todo llega a
			 *	commandManager_ResponseReceived(...)
			 * y se imprime en pantalla.
			 * 
			 * Nota que hay un error y dice "suscribe" en lugar de "subscribe".
			 * Esto cambiara en la siguiente version, por lo que puedes programar
			 * con "subscribe".
			*/
			connectionManager.Send(new Command("suscribe_var", "variable suscribe=writeany report=content"));

			/*
			 * La suscripcion es para la variable 'variable'
			 * reportando cuando cualquier modulo escriba y reportando todo
			 * el contenido de la variable.
			 * 
			 * Ahora realizo 100 escrituras a intervalo de 100ms aprox.
			 * Se reporta el tiempo de demora.
			*/
			for (int i = 0; i < 100; ++i)
			{
				// Se obtiene el valor a escribir en la variable en hex string
				byte[] iData = BitConverter.GetBytes(i);
				string sData = String.Empty;
				for (int j = 0; j < iData.Length; ++j)
					sData += iData[j].ToString("X2");

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

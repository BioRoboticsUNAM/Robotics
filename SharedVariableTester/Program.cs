using System;

/*
 * System.Net contiene lo mas basico de redes. De ella se usa solamente
 * IPAddress, EndPoint e IPEndPoint para conexiones cliente-servidor
*/
using System.Net;

/*
 * System.Net.Sockets contiene todo el acceso a WinSocks y las clases 
 * SocketTcpClient Y SocketTcpServer que yo hice para gestionar comunicaciones
 * con sockets TCP bidireccionales.
*/
using System.Net.Sockets;

/*
 * System.Threading es la libreria de hilos, concurrencia y paralelismo de .NET
 * incluye monitores, semaforos, timers, threads, etc.
*/
using System.Threading;

/*
 * Robotics.API esta formada por un conjunto de clases que simpliican el desarrollo
 * bajo la arquitectura actual del software del robot.
 * 
 * Command				Encapsula un comando y provee metodos para su
 *						reconocimiento
 * Response				Encapsula una respuesta y provee metodos para su
 *						reconocimiento
 * ConnectionManager	Gestion de comunicaciones Cliente/Servidor
 * CommandManager		Gestion de comandos en 2o plano y ejecucion (a)sincrona de
 *						los mismos.
 * SyncCommandExecuter	Clase base para la ejecucion de comandos sincrona en el
 *						mismo hilo del CommandManager. No permite comunicaciones
 *						con otros modulos y se espera respuesta inmediata.
 * AsyncCommandExecuter	Clase base para la ejecucion de comandos asincrona. Cada
 *						comando es ejecutado en un hilo independiente, aunque no
 *						permite que se ejecuten de manera simultanea dos instancias
 *						del mismo comando
*/
using Robotics.API;

namespace SharedVariableTester
{
	class Program
	{

		static void Main(string[] args)
		{
			/* Aqui proveo dos enfoques de conexion.
			 * Prueba cada uno segun te convenga.
			 * 
			 * Aparte podria hacerse con los sockets de .NET, pero es latoso
			 * ponerlos a trabajar asincronos. En ese caso te proporciono el
			 * codigo fuente de SocketTcpServer y SocketTcpClient. Hasta donde
			 * se el codigo funciona en mono.
			 * 
			*/

			ITester tester;

			//tester = new ConnectionManagerTester();
			//tester = new SocketServerTester();
			//tester = new VolumeTester();
			tester = new SubscriptionTester();

			tester.Setup("MVN-PLN1", 2025);
			tester.Run();

			// Esta linea deja a la consola en espera para que veas la salida
			// Se cierra con CTRL + C
			while(true) Thread.Sleep(100);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using Client;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using Server;

namespace ServerApplication
{     
    class Program
    {
        private static GameServer game = new GameServer();
        private static Thread _thread;
        
        private static void IncomingClientRequest(PacketHeader packetHeader, Connection connection, ClientRequest request)
        {
            game.HandleRequest(request, connection);
        }

        private static void IncomingServerRequest(PacketHeader packetHeader, Connection connection, ServerRequest request)
        {
            
        }
        
        private static void ClientDisconnected(Connection connection)
        {
            game.deleteFromWaitRoom(connection);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[-] Client disconnected !");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void ClientConnected(Connection connection)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[+] New client !");
            Console.ForegroundColor = ConsoleColor.White;
            
            new Thread(() =>
            {
                game.FirstConnect(connection);                
            }).Start();
            
        }
        
        static void Main(string[] args)
        {
          
            NetworkComms.AppendGlobalIncomingPacketHandler<ServerRequest>("ServerRequest", IncomingServerRequest);
            NetworkComms.AppendGlobalIncomingPacketHandler<ClientRequest>("ClientRequest", IncomingClientRequest);
            NetworkComms.AppendGlobalConnectionCloseHandler(ClientDisconnected);
            NetworkComms.AppendGlobalConnectionEstablishHandler(ClientConnected);
            
            Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0));
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[+] Server listening for TCP conneciotn on:");
            foreach (var endPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
            {
                var localEndPoint = (IPEndPoint) endPoint;
                Console.WriteLine("[*] {0}:{1}", localEndPoint.Address, localEndPoint.Port);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;            
            Console.WriteLine("\n[!] Press any key to close server.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey(true);
            NetworkComms.Shutdown();
            System.Environment.Exit(0);
        }
    }
}
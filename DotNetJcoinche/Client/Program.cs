using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Client;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using Server;
using ServerApplication;

namespace ClientApplication
{
    class Program
    {
        private static bool         _shouldStop = false;
        private static GameClient   game; 
        
        private static void IncomingServerRequest(PacketHeader packetHeader,Connection connection, ServerRequest request)
        {
            game.HandleRequest(request);
        }
        
        private static void IncomingClientRequest(PacketHeader packetHeader,Connection connection, ClientRequest request)
        {
            Console.WriteLine("ClientRequest received");
        }

        private static void connectionDown(Connection connection)
        {
            Console.WriteLine("Cya");
            _shouldStop = true;
            lock (game)
            {
                Monitor.Pulse(game);
            }
        }
        
        static void Main(string[] args)
        {
            Connection _socket = null;
       
            if (args.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("./Server.exe [IP SERVER] [PORT]");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            
            string serverIP = args[0];
            int serverPort = int.Parse(args[1]);

            try{
                _socket = TCPConnection.GetConnection(new ConnectionInfo(serverIP, serverPort));
                _socket.AppendIncomingPacketHandler<ServerRequest>("ServerRequest", IncomingServerRequest);
                _socket.AppendIncomingPacketHandler<ClientRequest>("ServerRequest", IncomingClientRequest);
                _socket.AppendShutdownHandler(connectionDown);
                game = new GameClient(_socket);
            }catch (NetworkCommsDotNet.ConnectionSetupException exp)
            {
                Console.WriteLine("Invlide IP or Port, please check");
                return;
            }

            lock (game)
            {
                Monitor.Wait(game);
            }
        }
    }
}
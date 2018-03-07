using System;
using System.Collections.Generic;
using System.Threading;
using NetworkCommsDotNet.Connections;

namespace Server
{
    public class GameServer
    {
        private Board _board = new Board();

        private readonly Dictionary<OpCodeEnum, Action<ClientRequest, Connection>> OpHandler =
            new Dictionary<OpCodeEnum, Action<ClientRequest, Connection>>();

        private readonly List<ClientSide> waiting_room = new List<ClientSide>();

        public GameServer()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[+] GameServer Started !");
            Console.ForegroundColor = ConsoleColor.White;
            OpHandler.Add(OpCodeEnum.NAME, setName);
            OpHandler.Add(OpCodeEnum.BET, setBet);
            OpHandler.Add(OpCodeEnum.PLAY, player_played);
        }

        private ClientSide getClientSideByConnection(Connection connection)
        {
            ClientSide ret = null;

            foreach (var user in waiting_room)
                if (user.Socket == connection)
                    ret = user;
            return ret;
        }

        public void setName(ClientRequest resp, Connection connection)
        {
            ClientSide user = getClientSideByConnection(connection);

            if (user != null)
            {
                user.Name = resp.Name;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[*] Name of new client : " + user.Name);
                Console.ForegroundColor = ConsoleColor.White;
                if (this._board.addPlayer(user))
                    waiting_room.Remove(user);
            }
        }
        
        
        public void FirstConnect(Connection connection)
        {
            var tmp = new ClientSide(connection);
            waiting_room.Add(tmp);
            var req = new ServerRequest();
            req.Opcode = OpCodeEnum.WELCOME;
            Thread.Sleep(1000);
            connection.SendObject("ServerRequest", req);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[*] User added to waiting room");
            Console.WriteLine("[*] Witing room size : " + waiting_room.Count);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void deleteFromWaitRoom(Connection connection)
        {
            if (getClientSideByConnection(connection) != null)
            {
                waiting_room.Remove(getClientSideByConnection(connection));

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[!] Client removed from waiting room !");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }
            _board.interrupt_game();
            _board = new Board();
            waitingToBoard();
        }
        
        public void HandleRequest(ClientRequest req, Connection connection)
        {
            var   tmp = OpHandler[req.Opcode];
            tmp.Invoke(req, connection);
        }
        
        private void waitingToBoard(){
            foreach (var user in waiting_room)
            {
                if (user.Name != ""){
                    if ((this._board.addPlayer(user)))
                    {
                        waiting_room.Remove(user);
                    }
                }
            }
        }
        
        public void setBet(ClientRequest resp, Connection connection)
        {
            _board.setBet(resp);
        }
        
        public void player_played(ClientRequest req, Connection connection){
            _board.client_play(req);
        }
    }
}
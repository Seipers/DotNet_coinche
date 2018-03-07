using System.Collections.Generic;
using NetworkCommsDotNet.Connections;

namespace Server
{
    public class Team
    {
        private ClientSide P1;
        private ClientSide P2;
        private int score = 0;
        private int scoreDraw = 0;
        private Bet bet = new Bet();

        public Team() {}

        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        public int ScoreDraw
        {
            get { return scoreDraw; }
            set { scoreDraw = value; }
        }

        public Bet Bet
        {
            get { return bet; }
            set { bet = value; }
        }
                
        public void notifyScore(int point, string team){
            ServerRequest req = new ServerRequest();

            req.Opcode =  OpCodeEnum.WINDRAW;
            req.Points =  point;
            req.Team   =  team;
            P1.Socket.SendObject("ServerRequest", req);
            P2.Socket.SendObject("ServerRequest", req);
        }
        
        public void disconnect()
        {
            P1?.Socket.CloseConnection(true);
            P2?.Socket.CloseConnection(true);
        }
        
        public bool addPlayer(ClientSide player)
        {
            if (P1 == null)
                P1 = player;
            else if (P2 == null)
                P2 = player;
            else
                return false;
            return true;
        }
        
        public int size()
        {
            int i = 0;

            if (P1 != null)
                i++;
            if (P2 != null)
                i++;
            return (i);
        }
        
        public bool isFull()
        {
            return this.P1 != null && this.P2 != null;
        }
        
        public void AskBet(int player)
        {
            Connection socket;
            ServerRequest req = new ServerRequest();
        
            if (player == 0)
                socket = this.P1.Socket;
            else
                socket = this.P2.Socket;
            req.Opcode = OpCodeEnum.ASKBET;
            socket.SendObject("ServerRequest", req);
        }
        
        public void sendCard(int player, List<Card> deck)
        {
            Connection socket;
            ServerRequest req = new ServerRequest();

            if (player == 0)
            {
                P1.Cards = deck;
                socket = P1.Socket;
            }
            else
            {
                P2.Cards = deck;
                socket = P2.Socket;
            }
            req.Opcode = OpCodeEnum.DEAL;
            req.Cards = deck;
            socket.SendObject("ServerRequest",req);
        }

        public void setMyBet(int i, ClientRequest resp)
        {
            if (!resp.Mybet.Pass && !resp.Mybet.Surcoinche)
                this.bet = resp.Mybet;
            ServerRequest serverRequest = new ServerRequest();
            Connection socket;
            serverRequest.Opcode = OpCodeEnum.OTHERBET;
            serverRequest.Mybet = resp.Mybet;
            serverRequest.Team = "Ally";
            if (i != 0)
                socket = P1.Socket;
            else
                socket = P2.Socket;
            socket.SendObject("ServerRequest", serverRequest);
        }
        
        public void setEnemyBet(ClientRequest resp)
        {
            ServerRequest serverRequest = new ServerRequest();
            Connection socket;
            serverRequest.Opcode = OpCodeEnum.OTHERBET;
            serverRequest.Mybet = resp.Mybet;
            serverRequest.Team = "Enemy";
            socket = P1.Socket;
            socket.SendObject("ServerRequest" ,serverRequest);
            socket = P2.Socket;
            socket.SendObject("ServerRequest" ,serverRequest);
        }
        
        public void sendWinnerRound(Bet bet, string str){
            ServerRequest req = new ServerRequest();

            req.Opcode  = OpCodeEnum.DEALER;
            req.Team    = str;
            req.Mybet   = bet;
            P1.Socket.SendObject("ServerRequest", req);
            P2.Socket.SendObject("ServerRequest", req);
        }
        
        public void play_card(int player){
            ServerRequest    req = new ServerRequest();

            req.Opcode = OpCodeEnum.ASKPLAY;
            if (player == 0){
                P1.Socket.SendObject("ServerRequest", req);
            }else{
                P2.Socket.SendObject("ServerRequest", req);
            }
        }
        
        public bool play(int index, Card card_played, List<Card> hand){
            bool ret = false;
            ServerRequest req = new ServerRequest();

            req.Opcode = OpCodeEnum.OTHERPLAY;
            req.Team = "Ally ";
            req.Card = card_played;
            if (index == 0) {
                if ((ret = P1.play(card_played, hand)) == true)
                    P2.Socket.SendObject("ServerRequest", req);
            }
            else {
                if ((ret = P2.play(card_played, hand)) == true)
                    P1.Socket.SendObject("ServerRequest", req);
            }
            return (ret);
        }
        
        public void EnemyPlay(Card mycard) {
            ServerRequest req = new ServerRequest();

            req.Opcode = OpCodeEnum.OTHERPLAY;
            req.Team = "Enemy ";
            req.Card = mycard;
            P1.Socket.SendObject("ServerRequest", req);
            P2.Socket.SendObject("ServerRequest", req);
        }
        
        public void sendScore(int sc) {
            ServerRequest req = new ServerRequest();

            req.EnnemyPoints = sc;
            req.Points = score;
            req.Opcode = OpCodeEnum.SCORE;
            P1.Socket.SendObject("ServerRequest", req);
            P2.Socket.SendObject("ServerRequest", req);
        }
    }
}
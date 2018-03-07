using System;
using System.Collections.Generic;
using System.Security;
using NetworkCommsDotNet.Connections;
using Server;
using ServerApplication;

namespace Client
{
    public class GameClient
    {
        private Connection client;
        private List<Card>  cards;
        private List<Card>  hand = new List<Card>();
        private Dictionary<OpCodeEnum, Action<ServerRequest>> ReqContener = new Dictionary<OpCodeEnum, Action<ServerRequest>>();
        private int maxbet = 80;
        private bool coinche = false;
        private int pass = 0;

        public GameClient(Connection client)
        {
            this.client = client;
            this.ReqContener.Add(OpCodeEnum.WELCOME, new Action<ServerRequest>(WelcomeFuction));
            this.ReqContener.Add(OpCodeEnum.DEAL, new Action<ServerRequest>(getCard));
            this.ReqContener.Add(OpCodeEnum.ASKBET, new Action<ServerRequest>(askBet));
            this.ReqContener.Add(OpCodeEnum.OTHERBET, new Action<ServerRequest>(getOtherBet));
            this.ReqContener.Add(OpCodeEnum.DEALER, new Action<ServerRequest>(dealer_info));
            this.ReqContener.Add(OpCodeEnum.ASKPLAY, new Action<ServerRequest>(play_card));
            this.ReqContener.Add(OpCodeEnum.OTHERPLAY, new Action<ServerRequest>(otherPlay));
            this.ReqContener.Add(OpCodeEnum.WINDRAW, new Action<ServerRequest>(getWinHand));
            this.ReqContener.Add(OpCodeEnum.SCORE, new Action<ServerRequest>(Score));
        }
        
        public void HandleRequest(ServerRequest req)
        {
            Action<ServerRequest> tmp = ReqContener[req.Opcode];
            tmp.Invoke(req);
        }

        public void WelcomeFuction(ServerRequest req)
        {
            ClientRequest tmp = new ClientRequest();
            tmp.Opcode = OpCodeEnum.NAME;
            //scan = new Scanner(System.in);
            Console.WriteLine("What's your name ? ");
            String str = "";
            while (string.IsNullOrEmpty(str))
                str = Console.ReadLine();
            tmp.Name = str;
            this.client.SendObject("ClientRequest", tmp);
        }
        
        public void askBet(ServerRequest req)
        {
            Console.WriteLine("\nWhat's your bet ?");        
            ClientRequest resp = new ClientRequest();
            Bet mybet = new Bet();
            int choice = 0;

            while (choice < 1 || choice > 4)
            {
                Console.WriteLine("Choose ?\n1. Play | 2. Pass | 3. Coinche | 4. SurCoinche");
                try
                {
                    choice = int.Parse(Console.ReadLine());
                }
                catch (FormatException e)
                {
                    choice = 0;
                }
                catch (ArgumentNullException e)
                {
                    choice = 0;
                }
                catch (OverflowException e)
                {
                    choice = 0;
                }
                if (choice == 3 && (coinche || pass > 0 || maxbet == 80))
                {
                    Console.WriteLine("Can't coinche!");
                    choice = 0;
                }
                if (choice == 4 && (!coinche || pass % 2 == 1))
                {
                    Console.WriteLine("Can't surcoinche!");
                    choice = 0;
                }
                if (maxbet >= 160 && choice == 1) {
                    Console.WriteLine("Max bet already reached");
                    choice = 0;
                }
            }
            if (choice == 1)
                mybet = this.Bet(mybet);
            else if (choice == 2)
                mybet.Pass = true;
            else if (choice == 3)
                mybet.Coinche = true;
            else
                mybet.Surcoinche = true;
            resp.Opcode = OpCodeEnum.BET;
            resp.Mybet = mybet;
            client.SendObject("ClientRequest", resp);            
        }
        
        public Bet Bet(Bet mybet)
        {
            int color = 0;
            while (color < 1 || color > 4)
            {
                Console.WriteLine("Which ?\n1. Diamond | 2. Heart | 3. Club | 4. Spade");
                try
                {
                    color = int.Parse(Console.ReadLine());
                }
                catch (FormatException e)
                {
                    color = 0;
                }
                catch (ArgumentNullException e)
                {
                    color = 0;
                }
                catch (OverflowException e)
                {
                    color = 0;
                }
            }
                
            mybet.Color = CardEnum.getById(color);
            int bet = 0;
            while (bet < maxbet || bet > 160)
            {
                int i = this.maxbet;
                Console.Write("How many ?");
                while (i <= 160)
                {
                    Console.Write(" " + i);
                    if (i <= 150)
                        Console.Write(" |");
                    else
                        Console.WriteLine();
                    i += 10;
                }
                try
                {
                    bet = int.Parse(Console.ReadLine());
                }
                catch (FormatException e)
                {
                    bet = 0;
                }
                catch (ArgumentNullException e)
                {
                    bet = 0;
                }
                catch (OverflowException e)
                {
                    bet = 0;
                }
                if (bet % 10 != 0)
                    bet = 0;
            }
            mybet.Bet1 = (bet);
            return (mybet);
        } 
        
        public void getOtherBet(ServerRequest req)
        {
            Console.Write(req.Team + " ");
            Bet other = req.Mybet;

            if (other.Pass)
                pass++;
            else
                pass = 0;
            if (other.Pass)
                Console.WriteLine(" pass");
            else if (other.Coinche) {
                Console.WriteLine(" coinche");
                coinche = true;
            }
            else if (other.Surcoinche)
                Console.WriteLine(" surcoinche");
            else
            {
                this.maxbet = other.Bet1 + 10;
                Console.WriteLine("bet " + other.Color.Name + " " + other.Bet1);
            }
        }
        
        private void printCard()
        {
            int i = 0;
            foreach (Card tmp in cards)
            {
                Console.Write(tmp.Color.Code);
                Console.Write(tmp.Number.Code);
                if (i != cards.Count - 1)
                    Console.Write(" | ");
                i++;
            }
            Console.WriteLine();
        }
        
        public void getCard(ServerRequest req)
        {
            this.cards = req.Cards;
            this.printCard();
        }

        public void dealer_info(ServerRequest req){
            Console.WriteLine(req.Team + " win this round with " + req.Mybet.Color.Name + " bet : " + req.Mybet.Bet1);
        }
        
        public void play_card(ServerRequest req){
            bool turn_end = false;
            int     index = -1;

            while (!turn_end) {
                printCard();
                Console.Write("It's your turn, play card (Type index of your card) !\n=>");
                try
                {
                    index = int.Parse(Console.ReadLine());
                }
                catch (FormatException e)
                {
                    index = -1;
                }
                catch (ArgumentNullException e)
                {
                    index = -1;
                }
                catch (OverflowException e)
                {
                    index = -1;
                }
                if (index < 0 || index > cards.Count - 1)
                    Console.WriteLine("Invalid Index !");
                else if (!can_play(index))
                    Console.WriteLine("Don't play that !");
                else
                turn_end = true;
            }
            ClientRequest resp = new ClientRequest();
            resp.Mycard = cards[index];
            cards.RemoveAt(index);
            resp.Opcode = (OpCodeEnum.PLAY);
            client.SendObject("ClientRequest", resp);
        }
        
        private bool can_play(int index) {
            if (hand.Count > 0) {
                if (cards[index].Color.Type != hand[0].Color.Type) {
                    foreach (Card tmp in cards) {
                        if (tmp.Color.Type == hand[0].Color.Type)
                            return false;
                    }
                }
            }
            return true;
        }

        public void otherPlay(ServerRequest req)
        {
            hand.Add(req.Card);
            Console.WriteLine(req.Team + "played" + req.Card.Color.Name + " " + req.Card.Number.Name);
        }

        public void getWinHand(ServerRequest req)
        {
            hand.Clear();
            Console.WriteLine(req.Team + " score " + req.Points);
        }

        public void Score(ServerRequest req)
        {
            Console.WriteLine("Ally :" + req.Points + " point(s)");
            Console.WriteLine("Enemy :" + req.EnnemyPoints + " point(s)");
            maxbet = 80;
        }
    }
}
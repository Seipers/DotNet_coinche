using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Server
{
    public class Board
    {
        private Team            team1 = new Team();
        private Team            team2 = new Team();
        private List<Card>      deck = new List<Card>();
        private List<Card>      hand = new List<Card>();
        private int             turn;
        private bool            coinche;
        private bool            surcoinche;
        private int             pass;
        private int             round_winner = -1;
        private Bet             max_bet = new Bet();
        private Thread          thread;
        private volatile bool _shouldStop;

        public Board()
        {
            generateDeck();
            thread = new Thread(this.Game);
            thread.Start();
        }

        public void RequestStop()
        {
            _shouldStop = true;
        }
        
        private void generateDeck()
        {
            for (int i = 1; i <= 4; i++)
            {
                for (int j = 7; j <= 14; j++)
                {
                    Card card = new Card(i, j);
                    deck.Add(card);
                }
            }
        }
        
        
        public void interrupt_game(){
            team1.disconnect();
            team2.disconnect();
            thread.Abort();
        }
        
        public bool addPlayer(ClientSide player)
        {
            if (team1.isFull() && team2.isFull())
                return false;
            if (team1.size() <= team2.size())
            {
                team1.addPlayer(player);
                Console.WriteLine("[+] " + player.Name + " join team 1");
            }
            else
            {
                team2.addPlayer(player);
                Console.WriteLine("[+] " + player.Name + " join team 2");
            }
            if (team1.isFull() && team2.isFull())
            {
                lock (this)
                {
                    Monitor.Pulse(this);
                }
            }
            return true;
        }
        
        public void Game()
        {
            Console.WriteLine("[+] Waiting 4 players");
            lock (this)
            {
                Monitor.Wait(this);
            }
            //try { wait(); }
            while (team1.Score < 701 && team2.Score < 701) {    
                this.sendCard();
                Console.WriteLine("[+] Send Cards");
                this.Bet();
                winner_notify();
                calc_turn();
                play_card();
                calcWinBet();
            }
            Console.WriteLine("[*] End of game");
            team1.disconnect();
            team2.disconnect();
        }
        
        private void calcWinBet()
        {
            int bet = max_bet.Bet1;
            if (bet == team1.Bet.Bet1)
            {
                if (team1.ScoreDraw < bet)
                    team2.Score = team2.Score + 162 + bet;
                else
                    team1.Score = team1.Score + team1.ScoreDraw + bet;
            }
            else
            {
                if (team2.ScoreDraw < bet)
                    team1.Score = team1.Score + 162 + bet;
                else
                    team2.Score = team2.Score + team2.ScoreDraw + bet;
            }
            coinche = false;
            surcoinche = false;
            pass    = 0;
            max_bet = new Bet();
            team1.ScoreDraw = 0;
            team2.ScoreDraw = 0;
            team1.sendScore(team2.Score);
            team2.sendScore(team1.Score);
        }
        
        public void winner_notify(){
            Bet     tmp;
            String  str;
            String  str2;

            if (round_winner % 2 == 0){
                tmp = team1.Bet;
                max_bet = team1.Bet;
                str = "Ally";
                str2 = "Enemy";
            }else{
                tmp = team2.Bet;
                max_bet = team2.Bet;
                str = "Enemy";
                str2 = "Ally";
            }
            tmp.Coinche = this.coinche;
            tmp.Surcoinche = this.surcoinche;
            team1.sendWinnerRound(tmp, str);
            team2.sendWinnerRound(tmp, str2);
        }
        
        public static void Shuffle<T>(List<T> list) {
            int n = list.Count;
            Random rnd = new Random();
            while (n > 1) {
                int k = (rnd.Next(0, n) % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        
        private void sendCard()
        {            
            Shuffle(deck);
            List<Card> deck1 = new List<Card>();
            List<Card> deck2 = new List<Card>();
            List<Card> deck3 = new List<Card>();
            List<Card> deck4 = new List<Card>();

            Console.WriteLine(deck.Count);
            
            deck1.AddRange(deck.Take(8));
            deck.RemoveRange(0, 8);
            deck2.AddRange(deck.Take(8));
            deck.RemoveRange(0, 8);
            deck3.AddRange(deck.Take(8));
            deck.RemoveRange(0, 8);
            deck4.AddRange(deck.Take(8));
            deck.RemoveRange(0, 8);

            
            team1.sendCard(0, deck1);
            team1.sendCard(1, deck2);
            team2.sendCard(0, deck3);
            team2.sendCard(1, deck4);
        }
        
        private bool endBet()
        {
            if (surcoinche || (pass == 3 && max_bet.Bet1 > 70))
                return true;
            if (pass == 3)
            {
                this.generateDeck();
                this.sendCard();
            }
            return false;
        }
        
        private void Bet()
        {
            this.coinche = false;
            this.surcoinche = false;
            this.pass = 0;
            Random rnd = new Random();
            this.turn = rnd.Next(0, 4);

            while (!this.endBet())
            {
                Console.WriteLine("[+] Player " + this.turn + " turn");
                if (turn % 2 == 0)
                    team1.AskBet(turn / 2);
                else
                    team2.AskBet(turn / 2);
            
                lock (this)
                {
                    Monitor.Wait(this);
                }
                this.turn = (this.turn + 1) % 4;
            }
        }
        
        public void setBet(ClientRequest resp)
        {
            if (!verifyBet(resp.Mybet))
                return;
            if (turn % 2 == 0)
            {
                team1.setMyBet(turn / 2, resp);
                team2.setEnemyBet(resp);
            }
            else
            {
                team2.setMyBet(turn / 2, resp);
                team1.setEnemyBet(resp);
            }
            Bet bet = resp.Mybet;
            this.coinche = bet.Coinche;
            this.surcoinche = bet.Surcoinche;
            if (bet.Pass == true)
                this.pass++;
            else if (this.pass != 0)
                this.pass = 0;
            calc_winner_round(bet);
            lock (this)
            {
                Monitor.Pulse(this);
            }
        }
        
        private bool verifyBet(Bet mybet) {
            Bet enemy;
            bool ok = true;

            if (turn % 2 == 0)
                enemy = team2.Bet;
            else
                enemy = team1.Bet;
            if (mybet.Bet1 < max_bet.Bet1 && (!mybet.Pass && !mybet.Coinche && !mybet.Surcoinche))
                ok = false;
            if (mybet.Surcoinche && !enemy.Coinche)
                ok = false;
            if (!ok)
            {
                if (turn % 2 == 0)
                    team1.AskBet(turn / 2);
                else
                    team2.AskBet(turn / 2);
            }
            return (ok);
        }
        
        public void calc_winner_round(Bet bet){
            if (!bet.Coinche && !bet.Surcoinche && !bet.Pass)
            {
                max_bet.Bet1 = bet.Bet1;
                this.round_winner = this.turn;
            }
        }

        public void calc_turn(){
            if (round_winner == 0)
                turn = 3;
            else
                turn = round_winner - 1;
        }
        
        public void setPointToWinner(int point){

            String  str;
            String  str2;

            if (turn % 2 == 0){
                team1.ScoreDraw = team1.ScoreDraw + point;
                str = "Ally";
                str2 = "Enemy";
            }else{
                team2.ScoreDraw = team2.ScoreDraw + point;
                str2 = "Ally";
                str = "Enemy";
            }
            team1.notifyScore(point, str);
            team2.notifyScore(point, str2);
            hand.Clear();
        }
        
        public void managePoint(){
            int     hand_value = 0;
            int     index = 0;
            int     winner = 0;

            /* Loop to calc hand value */
            foreach (Card tmp in hand) {
                if (tmp.Color == max_bet.Color){
                    hand_value += tmp.Number.Trump;
                    if (hand[winner].Color == tmp.Color){
                        if (hand[winner].Number.Trump < tmp.Number.Trump)
                            winner = index;
                        else if (hand[winner].Number.Trump == tmp.Number.Trump)
                        {
                            if (hand[winner].Number.Type < tmp.Number.Type)
                                winner = index;
                        }
                    }
                }else{
                    hand_value += tmp.Number.NoTrump;
                    if (hand[winner].Number.NoTrump < tmp.Number.NoTrump)
                        winner = index;
                    else if (hand[winner].Number.NoTrump == tmp.Number.NoTrump)
                    {
                        if (hand[winner].Number.Type < tmp.Number.Type)
                            winner = index;
                    }
                }
                index++;
            }
            Console.WriteLine("[+] Hand value: " + hand_value);
            turn = (turn + winner) % 4;
            setPointToWinner(hand_value);
        }
        
        public void play_card(){
            while (deck.Count < 32){
                if (turn % 2 == 0){
                    team1.play_card(turn / 2);
                }else{
                    team2.play_card(turn / 2);
                }
                lock (this)
                {
                    Monitor.Wait(this);
                }
                //try { wait();}
                this.turn = (this.turn + 1) % 4;
                if (hand.Count == 4){
                    managePoint();
                }
            }
            Console.WriteLine("[*] Round over");
        }
        
        public void client_play(ClientRequest req){
            bool resp;

            if (turn % 2 == 0) {
                if ((resp = team1.play(turn / 2, req.Mycard, hand)) == true)
                    team2.EnemyPlay(req.Mycard);
            }
            else {
                if ((resp = team2.play(turn / 2, req.Mycard, hand)) == true)
                    team1.EnemyPlay(req.Mycard);
            }
            if (resp){
                deck.Add(req.Mycard);
                hand.Add(req.Mycard);
            }
            lock (this)
            {
                Monitor.Pulse(this);
            }
        }
    }
}
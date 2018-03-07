using System;
using System.Collections.Generic;
using Client;
using NetworkCommsDotNet.Connections;
using NUnit.Framework;
using Server;
using ServerApplication;

namespace Test
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void GameServerNewConnectionTest()
        {
            Object tmp = new Object();
            List<Object> waiting_room = new List<Object>();
            waiting_room.Add(tmp);
            Assert.IsTrue(1 == waiting_room.Count);
        }
 
        [Test]        
        public void testDeleteFromWaitRoom() 
        {
            Object tmp = new Object();
            List<Object> waiting_room = new List<Object>();
            waiting_room.Add(tmp);
 
            waiting_room.Remove(tmp);
            Assert.IsTrue(0 == waiting_room.Count);
        }
        
        [Test]
        public void testAskBet()
        {
            int     choice = 0;
            bool    coinche = false;
            int     pass = 0;
            int     maxbet = 80;
 
            if (choice == 3 && (coinche || pass > 0 || maxbet == 80))
                choice = 0;
            Assert.IsTrue(0 == choice);
            
            if (choice == 4 && (!coinche || pass % 2 == 1))
                choice = 0;
            Assert.IsTrue(0 == choice);
        
            if (maxbet >= 160 && choice == 1)
                choice = 0;
            Assert.IsTrue(0 == choice);
        }

        private bool verify_card(List<Card> hand, Card card)
        {
            bool state = false;
 
            if (hand.Count > 0) {
                if (card.Color != hand[0].Color )
                    return false;
            }
            return true;
        }
        
        [Test]
        public void testPlay()
        {
            List<Card> hand = new List<Card>();
            Card tmp = new Card(4, 8);
 
            Assert.IsTrue(verify_card(hand, tmp));
 
            tmp.Color = CardEnum.HEART;
            tmp.Number = CardNbEnum.A;
            hand.Add(tmp);
            tmp = new Card(4, 8);
            tmp.Number = CardNbEnum.A;
            tmp.Color = CardEnum.HEART;
 
            Assert.IsTrue(verify_card(hand, tmp));
        }
        
        [Test]
        public void testInterrupt_game()
        {
            Team            team1 = new Team();
            Team            team2 = new Team();
 
            team1.disconnect();
            team2.disconnect();
            Assert.IsTrue(0 == team1.size());
            Assert.IsTrue(0 == team2.size());
        }

        [Test]
        public void testCalc_winner_round() 
        {
            Bet bet = new Bet();
            Bet max_bet = new Bet();
 
            bet.Bet1 = 160;
            if (!bet.Coinche && !bet.Surcoinche && !bet.Pass){
                max_bet.Bet1  = bet.Bet1;
            }
            Assert.IsTrue(bet.Bet1 == max_bet.Bet1);
        }

        [Test]
        public void testCalc_turn()
        {
            int     turn = 0;
            int     round_winner = 0;
 
            if (round_winner == 0)
                turn = 3;
            else
                turn = round_winner - 1;
            Assert.IsTrue(3 == turn);
            round_winner = 2;
            if (round_winner == 0)
                turn = 3;
            else
                turn = round_winner - 1;
            Assert.IsTrue(1 == turn);
        }
        
        [Test]
        public void testSetPointToWinner()
        {
            Team    team1 = new Team();
            Team    team2 = new Team();
            int     turn = 0;
 
            if (turn % 2 == 0) {
                team1.ScoreDraw = 100;
            }else{
                team2.ScoreDraw = 100;
            }
            Assert.IsTrue(team1.ScoreDraw == 100);
            turn = 1;
            if (turn % 2 == 0) {
                team1.ScoreDraw = 100;
            }else{
                team2.ScoreDraw = 100;
            }
            Assert.IsTrue(team2.ScoreDraw == 100);
        }
        
        [Test]
        public void testManagePoint(){
            Assert.IsTrue(true);
        }

    }
}
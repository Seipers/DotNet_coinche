using System;
using System.Collections.Generic;
using NetworkCommsDotNet.Connections;

namespace Server
{
    public class ClientSide
    {
        string              name = "";
        Connection          socket;
        List<Card>          cards;

        public ClientSide(Connection socket)
        {
            this.socket = socket;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Connection Socket => socket;

        public List<Card> Cards
        {
            get { return cards; }
            set { cards = value; }
        }

        private bool verify_card(Card card, List<Card> hand)
        {
            if (hand.Count > 0) {
                if (card.Color != hand[0].Color) {
                    
                    foreach (Card tmp in cards) {
                        if (tmp.Color == hand[0].Color)
                            return false;
                    }
                }
            }
            return true;
        }
        
        private void resend_card(Card card_played){
            ServerRequest   req = new ServerRequest();

            req.Opcode = OpCodeEnum.BADCARD;
            req.Card = card_played;
            socket.SendObject("ServerRequest", req);
        }
        
        public bool play(Card card_played, List<Card> hand) {

            if (!verify_card(card_played, hand))
            {
                Console.WriteLine("Bad Play");
                resend_card(card_played);
                return false;
            }
            foreach (Card tmp in cards)
            {
                if (tmp.Color.Type == card_played.Color.Type && tmp.Number.Type == card_played.Number.Type) {
                    Console.WriteLine("[*] " + name + " Played " + card_played.Color.Name + " " + card_played.Number.Name);
                    return (true);
                }
            }
            Console.WriteLine(cards.Count);
            Console.WriteLine("Bad Card");
            resend_card(card_played);
            return false;
        }
    }
}
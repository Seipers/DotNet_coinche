using ProtoBuf;
using ServerApplication;

namespace Server
{
    [ProtoContract]
    public class Bet
    {
        [ProtoMember(1)] private CardEnum    color;
        [ProtoMember(2)] private int         bet;
        [ProtoMember(3)] private bool        coinche;
        [ProtoMember(4)] private bool        surcoinche;
        [ProtoMember(5)] private bool        pass;

        public Bet()
        {
            this.bet = 0;
            this.coinche = false;
            this.surcoinche = false;
            this.pass = false;
        }

        public CardEnum Color
        {
            get { return color; }
            set { color = value; }
        }

        public int Bet1
        {
            get { return bet; }
            set { bet = value; }
        }

        public bool Coinche
        {
            get { return coinche; }
            set { coinche = value; }
        }

        public bool Surcoinche
        {
            get { return surcoinche; }
            set { surcoinche = value; }
        }

        public bool Pass
        {
            get { return pass; }
            set { pass = value; }
        }
    }
}
using Client;
using ProtoBuf;
using ServerApplication;

namespace Server
{
    [ProtoContract]
    public class Card
    {
        [ProtoMember(1)]private CardEnum       color;
        [ProtoMember(2)]private CardNbEnum     number;

        private Card()
        {
        }

        public Card(CardEnum color, CardNbEnum number)
        {
            this.color = color;
            this.number = number;
        }

        public CardEnum Color
        {
            get { return color; }
            set { color = value; }
        }

        public CardNbEnum Number
        {
            get { return number; }
            set { number = value; }
        }
    }
}
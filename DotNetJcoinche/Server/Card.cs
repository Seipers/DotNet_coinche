using Client;
using ProtoBuf;
using ServerApplication;

namespace Server
{
    [ProtoContract]
    public class Card
    {
        [ProtoMember(1)] private CardEnum color;
        [ProtoMember(2)] private CardNbEnum number;

        
        private Card()
        {
        }
        
        public Card(int color, int number)
        {
            int i = 0;
            foreach (var tmp in CardEnum.Values)
            {
                if (i == color - 1)
                {
                    this.color = tmp;
                    break;
                }
                i++;
            }
            i = 0;
            foreach (var tmp in CardNbEnum.Values)
            {
                if (i == number - 7)
                {
                    this.number = tmp;
                    break;
                }
                i++;
            }
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
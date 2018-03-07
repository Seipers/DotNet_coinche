using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ProtoBuf;

namespace Client
{
    [ProtoContract]
    public class CardNbEnum
    {
        public static readonly CardNbEnum _7 = new CardNbEnum(7, "Seven", "7", 0, 0);
        public static readonly CardNbEnum _8 = new CardNbEnum(8, "Eight", "8",0, 0);
        public static readonly CardNbEnum _9 = new CardNbEnum(9, "Nine", "9",14, 0);
        public static readonly CardNbEnum _10 = new CardNbEnum(10, "Ten", "10", 10, 10);
        public static readonly CardNbEnum J = new CardNbEnum(11, "Jack", "J", 20, 2);
        public static readonly CardNbEnum Q = new CardNbEnum(12, "Queen", "Q", 3, 3);
        public static readonly CardNbEnum K = new CardNbEnum(13, "King", "K", 4, 4);
        public static readonly CardNbEnum A = new CardNbEnum(1, "As", "A", 11, 11);

        public static IEnumerable<CardNbEnum> Values
        {
            get
            {
                yield return _7;
                yield return _8;
                yield return _9;
                yield return _10;
                yield return J;
                yield return Q;
                yield return K;
                yield return A;
            }
        }
        
        [ProtoMember(1)]private readonly int type;
        [ProtoMember(2)]private readonly string name;
        [ProtoMember(3)]private readonly string code;
        [ProtoMember(4)]private int trump;
        [ProtoMember(5)]private int notrump;

        public CardNbEnum(int type, string name, string code, int trump, int notrump)
        {
            this.type = type;
            this.name = name;
            this.code = code;
            this.trump = trump;
            this.notrump = notrump;
        }
        
        protected CardNbEnum() {}

        public int Type => type;
        public string Name => name;
        public string Code => code;
        public int Trump => trump;
        public int NoTrump => notrump;
    }
}
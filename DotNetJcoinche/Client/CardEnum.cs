using System;
using System.Collections.Generic;
using ProtoBuf;

namespace ServerApplication
{
    [ProtoContract]
    public class CardEnum
    {
      public static readonly CardEnum DIAMOND   = new CardEnum(1, "diamond", "D");
      public static readonly CardEnum HEART     = new CardEnum(2, "heart", "H");
      public static readonly CardEnum CLUB      = new CardEnum(3, "club", "C");
      public static readonly CardEnum SPADE     = new CardEnum(4, "spade", "S");

        public static IEnumerable<CardEnum> Values
        {
            get
            {
                yield return DIAMOND;
                yield return HEART;
                yield return CLUB;
                yield return SPADE;
            }
        }

        [ProtoMember(1)]private readonly int        type;
        [ProtoMember(2)]private readonly string     name;
        [ProtoMember(3)]private readonly string     code;

        protected CardEnum() { }

        public static CardEnum getById(int id)
        {
            int i = 1;
            foreach (CardEnum tmp in Values)
            {
                if (i == id)
                    return tmp;
            }
            return DIAMOND;
        }
                
        public CardEnum(int type, string name, string code)
        {
            this.type = type;
            this.name = name;
            this.code = code;
        }

        public int Type => type;

        public string Name => name;

        public string Code => code;
    }
}
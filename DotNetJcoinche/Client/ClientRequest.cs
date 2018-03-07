using System;
using ProtoBuf;

namespace Server
{
    [ProtoContract]
    public class ClientRequest
    {
        [ProtoMember(1)] OpCodeEnum     opcode;
        [ProtoMember(2)] string         name;
        [ProtoMember(3)] Bet            mybet;
        [ProtoMember(4)] Card           mycard;

        public ClientRequest(){}

        public OpCodeEnum Opcode
        {
            get { return opcode; }
            set { opcode = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Bet Mybet
        {
            get { return mybet; }
            set { mybet = value; }
        }

        public Card Mycard
        {
            get { return mycard; }
            set { mycard = value; }
        }
    }
}
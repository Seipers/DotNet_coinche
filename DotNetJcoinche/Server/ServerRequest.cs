using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server
{
    [ProtoContract]
    public class ServerRequest
    {
        [ProtoMember(1)] OpCodeEnum  opcode;
        [ProtoMember(2)] int         nb_player;
        [ProtoMember(3)] List<Card>  cards;
        [ProtoMember(4)] Card        card;
        [ProtoMember(5)] string      team;
        [ProtoMember(6)] string      name;
        [ProtoMember(7)] Bet         mybet;
        [ProtoMember(8)] int         points;
        [ProtoMember(9)] int         Ennemy_points;

        public ServerRequest(){}

        public OpCodeEnum Opcode
        {
            get { return opcode; }
            set { opcode = value; }
        }

        public int NbPlayer
        {
            get { return nb_player; }
            set { nb_player = value; }
        }

        public List<Card> Cards
        {
            get { return cards; }
            set { cards = value; }
        }

        public Card Card
        {
            get { return card; }
            set { card = value; }
        }

        public string Team
        {
            get { return team; }
            set { team = value; }
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

        public int Points
        {
            get { return points; }
            set { points = value; }
        }

        public int EnnemyPoints
        {
            get { return Ennemy_points; }
            set { Ennemy_points = value; }
        }
    }
}
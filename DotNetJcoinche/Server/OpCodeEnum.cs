using ProtoBuf;

namespace Server
{
    [ProtoContract]
    public enum OpCodeEnum
    {
        WELCOME = 1,
        DEAL = 2,
        ASKBET = 3,
        OTHERBET = 4,
        DEALER = 5,
        ASKPLAY = 6,
        OTHERPLAY = 7,
        WINDRAW = 8,
        SCORE = 9,
        BADCARD = 50,
        NAME = 100,
        BET = 101,
        PLAY = 102
    };
}
using System;
using MessagePack;

namespace MsgPack
{
    [Union(0, typeof(LeftClass))]
    [Union(1, typeof(RightClass))]
    public interface IUnionSample
    {
    }

    [MessagePackObject]
    public class LeftClass : IUnionSample
    {
        [Key(0)]
        public int Left { get; set; }
    }

    [MessagePackObject]
    public class RightClass : IUnionSample
    {
        [Key(0)]
        public string Right { get; set; }
    }

    public static class Capability
    {
        public static void Run()
        {
            IUnionSample data = new LeftClass() { Left = 999 };

            var bin = MessagePackSerializer.Serialize(data);
            Console.WriteLine(MessagePackSerializer.ToJson(bin));

            var reData = MessagePackSerializer.Deserialize<IUnionSample>(bin);
            Console.WriteLine(((LeftClass)reData).Left);

            // type-switch of C# 7.0
            /*switch (reData)
            {
                case LeftClass x:
                    Console.WriteLine(x.Left);
                    break;
                case RightClass x:
                    Console.WriteLine(x.Right);
                    break;
                default:
                    break;
            }*/
        }
    }
}

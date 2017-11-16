using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using System.Diagnostics;
using Newtonsoft.Json;

namespace MsgPack
{
    public static class Tests
    {
        [MessagePackObject]
        public class Student
        {
            [Key(0)]
            public string Name { get; set; }

            [Key(1)]
            public int Age { get; set; }

            [Key(2)]
            public string FavoriteSubject { get; set; }

            [IgnoreMember]
            public string FullDataToString { get { return "Name: " + Name + "\n" + "Age: " + Age + "\n" + "FavoriteSubject: " + FavoriteSubject; } }
        }

        [MessagePackObject]
        public class SuperStudent
        {
            [Key(0)]
            public string Name { get; set; }

            [Key(1)]
            public int Age { get; set; }

            [Key(2)]
            public List<int> FavoriteNumbers { get; set; }

            [Key(3)]
            public List<string> FavoriteFriends { get; set; }
        }

        private static Student CreateStudent()
        {
            return new Student
            {
                Age = 21,
                Name = "Николай",
                FavoriteSubject = "Проектирование и администрирование сетей"
            };
        }

        private static SuperStudent CreateSuperStudent()
        {
            return new SuperStudent
            {
                Age = 21,
                Name = "Николай",
                FavoriteNumbers = Enumerable.Range(1, 10000000).ToList(), // тут 10 миллионов хех.
                FavoriteFriends = Enumerable.Range(1, 10000000).Select(x => x.ToString()).ToList()
            };
        }

        private static void TestSerializeDeserialize()
        {
            var bytes = MessagePackSerializer.Serialize(CreateStudent());
            var student = MessagePackSerializer.Deserialize<Student>(bytes);

            Console.WriteLine(student.FullDataToString);

            var json = MessagePackSerializer.ToJson(bytes);
            Console.WriteLine(json);
        }

        private static void TestSuperStudent(SuperStudent supStudent) // ~ 3sec, колебалось от 2,5 до 3,5
        {
            var bytes = MessagePackSerializer.Serialize(supStudent);
            Console.WriteLine(bytes.Length);
            var student = MessagePackSerializer.Deserialize<SuperStudent>(bytes);

            //Console.WriteLine(student.FullDataToString);
        }

        private static void TestSuperStudentJson(SuperStudent supStudent) // > 10 sec xex.
        {
            var json = JsonConvert.SerializeObject(CreateSuperStudent());
            Console.WriteLine(json.Length);
            Console.WriteLine(JsonConvert.DeserializeObject<SuperStudent>(json));

            //Console.WriteLine(student.FullDataToString);
        }

        public static void RunTest(int number)
        {
            Stopwatch sw = new Stopwatch();
            switch (number)
            {
                case 1:
                    TestSerializeDeserialize();
                    break;
                case 2:
                    sw.Start();
                    TestSuperStudent(CreateSuperStudent());
                    sw.Stop();
                    Console.WriteLine("Время msgpack:" + (sw.ElapsedMilliseconds / 1000.0).ToString());
                    break;
                case 3:
                    sw.Start();
                    TestSuperStudentJson(CreateSuperStudent());
                    sw.Stop();
                    Console.WriteLine("Время json" + (sw.ElapsedMilliseconds / 1000.0).ToString());
                    break;
                default:
                    sw.Start();
                    TestSerializeDeserialize();
                    sw.Stop();
                    Console.WriteLine((sw.ElapsedMilliseconds / 1000.0).ToString());
                    break;
            }
        }
    }
}

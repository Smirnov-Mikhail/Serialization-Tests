using System;
using ProtoBuf;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MsgPack
{
    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }

    [Serializable] // <-- Только для BinaryFormatter
    [ProtoContract]
    public class Task
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public DateTime CreatedAt { get; set; }

        [ProtoMember(3)]
        public string CreatedBy { get; set; }

        [ProtoMember(4)]
        public TaskPriority Priority { get; set; }

        [ProtoMember(5)]
        public string Content { get; set; }
    }

    public static class Protobuf
    {
        public static void Run()
        {
            var tasks = new List<Task>
                            {
                                new Task
                                    {
                                        Id = 1,
                                        CreatedBy = "Steve Jobs",
                                        CreatedAt = DateTime.Now,
                                        Priority = TaskPriority.High,
                                        Content = "Invent new iPhone"
                                    },
                                new Task
                                    {
                                        Id = 2,
                                        CreatedBy = "Steve Steve",
                                        CreatedAt = DateTime.Now.AddDays(-7),
                                        Priority = TaskPriority.Low,
                                        Content = "Install Skype"
                                    }
                            };

            Console.WriteLine("The test of binary formatter:");

            const string file1 = "tasks1.bin";

            TestBinaryFormatter(tasks, file1, 1000);
            TestBinaryFormatter(tasks, file1, 2000);
            TestBinaryFormatter(tasks, file1, 3000);
            TestBinaryFormatter(tasks, file1, 4000);
            TestBinaryFormatter(tasks, file1, 5000);

            Console.WriteLine("\nThe test of protobuf-net:");

            const string file2 = "tasks2.bin";

            TestProtoBuf(tasks, file2, 1000);
            TestProtoBuf(tasks, file2, 2000);
            TestProtoBuf(tasks, file2, 3000);
            TestProtoBuf(tasks, file2, 4000);
            TestProtoBuf(tasks, file2, 5000);

            Console.WriteLine("\nThe comparision of file size:");

            Console.WriteLine("The size of {0} is {1} bytes", file1, (new FileInfo(file1)).Length);
            Console.WriteLine("The size of {0} is {1} bytes", file2, (new FileInfo(file2)).Length);

            Console.ReadKey();
        }

        private static void TestBinaryFormatter(IList<Task> tasks, string fileName, int iterationCount)
        {
            var stopwatch = new Stopwatch();
            var formatter = new BinaryFormatter();
            using (var file = File.Create(fileName))
            {
                stopwatch.Restart();

                for (var i = 0; i < iterationCount; i++)
                {
                    file.Position = 0;
                    formatter.Serialize(file, tasks);
                    file.Position = 0;
                    var restoredTasks = (List<Task>)formatter.Deserialize(file);
                }

                stopwatch.Stop();

                Console.WriteLine("{0} iterations in {1} ms", iterationCount, stopwatch.ElapsedMilliseconds);
            }
        }

        private static void TestProtoBuf(IList<Task> tasks, string fileName, int iterationCount)
        {
            var stopwatch = new Stopwatch();
            using (var file = File.Create(fileName))
            {
                stopwatch.Restart();

                for (var i = 0; i < iterationCount; i++)
                {
                    file.Position = 0;
                    Serializer.Serialize(file, tasks);
                    file.Position = 0;
                    var restoredTasks = Serializer.Deserialize<List<Task>>(file);
                }

                stopwatch.Stop();

                Console.WriteLine("{0} iterations in {1} ms", iterationCount, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
namespace threadTest
{
    class Program1
    {
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string n, int type);
        static void Main3(string[] args)
        {
            int hh = Environment.ProcessorCount;
            int iii = MessageBox((IntPtr)0, "提示標題", "內容", 0);
            ThreadTest tt = new ThreadTest();
            new Thread(tt.Go).Start();
            
            tt.Go();
            Console.WriteLine(tt.count + ":" + tt.done);
            Console.WriteLine(tt.count + ":" + tt.done);
            Console.ReadKey();
            Thread t = new Thread(writeY) { Name="Qoo" };
            //t.Name = "t1"; //t.Priority = ThreadPriority.Highest;
            Thread t2 = new Thread(writeY1);
            Thread t3 = new Thread(T3Func);
            //Thread t4 = new Thread(writeY3);
            Console.Write("|" + t2.ManagedThreadId + "|");
            t.Start();
            t.Join();
            Thread r = new Thread(() => Console.WriteLine("Hello!!"));
            r.Start();
            t2.Start(); t2.Join();
            t3.Start("======");
            new Thread(() => 
            {
                Console.WriteLine("I'm running on another thread!");
                Console.WriteLine("this is so easy!");
            }).Start();
            new Thread(delegate() {
                Console.WriteLine("this is second delegate use C#2.0!");
            }).Start();
            //t4.Start(); //t2.Join();
            Thread.Yield();
            Thread.Sleep(TimeSpan.FromSeconds(3));//delay 3 second
            Console.Write("Main thread:" + Thread.CurrentThread.ManagedThreadId + ":");
            Console.Write(" " + (Thread.CurrentThread.Name = "TEST") + " ");
            //Console.Write("|" + t.IsAlive.ToString() + "|" + t.IsBackground.ToString() + "|");
            //t4.Join();
            int qoo = 0;
            for (int i = 0; i < 10; i++)
            {
                int tmp = i;
                new Thread(() => Console.Write("\n " + (qoo = tmp) + "<=why default=>" + i + " ")).Start();
            }
            Console.Write("anymouseThread[" + qoo + "]");
                for (int i = 0; i < 1000; i++)
                    Console.Write("X");
        }

        static void T3Func(object msg)
        {
            Console.WriteLine(msg);
            string m = (string)msg;
            Console.WriteLine(m);
        }
        
        private static void writeY3(object obj)
        {
            for (int i = 0; i < 1000; i++)
                Console.Write("3");
        }

        private static void writeY2(object obj)
        {
            for (int i = 0; i < 1000; i++)
                Console.Write("2");
        }

        private static void writeY1(object obj)
        {
            for (int i = 0; i < 1000; i++)
                Console.Write("1");
        }
        static void writeY()
        {
            for (int i = 0; i < 1000; i++)
                Console.Write("O");
        }
        void WriteO()
        {
            for (int i = 0; i < 1000; i++)
                Console.Write("O");
        }
    }
}

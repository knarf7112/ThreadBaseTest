using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace threadTest
{
    class ThreadTest
    {
        [DllImport("User32.dll", CharSet=CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string n, int type);
        public bool done;
        public int count;
        public static bool done2;
        public ThreadTest()
        {
            done = false;
            count = 0;
        }
        static void Main2()
        {
            ThreadTest tt = new ThreadTest();
            new Thread(tt.Go).Start();//當thread引用公用的目標實例時,則會共享資料
            tt.Go(); //所以幾乎只會輸出一次done
            Console.ReadKey();
            WebClient cc = new WebClient();
            
        }
        static void Main4(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread worker = new Thread(delegate() { Console.ReadLine(); });
            if (args.Length > 0)
            {
                worker.IsBackground = true;
            }
            worker.Start();
            //Thread.CurrentThread.Name = "main";
            //Thread worker = new Thread(WriteText);
            //worker.Name = "worker";
            //worker.Start();

            //Thread tt = new Thread(new ThreadStart(Go2));//使用ThreadStart委派來調用靜態方法
            //tt.Start();
            //Go2();
            //--------------

            //Thread t2 = new Thread(Go3);
            //t2.Start(true);//把Go3方法的需要的參數在Start時輸入
            //Go3(false);
            //string text = "Before";
            //Thread t3 = new Thread(delegate() { WriteText(text); });
            //text = "After";
            //t3.Start();

            //WriteText(new object());
            Console.ReadKey();
            
        }
        static void Main5()
        {
            Thread.CurrentThread.Name = "Main";
            Task taskA = new Task(()=>Console.WriteLine("Hello from taskA"));
            taskA.Start();
            taskA.Wait();//.Wait() method to ensure the task completed
            // so output: 
            //       Hello from taskA. 
            //       Hello from thread 'Main'.
            Console.WriteLine("Hello from thread '{0}'.", Thread.CurrentThread.Name);
            //ThreadPool.SetMinThreads(1, 3);
            int i,j;
            ThreadPool.GetMinThreads(out i, out j);
            Console.WriteLine(i + " : " + j);
            var s =Thread.CurrentThread.IsThreadPoolThread;
            ThreadPool.QueueUserWorkItem(new WaitCallback((object obj) => Console.WriteLine("qoo")));
            //taskA.Wait();//if is this that will display:
            //       Hello from thread 'Main'. 
            //       Hello from taskA.
            Console.ReadKey();
        }
        static void Main6()
        {
            Action a = () => { 
                int i = 0;
                Console.WriteLine(i + ":action->");
                
            };
            //Task is System.Threading.Task
            Task tResult = Task.Factory.StartNew(a);//this is nongeneric task
            Task.Factory.StartNew(Go2);
            Console.WriteLine("-------------");
            tResult.Wait();//wait for it to complete by calling Wait();
            Console.ReadKey();
        }

        static void Main7()
        {
            //generic Task<TResult> 是非泛型(Nongeneric)Task的子類別
            //start the task executing
            Task<string> task = Task.Factory.StartNew<string>(() => WebDownloadString("http://www.google.com"));
            
            //we can do other work here and it will execute in parallel
            RunSomeOtherMethod();

            // When we need the task's return value, we query its Result property:
            // If it's still executing, the current thread will now block (wait)
            // until the task finishes
            Console.WriteLine(task.Result + " : " + task.Result.Length);
            Console.ReadKey();
        }
        static void Main8()
        {
            ThreadPool.QueueUserWorkItem(P1);
            ThreadPool.QueueUserWorkItem(P1, 123);//前面P1表示傳入的方法,後面123表示P1的參數
            object o = new { a = 456,b = "ff"};
            Type t = o.GetType();
            var x = t.GetProperty("a").GetValue(o);
            ParameterizedThreadStart e = new ParameterizedThreadStart(P1); 
            e.Invoke(x);
            Console.ReadKey();
        }
        static void Main9()
        {
            int input = 1000000;
            Func<int, int> method = Work;
            IAsyncResult cookie = method.BeginInvoke(input, null, null);
            //
            // ... here's where we can do other work in parallel...
            //
            Console.WriteLine("Calculate...........");
            int result = method.EndInvoke(cookie);
            Console.WriteLine("result length:" + result);
            Console.WriteLine("Calculate...End......");
            Console.ReadKey();
        }
        static int Work(int s)
        {
            int tmp = 0;
            for (int i = 0; i < s; i++)
            {
                tmp += i;
            }
            Thread.Sleep(3000);
                return tmp;
        }
        static void P1(object data)
        {
            Console.WriteLine("Hello from the ThreadPool! " + data);
        }
        private static void RunSomeOtherMethod()
        {
            Console.WriteLine("--------------------------------------");
        }

        private static string WebDownloadString(string url)
        {
            using (WebClient wc = new WebClient())
                return wc.DownloadString(url);
        }
        private static void WriteText(object obj)
        {
            Console.WriteLine("this thread name :" + Thread.CurrentThread.Name);
        }
        public static void WriteText(string text)
        {
            Console.WriteLine(text);
        }
        public delegate void ParameterizedThreadStart(object obj);
        public static void Go2()
        {
            if (!done2)
            {
                done2 = true;
                Console.WriteLine("Done");
                //done = true;//done放後面使得輸出兩次的機會大幅提升
            }
        }
        public static void Go3(object upperCase)
        {
            bool upper = (bool)upperCase;
            Console.WriteLine(upper ? "HELLO!" : "hello!");
        }
        public void Go()
        {
            if (!done)
            {
                done = true;
                Console.WriteLine("Done");
                //done = true;//done放後面使得輸出兩次的機會大幅提升
            }
            count++;
        }
    }
}

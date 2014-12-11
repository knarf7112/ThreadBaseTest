using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace threadTest
{
    class Program
    {
        static void Main()
        {
            //在.Net Framework 4.0之前沒有Task Parallel Library所以無法使用Task類別
            //所以只能使用ThreadPool.QueueUserWorkItem 和 Asynchronous delegate
            //兩者不同之處為Asynchronous delegate 可從thread執行後return data,且可編組任何例外回給呼叫者
            ThreadPool.QueueUserWorkItem(Go4);//data will be null with this.
            ThreadPool.QueueUserWorkItem(Go4, "Qoo");
            Console.ReadKey();
            //ThreadPool.QueueUserWorkItem doesn’t return an object to help you subsequently manage execution. Also, 
            //you must explicitly deal with exceptions in the target code—unhandled exceptions will take down the program.

        }
        static void Go4(object data)//因為要滿足此委派=>WaitCallBack(object xxx) <= xxx參數只能是object型別
        {
            Console.WriteLine("Hello from the thread pool! " + data);
        }
        static void Main15()
        {
            // Start the task executing://Task.Factory.StartNew<string> ===可省略===> Task.Factory.StartNew
            Task<string> task = Task.Factory.StartNew<string>(
                () => DownloadString("http://www.google.com")
                );

            //這邊可以並行做其他作業
            RunSomeOtherMethod();

            //當需要委派的任務回傳資料,就去查task的Result屬性
            // If it's still executing, the current thread will now block (wait) until the task finishes
            if (task.Wait(1000))//如果需要等待完成執行的話使用Wait方法
            {
                string result = task.Result;//委派任務回傳的結果
                Console.WriteLine(result);
            }
            //任何未處理的異常都會自動的重新拋出當去查Result屬性時,透過AggregateException
            Console.ReadKey();
        }
        static string DownloadString(string url)
        {
            using (WebClient web = new WebClient())
            {
                return web.DownloadString(url);
            }
        }
        static void RunSomeOtherMethod()
        {
            //做其他事情
        }
        static void Main14()
        {
            //TPL(Task Parallel Library)任務並行函式庫
            Task.Factory.StartNew(GoThreadPool);
            //can wait for it to complete by calling its Wait method
            for (int i = 0; i < 10; i++)
            {
                Task<string> task = Task.Factory.StartNew<string>(GoAndReturnValue);//加上<string>表示回傳資料型別   
                Console.WriteLine(task.Result);
                
            }
            
            //Console.WriteLine(task.Result);
            //發現使用Task委派執行方法時,頂多先後順序不一定,但資料不會交錯,不像 Main7
            GoAndReturnValue();
            //ThreadPool是背景thread
            //使用pooled thread 不能 set Name,所以debugging困難(但可以附加一個敘述)
            //ThreadPool可自由改變執行的優先順序
            bool checkThreadPool = Thread.CurrentThread.IsThreadPoolThread;//可查詢看看此thread是否為ThreadPool的Thread
            Console.ReadKey();
        }
        static string GoAndReturnValue()
        {
            string tmp = null;
            for (int i = 0; i < 10; i++)
            {
                tmp += i.ToString();
            }
            //Console.WriteLine(tmp);
            return tmp;
        }
        static void GoThreadPool()
        {
            Console.WriteLine("Hello from the Thread Pool!");
        }
        static void Main13()
        {
            //Thread Pooling
            //因為每當start一個thread,就要花費幾百微秒(ms)去組織一個新的私有區域變數堆疊
            //每個Thread(預設)會消耗1MB左右的記憶體
            //ThreadPool用來減少這些開銷,透過分享與循環thread,使多執行緒可在更細緻的應用時不會有性能上的損失
            //There are a number of ways to enter the thread pool:
                //• Via the Task Parallel Library or PLINQ (from Framework 4.0)
                //• By calling ThreadPool.QueueUserWorkItem
                //• Via asynchronous delegates
                //• Via BackgroundWorker

            //The following constructs use the thread pool indirectly:
                //• WCF,Remoting,ASP.NET, and ASMX Web Services application servers
                //• System.Timers.Timer and System.Threading.Timer
                //• Framework methods that end in Async,
                    //such as those on WebClient(the event-based asynchronous pattern),
                    //and most BeginXXX methods (the asynchronous programming model pattern)
        }
        static void Main12()
        {
            //未處理的異常會導致程式關閉,加上一個醜陋的對話視窗
            //所以通常exception的處理是寫在丟入thread的方法內
            try
            {
                new Thread(GoEx).Start();
            }
            catch (Exception ex)
            {
                //這邊捕捉不到錯誤
                // we'll never get here!
                Console.WriteLine("Exception!");
            }
            finally
            {
                //Console.WriteLine("Finally!!!");
            }

            new Thread(GoEx2).Start();

            //The “global” exception handling events for WPF and Windows Forms applications
            //(Application.DispatcherUnhandledException and Application.ThreadException) fire only for exceptions
            //thrown on the main UI thread. You still must handle exceptions on worker threads manually.

            //AppDomain.CurrentDomain.UnhandledException fires on any unhandled
            //exception, but provides no means of preventing the
            //application from shutting down afterward.
        }
        static void GoEx2()
        {
            try { 
                throw null;// The NullReferenceException will get caught below
            }
            catch (Exception ex)
            {
                //通常記錄log,/and/or 告訴其他的thread,此thread已經GG了
                Console.WriteLine("Exception 2!");
                Console.ReadKey();
            }
        }
        static void GoEx()
        {
            //這個會丟出一個例外錯誤,但因為沒catch所以會卡住
            //throw null;// Throws a NullReferenceException
        }
        static void Main11()
        {
            //要考慮提升thread優先權會造成其他thread資源不足
            //這部分議題比較複雜在Page818
            Thread t = new Thread(Go);
            t.Priority  = ThreadPriority.Highest;//只設定thread優先權是無效的,因為還是要透過process來做執行
            t.Start();
            using (Process p = Process.GetCurrentProcess())
            {  
                p.PriorityClass = ProcessPriorityClass.High;//ProcessPriorityClass.High is actually one notch short of the highest priority: RealTime
                //儘量別設成RealTime 否則進入無窮迴圈後只有重開機的份
            }
            Console.ReadKey();
        }
        static void Main10(string[] args)
        {
            //前景thread(foreground thread)會保持應用程式活者,只要他們當中有人再運行
            //背景thread(background thread)則不會
            //但是當前景thread全都終止時,背景thread會被強制終止,即使正在運行
            //但是若是有使用finally或using的blocks去做清理作業(刪除暫存檔案或釋放資源),因為已經被強制關閉了,所以作業無法執行
            //若要避免這個,正確的應該是背景thread退出應用後再做finally要做的動作
            //下面有兩個避免的方法
            // 1. if you've created the thread yourself, call Join on the thread
            // 2. if you're on a pooled thread use an event wait handle
            //以上兩種建議設定timeout時間.Join(500)之類的
            //前景與背景thread在執行期間沒有分啥優先順序和資源分配
            Thread worker = new Thread(() =>  
                Console.ReadLine()
            );
            if (args.Length == 0) worker.IsBackground = true;//因為worker被設定為背景thread,所以當前景thread執行完後就關閉應用了,背景thread要做的ReadLine就被強制終止了
            //if an argument is passed to Main(), the worker is assigned background status, 
            //and the program exits almost immediately as the main thread ends(terminating the ReadLine).
            //if (args.Length > 0) worker.IsBackground = true;//will run readLine()
            worker.Start();
        }
        static void Main9()
        {
            //thread 想要分享資料的話 可使用一個共同的參考
            //通常使用field (欄位變數)
            Introducer intro = new Introducer();
            intro.Msg = "Hello";
            var t = new Thread(intro.Run);
            t.Start();
            t.Join();
            Console.WriteLine(intro.Reply);
            // Output:
            //Hello
            //Hi right back!
            Console.ReadKey();
        }

        class Introducer
        {
            public string Msg;
            public string Reply;
            public void Run()
            {
                Console.WriteLine(Msg);
                Reply = "hi right reply";
            }
        }
        static void Main8()
        {
            //thread和thread之間會穿插,但方法已獨立分離這些操作
            new Thread(Go3).Start(5);//call new thread
            new Thread(Go3).Start(8);
            Go3(5);// call main thread
            Console.ReadKey();
            //Each thread gets a separate copy of the cycles variable as it enters the
            //Go method and so is unable to interfere with another concurrent thread. The CLR and operating
            //system achieve this by assigning each thread its own private memory stack for local variables.
        }
        static void Go3(object maxobj)
        {
            int max = (int)maxobj;
            for (int i = 0; i < max; i++)
            {
                Console.Write(i);
            }
            Console.WriteLine();
        }
        static void Main7()
        {
            //探討thread會分享或共用資料
            //for (int i = 0; i < 10; i++)
            //{
            //    new Thread(() => Console.Write(i)).Start();//112350784
            //    //The problem is that the i variable refers to the
            //    //same memory location throughout the loop’s lifetime
            //}
            //for (int j = 0; j < 20; j++)
            //{
               Console.WriteLine();
                for (int i = 0; i < 10; i++)
                {
                    int tmp = i;//問題其實還是沒解決  還是有部分不會照順序
                    new Thread(()=>Console.Write(tmp)).Start();//0123456789
                } 
            //}
                Console.WriteLine();
                string text = "t1";
                Thread t1 = new Thread(() => Console.WriteLine(text));
                text = "t2";
                Thread t2 = new Thread(() => Console.WriteLine(text));
                t1.Start();
                t2.Start();
            Console.ReadKey();
        }

        static void Main6()
        {
            Thread t = new Thread(Print2);
            t.Start("Hello from t");//Print2方法需要帶入的參數
            //This works because Thread’s constructor is overloaded to accept either of two delegates:
                //public delegate void ThreadStart();
                //public delegate void ParameterizedThreadStart (object obj);
            //The limitation of ParameterizedThreadStart is that it accepts only one argument.
            //And because it’s of type object, it usually needs to be cast.
        }

        static void Print2(object msg)
        {
            string msg2 = (string)msg;//cast here
            Console.WriteLine(msg2);
        }

        static void Main5() 
        {
            new Thread(() =>
            {//block
                Console.WriteLine("I am running on other thread");
                Console.WriteLine("this is so easy!");
            }
            ).Start();

            //or

            new Thread(delegate()
            {
                Console.WriteLine("I am running on other thread2");
                Console.WriteLine("this is so easy2!");
            }).Start();

            Console.ReadKey();
        }

        static void Main4()
        {
            Thread t = new Thread(() => Print("Hello for t!"));
            t.Start();
            Console.ReadKey();
        }

        static void Print(string msg) { Console.WriteLine(msg); }

        static void Main3()
        {
            Thread.Sleep(TimeSpan.FromMinutes(1));//thread sleep 1 minute
            Thread.Sleep(500);// it will pauses the current thread for a specified period
            // it's will blocked

            Thread.Sleep(0);//present relinguishes(丟棄) the thread's current time slice(片) immediately
            Thread.Yield();//voluntily(自動地) handing over(移交) the CPU to the threads
        }

        static void Main2()
        {
            Thread t = new Thread(Go);
            t.Start();
            t.Join();//1);//thread blocked when set timeout and no time out
            Console.WriteLine("Thread t has ended");
            for (int i = 0; i < 1000; i++)
                Console.Write("X");
            Console.ReadKey();
        }

        static void Go()
        {
            for (int i = 0; i < 1000; i++)
                if(Thread.CurrentThread.IsAlive)//now thread is live
                Console.Write("O");
        }
        static void Main1(string[] args)
        {
            Thread tt = new Thread(WriteY);
            tt.Start();
            //Simultaneously, do something on
            for (int i = 0; i < 1000;i++ )
                Console.Write("x");
            Console.ReadKey();
        }

        static void WriteY()
        {
            for (int i = 0; i < 1000; i++)
                Console.Write("o");
        }
        
    }
}

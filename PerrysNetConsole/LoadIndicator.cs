using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public class LoadIndicator
    {

        public static String DEFAULT_MESSAGE = "Please wait...";

        public bool IsRunning { get; protected set; }
        protected volatile bool StopPending;
        protected Thread CurrentThread;
        protected String[] animation = new String[] { "-", "\\", "|", "/" };
        public ConsoleColor? ForegroundColor { get; set; }
        public ConsoleColor? BackgroundColor { get; set; }
        public String Message { get; set; }

        public LoadIndicator()
        {
            this.StopPending = false;
            this.IsRunning = false;
            this.CurrentThread = null;
            this.ForegroundColor = ConsoleColor.Black;
            this.BackgroundColor = ConsoleColor.Yellow;
            this.Message = DEFAULT_MESSAGE;
        }

        public void Start()
        {
            var message = this.Message;
            bool writtenonce = false;
            this.CurrentThread = new Thread(() =>
            {
                try
                {
                    while (!this.StopPending)
                    {
                        foreach (var frame in this.animation)
                        {
                            this.Clear();
                            CoEx.Write(" {0} ", this.BackgroundColor, this.ForegroundColor, frame);
                            CoEx.Write(" {0} ", message);
                            writtenonce = true;
                            Thread.Sleep(100);
                        }
                    }
                }
                catch { }
                finally
                {
                    if (writtenonce)
                    {
                        this.Clear();
                    }
                }
            });

            this.CurrentThread.Start();
            this.IsRunning = true;
            this.StopPending = false;
        }

        public void Stop()
        {
            if (this.CurrentThread != null && this.StopPending == false && this.IsRunning)
            {
                this.StopPending = true;
                this.CurrentThread.Join();
                this.CurrentThread = null;
                this.StopPending = false;
                this.IsRunning = false;
            }
        }

        protected void Clear()
        {
            CoEx.Seek(0, CoEx.CursorY, true);
        }

    }
}

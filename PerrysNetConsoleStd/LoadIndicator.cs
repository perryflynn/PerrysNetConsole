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

        public static string DEFAULT_MESSAGE = "Please wait...";

        public bool IsRunning { get; protected set; }
        protected volatile bool StopPending;
        protected Thread CurrentThread;
        protected LoadAnimation Animation = new LoadAnimation();
        public ColorScheme Color { get; set; }
        public string Message { get; set; }

        public LoadIndicator()
        {
            this.StopPending = false;
            this.IsRunning = false;
            this.CurrentThread = null;
            this.Color = CoEx.ColorTitlePrimary;
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
                    while (this.StopPending == false)
                    {
                        if (writtenonce)
                        {
                            CoEx.Seek(1, null);
                            CoEx.Write("{0}", this.Color, this.Animation.NextFrame());
                        }
                        else
                        {
                            this.Clear();
                            CoEx.Write(" {0} ", this.Color, this.Animation.NextFrame());
                            CoEx.Write(" {0} ", message);
                            writtenonce = true;
                        }

                        if (this.StopPending) { break; }
                        Thread.Sleep(100);
                    }

                    this.Clear();
                    CoEx.Write(" {0} ", this.Color, LoadAnimation.ANIMATIONCOMPLETE);
                    CoEx.Write(" {0} ", message);
                    Thread.Sleep(500);
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

            CoEx.CursorVisible = false;
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

            CoEx.CursorVisible = true;
        }

        protected void Clear()
        {
            CoEx.Seek(0, CoEx.CursorY, true);
        }

    }
}

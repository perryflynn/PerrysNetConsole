using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Msg = PerrysNetConsole.Message;

namespace PerrysNetConsole
{
    public class Progress : IDisposable
    {   
        // Progress bar styles

        public static string PERCFORMAT = "{0:0.0}% ";
        public static string PERCUNKNOWN = "??.??% ";
        public static int WAITBARLENGTHPERC = 20;
        public static string BARBEGIN = "│";
        public static string BAREND = "│";
        public static char BARPROGRESS = '█';
        public static char BAREMPTY = ' ';
        public static char BARPROGRESSTIP = '▌';

        protected LoadAnimation Animation = new LoadAnimation();

        /// <summary>
        /// Thread synchronization helper object
        /// </summary>
        protected Object instancelock = new Object();

        /// <summary>
        /// Has the progress bar pending changes?
        /// </summary>
        public bool IsDirty { get; protected set; }

        /// <summary>
        /// Unknown percentage, waiting animation
        /// </summary>
        protected bool iswaiting;
        public bool IsWaiting {
            get
            {
                return this.iswaiting;
            }
            set
            {
                bool old = this.iswaiting;
                this.iswaiting = value;
                if (old == true && this.iswaiting == false)
                {
                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Whitespace padding for waiting animation
        /// </summary>
        protected int WaitingPadding { get; set; }

        /// <summary>
        /// 1 or -1
        /// </summary>
        protected int WaitingIncrement { get; set; }

        /// <summary>
        /// Percentage before the last update
        /// </summary>
        protected double RecentPercentage { set; get; }

        /// <summary>
        /// Current percentage
        /// </summary>
        protected double percentage;
        public double Percentage
        {
            get
            {
                return this.percentage;
            }
            set
            {
                lock (this.instancelock)
                {
                    this.percentage = Math.Round(value, 5);
                    if (this.percentage < 0)
                    {
                        this.percentage = 0;
                    }
                    if (this.percentage > 100)
                    {
                        this.percentage = 100;
                    }

                    this.IsDirty = true;
                }
            }
        }

        /// <summary>
        /// Messages queued for drawing
        /// </summary>
        protected List<Message> MessageQueue { get; set; }

        /// <summary>
        /// Start Position
        /// </summary>
        public int StartY { get; protected set; }

        /// <summary>
        /// Uses status messages
        /// </summary>
        public bool IsUsingMessages { get; protected set; }

        /// <summary>
        /// Initialization 
        /// </summary>
        public bool IsInitialized { get; protected set; }

        /// <summary>
        /// Thread stop pending
        /// </summary>
        protected bool StopPending { get; set; }

        /// <summary>
        /// Progressbar is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.CurrentThread != null && this.CurrentThread.ThreadState == ThreadState.Running;
            }
        }

        /// <summary>
        /// The drawing thread
        /// </summary>
        protected Thread CurrentThread { set; get; }


        public Progress()
        {
            this.percentage = 0;
            this.MessageQueue = new List<Msg>();
            this.WaitingPadding = 0;
            this.WaitingIncrement = 1;
        }

        /// <summary>
        /// Store start position and launch progress bar
        /// </summary>
        public void Start()
        {
            if (this.CurrentThread != null)
            {
                throw new Exception("Already running");
            }

            CoEx.CursorVisible = false;

            this.StartY = CoEx.CursorY;
            this.IsInitialized = false;
            this.IsUsingMessages = false;
            this.StopPending = false;
            this.IsDirty = true;
            this.Percentage = 0;
            this.MessageQueue.Clear();

            this.CurrentThread = new Thread(() =>
            {
                while (true)
                {
                    this.Draw(false);
                    if (this.StopPending)
                    {
                        this.Draw(true);
                        break;
                    }
                    Thread.Sleep(100);
                }
            });

            this.CurrentThread.Start();
        }

        /// <summary>
        /// Stop background drawer thread
        /// </summary>
        public void Stop()
        {
            if (this.CurrentThread != null && this.StopPending == false)
            {
                this.StopPending = true;
                this.CurrentThread.Join();
                this.CurrentThread = null;
                this.StopPending = false;
                this.IsInitialized = false;
            }

            CoEx.CursorVisible = true;
        }

        /// <summary>
        /// Clear progress bar
        /// </summary>
        protected void Clear()
        {
            CoEx.Seek(0, this.StartY, true);
        }

        /// <summary>
        /// Draw the progress
        /// </summary>
        /// <param name="force">force drawing, ignore dirty state</param>
        protected void Draw(bool force)
        {
            double cpercentage, crecentpercentage;
            bool cisdirty, cismessages;
            List<Msg> cmsg;

            lock (this.instancelock)
            {
                cpercentage = this.Percentage;
                crecentpercentage = this.RecentPercentage;
                cisdirty = this.IsDirty;
                cismessages = this.IsUsingMessages;
                cmsg = this.MessageQueue.ToList();

                this.RecentPercentage = this.Percentage;
                this.MessageQueue.Clear();
            }

            //--> Draw bar
            if (this.IsWaiting || cisdirty || force)
            {
                if (this.IsInitialized)
                {
                    CoEx.Seek(0, null, true);
                }

                // Draw messages
                if (cmsg.Count > 0)
                {
                    if (this.IsInitialized && this.IsUsingMessages)
                    {
                        CoEx.Seek(0, -1, true);
                    }

                    foreach (var msg in cmsg)
                    {
                        this.PrintMessage(msg);
                    }

                    this.IsUsingMessages = true;
                    CoEx.WriteLine();
                }

                String percstr = this.IsWaiting ? PERCUNKNOWN : String.Format(PERCFORMAT, cpercentage);
                String loadingstr = String.Format(" {0} ", cpercentage >= 100 ? LoadAnimation.ANIMATIONCOMPLETE : this.Animation.NextFrame.ToString());
                int barmax = CoEx.Width - loadingstr.Length - 1 - percstr.Length - BARBEGIN.Length - BAREND.Length - 1;

                CoEx.Write(loadingstr, CoEx.TITLEBG, CoEx.TITLEFG);
                CoEx.Write(" " + percstr + BARBEGIN);

                if (this.IsWaiting)
                {
                    // Draw waiting animation
                    double progresslength = Math.Round(barmax * WAITBARLENGTHPERC / 100.0);
                    int whitespacelength = (int)(barmax - progresslength);

                    this.WaitingPadding += this.WaitingIncrement;
                    if (this.WaitingPadding > whitespacelength || this.WaitingPadding < 0)
                    {
                        this.WaitingIncrement = this.WaitingIncrement > 0 ? -1 : 1;
                        this.WaitingPadding += this.WaitingIncrement;
                    }

                    String beforeprogress = "".PadLeft(this.WaitingPadding, BAREMPTY);
                    String progress = "".PadLeft((int)progresslength, BARPROGRESS);
                    String afterprogress = "".PadLeft(whitespacelength - this.WaitingPadding, BAREMPTY);

                    CoEx.Write(beforeprogress);
                    CoEx.Write(progress, CoEx.TITLEFGSEC, CoEx.TITLEBGSEC);
                    CoEx.Write(afterprogress);
                }
                else
                {
                    // Draw progress bar
                    int fill = (int)Math.Round(barmax * crecentpercentage / 100.0);
                    int newfill = (int)Math.Round(barmax * (cpercentage - crecentpercentage) / 100.0);

                    String progress = "".PadLeft(fill, BARPROGRESS);
                    String newprogress = "".PadLeft(newfill, BARPROGRESS);
                    String empty = "".PadLeft(barmax - (fill + newfill), BAREMPTY);

                    CoEx.Write(progress, CoEx.TITLEFGSEC, CoEx.TITLEBGSEC);
                    CoEx.Write(newprogress, CoEx.TITLEFG, CoEx.TITLEBG);
                    CoEx.Write(empty);
                }

                CoEx.Write(BAREND);
                this.IsInitialized = true;
                
                // Set dirty status to false
                if (this.IsDirty)
                {
                    lock (this.instancelock)
                    {
                        this.IsDirty = false;
                    }
                }
            }
            //--> No action nessasary, only draw load indicator
            else if (cpercentage < 100)
            {
                String loadingstr = String.Format(" {0} ", cpercentage >= 100 ? LoadAnimation.ANIMATIONCOMPLETE : this.Animation.NextFrame.ToString());
                CoEx.Seek(0, null);
                CoEx.Write(loadingstr, CoEx.TITLEBG, CoEx.TITLEFG);
            }
        }

        protected void PrintMessage(Msg msg)
        {
            String level = "";
            ConsoleColor color = ConsoleColor.Gray;
            if (msg.Level == Msg.LEVEL.DEBUG)
            {
                color = ConsoleColor.Cyan;
                level = "DBG";
            }
            else if (msg.Level == Msg.LEVEL.INFO)
            {
                color = ConsoleColor.Blue;
                level = "INF";
            }
            else if (msg.Level == Msg.LEVEL.WARN)
            {
                color = ConsoleColor.Yellow;
                level = "WRN";
            }
            else if (msg.Level == Msg.LEVEL.ERROR)
            {
                color = ConsoleColor.Red;
                level = "ERR";
            }
            else if (msg.Level == Msg.LEVEL.SUCCESS)
            {
                color = ConsoleColor.Green;
                level = "SUC";
            }

            CoEx.SetColor(null, color);
            CoEx.Write("[{0}] ", level);
            CoEx.ResetColor();

            CoEx.WriteLine(msg.ToString());
        }

        public void Message(Msg.LEVEL? lvl, String msg)
        {
            if (!String.IsNullOrEmpty(msg))
            {
                lock (this.instancelock)
                {
                    this.MessageQueue.Add(new Msg() { Level = lvl ?? Msg.LEVEL.INFO, Text = msg });
                    this.IsDirty = true;
                }
            }
        }

        public void Message(String msg)
        {
            this.Message(Msg.LEVEL.INFO, msg);
        }

        public void Update(double perc)
        {
            this.Percentage = perc;
        }

        public void Update(long current, long max)
        {
            this.Percentage = 100.0 * (0.0 + current) / (0.0 + max);
        }
        
        public void Update(long current, long max, String msg)
        {
            this.Update(current, max, Msg.LEVEL.INFO, msg);
        }

        public void Update(long current, long max, Msg.LEVEL lvl, String msg)
        {
            if(!String.IsNullOrEmpty(msg))
            {
                this.Message(lvl, msg);
            }
            this.Update(current, max);
        }

        public void Dispose()
        {
            this.Stop();
            this.Clear();
        }

    }
}

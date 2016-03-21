using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public class Progress
    {
        public enum LEVEL { DEBUG, INFO, WARN, ERROR, SUCCESS };

        public static string PERCFORMAT = "{0:0.0}%";
        public static string BARBEGIN = " [";
        public static string BAREND = "]";
        public static char BARPROGRESS = '#';
        public static char BAREMPTY = ' ';
        public static char BARPROGRESSTIP = '>';

        protected double percentage;
        public double Percentage
        {
            get
            {
                return this.percentage;
            }
            set
            {
                this.percentage = value;
                if(this.percentage<0)
                {
                    this.percentage = 0;
                }
                if(this.percentage>100)
                {
                    this.percentage = 100;
                }
                this.Draw();
            }
        }

        public bool IsUsingMessages { get; protected set; }
        public bool IsInitialized { get; protected set; }
        public bool IsRunning { get; protected set; }

        public Progress()
        {
            this.Percentage = 0;
        }

        protected void Draw()
        {
            String percstr = String.Format(PERCFORMAT, this.Percentage);
            
            int barmax = CoEx.Width - percstr.Length - BARBEGIN.Length - BAREND.Length-1;
            int fill = (int)Math.Round((barmax * this.Percentage / 100));

            String progress = ("".PadLeft(fill, BARPROGRESS));
            
            if (this.Percentage > 0 && this.Percentage < 100 && progress.Length > 1)
            {
                progress = progress.Substring(0, progress.Length - 1) + BARPROGRESSTIP;
            }

            String barcontent = progress + ("".PadLeft(barmax - fill, BAREMPTY));
            
            if (this.IsInitialized)
            {
                CoEx.Seek(0, (this.IsUsingMessages ? -2 : -1));
            }
            
            if (this.IsUsingMessages)
            {
                CoEx.WriteLine();
            }
            
            this.IsInitialized = true;
            CoEx.WriteLine(percstr + BARBEGIN + barcontent + BAREND);
        }

        public void Start()
        {
            this.IsRunning = true;
            this.Draw();
        }

        public void Message(LEVEL lvl, String msg)
        {
            if (this.IsInitialized)
            {
                CoEx.Seek(0, (this.IsUsingMessages ? -2 : -1));
                CoEx.WriteLine("".PadLeft(CoEx.Width, ' '));
                if (this.IsUsingMessages)
                {
                    CoEx.WriteLine("".PadLeft(CoEx.Width, ' '));
                }
                CoEx.Seek(0, (this.IsUsingMessages ? -2 : -1));
            }

            this.IsUsingMessages = true;

            String level = "";
            ConsoleColor color = ConsoleColor.Gray;
            if(lvl==LEVEL.DEBUG)
            {
                color = ConsoleColor.Cyan;
                level = "DBG";
            }
            else if(lvl==LEVEL.INFO)
            {
                color = ConsoleColor.Blue;
                level = "INF";
            }
            else if(lvl==LEVEL.WARN)
            {
                color = ConsoleColor.Yellow;
                level = "WRN";
            }
            else if(lvl==LEVEL.ERROR)
            {
                color = ConsoleColor.Red;
                level = "ERR";
            }
            else if(lvl==LEVEL.SUCCESS)
            {
                color = ConsoleColor.Green;
                level = "SUC";
            }

            CoEx.SetColor(null, color);
            CoEx.Write("[{0}] ", level);
            CoEx.ResetColor();

            CoEx.WriteLine("({0}) {1}", DateTime.Now.ToString("HH:mm:ss"), msg);
            this.IsInitialized = false;
        }

        public void Message(String msg)
        {
            this.Message(LEVEL.INFO, msg);
        }

        public void Update(double perc)
        {
            this.Percentage = perc;
        }

        public void Update(int current, int max)
        {
            this.Percentage = 100 * (0.0 + current) / (0.0 + max);
        }

        public void Update(int current, int max, String msg)
        {
            this.Update(current, max, LEVEL.INFO, msg);
        }

        public void Update(int current, int max, LEVEL lvl, String msg)
        {
            if(!String.IsNullOrEmpty(msg))
            {
                this.Message(lvl, msg);
            }
            this.Update(current, max);
        }

    }
}

using PerrysNetConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {

        /**
         * Example Data
         */
        protected static string[] ScrollText = new string[] {
            ":  Episode IV  ",
            ":  A NEW HOPE  ",
            "",
            "It is a period of civil war.",
            "Rebel spaceships, striking",
            "from a hidden base, have won",
            "their first victory against",
            "the evil Galactic Empire.",
            "",
            "During the battle, Rebel",
            "spies managed to steal secret",
            "plans to the Empire's",
            "ultimate weapon, the DEATH",
            "station with enough power",
            "to destroy an entire planet.",
            "",
            "Pursued by the Empire's",
            "sinister agents, Princess",
            "Leia races home aboard her",
            "starship, custodian of the",
            "stolen plans that can save her",
            "people and restore",
            "freedom to the galaxy...."
        };

        protected static string[] headerdata = new string[]
        {
            "Name",
            "Project",
            "Hours",
            "Comment",
        };

        protected static string[][] exambledata = new string[][]
        {
            new string[] { "Unittest", "Project A", "12", "Success" },
            new string[] { "Bugfix", "Project XY", "1", "" },
            new string[] { "Feature", "Shopsystem", "32", "Next: Unittesting" }
        };

        protected static string[][] exambledatalong = new string[][]
        {
            new string[] { "Rollout new Database Schema", "Shop 2.0", "12", "Need more testing, it was very complicated to migrate the records into the new schema." },
            new string[] { "Unittesting", "DBLib", "2", "Success" },
            new string[] { "CVS to GIT", "Shop 2.0", "4", "" },
            new string[] { "", "", "", "" }
        };

        protected static RowConf header;
        protected static RowCollection rc1;
        protected static RowCollection rc2;


        protected static void DemoLoadIndicator()
        {
            CoEx.WriteTitleLarge("Loading indicator");
            CoEx.WriteLine();

            LoadIndicator indicator = new LoadIndicator() { Message = "Load the awesomeness" };
            indicator.Start();
            System.Threading.Thread.Sleep(5000);
            indicator.Stop();
        }


        protected static void DemoProgressBar()
        {
            CoEx.WriteTitleLarge("Simple Progressbar");
            CoEx.WriteLine();

            // using() { } is optional, see Start(), Stop() and Clear()
            using (Progress simpr = new Progress())
            {
                simpr.Start();

                do
                {
                    simpr.Percentage += 0.20;
                    System.Threading.Thread.Sleep(5);
                }
                while (simpr.Percentage < 100);

                System.Threading.Thread.Sleep(1000);
            }
        }


        protected static void DemoTimeout()
        {
            CoEx.WriteTitleLarge("Basic timeout");
            CoEx.WriteLine();

            CoEx.Timeout(3);
        }


        protected static void DemoScrolltext()
        {
            var width = CoEx.WindowWidth;
            var height = CoEx.WindowHeight;
            var bheight = CoEx.BufferHeight;

            CoEx.WindowHeight = 20;
            CoEx.BufferWidth = CoEx.WindowWidth = 40;
            CoEx.BufferHeight = ScrollText.Length + (2 * CoEx.WindowHeight);

            ulong pos = CoEx.RealCursorY;
            for (int i = 0; i < CoEx.WindowHeight; i++) { CoEx.WriteLine(); }

            foreach (var line in ScrollText)
            {
                var temp = line;
                var istitle = temp.StartsWith(":");
                if (istitle) { temp = temp.Substring(1); }
                var row = RowConf.Create(new string[] { temp }).SetAlignment(RowConf.ALIGNCENTER);
                if (istitle) { row = row.PresetTitle(); }
                CoEx.WriteColumns(row);
            }

            int length = (int)(CoEx.RealCursorY - pos);

            for (int i = 0; i < CoEx.WindowHeight; i++) { CoEx.WriteLine(); }

            CoEx.Scroll(0, (int)(pos - CoEx.RealCursorY));

            for (int i = 0; i < length; i++)
            {
                CoEx.BufferViewportY++;
                System.Threading.Thread.Sleep(600);
            }

            System.Threading.Thread.Sleep(2000);

            CoEx.WindowWidth = CoEx.BufferWidth = width;
            CoEx.WindowHeight = height;
            CoEx.BufferHeight = bheight;
        }


        protected static void DemoProgressBarMessages()
        {
            Tuple<Message.LEVEL, string>[] states = new Tuple<Message.LEVEL, string>[]
            {
                new Tuple<Message.LEVEL,string>(Message.LEVEL.INFO, "Initialize"),
                new Tuple<Message.LEVEL,string>(Message.LEVEL.INFO, "This is a message on INFO level"),
                new Tuple<Message.LEVEL,string>(Message.LEVEL.DEBUG, "Debugging message"),
                new Tuple<Message.LEVEL,string>(Message.LEVEL.WARN, "Serious warning, be careful!"),
                new Tuple<Message.LEVEL,string>(Message.LEVEL.ERROR, "Fatal error, its too late to be careful..."),
                new Tuple<Message.LEVEL,string>(Message.LEVEL.SUCCESS, "Finally done with this demo!")
            };

            CoEx.WriteTitleLarge("Progress bar with status messages");
            CoEx.WriteLine();

            // Example without using() { }
            Progress prog = new Progress();
            prog.Start();

            int perc = 0;
            for (int i = 0; i < states.Length; i++)
            {
                // Post message
                prog.Message(states[i].Item1, states[i].Item2);

                // Increase by 20 percent in small steps
                if (states[i].Item1 == Message.LEVEL.DEBUG)
                {
                    prog.Message(Message.LEVEL.DEBUG, "Ooops, dont know how long this will take, please wait...");
                    prog.IsWaiting = true;
                    System.Threading.Thread.Sleep(20000);
                    prog.IsWaiting = false;
                }
                else
                {
                    for (int j = 0; j < 100 / states.Length; j++)
                    {
                        perc++;
                        prog.Update(perc);
                        System.Threading.Thread.Sleep(20);
                    }
                }

                // wait 2 seconds
                System.Threading.Thread.Sleep(2000);
            }

            prog.Update(100);
            prog.Stop();

            System.Threading.Thread.Sleep(2000);
        }


        protected static void DemoPrompt()
        {
            Prompt prompt = new Prompt()
            {
                AllowEmpty = false,
                Prefix = "Choose your choice",
                Default = "okay",
                ValidateChoices = true,
                ChoicesText = new Dictionary<string, string>() {
                    { "yes", "Yea, lets go" },
                    { "sure", "Sure, why not" },
                    { "okay", "Okay, go for it" },
                    { "yay", "Yay, kay, thx, bye" }
                }
            };

            CoEx.WriteTitleLarge("Welcome, lets get started with a basic prompt");
            CoEx.WriteLine();

            prompt.DoPrompt();
        }


        protected static void DemoIntro()
        {
            CoEx.WriteTitleLarge("Welcome to PerrysNetConsole Demo Application");
            CoEx.WriteTitleLarge("Code & Concept by Christian Blechert");
            CoEx.WriteLine();

            CoEx.WriteLine("This is the introduction for a highlighted text");
            CoEx.WriteHl("Highlighted text");

            CoEx.WriteLine();
            CoEx.WriteLine("And here comes text highlighting in multiple columns");

            CoEx.WriteLine();
            CoEx.WriteHl("Highlighted text", "in multiple columns");

            CoEx.WriteLine();
            CoEx.WriteHl("0", "1", "2", "3", "4", "5", "6", "7", "8", "9");
            CoEx.WriteHl("10", "11", "12", "13", "14", "15", "16", "17", "18", "19");

            CoEx.WriteLine();
            CoEx.WriteHl("And what happens when the text is too long?", "The really really really really really really really really really really long text will be wrapped. Is this quite cool? Yes, it is. :-)");

            CoEx.WriteLine();
            CoEx.WriteLine();
            CoEx.WriteLine("Oh and normal text of couse, nevermind, go ahead!");
        }


        protected static void DemoBasicColumns()
        {
            CoEx.WriteTitleLarge("Basic column system, no custom column length, no custom formatting");
            CoEx.WriteLine();

            // loop both exampledata arrays
            foreach (var data in new string[][][] { exambledata, exambledatalong })
            {
                // print single row from one of the exampledata arrays
                foreach (var row in data)
                {
                    CoEx.WriteColumns(row); // print the row
                }
            }
        }


        protected static void DemoBasicTable()
        {
            CoEx.WriteTitleLarge("Tables - Chapter 1");
            CoEx.WriteLine();
            CoEx.WriteTitle("The basic table");
            rc1.Settings.StretchHorizontal = false;
            CoEx.WriteTable(rc1);
            rc1.Settings.StretchHorizontal = true;

            CoEx.WriteLine();
            CoEx.WriteTitle("The basic table with header, stretched to window width");
            rc1.Import(0, header);
            CoEx.WriteTable(rc1);

            CoEx.WriteLine();
            CoEx.WriteTitle("The basic table, border disabled");
            rc1.Settings.Border.Enabled = false; // override default
            CoEx.WriteTable(rc1);
        }


        protected static void BasicTableLongText()
        {
            CoEx.WriteTitleLarge("Tables - Chapter 2");
            CoEx.WriteLine();
            CoEx.WriteTitle("The basic table with long text");
            rc2.Import(0, header);
            CoEx.WriteTable(rc2);
        }


        protected static void DemoTableSynchronized()
        {
            var length = RowCollection.Create().Import(rc1).Import(rc2).Length;
            rc1.Length = length;
            rc1.Settings.Border.Enabled = true; // enable border again
            rc1.Settings.Border.HorizontalLineBody = BorderConf.HorizontalLineAfterHeaderFunc;
            rc2.Length = length;
            rc2.Settings.Border.Enabled = true;

            CoEx.WriteTitleLarge("Tables - Chapter 3");
            CoEx.WriteLine();
            CoEx.WriteTitle("Synchonize column length over multiple tables");
            CoEx.WriteLine();
            CoEx.WriteTitle("The basic table, border header-only");
            CoEx.WriteTable(rc1);
            CoEx.WriteLine();
            CoEx.WriteTitle("The basic table with long text");
            CoEx.WriteTable(rc2);
        }


        protected static void DemoConditionalStyles()
        {
            CoEx.WriteTitleLarge("Tables - Chapter 4");
            CoEx.WriteLine();
            CoEx.WriteTitle("Colorize cells by conditions");

            // colorize column by index
            rc1.Settings.Color = delegate (RowConf me, int colindex, string s)
            {
                var temp = new ColorScheme(null, ConsoleColor.Blue);
                switch (colindex)
                {
                    case 0: temp.Background = ConsoleColor.Cyan; break;
                    case 1: temp.Background = ConsoleColor.White; break;
                    case 2: temp.Background = ConsoleColor.Magenta; break;
                    case 3: temp.Background = ConsoleColor.Red; break;
                    default: temp.Background = ConsoleColor.Gray; break;
                }
                return temp;
            };

            // Highlight padding every second column
            rc1.Settings.IsHighlightPadding = delegate (RowConf me, int colindex, string s)
            {
                return (colindex % 2) == 0;
            };

            CoEx.WriteLine();
            CoEx.WriteTitle("Background color based on column index");
            CoEx.WriteTable(rc1);

            // if hours>10 is column 2 yellow and column 0 red
            rc2.Settings.Color = delegate (RowConf me, int colindex, string s)
            {
                double hours;
                bool success = double.TryParse(me.Data[2], out hours);

                if (colindex == 2 && success && hours >= 10)
                {
                    return new ColorScheme(ConsoleColor.Yellow, ConsoleColor.Blue);
                }
                else if (colindex == 0 && success && hours >= 10)
                {
                    return new ColorScheme(ConsoleColor.Red, null);
                }
                else
                {
                    return null;
                }
            };

            // highlight padding in column 0
            rc2.Settings.IsHighlightPadding = delegate (RowConf me, int colindex, string s)
            {
                return (colindex == 0);
            };

            CoEx.WriteLine();
            CoEx.WriteTitle("Background color based on hours value");
            CoEx.WriteTable(rc2);
        }


        protected static void DemoAlignment()
        {
            rc2.Settings.Align = delegate (RowConf me, int colindex, string s)
            {
                switch (colindex)
                {
                    case 0: return RowCollectionSettings.ALIGN.RIGHT;
                    case 1: return null;
                    case 2: return RowCollectionSettings.ALIGN.RIGHT;
                    case 3: return RowCollectionSettings.ALIGN.CENTER;
                    default: return RowCollectionSettings.ALIGN.LEFT;
                }
            };

            rc2.Settings.Border.HorizontalLineBody = delegate (RowConf r) { return true; };

            CoEx.WriteTitleLarge("Tables - Chapter 5");
            CoEx.WriteLine();
            CoEx.WriteTitle("Column alignment");
            CoEx.WriteTable(rc2);
        }


        public static void DemoGraph()
        {
            CoEx.WriteTitleLarge("Simple Graph");
            CoEx.WriteLine();

            var graph = new SimpleGraph()
            {
                Height = 10
            };

            var list = new List<double>() { 2, 1, 8, 16, 42, 0, 50, 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 0, 10, 20, 30, 40, 50, 40, 30, 20, 10 };

            var i = 0;
            var temp = DateTime.Now.Date;
            var dateList = list.ToDictionary(key => temp.AddDays(i++).ToString("ddMMM"), value => value);

            graph.Draw(dateList /*.Select(v => v * 42).ToList()*/ /*.OrderBy(v => v).ToList()*/);
        }


        static void Main(string[] args)
        {
            /**
             * Console defaults
             */
            CoEx.WindowHeight = CoEx.WindowHeightMax - 10;
            CoEx.BufferHeight = 256;


            /**
             * Table defaults
             */
            RowCollection.DefaultSettings.Border.Enabled = true; // enable bordering
            RowCollection.DefaultSettings.Border.HorizontalLineBody = BorderConf.HorizontalLineAlwaysOnFunc; // line between the rows
            
            header = RowConf.Create(headerdata).PresetTH();
            rc1 = RowCollection.Create(exambledata);
            rc2 = RowCollection.Create(exambledatalong);


            /**
             * Demo parts
             */
            Continue();

            DemoLoadIndicator();
            CoEx.Clear();

            DemoGraph();
            Continue();
            CoEx.Clear();

            DemoProgressBar();
            CoEx.Clear();

            DemoTimeout();

            DemoScrolltext();
            CoEx.Clear();

            DemoProgressBarMessages();
            CoEx.Clear();

            DemoPrompt();
            CoEx.Clear();
            
            DemoIntro();
            Continue();
            
            DemoBasicColumns();
            Continue();

            DemoBasicTable();
            Continue();

            BasicTableLongText();
            Continue();
            
            DemoTableSynchronized();
            Continue();

            DemoConditionalStyles();
            Continue();

            DemoAlignment();


            /**
             * The End
             */
            CoEx.WriteLine();
            CoEx.WriteHl("Program finished.");
            CoEx.Confirm("Exit?");
        }


        /// <summary>
        /// The "Continue (Enter)" dialog
        /// </summary>
        private static void Continue()
        {
            CoEx.WriteLine();
            CoEx.PressAnyKey();
            CoEx.Clear();
        }


    }
}

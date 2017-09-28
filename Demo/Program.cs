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
        protected static String[] headerdata = new String[]
        {
            "Name",
            "Project",
            "Hours",
            "Comment",
        };

        protected static String[][] exambledata = new String[][]
        {
            new String[] { "Unittest", "Project A", "12", "Success" },
            new String[] { "Bugfix", "Project XY", "1", "" },
            new String[] { "Feature", "Shopsystem", "32", "Next: Unittesting" }
        };

        protected static String[][] exambledatalong = new String[][]
        {
            new String[] { "Rollout new Database Schema", "Shop 2.0", "12", "Need more testing, it was very complicated to migrate the records into the new schema." },
            new String[] { "Unittesting", "DBLib", "2", "Success" },
            new String[] { "CVS to GIT", "Shop 2.0", "4", "" },
            new String[] { "", "", "", "" }
        };

        protected static RowConf header = RowConf.Create(headerdata).PresetTH();
        protected static RowCollection rc1 = RowCollection.Create(exambledata);
        protected static RowCollection rc2 = RowCollection.Create(exambledatalong);


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
                    simpr.Percentage += 0.1;
                    System.Threading.Thread.Sleep(5);
                }
                while (simpr.Percentage < 100);

                System.Threading.Thread.Sleep(2000);
            }

            CoEx.WriteLine("Progress done!");

            System.Threading.Thread.Sleep(1000);
        }


        protected static void DemoProgressBarMessages()
        {
            Tuple<Message.LEVEL, String>[] states = new Tuple<Message.LEVEL, String>[]
            {
                new Tuple<Message.LEVEL,String>(Message.LEVEL.INFO, "Initialize"),
                new Tuple<Message.LEVEL,String>(Message.LEVEL.INFO, "This is a message on INFO level"),
                new Tuple<Message.LEVEL,String>(Message.LEVEL.DEBUG, "Debugging message"),
                new Tuple<Message.LEVEL,String>(Message.LEVEL.WARN, "Serious warning, be careful!"),
                new Tuple<Message.LEVEL,String>(Message.LEVEL.ERROR, "Fatal error, its too late to be careful..."),
                new Tuple<Message.LEVEL,String>(Message.LEVEL.SUCCESS, "Finally done with this demo!")
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
                for (int j = 0; j < 100 / states.Length; j++)
                {
                    perc++;
                    prog.Update(perc);
                    System.Threading.Thread.Sleep(20);
                }

                // wait 5 seconds
                System.Threading.Thread.Sleep(2000);
            }

            prog.Update(100);
            prog.Stop();

            CoEx.WriteLine();
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
            foreach (var data in new String[][][] { exambledata, exambledatalong })
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
            CoEx.WriteTable(rc1);

            CoEx.WriteLine();
            CoEx.WriteTitle("The basic table with header");
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
            CoEx.WriteTable(rc2);
        }


        protected static void DemoTableSynchronized()
        {
            var length = RowCollection.Create().Import(rc1).Import(rc2).Length;
            rc1.Length = length;
            rc1.Settings.Border.Enabled = true; // enable border again
            rc1.Settings.Border.HorizontalLineBody = BorderConf.HorizontalLineAfterHeaderFunc;
            rc2.Length = length;

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
            rc1.Settings.BackgroundColor = delegate (RowConf me, int colindex, String s)
            {
                switch (colindex)
                {
                    case 0: return ConsoleColor.Cyan;
                    case 1: return ConsoleColor.White;
                    case 2: return ConsoleColor.Magenta;
                    case 3: return ConsoleColor.Red;
                    default: return ConsoleColor.Gray;
                }
            };

            // Text is blue
            rc1.Settings.ForegroundColor = delegate (RowConf me, int colindex, String s)
            {
                return ConsoleColor.Blue;
            };

            // Highlight padding every second column
            rc1.Settings.IsHighlightPadding = delegate (RowConf me, int colindex, String s)
            {
                return (colindex % 2) == 0;
            };

            CoEx.WriteLine();
            CoEx.WriteTitle("Background color based on column index");
            CoEx.WriteTable(rc1);

            // if hours>10 is column 2 yellow and column 0 red
            rc2.Settings.BackgroundColor = delegate (RowConf me, int colindex, String s)
            {
                double hours;
                bool success = Double.TryParse(me.Data[2], out hours);

                if (colindex == 2 && success && hours >= 10)
                {
                    return ConsoleColor.Yellow;
                }
                else if (colindex == 0 && success && hours >= 10)
                {
                    return ConsoleColor.Red;
                }
                else
                {
                    return null;
                }
            };

            // if background yellow, make foreground blue
            rc2.Settings.ForegroundColor = delegate (RowConf me, int colindex, String s)
            {
                if (rc2.Settings.BackgroundColor(me, colindex, s) == ConsoleColor.Yellow)
                {
                    return ConsoleColor.Blue;
                }
                else
                {
                    return null;
                }
            };

            // highlight padding in column 0
            rc2.Settings.IsHighlightPadding = delegate (RowConf me, int colindex, String s)
            {
                return (colindex == 0);
            };

            CoEx.WriteLine();
            CoEx.WriteTitle("Background color based on hours value");
            CoEx.WriteTable(rc2);
        }


        protected static void DemoAlignment()
        {
            rc2.Settings.Align = delegate (RowConf me, int colindex, String s)
            {
                switch (colindex)
                {
                    case 0: return RowCollectionSettings.ALIGN.RIGHT;
                    case 1: return null;
                    case 2: case 3: return RowCollectionSettings.ALIGN.CENTER;
                    default: return RowCollectionSettings.ALIGN.LEFT;
                }
            };

            CoEx.WriteTitleLarge("Tables - Chapter 5");
            CoEx.WriteLine();
            CoEx.WriteTitle("Column alignment");
            CoEx.WriteTable(rc2);
        }


        static void Main(string[] args)
        {
            /**
             * Console defaults
             */
            Console.WindowHeight = Console.LargestWindowHeight - 10;

            /**
             * Table defaults
             */
            RowCollection.DefaultSettings.Border.Enabled = true; // enable bordering
            RowCollection.DefaultSettings.Border.HorizontalLineBody = BorderConf.HorizontalLineAlwaysOnFunc; // line between the rows
            rc1.Import(0, header);
            rc2.Import(0, header);

            /**
             * Demo parts
             */

            /*
            DemoPrompt();
            CoEx.Clear();


            DemoLoadIndicator();
            CoEx.Clear();
            */

            DemoProgressBar();
            CoEx.Clear();

            DemoProgressBarMessages();
            Continue();


            /*
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
            */

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

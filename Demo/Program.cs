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
        static void Main(string[] args)
        {

            Console.WindowHeight = Console.LargestWindowHeight - 10;


            /**
             * Example Data
             */
            String[] headerdata = new String[]
            {
                "Name",
                "Project",
                "Hours",
                "Comment",
            };

            String[][] exambledata = new String[][]
            {
                new String[] { "Unittest", "Project A", "12", "Success" },
                new String[] { "Bugfix", "Project XY", "1", "" },
                new String[] { "Feature", "Shopsystem", "32", "Next: Unittesting" }
            };

            String[][] exambledatalong = new String[][]
            {
                new String[] { "Rollout new Database Schema", "Shop 2.0", "12", "Need more testing, it was very complicated to migrate the records into the new schema." },
                new String[] { "Unittesting", "DBLib", "2", "Success" },
                new String[] { "CVS to GIT", "Shop 2.0", "4", "" },
                new String[] { "", "", "", "" }
            };


            /**
             * Prompt
             */
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

            CoEx.Clear();


            /**
             * Load indicator
             */
            CoEx.WriteTitleLarge("Loading indicator");
            CoEx.WriteLine();

            LoadIndicator indicator = new LoadIndicator() { Message = "Load the awesomeness" };
            indicator.Start();
            System.Threading.Thread.Sleep(5000);
            indicator.Stop();

            CoEx.Clear();


            /**
             * Simple Progress Bar
             */
            CoEx.WriteTitleLarge("Simple Progressbar");
            CoEx.WriteLine();
            
            Progress simpr = new Progress();
            simpr.Start();

            while (simpr.Percentage < 100)
            {
                simpr.Percentage += 0.5;
                System.Threading.Thread.Sleep(15);
            }

            System.Threading.Thread.Sleep(1000);
            CoEx.Clear();


            /**
             * Progress Bar with Log
             */
            Tuple<Progress.LEVEL, String>[] states = new Tuple<Progress.LEVEL, String>[]
            {
                new Tuple<Progress.LEVEL,String>(Progress.LEVEL.INFO, "Initialize"),
                new Tuple<Progress.LEVEL,String>(Progress.LEVEL.INFO, "Contact the spy server"),
                new Tuple<Progress.LEVEL,String>(Progress.LEVEL.WARN, "Report launch of this application to NSA"),
                new Tuple<Progress.LEVEL,String>(Progress.LEVEL.ERROR, "Could not contact NSA!"),
                new Tuple<Progress.LEVEL,String>(Progress.LEVEL.SUCCESS, "Welcome! :-)")
            };
            
            CoEx.WriteTitleLarge("Progress bar with status messages");
            CoEx.WriteLine();
            
            Progress prog = new Progress();
            prog.Start();

            for (int i = 0; i < states.Length; i++)
            {
                prog.Update(i + 1, states.Length, states[i].Item1, states[i].Item2);
                System.Threading.Thread.Sleep(1000);
            }

            Continue();


            /**
             * Intro
             */
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
            
            Continue();


            /**
             * Basic columns
             */
            CoEx.WriteTitleLarge("Basic column system, no custom column length, no custom formatting");
            CoEx.WriteLine();

            // loop both exampledata arrays
            foreach (var data in new String[][][] { exambledata, exambledatalong})
            {
                // print single row from one of the exampledata arrays
                foreach(var row in data)
                {
                    CoEx.WriteColumns(row); // print the row
                }
            }

            Continue();


            /**
             * Table Defaults
             */
            RowCollection.DefaultSettings.Border.Enabled = true; // enable bordering
            RowCollection.DefaultSettings.Border.HorizontalLineBody = BorderConf.HorizontalLineAlwaysOnFunc; // line between the rows


            /**
             * Create header
             */
            var header = RowConf.Create(headerdata).PresetTH();


            /**
             * Basic table
             */
            var rc1 = RowCollection.Create(exambledata);
            CoEx.WriteTitleLarge("Tables - Chapter 1");
            CoEx.WriteLine();
            CoEx.WriteTitle("The basic table");
            CoEx.WriteTable(rc1);
            
            CoEx.WriteLine();
            CoEx.WriteTitle("The basic table with header");
            rc1.Import(0, header);
            CoEx.WriteTable(rc1);

            CoEx.WriteLine();
            CoEx.WriteTitle("The basic table, border disabled");
            rc1.Settings.Border.Enabled = false; // override default
            CoEx.WriteTable(rc1);

            Continue();


            /**
             * Basic table with long text
             */
            CoEx.WriteTitleLarge("Tables - Chapter 2");
            CoEx.WriteLine();
            CoEx.WriteTitle("The basic table with long text");
            var rc2 = RowCollection.Create(exambledatalong);
            rc2.Import(0, header);
            CoEx.WriteTable(rc2);

            Continue();


            /**
             * Synchonize length over multiple tables
             */
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

            Continue();


            /**
             * Conditional coloring
             */
            CoEx.WriteTitleLarge("Tables - Chapter 4");
            CoEx.WriteLine();
            CoEx.WriteTitle("Colorize cells by conditions");

            // colorize column by index
            rc1.Settings.BackgroundColor = delegate(RowConf me, int colindex, String s) 
            {
                switch(colindex)
                {
                    case 0: return ConsoleColor.Cyan;
                    case 1: return ConsoleColor.White;
                    case 2: return ConsoleColor.Magenta;
                    case 3: return ConsoleColor.Red;
                    default: return ConsoleColor.Gray;
                }
            };

            // Text is blue
            rc1.Settings.ForegroundColor = delegate(RowConf me, int colindex, String s) 
            { 
                return ConsoleColor.Blue; 
            };

            // Highlight padding every second column
            rc1.Settings.IsHighlightPadding = delegate(RowConf me, int colindex, String s)
            {
                return (colindex % 2) == 0;
            };

            CoEx.WriteLine();
            CoEx.WriteTitle("Background color based on column index");
            CoEx.WriteTable(rc1);

            // if hours>10 is column 2 yellow and column 0 red
            rc2.Settings.BackgroundColor = delegate(RowConf me, int colindex, String s)
            {
                double hours;
                bool success = Double.TryParse(me.Data[2], out hours);
                
                if(colindex==2 && success && hours>=10)
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
            rc2.Settings.ForegroundColor = delegate(RowConf me, int colindex, String s)
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
            rc2.Settings.IsHighlightPadding = delegate(RowConf me, int colindex, String s)
            {
                return (colindex == 0);
            };
            
            CoEx.WriteLine();
            CoEx.WriteTitle("Background color based on hours value");
            CoEx.WriteTable(rc2);

            Continue();


            /**
             * Column alignment
             */
            rc2.Settings.Align = delegate(RowConf me, int colindex, String s)
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

using System;
using System.Linq;
using System.Text;
using System.Threading;

/**
    Unsupported functions by unix systems
    https://github.com/dotnet/corefx/tree/master/src/System.Console/src/System

    NumberLock
    CapsLock
    CursorSize.set
    Title.get
    Beep
    BufferWidth.set
    BufferHeight.set
    SetBufferSize
    WindowLeft.set
    WindowTop.set
    WindowWidth.set
    WindowHeight.set
    SetWindowPosition
    SetWindowSize
    CursorVisible.get
    MoveBufferArea
    MoveBufferArea

    ConsoleColor UnknownColor = (ConsoleColor)(-1);
*/

namespace PerrysNetConsole
{

    public delegate void ConsoleWriteEventHandler(string text);
    public delegate void ConsoleChangeColorEventHandler(ColorScheme scheme);

    public static class CoEx
    {

        //--> Defaults

        /// <summary>
        /// Default text for PressAnyKey()
        /// </summary>
        public static string PressAnyKeyDefaultText { get; set; } = "Press any key... ";

        /// <summary>
        /// Default text for Timeout()
        /// </summary>
        public static string TimeoutDefaultText { get; set; } = "Wait for {0} seconds... ";

        /// <summary>
        /// Space between the content and the horizontal end of the console buffer
        /// </summary>
        public static int BufferPadding { get; set; } = 1;

        /// <summary>
        /// Default padding for unbordered columns
        /// </summary>
        public static int ColumnPadding { get; set; } = 2;

        /// <summary>
        /// Default padding for a table cell
        /// </summary>
        public static int TableCellPadding { get; set; } = 1;

        /// <summary>
        /// Default color for text
        /// Will used when the color cannot be detected
        /// </summary>
        public static ConsoleColor DefaultForegroundColor { get; set; } = ConsoleColor.Gray;

        /// <summary>
        /// Default color for background
        /// Will used when the color cannot be detected
        /// </summary>
        public static ConsoleColor DefaultBackgroundColor { get; set; } = ConsoleColor.Black;


        //--> Colors

        /// <summary>
        /// Used if the color cannot recognized on the current system
        /// </summary>
        public static ConsoleColor UnknownColor { get { return (ConsoleColor)(-1); } }

        /// <summary>
        /// Get or set the current foreground color
        /// </summary>
        public static ConsoleColor ForegroundColor { get { return Console.ForegroundColor; } set { SetColor(null, value); } }

        /// <summary>
        /// Get the foreground color or a default value if a color cannot be detected
        /// </summary>
        public static ConsoleColor ForegroundColorOrDefault
        {
            get { return ForegroundColor == UnknownColor ? DefaultForegroundColor : ForegroundColor; }
        }

        /// <summary>
        /// Get or set the current background color
        /// </summary>
        public static ConsoleColor BackgroundColor { get { return Console.BackgroundColor; } set { SetColor(value, null); } }

        /// <summary>
        /// Get the background color or a default value if a color cannot be detected
        /// </summary>
        public static ConsoleColor BackgroundColorOrDefault
        {
            get { return BackgroundColor == UnknownColor ? DefaultBackgroundColor : BackgroundColor; }
        }

        /// <summary>
        /// Get or set the current color as ColorScheme object
        /// </summary>
        public static ColorScheme CurrentColorScheme
        {
            get { return new ColorScheme(BackgroundColorOrDefault, ForegroundColorOrDefault); }
            set { SetColor(value); }
        }

        /// <summary>
        /// Default ColorScheme for a table column header
        /// </summary>
        public static ColorScheme ColorTableHeading { get; set; } = new ColorScheme(ConsoleColor.Green, ConsoleColor.Blue);

        /// <summary>
        /// Default ColorScheme for a primary title
        /// </summary>
        public static ColorScheme ColorTitlePrimary { get; set; } = new ColorScheme(ConsoleColor.Gray, ConsoleColor.Black);

        /// <summary>
        /// Default ColorScheme for a secondary title
        /// </summary>
        public static ColorScheme ColorTitleSecondary { get; set; } = new ColorScheme(ConsoleColor.DarkGray, ConsoleColor.Black);

        /// <summary>
        /// Default ColorScheme for a highlighted text
        /// </summary>
        public static ColorScheme ColorHighlight { get; set; } = new ColorScheme(ConsoleColor.Yellow, ConsoleColor.Blue);


        //--> Window Properties

        /// <summary>
        /// Current buffer width without padding
        /// </summary>
        public static int Width { get { return BufferWidth - BufferPadding; } }

        /// <summary>
        /// Current Buffer height
        /// </summary>
        public static int Height { get { return BufferHeight; } }

        /// <summary>
        /// Horizontal cursor position
        /// </summary>
        public static int CursorX
        {
            get { return Console.CursorLeft; }
            set { Console.CursorLeft = value; }
        }

        /// <summary>
        /// Vertical cursor position
        /// </summary>
        public static int CursorY
        {
            get { return Console.CursorTop; }
            set { Console.CursorTop = value; }
        }

        private static bool _cursorvisible;

        /// <summary>
        /// Cursor visibility
        /// </summary>
        public static bool CursorVisible
        {
            get { return _cursorvisible; }
            set { Console.CursorVisible = value; _cursorvisible = value; }
        }

        /// <summary>
        /// Console Buffer width
        /// </summary>
        /// <exception cref="System.PlatformNotSupportedException">Setter not supported on some systems</exception>
        public static int BufferWidth
        {
            get { return Console.BufferWidth; }
            set { Console.BufferWidth = value; }
        }

        /// <summary>
        /// Console buffer height
        /// </summary>
        /// <exception cref="System.PlatformNotSupportedException">Setter not supported on some systems</exception>
        public static int BufferHeight
        {
            get { return Console.BufferHeight; }
            set { Console.BufferHeight = value; }
        }

        /// <summary>
        /// Console window width
        /// </summary>
        /// <exception cref="System.PlatformNotSupportedException">Setter not supported on some systems</exception>
        public static int WindowWidth
        {
            get { return Console.WindowWidth; }
            set { Console.WindowWidth = value; }
        }

        /// <summary>
        /// Console window height
        /// </summary>
        /// <exception cref="System.PlatformNotSupportedException">Setter not supported on some systems</exception>
        public static int WindowHeight
        {
            get { return Console.WindowHeight; }
            set { Console.WindowHeight = value; }
        }

        /// <summary>
        /// Maximum possible console window width
        /// </summary>
        public static int WindowWidthMax { get { return Console.LargestWindowWidth; } }

        /// <summary>
        /// Maximum possible console window height
        /// </summary>
        public static int WindowHeightMax { get { return Console.LargestWindowHeight; } }

        /// <summary>
        /// Horizontal offset of the buffer shown in window
        /// </summary>
        /// <exception cref="System.PlatformNotSupportedException">Setter not supported on some systems</exception>
        public static int WindowPosX
        {
            get { return Console.WindowLeft; }
            set { Console.WindowLeft = value; }
        }

        /// <summary>
        /// Vertical offset of the buffer shown in window 
        /// </summary>
        /// <exception cref="System.PlatformNotSupportedException">Setter not supported on some systems</exception>
        public static int WindowPosY
        {
            get { return Console.WindowTop; }
            set { Console.WindowTop = value; }
        }

        /// <summary>
        /// Output encoding
        /// </summary>
        public static Encoding OutputEncoding
        {
            get { return Console.OutputEncoding; }
            set { Console.OutputEncoding = value; }
        }

        /// <summary>
        /// Vertical position on console buffer
        /// Aka: The number of written console lines
        /// </summary>
        public static ulong RealCursorY { get; private set; }

        /// <summary>
        /// Fired when writing text to the console
        /// </summary>
        public static event ConsoleWriteEventHandler OnWrite;

        /// <summary>
        /// Fired when changing the color
        /// </summary>
        public static event ConsoleChangeColorEventHandler OnColorChange;




        static CoEx()
        {
            CursorVisible = true;
        }

        private static bool TryPlatform(Action action)
        {
            try
            {
                action.Invoke();
                return true;
            }
            catch (PlatformNotSupportedException)
            {
                return false;
            }
        }

        public static void Clear()
        {
            Console.Clear();
        }

        public static void Seek(int? x, int? y)
        {
            Seek(x, y, false);
        }

        public static void Seek(int? x, int? y, bool clear)
        {
            if (x.HasValue == false)
            {
                x = CursorX;
            }
            else if(x<0)
            {
                x = CursorX + x;
            }

            if (y.HasValue == false)
            {
                y = CursorY;
            }
            else if(y<0)
            {
                y = CursorY + y;
            }

            if (clear && y <= CursorY)
            {
                for (int i = CursorY; i >= y; i--)
                {
                    CursorX = 0;
                    Write("".PadLeft(Width, ' '));
                    CursorX = 0;
                    if (i > y)
                    {
                        CursorY = CursorY - 1;
                        RealCursorY--;
                    }
                }
            }
            else if (y.Value >= 0 && y.Value <= BufferHeight)
            {
                var temp = y.Value - CursorY;
                if (temp < 0)
                {
                    RealCursorY -= (ulong)(0 - temp);
                }
                else if (temp > 0)
                {
                    RealCursorY += (ulong)temp;
                }
                
                CursorY = y.Value;
            }

            if (x.Value >= 0 && x.Value <= BufferWidth)
            {
                CursorX = x.Value;
            }
        }

        public static void GoUp()
        {
            Seek(0, -1);
        }

        public static void Scroll(int x, int y)
        {
            if (x < 0)
            {
                x = CursorX + x;
            }

            if (x < 0)
            {
                x = 0;
            }

            if (y < 0)
            {
                y = CursorY + y;
            }

            if (y < 0)
            {
                y = 0;
            }

            if (x + WindowWidth > BufferWidth)
            {
                x = BufferWidth - WindowWidth;
            }

            if (y + WindowHeight > BufferHeight)
            {
                y = BufferHeight - WindowHeight;
            }

            WindowPosX = x;
            WindowPosY = y;
        }

        public static void DelayedScroll(int x, int y, int delayms)
        {
            new Thread(() =>
            {
                Thread.Sleep(delayms);
                Scroll(x, y);
            }).Start();
        }

        public static void ResetColor(bool supressevent)
        {
            Console.ResetColor();
            if (supressevent == false && OnColorChange != null)
            {
                OnColorChange(CurrentColorScheme);
            }
        }

        public static void ResetColor()
        {
            ResetColor(false);
        }

        public static void SetColor(ColorScheme scheme)
        {
            scheme = scheme ?? new ColorScheme();
            SetColor(scheme.Background, scheme.Foreground);
        }

        public static void SetColor(ConsoleColor? bg, ConsoleColor? fg)
        {
            ResetColor(true);
            if (fg.HasValue)
            {
                Console.ForegroundColor = fg.Value;
            }
            if (bg.HasValue)
            {
                Console.BackgroundColor = bg.Value;
            }

            OnColorChange?.Invoke(CurrentColorScheme);
        }

        public static void SetColor(RowConf cfg, int column, string content)
        {
            if (cfg.IsColorize == null || cfg.IsColorize(cfg, column, content))
            {
                ColorScheme color = cfg.Color == null ? null : cfg.Color(cfg, column, content);
                SetColor(color);
            }
        }

        public static string ReadLine()
        {
            var temp = Console.ReadLine();
            RealCursorY++;
            return temp;
        }

        public static string ReadKeyChar()
        {
            return Console.ReadKey().KeyChar.ToString().Trim();
        }

        public static bool Confirm(string msg)
        {
            Write("{0} (Enter) ", msg);
            string res = ReadLine();
            return (res == "");
        }

        public static void PressAnyKey(string message)
        {
            Write(message + " ");
            ReadKeyChar();
            WriteLine();
        }

        public static void PressAnyKey()
        {
            PressAnyKey(PressAnyKeyDefaultText);
        }

        public static void Timeout(string message, int seconds)
        {
            CursorVisible = false;
            for (int i = seconds; i >= 0; i--)
            {
                Seek(0, null, true);
                Write(string.Format(message, i));
                Thread.Sleep(1000);
            }
            Seek(0, null, true);
            CursorVisible = true;
        }

        public static void Timeout(int seconds)
        {
            Timeout(TimeoutDefaultText, seconds);
        }

        public static void Timeout()
        {
            Timeout(TimeoutDefaultText, 5);
        }

        public static void Write(string str, ConsoleColor? background, ConsoleColor? foreground)
        {
            if (foreground.HasValue || background.HasValue)
            {
                SetColor(background, foreground);
            }

            RealCursorY += (ulong)str.ToCharArray().Where(v => v == '\n').Count();
            Console.Write(str);

            OnWrite?.Invoke(str);

            if (foreground.HasValue || background.HasValue)
            {
                ResetColor(true);
            }
        }

        public static void Write(string str, ColorScheme color)
        {
            Write(str, color.Background, color.Foreground);
        }

        public static void Write(string str)
        {
            Write(str, new ColorScheme(null, null));
        }

        public static void Write(string[] strings)
        {
            strings.ToList().ForEach(v => Write(v));
        }

        public static void WriteLine(string str)
        {
            Write(str + Environment.NewLine);
        }

        public static void WriteLine(string str, ConsoleColor? background, ConsoleColor? foreground)
        {
            Write(str + Environment.NewLine, background, foreground);
        }

        public static void WriteLine(string str, ColorScheme color)
        {
            WriteLine(str, color.Background, color.Foreground);
        }

        public static void WriteLine(string[] strings)
        {
            strings.ToList().ForEach(v => WriteLine(v));
        }

        public static void Write(string format, ConsoleColor? background, ConsoleColor? foreground, params object[] args)
        {
            Write(string.Format(format, args), background, foreground);
        }

        public static void Write(string format, ColorScheme color, params object[] args)
        {
            Write(format, color.Background, color.Foreground, args);
        }

        public static void Write(string format, params object[] args)
        {
            Write(string.Format(format, args));
        }

        public static void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        public static void WriteLine(string format, ConsoleColor? background, ConsoleColor? foreground, params object[] args)
        {
            WriteLine(string.Format(format, args), background, foreground);
        }

        public static void WriteLine(string format, ColorScheme color, params object[] args)
        {
            WriteLine(format, color.Background, color.Foreground, args);
        }

        public static void WriteLine()
        {
            WriteLine("");
        }

        public static void WriteBorderRow(BorderConf bconf, LengthCollection lconf)
        {
            Write("" + bconf.CharLeft + ("".PadLeft(TableCellPadding, bconf.CharBody)));
            for (int i = 0; i < lconf.Count; i++)
            {
                Write("".PadLeft(lconf.BorderedLength.Items[i].Length + TableCellPadding, bconf.CharBody));
                if (i < lconf.Count - 1)
                {
                    Write("" + bconf.CharCorner + ("".PadLeft(TableCellPadding, bconf.CharBody)));
                }
            }
            Write("" + bconf.CharRight);
            WriteLine();
        }

        public static void WriteColumnsColored(RowConf cfg)
        {
            // Build column wrapping
            string[][] sublines = cfg.WordwrappedData;

            // Print columns
            int maxsublines = sublines.Max(v => v.Length);
            for (int j = 0; j < maxsublines; j++)
            {
                if (cfg.Border.Enabled)
                {
                    Write(cfg.Border.CellVerticalLine + ("".PadLeft(TableCellPadding, ' ')));
                }

                for (int i = 0; i < sublines.Length; i++)
                {
                    string item = (sublines[i].Length > j ? sublines[i][j] : "");
                    int padlen = cfg.Length.Items[i].Length - item.Length;
                    string pad = "".PadRight(padlen, ' ');
                    bool hlpadding = cfg.IsHighlightPadding != null && cfg.IsHighlightPadding(cfg, i, item);
                    RowCollectionSettings.ALIGN align = (cfg.Align == null ? null : cfg.Align(cfg, i, item)) ?? RowCollectionSettings.ALIGN.LEFT;
                    
                    ResetColor();

                    if (align == RowCollectionSettings.ALIGN.RIGHT)
                    {
                        if (hlpadding) { SetColor(cfg, i, item); }
                        Write(pad);
                        SetColor(cfg, i, item);
                        Write(item);
                    }
                    else if (align == RowCollectionSettings.ALIGN.CENTER)
                    {
                        int before = (int)((0.0 + padlen) / 2.0);
                        int after = (int)Math.Ceiling(((0.0 + padlen) / 2.0));

                        if (hlpadding) { SetColor(cfg, i, item); }
                        Write("".PadLeft(before, ' '));
                        SetColor(cfg, i, item);
                        Write(item);
                        if (!hlpadding) { ResetColor(); }
                        Write("".PadLeft(after, ' '));
                    }
                    else
                    {
                        SetColor(cfg, i, item);
                        Write(item);
                        if (!hlpadding) { ResetColor(); }
                        Write(pad);
                    }

                    ResetColor();

                    if (cfg.Border.Enabled)
                    {
                        Write(("".PadLeft(TableCellPadding, ' ')) + cfg.Border.CellVerticalLine);
                        if (i < sublines.Length - 1)
                        {
                            Write(("".PadLeft(TableCellPadding, ' ')));
                        }
                    }
                    else if (i < sublines.Length - 1)
                    {
                        Write(("".PadLeft(ColumnPadding, ' ')));
                    }

                }
                WriteLine();
            }
        }

        public static void WriteTable(RowCollection rows)
        {
            int i = 0;
            foreach (var item in rows.AsTable())
            {
                if (item.Border.Enabled && item.Border.HorizontalLineBody(item))
                {
                    WriteBorderRow(item.Border, item.RealLength);
                }
                WriteColumnsColored(item);
                i++;
            }

            if (rows.Items.First().Border.Enabled)
            {
                WriteBorderRow(rows.Items.First().Border.SetMode(BorderConf.ROWMODE.END), rows.Length);
            }
        }

        public static void WriteColumns(params string[] s)
        {
            WriteColumnsColored(RowConf.Create(s).SetHlPadding(false));
        }

        public static void WriteColumns(LengthCollection length, params string[] s)
        {
            WriteColumnsColored(RowConf.Create(length, s));
        }

        public static void WriteTH(params string[] s)
        {
            WriteColumnsColored(RowConf.Create(s).PresetTH());
        }

        public static void WriteTH(LengthCollection length, params string[] s)
        {
            WriteColumnsColored(RowConf.Create(length, s).PresetTH());
        }

        public static void WriteHl(params string[] s)
        {
            WriteColumnsColored(RowConf.Create(s).PresetHL());
        }

        public static void WriteHl(LengthCollection length, params string[] s)
        {
            WriteColumnsColored(RowConf.Create(length, s).PresetHL());
        }

        public static void WriteTitle(params string[] s)
        {
            WriteColumnsColored(RowConf.Create(s).PresetTitle().SetBordered(false));
        }

        public static void WriteTitle(LengthCollection length, string[] s)
        {
            WriteColumnsColored(RowConf.Create(length, s).PresetTitle().SetBordered(false));
        }

        public static void WriteTitleLarge(params string[] s)
        {
            WriteColumnsColored(RowConf.Create(s).PresetTitle().SetBordered(false).SetAlignment(RowConf.ALIGNCENTER).SetHlPadding(true));
        }

        public static void WriteTitleLarge(LengthCollection length, string[] s)
        {
            WriteColumnsColored(RowConf.Create(length, s).PresetTitle().SetBordered(false).SetAlignment(RowConf.ALIGNCENTER).SetHlPadding(true));
        }

    }
}

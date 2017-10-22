using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace PerrysNetConsole
{

    public delegate void ConsoleWriteEventHandler(String text);
    public delegate void ConsoleChangeColorEventHandler(ColorScheme scheme);

    public static class CoEx
    {

        public static String DEFAULT_PRESSANYKEYMSG = "Press any key... ";
        public static String DEFAULT_TIMEOUTMSG = "Wait for {0} seconds... ";

        public static int BUFFERPADDING = 1;
        public static int COLUMNPADDING = 2;
        public static int TBLCELLPADDING = 1;

        public static ColorScheme COLORTABLEHEADING = new ColorScheme(ConsoleColor.Green, ConsoleColor.Blue);
        public static ColorScheme COLORTITLE = new ColorScheme(ConsoleColor.Gray, ConsoleColor.Black);
        public static ColorScheme COLORTITLESEC = new ColorScheme(ConsoleColor.DarkGray, ConsoleColor.Black);
        public static ColorScheme COLORHL = new ColorScheme(ConsoleColor.Yellow, ConsoleColor.Blue);

        public static int Width { get { return BufferWidth - BUFFERPADDING; } }
        public static int Height { get { return BufferHeight; } }
        public static int CursorX { get { return Console.CursorLeft; } set { Console.CursorLeft = value; } }
        public static int CursorY { get { return Console.CursorTop; } set { Console.CursorTop = value; } }
        public static bool CursorVisible { get { return Console.CursorVisible; } set { Console.CursorVisible = value; } }
        public static int BufferWidth { get { return Console.BufferWidth; } set { Console.BufferWidth = value; } }
        public static int BufferHeight { get { return Console.BufferHeight; } set { Console.BufferHeight = value; } }
        public static int WindowWidth { get { return Console.WindowWidth; } set { Console.WindowWidth = value; } }
        public static int WindowHeight { get { return Console.WindowHeight; } set { Console.WindowHeight = value; } }
        public static int WindowWidthMax { get { return Console.LargestWindowWidth; } }
        public static int WindowHeightMax { get { return Console.LargestWindowHeight; } }
        public static int WindowPosX { get { return Console.WindowLeft; } set { Console.WindowLeft = value; } }
        public static int WindowPosY { get { return Console.WindowTop; } set { Console.WindowTop = value; } }
        public static Encoding OutputEncoding { get { return Console.OutputEncoding; } set { Console.OutputEncoding = value; } }
        public static ConsoleColor ForegroundColor { get { return Console.ForegroundColor; } }
        public static ConsoleColor BackgroundColor { get { return Console.BackgroundColor; } }

        public static ulong RealCursorY { get; private set; }

        public static event ConsoleWriteEventHandler OnWrite;
        public static event ConsoleChangeColorEventHandler OnColorChange;

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
                OnColorChange(new ColorScheme(Console.BackgroundColor, Console.ForegroundColor));
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

            if (OnColorChange != null)
            {
                OnColorChange(new ColorScheme(Console.BackgroundColor, Console.ForegroundColor));
            }
        }

        public static void SetColor(RowConf cfg, int column, String content)
        {
            if (cfg.IsColorize == null || cfg.IsColorize(cfg, column, content))
            {
                ColorScheme color = cfg.Color == null ? null : cfg.Color(cfg, column, content);
                SetColor(color);
            }
        }

        public static String ReadLine()
        {
            var temp = Console.ReadLine();
            RealCursorY++;
            return temp;
        }

        public static String ReadKeyChar()
        {
            return Console.ReadKey().KeyChar.ToString().Trim();
        }

        public static bool Confirm(String msg)
        {
            Write("{0} (Enter) ", msg);
            String res = ReadLine();
            return (res == "");
        }

        public static void PressAnyKey(String message)
        {
            Write(message + " ");
            ReadKeyChar();
            WriteLine();
        }

        public static void PressAnyKey()
        {
            PressAnyKey(DEFAULT_PRESSANYKEYMSG);
        }

        public static void Timeout(String message, int seconds)
        {
            CursorVisible = false;
            for (int i = seconds; i >= 0; i--)
            {
                Seek(0, null, true);
                Write(String.Format(message, i));
                Thread.Sleep(1000);
            }
            Seek(0, null, true);
            CursorVisible = true;
        }

        public static void Timeout(int seconds)
        {
            Timeout(DEFAULT_TIMEOUTMSG, seconds);
        }

        public static void Timeout()
        {
            Timeout(DEFAULT_TIMEOUTMSG, 5);
        }

        public static void Write(String str, ConsoleColor? background, ConsoleColor? foreground)
        {
            if (foreground.HasValue || background.HasValue)
            {
                SetColor(background, foreground);
            }

            RealCursorY += (ulong)str.ToCharArray().Where(v => v == '\n').Count();
            Console.Write(str);

            if (OnWrite != null)
            {
                OnWrite(str);
            }

            if (foreground.HasValue || background.HasValue)
            {
                Console.ResetColor();
            }
        }

        public static void Write(String str, ColorScheme color)
        {
            Write(str, color.Background, color.Foreground);
        }

        public static void Write(String str)
        {
            Write(str, new ColorScheme(null, null));
        }

        public static void Write(String[] strings)
        {
            strings.ToList().ForEach(v => Write(v));
        }

        public static void WriteLine(String str)
        {
            Write(str + Environment.NewLine);
        }

        public static void WriteLine(String str, ConsoleColor? background, ConsoleColor? foreground)
        {
            Write(str + Environment.NewLine, background, foreground);
        }

        public static void WriteLine(String str, ColorScheme color)
        {
            WriteLine(str, color.Background, color.Foreground);
        }

        public static void WriteLine(String[] strings)
        {
            strings.ToList().ForEach(v => WriteLine(v));
        }

        public static void Write(String format, ConsoleColor? background, ConsoleColor? foreground, params object[] args)
        {
            Write(String.Format(format, args), background, foreground);
        }

        public static void Write(String format, ColorScheme color, params object[] args)
        {
            Write(format, color.Background, color.Foreground, args);
        }

        public static void Write(String format, params object[] args)
        {
            Write(String.Format(format, args));
        }

        public static void WriteLine(String format, params object[] args)
        {
            WriteLine(String.Format(format, args));
        }

        public static void WriteLine(String format, ConsoleColor? background, ConsoleColor? foreground, params object[] args)
        {
            WriteLine(String.Format(format, args), background, foreground);
        }

        public static void WriteLine(String format, ColorScheme color, params object[] args)
        {
            WriteLine(format, color.Background, color.Foreground, args);
        }

        public static void WriteLine()
        {
            WriteLine("");
        }

        public static void WriteBorderRow(BorderConf bconf, LengthCollection lconf)
        {
            Write("" + bconf.CharLeft + ("".PadLeft(TBLCELLPADDING, bconf.CharBody)));
            for (int i = 0; i < lconf.Count; i++)
            {
                Write("".PadLeft(lconf.BorderedLength.Items[i].Length + TBLCELLPADDING, bconf.CharBody));
                if (i < lconf.Count - 1)
                {
                    Write("" + bconf.CharCorner + ("".PadLeft(TBLCELLPADDING, bconf.CharBody)));
                }
            }
            Write("" + bconf.CharRight);
            WriteLine();
        }

        public static void WriteColumnsColored(RowConf cfg)
        {
            // Build column wrapping
            String[][] sublines = cfg.WordwrappedData;

            // Print columns
            int maxsublines = sublines.Max(v => v.Length);
            for (int j = 0; j < maxsublines; j++)
            {
                if (cfg.Border.Enabled)
                {
                    Write(cfg.Border.CellVerticalLine + ("".PadLeft(TBLCELLPADDING, ' ')));
                }

                for (int i = 0; i < sublines.Length; i++)
                {
                    String item = (sublines[i].Length > j ? sublines[i][j] : "");
                    int padlen = cfg.Length.Items[i].Length - item.Length;
                    String pad = "".PadRight(padlen, ' ');
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
                        Write(("".PadLeft(TBLCELLPADDING, ' ')) + cfg.Border.CellVerticalLine);
                        if (i < sublines.Length - 1)
                        {
                            Write(("".PadLeft(TBLCELLPADDING, ' ')));
                        }
                    }
                    else if (i < sublines.Length - 1)
                    {
                        Write(("".PadLeft(COLUMNPADDING, ' ')));
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

        public static void WriteColumns(params String[] s)
        {
            WriteColumnsColored(RowConf.Create(s).SetHlPadding(false));
        }

        public static void WriteColumns(LengthCollection length, params String[] s)
        {
            WriteColumnsColored(RowConf.Create(length, s));
        }

        public static void WriteTH(params String[] s)
        {
            WriteColumnsColored(RowConf.Create(s).PresetTH());
        }

        public static void WriteTH(LengthCollection length, params String[] s)
        {
            WriteColumnsColored(RowConf.Create(length, s).PresetTH());
        }

        public static void WriteHl(params String[] s)
        {
            WriteColumnsColored(RowConf.Create(s).PresetHL());
        }

        public static void WriteHl(LengthCollection length, params String[] s)
        {
            WriteColumnsColored(RowConf.Create(length, s).PresetHL());
        }

        public static void WriteTitle(params String[] s)
        {
            WriteColumnsColored(RowConf.Create(s).PresetTitle().SetBordered(false));
        }

        public static void WriteTitle(LengthCollection length, String[] s)
        {
            WriteColumnsColored(RowConf.Create(length, s).PresetTitle().SetBordered(false));
        }

        public static void WriteTitleLarge(params String[] s)
        {
            WriteColumnsColored(RowConf.Create(s).PresetTitle().SetBordered(false).SetAlignment(RowConf.ALIGNCENTER).SetHlPadding(true));
        }

        public static void WriteTitleLarge(LengthCollection length, String[] s)
        {
            WriteColumnsColored(RowConf.Create(length, s).PresetTitle().SetBordered(false).SetAlignment(RowConf.ALIGNCENTER).SetHlPadding(true));
        }

    }
}

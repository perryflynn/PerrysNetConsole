using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace PerrysNetConsole
{

    public delegate void ConsoleWriteEventHandler(String text);

    public static class CoEx
    {

        public static String DEFAULT_PRESSANYKEYMSG = "Press any key...";

        public static int BUFFERPADDING = 1;
        public static int COLUMNPADDING = 2;
        public static int TBLCELLPADDING = 1;
        public static ConsoleColor THFG = ConsoleColor.Blue;
        public static ConsoleColor THBG = ConsoleColor.Green;
        public static ConsoleColor TITLEFG = ConsoleColor.Black;
        public static ConsoleColor TITLEBG = ConsoleColor.Gray;
        public static ConsoleColor TITLEFGSEC = ConsoleColor.Black;
        public static ConsoleColor TITLEBGSEC = ConsoleColor.DarkGray;
        public static ConsoleColor HLBG = ConsoleColor.Yellow;
        public static ConsoleColor HLFG = ConsoleColor.Blue;


        public static int Width { get { return BufferWidth - BUFFERPADDING; } }
        public static int Height { get { return BufferHeight; } }
        public static int CursorX { get { return Console.CursorLeft; } set { Console.CursorLeft = value; } }
        public static int CursorY { get { return Console.CursorTop; } set { Console.CursorTop = value; } }
        public static bool CursorVisible { get { return Console.CursorVisible; } set { Console.CursorVisible = value; } }
        public static int BufferWidth { get { return Console.BufferWidth; } set { Console.BufferWidth = value; } }
        public static int BufferHeight { get { return Console.BufferHeight; } set { Console.BufferHeight = value; } }
        public static Encoding OutputEncoding { get { return Console.OutputEncoding; } set { Console.OutputEncoding = value; } }

        public static event ConsoleWriteEventHandler OnWrite;

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
                    }
                }
            }
            else if (y.Value >= 0 && y.Value <= BufferHeight)
            {
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
            if (x >= 0 && y >= 0 && x <= BufferWidth && y <= BufferHeight)
            {
                Console.SetWindowPosition(x, y);
            }
        }

        public static void DelayedScroll(int x, int y, int delayms)
        {
            new Thread(() =>
            {
                Thread.Sleep(delayms);
                Scroll(x, y);
            }).Start();
        }

        public static void ResetColor()
        {
            Console.ResetColor();
        }

        public static void SetColor(ConsoleColor? bg, ConsoleColor? fg)
        {
            ResetColor();
            if (fg.HasValue)
            {
                Console.ForegroundColor = fg.Value;
            }
            if (bg.HasValue)
            {
                Console.BackgroundColor = bg.Value;
            }
        }

        public static void SetColor(RowConf cfg, int column, String content)
        {
            if (cfg.IsColorize == null || cfg.IsColorize(cfg, column, content))
            {
                ConsoleColor? bg = cfg.BackgroundColor == null ? null : cfg.BackgroundColor(cfg, column, content);
                ConsoleColor? fg = cfg.ForegroundColor == null ? null : cfg.ForegroundColor(cfg, column, content);
                SetColor(bg, fg);
            }
        }

        public static String ReadLine()
        {
            return Console.ReadLine();
        }

        public static String ReadKeyChar()
        {
            return Console.ReadKey().KeyChar.ToString().Trim();
        }

        public static void Write(String str, ConsoleColor? background, ConsoleColor? foreground)
        {
            if (foreground.HasValue || background.HasValue)
            {
                SetColor(background, foreground);
            }

            Console.Write(str);

            if (foreground.HasValue || background.HasValue)
            {
                Console.ResetColor();
            }

            if (OnWrite != null)
            {
                OnWrite(str);
            }
        }

        public static void Write(String str)
        {
            Write(str, null, null);
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

        public static void WriteLine(String[] strings)
        {
            strings.ToList().ForEach(v => WriteLine(v));
        }

        public static void Write(String format, ConsoleColor? background, ConsoleColor? foreground, params object[] args)
        {
            Write(String.Format(format, args), background, foreground);
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

        public static void WriteLine()
        {
            WriteLine("");
        }

        public static bool Confirm(String msg)
        {
            Write("{0} (Enter) ", msg);
            String res = Console.ReadLine();
            return (res == "");
        }

        public static void PressAnyKey()
        {
            PressAnyKey(DEFAULT_PRESSANYKEYMSG, null);
        }

        public static void PressAnyKey(String message)
        {
            PressAnyKey(message, null);
        }

        public static void PressAnyKey(Action callback)
        {
            PressAnyKey(DEFAULT_PRESSANYKEYMSG, callback);
        }

        public static void PressAnyKey(String message, Action callback)
        {
            Write(message + " ");
            if (callback != null)
            {
                callback();
            }
            ReadKeyChar();
            WriteLine();
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

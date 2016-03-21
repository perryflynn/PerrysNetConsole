using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public static class CoEx
    {

        public static int BUFFERPADDING = 1;
        public static int COLUMNPADDING = 2;
        public static int TBLCELLPADDING = 1;
        public static ConsoleColor THFG = ConsoleColor.Blue;
        public static ConsoleColor THBG = ConsoleColor.Green;
        public static ConsoleColor TITLEFG = ConsoleColor.Black;
        public static ConsoleColor TITLEBG = ConsoleColor.Gray;
        public static ConsoleColor HLBG = ConsoleColor.Yellow;
        public static ConsoleColor HLFG = ConsoleColor.Blue;


        public static int Width { get { return Console.BufferWidth - BUFFERPADDING; } }
        public static int CursorX { get { return Console.CursorLeft; } set { Console.CursorLeft = value; } }
        public static int CursorY { get { return Console.CursorTop; } set { Console.CursorTop = value; } }

        public static void Clear()
        {
            Console.Clear();
        }

        public static void ResetColor()
        {
            Console.ResetColor();
        }

        public static void Seek(int x, int y)
        {
            if(x<0)
            {
                x = CursorX + x;
            }

            if(y<0)
            {
                y = CursorY + y;
            }

            CursorX = x;
            CursorY = y;
        }

        public static void GoUp()
        {
            Seek(0, -1);
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

        public static void Write(String format, params String[] args)
        {
            Console.Write(format, args);
        }

        public static void Write(String str)
        {
            Console.Write(str);
        }

        public static void WriteLine(String format, params String[] args)
        {
            Console.WriteLine(format, args);
        }

        public static void WriteLine(String str)
        {
            Console.WriteLine(str);
        }

        public static void WriteLine()
        {
            Console.WriteLine();
        }

        public static bool Confirm(String msg)
        {
            Write("{0} (Enter) ", msg);
            String res = Console.ReadLine();
            return (res == "");
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

    }
}

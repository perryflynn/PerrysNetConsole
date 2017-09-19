using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public class RowConf
    {

        public static Func<RowConf, int, String, ConsoleColor?> THFG = delegate(RowConf cfg, int index, String content) { return CoEx.THFG; };
        public static Func<RowConf, int, String, ConsoleColor?> THBG = delegate(RowConf cfg, int index, String content) { return CoEx.THBG; };
        public static Func<RowConf, int, String, ConsoleColor?> TITLEFG = delegate(RowConf cfg, int index, String content) { return CoEx.TITLEFG; };
        public static Func<RowConf, int, String, ConsoleColor?> TITLEBG = delegate(RowConf cfg, int index, String content) { return CoEx.TITLEBG; };
        public static Func<RowConf, int, String, ConsoleColor?> HLBG = delegate(RowConf cfg, int index, String content) { return CoEx.HLBG; };
        public static Func<RowConf, int, String, ConsoleColor?> HLFG = delegate(RowConf cfg, int index, String content) { return CoEx.HLFG; };
        public static Func<RowConf, int, String, bool> YESFUNC = delegate(RowConf cfg, int index, String content) { return true; };
        public static Func<RowConf, int, String, bool> NOFUNC = delegate(RowConf cfg, int index, String content) { return false; };
        public static Func<RowConf, int, String, RowCollectionSettings.ALIGN?> ALIGNLEFT = delegate (RowConf cfg, int idx, String text) { return RowCollectionSettings.ALIGN.LEFT; };
        public static Func<RowConf, int, String, RowCollectionSettings.ALIGN?> ALIGNCENTER = delegate (RowConf cfg, int idx, String text) { return RowCollectionSettings.ALIGN.CENTER; };
        public static Func<RowConf, int, String, RowCollectionSettings.ALIGN?> ALIGNRIGHT = delegate (RowConf cfg, int idx, String text) { return RowCollectionSettings.ALIGN.RIGHT; };

        public static RowConf Create(LengthCollection length, params String[] columns)
        {
            return new RowConf(length, columns);
        }

        public static RowConf Create(params String[] columns)
        {
            int column = CoEx.Width / columns.Length;
            LengthCollection length = new LengthCollection();
            for (int i = 0; i < columns.Length; i++) length.Items.Add(new LengthConf() { Length = column, Index = i });
            return Create(length, columns);
        }

        public static RowConf Create()
        {
            return new RowConf(new LengthCollection { }, new String[] { });
        }

        public String[] Data { get; set; }
        protected RowCollection parent;
        public RowCollection Parent { get { return this.parent; } set { this.parent = value; this.reallength = null; } }

        protected BorderConf border;
        public BorderConf Border
        {
            get
            {
                if (this.border == null && this.Parent != null)
                {
                    return this.Parent.Settings.Border;
                }
                else if (this.border == null)
                {
                    this.border = new BorderConf();
                }
                return this.border;
            }
            set
            {
                this.border = value;
            }
        }

        protected LengthCollection reallength;
        public LengthCollection RealLength
        {
            get { return this.reallength == null && this.Parent != null ? this.Parent.Length : this.reallength; }
            set { this.reallength = value; }
        }

        protected Func<RowConf, int, String, ConsoleColor?> foregroundcolor;
        public Func<RowConf, int, String, ConsoleColor?> ForegroundColor
        {
            get { return this.foregroundcolor == null && this.Parent != null ? this.Parent.Settings.ForegroundColor : this.foregroundcolor; }
            set { this.foregroundcolor = value; }
        }

        protected Func<RowConf, int, String, ConsoleColor?> backgroundcolor;
        public Func<RowConf, int, String, ConsoleColor?> BackgroundColor
        {
            get { return this.backgroundcolor == null && this.Parent != null ? this.Parent.Settings.BackgroundColor : this.backgroundcolor; }
            set { this.backgroundcolor = value; }
        }

        protected Func<RowConf, int, String, bool> iscolorize;
        public Func<RowConf, int, String, bool> IsColorize
        {
            get { return this.iscolorize == null && this.Parent != null ? this.Parent.Settings.IsColorize : this.iscolorize; }
            set { this.iscolorize = value; }
        }

        protected Func<RowConf, int, String, bool> ishighlightpadding;
        public Func<RowConf, int, String, bool> IsHighlightPadding
        {
            get { return this.ishighlightpadding == null && this.Parent != null ? this.Parent.Settings.IsHighlightPadding : this.ishighlightpadding; }
            set { this.ishighlightpadding = value; }
        }

        protected Func<RowConf, int, String, RowCollectionSettings.ALIGN?> align;
        public Func<RowConf, int, String, RowCollectionSettings.ALIGN?> Align
        {
            get { return this.align == null && this.Parent != null ? this.Parent.Settings.Align : this.align; }
            set { this.align = value; }
        }

        public int Index { get { return this.Parent == null ? -1 : this.Parent.Items.IndexOf(this); } }
        public bool IsFirst { get { return this.Index < 1; } }
        public bool IsLast { get { return this.Parent == null ? false : this.Index >= this.Parent.Count - 1; } }


        protected RowConf() { }

        protected RowConf(LengthCollection length, params String[] columns)
        {
            this.RealLength = length;
            this.Data = columns;

            if (this.RealLength.Count > this.Data.Length)
            {
                throw new ArgumentException("Not all column have length definitions!");
            }
        }

        public String[][] WordwrappedData
        {
            get
            {
                // Build column wrapping
                String[][] sublines = new String[this.Data.Length][];

                int i = 0;
                foreach (String item in this.Data)
                {
                    if (item.Length > (this.Length.Items[i].Length))
                    {
                        // Amount of lines
                        int parts = ((int)Math.Ceiling(((0.0 + item.Length) / ((0.0 + this.Length.Items[i].Length)))));
                        sublines[i] = new String[parts];

                        // Split string in lines
                        int start = 0;
                        for (int j = 0; j < parts; j++)
                        {
                            if (start + this.Length.Items[i].Length <= item.Length)
                            {
                                sublines[i][j] = item.Substring(start, this.Length.Items[i].Length).Trim();
                                start += this.Length.Items[i].Length;
                            }
                            else
                            {
                                sublines[i][j] = item.Substring(start).Trim();
                            }
                        }
                    }
                    else
                    {
                        // Single line
                        sublines[i] = new String[] { item };
                    }
                    i++;
                }

                if (sublines.Max(v => v.Length) > 1)
                {
                    var len = sublines.Length;
                }

                return sublines;
            }
        }

        public LengthCollection Length
        {
            get
            {
                if (this.Border.Enabled)
                {
                    return this.RealLength.BorderedLength;
                }
                else
                {
                    return this.RealLength.PaddedLength;
                }
            }
            set
            {
                this.RealLength = value;
            }
        }

        public RowConf SetLength(LengthCollection length)
        {
            this.RealLength = length;
            return this;
        }

        public RowConf SetBordered(bool b)
        {
            this.Border.Enabled = b;
            return this;
        }

        public RowConf SetBorderMode(BorderConf.ROWMODE mode)
        {
            this.Border.RowMode = mode;
            return this;
        }

        public RowConf SetFg(Func<RowConf, int, String, ConsoleColor?> c)
        {
            this.ForegroundColor = c;
            return this;
        }

        public RowConf SetFg(ConsoleColor? c)
        {
            return this.SetFg(delegate(RowConf cfg, int index, String content) { return c; });
        }

        public RowConf SetBg(Func<RowConf, int, String, ConsoleColor?> c)
        {
            this.BackgroundColor = c;
            return this;
        }

        public RowConf SetBg(ConsoleColor? c)
        {
            return this.SetBg(delegate(RowConf cfg, int index, String content) { return c; });
        }

        public RowConf SetColorize(bool b)
        {
            this.IsColorize = delegate(RowConf cfg, int index, String content) { return b; };
            return this;
        }

        public RowConf SetHlPadding(bool b)
        {
            this.IsHighlightPadding = delegate(RowConf cfg, int index, String content) { return b; };
            return this;
        }

        public RowConf SetAlignment(Func<RowConf, int, String, RowCollectionSettings.ALIGN?> alignfunc)
        {
            this.Align = alignfunc;
            return this;
        }

        public RowConf Clone()
        {
            return this.Clone(false);
        }

        public RowConf Clone(bool standalone)
        {
            var c = new RowConf()
            {
                border = (standalone ? this.Border.Clone() : this.border),
                Data = this.Data.ToList().ToArray(),
                RealLength = (standalone ? this.RealLength.Clone() : this.reallength),
                ForegroundColor = (standalone ? this.ForegroundColor : this.foregroundcolor),
                BackgroundColor = (standalone ? this.BackgroundColor : this.backgroundcolor),
                IsColorize = (standalone ? this.IsColorize : this.iscolorize),
                IsHighlightPadding = (standalone ? this.IsHighlightPadding : this.ishighlightpadding)
            };
            
            return c;
        }

        public RowConf PresetTH()
        {
            this.ForegroundColor = THFG;
            this.BackgroundColor = THBG;
            this.IsHighlightPadding = YESFUNC;
            this.IsColorize = YESFUNC;
            return this;
        }

        public RowConf PresetHL()
        {
            this.ForegroundColor = HLFG;
            this.BackgroundColor = HLBG;
            this.IsHighlightPadding = NOFUNC;
            return this;
        }

        public RowConf PresetTitle()
        {
            this.ForegroundColor = TITLEFG;
            this.BackgroundColor = TITLEBG;
            this.IsHighlightPadding = NOFUNC;
            return this;
        }

    }
}

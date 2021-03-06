﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public class RowConf
    {

        public static Func<RowConf, int, string, ColorScheme> COLORTH = delegate(RowConf cfg, int index, string content) { return CoEx.ColorTableHeading; };
        public static Func<RowConf, int, string, ColorScheme> COLORTITLE = delegate(RowConf cfg, int index, string content) { return CoEx.ColorTitlePrimary; };
        public static Func<RowConf, int, string, ColorScheme> COLORHL = delegate(RowConf cfg, int index, string content) { return CoEx.ColorHighlight; };
        public static Func<RowConf, int, string, bool> YESFUNC = delegate(RowConf cfg, int index, string content) { return true; };
        public static Func<RowConf, int, string, bool> NOFUNC = delegate(RowConf cfg, int index, string content) { return false; };
        public static Func<RowConf, int, string, RowCollectionSettings.ALIGN?> ALIGNLEFT = delegate (RowConf cfg, int idx, string text) { return RowCollectionSettings.ALIGN.LEFT; };
        public static Func<RowConf, int, string, RowCollectionSettings.ALIGN?> ALIGNCENTER = delegate (RowConf cfg, int idx, string text) { return RowCollectionSettings.ALIGN.CENTER; };
        public static Func<RowConf, int, string, RowCollectionSettings.ALIGN?> ALIGNRIGHT = delegate (RowConf cfg, int idx, string text) { return RowCollectionSettings.ALIGN.RIGHT; };

        public static RowConf Create(LengthCollection length, params string[] columns)
        {
            return new RowConf(length, columns);
        }

        public static RowConf Create(params string[] columns)
        {
            int column = CoEx.Width / columns.Length;
            LengthCollection length = new LengthCollection();
            for (int i = 0; i < columns.Length; i++) length.Items.Add(new LengthConf() { Length = column, Index = i });
            return Create(length, columns);
        }

        public static RowConf Create()
        {
            return new RowConf(new LengthCollection { }, new string[] { });
        }

        public string[] Data { get; set; }
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

        protected Func<RowConf, int, string, ColorScheme> color;
        public Func<RowConf, int, string, ColorScheme> Color
        {
            get { return this.color == null && this.Parent != null ? this.Parent.Settings.Color : this.color; }
            set { this.color = value; }
        }

        protected Func<RowConf, int, string, bool> iscolorize;
        public Func<RowConf, int, string, bool> IsColorize
        {
            get { return this.iscolorize == null && this.Parent != null ? this.Parent.Settings.IsColorize : this.iscolorize; }
            set { this.iscolorize = value; }
        }

        protected Func<RowConf, int, string, bool> ishighlightpadding;
        public Func<RowConf, int, string, bool> IsHighlightPadding
        {
            get { return this.ishighlightpadding == null && this.Parent != null ? this.Parent.Settings.IsHighlightPadding : this.ishighlightpadding; }
            set { this.ishighlightpadding = value; }
        }

        protected Func<RowConf, int, string, RowCollectionSettings.ALIGN?> align;
        public Func<RowConf, int, string, RowCollectionSettings.ALIGN?> Align
        {
            get { return this.align == null && this.Parent != null ? this.Parent.Settings.Align : this.align; }
            set { this.align = value; }
        }

        public int Index { get { return this.Parent == null ? -1 : this.Parent.Items.IndexOf(this); } }
        public bool IsFirst { get { return this.Index < 1; } }
        public bool IsLast { get { return this.Parent == null ? false : this.Index >= this.Parent.Count - 1; } }


        protected RowConf() { }

        protected RowConf(LengthCollection length, params string[] columns)
        {
            this.RealLength = length;
            this.Data = columns;

            if (this.RealLength.Count > this.Data.Length)
            {
                throw new ArgumentException("Not all column have length definitions!");
            }
        }

        public string[][] WordwrappedData
        {
            get
            {
                // Build column wrapping
                string[][] sublines = new string[this.Data.Length][];

                int i = 0;
                foreach (string item in this.Data)
                {
                    if (item.Length > (this.Length.Items[i].Length))
                    {
                        // Amount of lines
                        int parts = ((int)Math.Ceiling(((0.0 + item.Length) / ((0.0 + this.Length.Items[i].Length)))));
                        sublines[i] = new string[parts];

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
                        sublines[i] = new string[] { item };
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

        public RowConf SetColor(Func<RowConf, int, string, ColorScheme> c)
        {
            this.Color = c;
            return this;
        }

        public RowConf SetColorize(bool b)
        {
            this.IsColorize = delegate(RowConf cfg, int index, string content) { return b; };
            return this;
        }

        public RowConf SetHlPadding(bool b)
        {
            this.IsHighlightPadding = delegate(RowConf cfg, int index, string content) { return b; };
            return this;
        }

        public RowConf SetAlignment(Func<RowConf, int, string, RowCollectionSettings.ALIGN?> alignfunc)
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
                Color = (standalone ? this.Color : this.color),
                IsColorize = (standalone ? this.IsColorize : this.iscolorize),
                IsHighlightPadding = (standalone ? this.IsHighlightPadding : this.ishighlightpadding)
            };
            
            return c;
        }

        public RowConf PresetTH()
        {
            this.color = COLORTH;
            this.IsHighlightPadding = YESFUNC;
            this.IsColorize = YESFUNC;
            return this;
        }

        public RowConf PresetHL()
        {
            this.Color = COLORHL;
            this.IsHighlightPadding = NOFUNC;
            return this;
        }

        public RowConf PresetTitle()
        {
            this.Color = COLORTITLE;
            this.IsHighlightPadding = NOFUNC;
            return this;
        }

    }
}

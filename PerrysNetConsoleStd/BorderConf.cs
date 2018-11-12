using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public class BorderConf
    {

        public static readonly Func<RowConf, bool> HorizontalLineAfterHeaderFunc = delegate(RowConf r) { return (r.Index < 2); };
        public static readonly Func<RowConf, bool> HorizontalLineAlwaysOnFunc = delegate(RowConf r) { return true; };
        public static readonly Func<RowConf, bool> HorizontalLineAlwaysOffFunc = delegate(RowConf r) { return false; };
        
        public static Func<RowConf, bool> DefaultHorizontalLineFunc = HorizontalLineAfterHeaderFunc;

        public char CellLeftTop = '┌';
        public char CellRightTop = '┐';
        public char CellLeftBottom = '└';
        public char CellRightBottom = '┘';
        public char CellHorizontalJointTop = '┬';
        public char CellHorizontalJointbottom = '┴';
        public char CellVerticalJointLeft = '├';
        public char CellTJoint = '┼';
        public char CellVerticalJointRight = '┤';
        public char CellHorizontalLine = '─';
        public char CellVerticalLine = '│';

        public bool Enabled { get; set; }
        public Func<RowConf, bool> HorizontalLineBody { get; set; }

        public enum ROWMODE { BEGIN, CONTINUE, END };
        public ROWMODE RowMode { get; set; }

        public BorderConf Clone()
        {
            var c = new BorderConf()
            {
                CellLeftTop = this.CellLeftTop,
                CellRightTop = this.CellRightTop,
                CellLeftBottom = this.CellLeftBottom,
                CellRightBottom = this.CellRightBottom,
                CellHorizontalJointTop = this.CellHorizontalJointTop,
                CellHorizontalJointbottom = this.CellHorizontalJointbottom,
                CellHorizontalLine = this.CellHorizontalLine,
                CellTJoint = this.CellTJoint,
                CellVerticalJointLeft = this.CellVerticalJointLeft,
                CellVerticalJointRight = this.CellVerticalJointRight,
                CellVerticalLine = this.CellVerticalLine,
                Enabled = this.Enabled,
                RowMode = this.RowMode,
                HorizontalLineBody = this.HorizontalLineBody
            };
            return c;
        }

        public BorderConf()
        {
            this.RowMode = ROWMODE.CONTINUE;
            this.HorizontalLineBody = DefaultHorizontalLineFunc;
        }

        public char CharLeft
        {
            get
            {
                switch (this.RowMode)
                {
                    case ROWMODE.BEGIN:
                        return this.CellLeftTop;
                    case ROWMODE.END:
                        return this.CellLeftBottom;
                    default:
                        return this.CellVerticalJointLeft;
                }
            }
        }

        public char CharBody
        {
            get
            {
                return this.CellHorizontalLine;
            }
        }

        public char CharCorner
        {
            get
            {
                switch (this.RowMode)
                {
                    case ROWMODE.BEGIN:
                        return this.CellHorizontalJointTop;
                    case ROWMODE.END:
                        return this.CellHorizontalJointbottom;
                    default:
                        return this.CellTJoint;
                }
            }
        }

        public char CharRight
        {
            get
            {
                switch (this.RowMode)
                {
                    case ROWMODE.BEGIN:
                        return this.CellRightTop;
                    case ROWMODE.END:
                        return this.CellRightBottom;
                    default:
                        return this.CellVerticalJointRight;
                }
            }
        }

        public BorderConf SetMode(ROWMODE mode)
        {
            this.RowMode = mode;
            return this;
        }

    }
}

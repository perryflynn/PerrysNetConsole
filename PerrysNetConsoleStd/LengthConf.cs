using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public class LengthConf
    {

        public const double BigMinPercent = 20;

        public LengthCollection Parent { get; set; }
        public int Index { get; set; }

        public int OriginalLength { get; set; }
        public int Length { get; set; }
        public bool IsResized { get { return this.Length != this.OriginalLength; } }
        public double LengthPercent { get { return 100.0 * (0.0 + this.Length) / this.Parent.Items.Sum(v => v.Length); } }

        public int Diffrence { get { return this.Length - this.OriginalLength; } }
        public bool IsFull { get { return this.Diffrence <= 0; } }
        public double DiffrencePercent { get { return 100.0 * (0.0 + this.Diffrence) / this.Parent.Items.Sum(v => v.Diffrence); } }
        public double NotFullDiffrencePercent { get { return this.IsFull == false ? (100.0 * (0.0 + this.Diffrence) / this.Parent.NotFullItems.Sum(v => v.Diffrence)) : 0; } }

        public bool IsBig { get { return (this.LengthPercent >= BigMinPercent); } }
        public double BigLengthPercent { get { return this.IsBig ? (100.0 * (0.0 + this.Length) / this.Parent.BigItems.Sum(v => v.Length)) : 0; } }

        public LengthConf()
        {
            this.OriginalLength = 0;
            this.Index = 0;
            this.Length = 0;
        }



        public LengthConf Clone()
        {
            return new LengthConf()
            {
                Index = Index,
                OriginalLength = OriginalLength,
                Length = Length
            };
        }

    }
}

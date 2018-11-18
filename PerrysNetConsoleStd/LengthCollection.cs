using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public class LengthCollection
    {

        public List<LengthConf> Items { get; set; }

        public LengthCollection()
        {
            this.Items = new List<LengthConf>();
        }

        public void Import(LengthConf c)
        {
            c.Parent = this;
            this.Items.Add(c);
        }

        public int TotalLength { get { return this.Items.Sum(v => v.Length); } }
        public int Count { get { return this.Items.Count; } }

        public List<LengthConf> BigItems { get { return this.Items.Where(v => v.IsBig).ToList(); } }
        public List<LengthConf> NotFullItems { get { return this.Items.Where(v => v.IsFull == false).ToList(); } }

        public LengthCollection BorderedLength
        {
            get
            {
                int j = 0;
                LengthCollection blen = this.Clone();

                // total = (n*bordersize) + (n*padding*2)
                double total = (this.Count * 1) + (this.Count * CoEx.TableCellPadding * 2) + 1;

                // get space for borders by unsused characters
                var diff = blen.NotFullItems.Select(v => (total * v.NotFullDiffrencePercent / 100)).ToArray();
                foreach (var item in blen.NotFullItems)
                {
                    item.Length -= (int)Math.Round(diff[j]);
                    j++;
                }

                // if not enough steal remain characters from "big" columns
                if(total>diff.Sum())
                {
                    total = total - diff.Sum();
                    diff = blen.BigItems.Select(v => (total * v.BigLengthPercent / 100)).ToArray();
                    j = 0;
                    foreach (var item in blen.BigItems)
                    {
                        item.Length -= (int)Math.Round(diff[j]);
                        j++;
                    }
                }

                return blen;
            }
        }

        public LengthCollection PaddedLength
        {
            get
            {
                int j = 0;
                LengthCollection blen = this.Clone();

                if(blen.Items.Count <= 1)
                {
                    // Return full length if there is only one item in the collection
                    return blen;
                }

                double total = (this.Count * CoEx.ColumnPadding);

                // get space for paddings by unsused characters
                var diff = blen.NotFullItems.Select(v => (total * v.NotFullDiffrencePercent / 100)).ToArray();
                foreach (var item in blen.NotFullItems)
                {
                    item.Length -= (int)Math.Round(diff[j]);
                    j++;
                }

                // if not enough steal remain characters from "big" columns
                if (total > diff.Sum())
                {
                    total = total - diff.Sum();
                    diff = blen.BigItems.Select(v => (total * v.BigLengthPercent / 100)).ToArray();
                    j = 0;
                    foreach (var item in blen.BigItems)
                    {
                        item.Length -= (int)Math.Round(diff[j]);
                        j++;
                    }
                }

                return blen;
            }
        }

        public LengthCollection Clone()
        {
            var c = new LengthCollection();
            this.Items.ForEach(v => c.Import(v.Clone()));
            return c;
        }


    }
}

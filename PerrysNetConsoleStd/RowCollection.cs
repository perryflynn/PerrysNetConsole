﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public class RowCollection
    {

        protected static RowCollectionSettings defaultsettings;
        public static RowCollectionSettings DefaultSettings { 
            get { if (defaultsettings == null) { defaultsettings = new RowCollectionSettings(); } return defaultsettings; } 
            set { defaultsettings = value; } 
        }

        public static RowCollection Create(string[][] data)
        {
            var c = new RowCollection();
            foreach (var item in data)
            {
                c.Import(RowConf.Create(item));
            }
            return c;
        }

        public static RowCollection Create(List<string[]> data)
        {
            return Create(data.ToArray());
        }

        public static RowCollection Create(string[] data)
        {
            return Create(data.Select(v => new string[] { v }).ToArray());
        }

        public static RowCollection Create(RowConf header, params RowConf[] data)
        {
            var c = new RowCollection();
            c.Import(header);
            foreach (var item in data)
            {
                c.Import(item);
            }
            return c;
        }

        public static RowCollection Create(params RowConf[] data)
        {
            var c = new RowCollection();
            foreach (var item in data)
            {
                c.Import(item);
            }
            return c;
        }

        public static RowCollection Create(List<RowConf> data)
        {
            return Create(data.ToArray());
        }

        public static RowCollection Create(RowConf row)
        {
            return Create(new RowConf[] { row });
        }

        public static RowCollection Create()
        {
            return new RowCollection();
        }

        protected RowCollectionSettings settings;
        public RowCollectionSettings Settings {
            get
            {
                return this.settings;
            }
            set
            {
                if (this.settings != null)
                {
                    this.settings.OnHorizontalStretchChanged -= this.OnHorizontalStretchChanged;
                }
                this.settings = value;
                this.settings.OnHorizontalStretchChanged += this.OnHorizontalStretchChanged;
            }
        }

        public List<RowConf> Items { get; private set; }
        public int Count { get { return this.Items.Count; } }

        public bool IsCustomLength { get; set; }

        protected LengthCollection length;
        public LengthCollection Length
        {
            set
            {
                this.length = value;
                this.IsCustomLength = true;
            }
            get
            {
                return this.length;
            }
        }

        protected RowCollection()
        {
            this.Items = new List<RowConf>();
            this.Settings = DefaultSettings.Clone();
        }

        public RowCollection Clone()
        {
            return this.Clone(true);
        }

        public RowCollection Clone(bool includeitems)
        {
            var c = new RowCollection()
            {
                Settings = this.Settings,
                length = this.length,
                IsCustomLength = this.IsCustomLength
            };

            if (includeitems)
            {
                this.Items.ForEach(v => c.Import(v.Clone(false)));
            }

            return c;
        }

        protected void Import(RowConf item, int? index)
        {
            var temp = item.Clone();
            temp.Parent = this;
            if (index.HasValue && index.Value >= 0)
            {
                this.Items.Insert(index.Value, temp);
            }
            else
            {
                this.Items.Add(temp);
            }
            
            if (this.IsCustomLength == false)
            {
                this.length = this.CalcTableLength();
            }
        }

        public RowCollection Import(RowConf item)
        {
            this.Import(item, null);
            return this;
        }

        public RowCollection Import(int index, RowConf item)
        {
            this.Import(item, index);
            return this;
        }

        public RowCollection Import(RowCollection c)
        {
            c.Items.ForEach(v => this.Import(v.Clone()));
            return this;
        }

        public RowCollection Import(RowCollection[] cs)
        {
            this.Import(cs.ToList());
            return this;
        }

        public RowCollection Import(List<RowCollection> cs)
        {
            cs.ForEach(v => this.Import(v));
            return this;
        }
        
        protected void OnHorizontalStretchChanged(bool oldvalue, bool newvalue)
        {
            if (this.IsCustomLength == false)
            {
                this.length = this.CalcTableLength();
            }
        }

        public IEnumerable<RowConf> AsTable()
        {
            var items = this.Clone();
            foreach (var item in items.Items)
            {
                if (item.Index < 1)
                {
                    item.Border.RowMode = BorderConf.ROWMODE.BEGIN;
                }
                else if (item.Index < this.Items.Count)
                {
                    item.Border.RowMode = BorderConf.ROWMODE.CONTINUE;
                }
                else
                {
                    item.Border.RowMode = BorderConf.ROWMODE.END;
                }
                yield return item;
            }
        }

        protected LengthCollection CalcTableLength()
        {
            // Console width
            int maxlength = CoEx.Width;

            // Items
            var items = this.Items.ToArray().ToList();

            // initialize array with one field per column
            LengthCollection collection = new LengthCollection();
            for (int i = 0; i < this.Items.Max(v=>v.Data.Length); i++)
            {
                collection.Import(new LengthConf() { Index = i });
            }

            // iterate rows
            for (int i = 0; i < this.Items.Count; i++)
            {
                // iterate columns
                for (int j = 0; j < this.Items[i].Data.Length; j++)
                {
                    // when length of cell > current length
                    if (this.Items[i].Data[j] != null && this.Items[i].Data[j].Length > (collection.Items[j].Length))
                    {
                        collection.Items[j].Length = collection.Items[j].OriginalLength = this.Items[i].Data[j].Length;
                    }
                }
            }

            // fix overflow by percentage
            if (collection.TotalLength > maxlength)
            {
                // diffrence
                int diff = collection.TotalLength - maxlength;
                foreach (var item in collection.BigItems)
                {
                    // remove i percent of diff in column i if percent>=minperc
                    item.Length -= ((int)Math.Round((diff * item.BigLengthPercent / 100)));
                }
            }
            
            if (this.Settings.StretchHorizontal)
            {
                // add remaining length to last column
                if (collection.TotalLength < maxlength)
                {
                    int diff = maxlength - collection.TotalLength;
                    collection.Items.Last().Length += diff;
                }
            }
            else
            {
                // add length for padding and border to last column
                int borderpadding = (3 * (collection.Items.Count - 1)) + 4;
                if (collection.TotalLength + borderpadding < maxlength)
                {
                    collection.Items.Last().Length += borderpadding;
                }
            }

            return collection;
        }

        public RowCollection SetLength(LengthCollection len)
        {
            this.Length = len;
            return this;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public delegate void RowCollectionSettingsStretchHorizontalChanged(bool oldvalue, bool newvalue);

    public class RowCollectionSettings
    {

        public enum ALIGN { LEFT, CENTER, RIGHT };

        public BorderConf Border { get; set; }
        public Func<RowConf, int, String, ColorScheme> Color { set; get; }
        public Func<RowConf, int, String, bool> IsColorize { set; get; }
        public Func<RowConf, int, String, bool> IsHighlightPadding { set; get; }
        public Func<RowConf, int, String, ALIGN?> Align { set; get; }

        protected bool stretchhorizontal;
        public bool StretchHorizontal { get
            {
                return this.stretchhorizontal;
            }
            set
            {
                var old = this.stretchhorizontal;
                this.stretchhorizontal = value;
                if (this.OnHorizontalStretchChanged != null)
                {
                    OnHorizontalStretchChanged(old, value);
                }
            }
        }

        public event RowCollectionSettingsStretchHorizontalChanged OnHorizontalStretchChanged;

        public RowCollectionSettings()
        {
            this.Border = new BorderConf();
            this.stretchhorizontal = true;
        }

        public RowCollectionSettings Clone()
        {
            return new RowCollectionSettings()
            {
                stretchhorizontal = this.stretchhorizontal,
                Border = this.Border.Clone(),
                Color = this.Color,
                IsColorize = this.IsColorize,
                IsHighlightPadding = this.IsHighlightPadding
            };
        }

    }
}

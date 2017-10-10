using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public class RowCollectionSettings
    {

        public enum ALIGN { LEFT, CENTER, RIGHT };

        public BorderConf Border { get; set; }
        public Func<RowConf, int, String, ColorScheme> Color { set; get; }
        public Func<RowConf, int, String, bool> IsColorize { set; get; }
        public Func<RowConf, int, String, bool> IsHighlightPadding { set; get; }
        public Func<RowConf, int, String, ALIGN?> Align { set; get; }

        public RowCollectionSettings()
        {
            this.Border = new BorderConf();
        }

        public RowCollectionSettings Clone()
        {
            return new RowCollectionSettings()
            {
                Border = this.Border.Clone(),
                Color = this.Color,
                IsColorize = this.IsColorize,
                IsHighlightPadding = this.IsHighlightPadding
            };
        }

    }
}

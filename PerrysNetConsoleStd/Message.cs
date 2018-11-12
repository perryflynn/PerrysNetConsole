using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public class Message
    {

        public enum LEVEL { DEBUG, INFO, WARN, ERROR, SUCCESS };
        public LEVEL Level { get; set; }
        public DateTime Time { get; set; }
        public string Text { get; set; }

        public Message()
        {
            this.Level = LEVEL.INFO;
            this.Time = DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format("({0}) {1}", this.Time.ToString("HH:mm:ss"), this.Text);
        }

    }
}

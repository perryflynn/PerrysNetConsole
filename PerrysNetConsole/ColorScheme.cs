using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public class ColorScheme
    {

        public ConsoleColor? Foreground { get; set; }
        public ConsoleColor? Background { get; set; }

        public ColorScheme(ConsoleColor? background, ConsoleColor? foreground)
        {
            this.Foreground = foreground;
            this.Background = background;
        }

        public ColorScheme() : this(null, null)
        {
        }

        public void ApplyForeground()
        {
            CoEx.SetColor(null, this.Foreground);
        }

        public void ApplyBackground()
        {
            CoEx.SetColor(this.Background, null);
        }

        public void Apply()
        {
            CoEx.SetColor(this.Background, this.Foreground);
        }

        public ColorScheme Clone()
        {
            return new ColorScheme(this.Background, this.Foreground);
        }

        public ColorScheme Invert()
        {
            return new ColorScheme(this.Foreground, this.Background);
        }

    }
}

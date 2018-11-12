using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public class LoadAnimation
    {

        public static string ANIMATIONCOMPLETE { get { return "OK"; } }
        public static char[] ANIMATION { get { return new char[] { '-', '\\', '|', '/' }; } }

        protected int animationindex;
        protected int NextAnimationIndex
        {
            get
            {
                this.animationindex++;
                if (this.animationindex > ANIMATION.Length - 1)
                {
                    this.animationindex = 0;
                }
                return this.animationindex;
            }
        }

        public LoadAnimation()
        {
            this.animationindex = -1;
        }

        public char NextFrame()
        {
            return ANIMATION[this.NextAnimationIndex];
        }
    }
}

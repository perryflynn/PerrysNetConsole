using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerrysNetConsole
{
    public class LoadAnimation
    {

        public static String ANIMATIONCOMPLETE { get { return "OK"; } }
        public static char[] ANIMATION { get { return new char[] { '-', '\\', '|', '/' }; } }
        
        protected int animationindex;
        protected int AnimationIndex
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

        public char NextFrame
        {
            get
            {
                return ANIMATION[this.AnimationIndex];
            }
        }

        public LoadAnimation()
        {
            this.animationindex = -1;
        }

    }
}

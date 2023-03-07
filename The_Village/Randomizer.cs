using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Village
{
    public class Randomizer
    {   
        public virtual int RandomInt()
        {
            Random rnd = new Random();
            int rndInt = rnd.Next(0, 4);

            return rndInt;
        }
    }
}

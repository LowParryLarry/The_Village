using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Village
{
    internal class DatabaseConnection
    {
        internal void Save(Village village)
        {

            village.DaysGone= 0;

            //Daysgone: 0
            //Workerslots: 0 / 6
            //Underconstruction: 0
            //Buildings: 0
            //Workers: 0
            //Food: 10
            //Wood: 0
            //Metal: 0
            //Graveyard: 0


        }
        internal void Load()
        {
            //Load JSON från db => 
        }

    }
}

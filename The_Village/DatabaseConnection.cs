using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Village
{
    public class DatabaseConnection
    {
        //Egna fields?
        //Food
        //Metal
        //List<Worker>
        //osv .. ?


        public virtual void Save()
        {
            //Connect to db, send current village state
        }
        public virtual Village Load(string sqlQuery)
        {            
            //
            Village village = new Village();

            village.DaysGone = 99;
            village.Food = 99;            

            return village;
        }

    }
}

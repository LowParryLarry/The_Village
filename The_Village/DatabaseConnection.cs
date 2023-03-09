using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Village
{
    public class DatabaseConnection
    {
        public virtual void Save(Village village)
        {
            // Saves all game state variables to DB.
        }
        public virtual void Load()
        {
            // Loads from DB
        }

        public virtual int GetWood()
        {
            return 1;
        }
    }
}

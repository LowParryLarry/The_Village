using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Village
{
    public class Building : ICloneable
    {
        private string _name;
        private int _woodCost;
        private int _metalCost;
        private int _daysWorkedOn;
        private int _daysToComplete;
        private bool _isComplete;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int WoodCost
        {
            get { return _woodCost; }
            set { _woodCost = value; }
        }
        public int MetalCost
        {
            get { return _metalCost; }
            set { _metalCost = value; }
        }
        public int DaysToComplete
        {
            get { return _daysToComplete; }
            set { _daysToComplete = value; }
        }
        public int DaysWorkedOn
        {
            get { return _daysWorkedOn; }
            set { _daysWorkedOn = value; }
        }
        public bool IsComplete
        {
            get { return _isComplete; }
            set { _isComplete = value; }
        }



        public Building(string name, int woodCost, int metalCost, int daysToComplete, int daysWorkedOn, bool isComplete)
        {
            _name = name;
            _woodCost = woodCost;
            _metalCost = metalCost;
            _daysToComplete = daysToComplete;            
            _daysWorkedOn = daysWorkedOn;
            _isComplete = isComplete;
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}

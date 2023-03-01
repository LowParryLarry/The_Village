using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Village
{
    public class Worker : ICloneable
    {
        public delegate void DoWorkDelegate();

        private DoWorkDelegate _doWork;
        private string _name, _occupation;
        private bool _isHungry, _isAlive;
        private int _daysHungry;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Occupation
        {
            get { return _occupation; }
            set { _occupation = value; }
        }
        public bool IsHungry
        {
            get { return _isHungry; }
            set { _isHungry = value; }
        }
        public bool IsAlive
        {
            get { return _isAlive; }
            set { _isAlive = value; }
        }
        public int DaysHungry
        {
            get { return _daysHungry; }
            set { _daysHungry = value; }
        }
        public Worker(string name, string occupation, DoWorkDelegate addResourceFunction)
        {
            _name = name;
            _occupation = occupation;
            _isHungry = false;
            _isAlive = true;
            _daysHungry = 0;
            _doWork = addResourceFunction;
        }
        public void DoWork()
        {
            if (IsHungry == false) 
            {
                _doWork();
            }            
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}

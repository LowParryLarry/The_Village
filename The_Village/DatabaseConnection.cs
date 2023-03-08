using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Village
{
    public class DatabaseConnection
    {
        private int _food;
        private int _wood;
        private int _metal;
        private int _daysGone;        
        private List<Worker> _workers = new List<Worker>();
        private List<Worker> _graveyard = new List<Worker>();        
        private List<Building> _underConstruction = new List<Building>();
        private List<Building> _buildings = new List<Building>();
        public int Food 
        {
            get => _food;
            set => _food = value;
        }
        public int Wood
        {
            get => _wood;
            set => _wood = value;
        }
        public int Metal
        {
            get => _metal;
            set => _metal = value;
        }
        public int DaysGone
        {
            get => _daysGone;
            set => _daysGone = value;
        }
        public List<Worker> Workers
        {
            get => _workers;
            set => _workers = value;
        }
        public List<Worker> Graveyard
        {
            get => _graveyard;
            set => _graveyard = value;
        }
        public List<Building> UnderConstruction
        {
            get => _underConstruction;
            set => _underConstruction = value;
        }
        public List<Building> Buildings
        {
            get => _buildings;
            set => _buildings = value;
        }

        public virtual void Save(Village village)
        {
            _food = village.Food;
            _wood = village.Wood;
            _metal = village.Metal;
            _daysGone = village.DaysGone;
            _workers = village.Workers;
            _graveyard = village.Graveyard;
            _underConstruction = village.UnderConstruction;
            _buildings = village.Buildings;
        }
        public virtual Village Load()
        {            
            Village village = new Village();

            village.Food = _food;
            village.Wood = _wood; 
            village.Metal = _metal; 
            village.DaysGone = _daysGone; 
            village.Workers = _workers; 
            village.Graveyard = _graveyard; 
            village.UnderConstruction = _underConstruction;
            village.Buildings = _buildings;

            return village;
        }
    }
}

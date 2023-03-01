using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using The_Village;


namespace The_Village
{
    public class Village
    {
        private int _food, _wood, _metal, _daysGone;
        private List<Worker> _workerList = new List<Worker>();
        private List<Worker> _workers = new List<Worker>();
        private List<Worker> _graveyard = new List<Worker>();
        private List<Building> _buildingList = new List<Building>();
        private List<Building> _underConstruction = new List<Building>();
        private List<Building> _buildings = new List<Building>();
        private DatabaseConnection _dbConnection = new DatabaseConnection();

        public int Food
        {
            get { return _food; }
            set { _food = value; }
        }
        public int Wood
        {
            get { return _wood; }
            set { _wood = value; }
        }
        public int Metal
        {
            get { return _metal; }
            set { _metal = value; }
        }
        public int DaysGone
        {
            get { return _daysGone; }
            set { _daysGone = value; }
        }
        public List<Worker> Workers
        {
            get { return _workers; }
        }
        public List<Worker> WorkerList
        { get { return _workerList; } }
        public List<Building> Buildings 
        {
            get { return _buildings; }
        }
        public List<Building> BuildingList
        {
            get { return _buildingList; }
        }
        public List<Building> UnderConstruction 
        {
            get { return _underConstruction; }
        }
        public List<Worker> Graveyard
        {
            get { return _graveyard; }
        }

        internal DatabaseConnection DbConnection
        {
            get => _dbConnection;
            set => _dbConnection = value;
        }

        public Village()
        {
            VillageSetup();
           
            _food = 10;
            _wood = 0;
            _metal = 0;
            _daysGone = 0;            
        }
        public void VillageSetup()
        {
            for (int i = 0; i < 3; i++)
            {
                _buildings.Add(new Building("House", 5, 0, 3, 3, true));
            }

            _buildingList.Add(new Building("House", 5, 0, 3, 0, false));
            _buildingList.Add(new Building("Farm", 5, 2, 5, 0, false));
            _buildingList.Add(new Building("Woodmill", 5, 1, 5, 0, false));
            _buildingList.Add(new Building("Quarry", 3, 5, 7, 0, false));
            _buildingList.Add(new Building("Castle", 50, 50, 50, 0, false));

            _workerList.Add(new Worker("Worker", "Lumberjack", AddWood));
            _workerList.Add(new Worker("Worker", "Miner", AddMetal));
            _workerList.Add(new Worker("Worker", "Farmer", AddFood));
            _workerList.Add(new Worker("Worker", "Builder", Build));
        }
        public void Day()
        {
            FeedWorkers();

            foreach (Worker worker in _workers.ToList())
            {
                worker.DoWork();
            }

            BuryDead();

            DaysGone += 1;            
        }
        public void AddWorker(int workerChoice)
        {
            if (IsNewWorkerAllowed())
            {
                Worker fromList = _workerList[workerChoice];
                Worker newInstance = (Worker)fromList.Clone();

                _workers.Add(newInstance);
            }
        }   
        public void AddWood()
        {
            if (BuildingExists("Woodmill", _buildings))
            {
                _wood += 3; 
            }
            else
            {
                _wood += 1;
            }
        } 
        public void AddMetal()
        {
            if (BuildingExists("Quarry", _buildings))
            {
                _metal += 3;
            }
            else
            {
                _metal += 1;
            }
        }
        public void AddFood() 
        {
            if (BuildingExists("Farm", _buildings))
            {
                _food += 15;
            }
            else
            {
                _food += 5;
            }
        }
        public void Build()
        {   
            if (_underConstruction.Count > 0)
            {
                _underConstruction[0].DaysWorkedOn++;

                foreach (Building building in _underConstruction.ToList())
                {
                    if (building.DaysWorkedOn == building.DaysToComplete)
                    {
                        building.IsComplete = true;
                        _buildings.Add(building);
                        _underConstruction.Remove(building);
                    }                    
                }
            }
        }
        public void AddConstruction(int constructionChoice)
        {   
            if (ResourceCalculation(_buildingList[constructionChoice]))
            {
                Building fromList = _buildingList[constructionChoice];
                Building newInstance = (Building)fromList.Clone();

                _underConstruction.Add(newInstance);
            }
        }
        public bool ResourceCalculation(Building buildingToCheck)
        {
            if (_wood >= buildingToCheck.WoodCost && _metal >= buildingToCheck.MetalCost)
            {
                _wood -= buildingToCheck.WoodCost;
                _metal -= buildingToCheck.MetalCost;
                return true;
            }

            return false;
        }
        public bool IsNewWorkerAllowed()
        {
            if (TotalWorkerSlots() - _workers.Count > 0)
            {
                return true;
            }
            
            return false;
        }
        public void BuryDead()
        {
            foreach (Worker worker in _workers.ToList())
            {
                if (worker.IsAlive == false)
                {
                    _workers.Remove(worker);
                    _graveyard.Add(worker);
                }
            }
        }
        public void FeedWorkers()
        {
            foreach (Worker worker in Workers)
            {
                if (Food > 0)
                {
                    Food--;
                }
                else
                {
                    worker.IsHungry = true;
                    worker.DaysHungry += 1;
                }                

                KillWorkerCheck(worker);                                
            }          
        }
        public void KillWorkerCheck(Worker worker)
        {
            if (worker.DaysHungry >= 40)
            {
                worker.IsAlive = false;
            }

        }
        public bool BuildingExists(string buildingToCheck, List<Building> buildingList)
        {
            foreach (Building building in buildingList)
            {
                if (building.Name == buildingToCheck)
                {
                    return true;
                }
            }
            return false;
        }
        public int TotalWorkerSlots()
        {
            int totalWorkerSlots = 0;

            foreach (Building building in _buildings)
            {
                if (building.Name == "House")
                {
                    totalWorkerSlots += 2;
                }
            }
            return totalWorkerSlots;
        }
        public void PrintVillageStats()
        {            
            int left = 4;            

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($@"
╔═══════════════════════════╗
║                           ║  
║                           ║
║                           ║ 
║                           ║
║                           ║
║                           ║
║                           ║
║                           ║
║                           ║
║                           ║
║                           ║
╠═══════════════════════════╣
║                           ║
║                           ║
║                           ║
║                           ║
║                           ║
║                           ║
║                           ║
║                           ║
╚═══════════════════════════╝
");

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(left, 3); 
            Console.WriteLine($"Days gone: {_daysGone}");
            Console.SetCursorPosition(left, 4);
            Console.WriteLine($"Worker slots: {_workers.Count()}/{TotalWorkerSlots()}");
            Console.SetCursorPosition(left, 5);
            Console.WriteLine($"Under construction: {_underConstruction.Count}");
            Console.SetCursorPosition(left, 6);
            Console.WriteLine($"Buildings: {_buildings.Count - 3}");
            Console.SetCursorPosition(left, 7);
            Console.WriteLine($"Workers: {_workers.Count}");
            Console.SetCursorPosition(left, 8);
            Console.WriteLine($"Food: {_food}");
            Console.SetCursorPosition(left, 9);
            Console.WriteLine($"Wood: {_wood}");
            Console.SetCursorPosition(left, 10);
            Console.WriteLine($"Metal: {_metal}");
            Console.SetCursorPosition(left, 11);
            Console.WriteLine($"Graveyard: {_graveyard.Count}");
            Console.SetCursorPosition(0,0);

        }
        public void ClearBottomBox()
        {
            Console.SetCursorPosition(1, 15);
            Console.WriteLine("                           ");
            Console.SetCursorPosition(1, 16);
            Console.WriteLine("                           ");
            Console.SetCursorPosition(1, 17);
            Console.WriteLine("                           ");
            Console.SetCursorPosition(1, 18);
            Console.WriteLine("                           ");

        }
        public void PrintWorkers()
        {
            foreach (Worker worker in _workers)
            {
                Console.WriteLine($"Occupation: {worker.Occupation}, IsHungry: {worker.IsHungry}, DaysHungry: {worker.DaysHungry}, {worker.GetHashCode()}");
            }
        }        
        public void SaveProgress() //Ska implementeras
        {

        }
        public void LoadProgress() //Ska implementeras
        {

        }
    }
}

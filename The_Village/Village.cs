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
        private Randomizer _randomizer;
        private DatabaseConnection _dbConnection;      
        private List<Worker> _workerList = new List<Worker>();
        private List<Worker> _workers = new List<Worker>();
        private List<Worker> _graveyard = new List<Worker>();
        private List<Building> _buildingList = new List<Building>();
        private List<Building> _underConstruction = new List<Building>();
        private List<Building> _buildings = new List<Building>();
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
            set { _workers = value; }
        }
        public List<Worker> WorkerList
        {
            get { return _workerList; }
            set { _workerList = value; }
        }
        public List<Building> Buildings 
        {
            get { return _buildings; }
            set { _buildings = value; }
        }
        public List<Building> BuildingList
        {
            get { return _buildingList; }
            set { _buildingList = value; }
        }
        public List<Building> UnderConstruction 
        {
            get { return _underConstruction; }
            set { _underConstruction = value; }
        }
        public List<Worker> Graveyard
        {
            get { return _graveyard; }
            set { _graveyard = value; }
        }
        public DatabaseConnection DBConnection
        {
            get => _dbConnection;
            set => _dbConnection = value;
        }
        public Randomizer Randomizer
        {
            get => _randomizer;
            set => _randomizer = value;
        }
        public Village()
        {
            VillageSetup();
           
            _food = 10;
            _wood = 0;
            _metal = 0;
            _daysGone = 0;
            _dbConnection = new DatabaseConnection();
            _randomizer = new Randomizer();
        }
        public void VillageSetup()
        {
            //Village starts with 3 houses.
            for (int i = 0; i < 3; i++)
            {
                _buildings.Add(new Building("House", 5, 0, 3, 3, true));
            }

            //Reference list for adding buildings
            _buildingList.Add(new Building("House", 5, 0, 3, 0, false));
            _buildingList.Add(new Building("Farm", 5, 2, 5, 0, false));
            _buildingList.Add(new Building("Woodmill", 5, 1, 5, 0, false));
            _buildingList.Add(new Building("Quarry", 3, 5, 7, 0, false));
            _buildingList.Add(new Building("Castle", 50, 50, 50, 0, false));

            //Reference list for adding workers
            _workerList.Add(new Worker("Worker", "Lumberjack", AddWood));
            _workerList.Add(new Worker("Worker", "Miner", AddMetal));
            _workerList.Add(new Worker("Worker", "Farmer", AddFood));
            _workerList.Add(new Worker("Worker", "Builder", Build));
        }
        public void Day()
        {
            FeedWorkers();

            WorkersDoWork();

            BuryDead();

            DaysGone += 1;            
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
            /*
            Takes the first building under construction and increase DaysWorkedOn.
            If DaysWorkedOn == DaysToComplete the building is moved from UnderConstruction
            to Buildings and IsComplete tag is set to true.

            This function is used by the Worker class through a delegate.
             */

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
        public bool IsNewWorkerAllowed()
        {
            //Checks if a new worker is allowed to be added.            

            if (TotalWorkerSlots() - _workers.Count > 0)
            {
                return true;
            }
            
            return false;
        }
        public void AddWorker(int workerChoice)
        {
            /*
            Adds a worker from reference list _workerList. The Clone() method comes
            from the interface ICloneable. It makes it possible to copy an object
            and create a new instance in _workers.
            */

            if (IsNewWorkerAllowed())
            {
                Worker fromList = _workerList[workerChoice];
                Worker newInstance = (Worker)fromList.Clone();

                _workers.Add(newInstance);
            }
        }   
        public bool ResourceManager(Building buildingToCheck)
        {
            /*
            Checks if village can afford the specified building. If so,
            the resources are subtracted from the village instance.
            */

            if (_wood >= buildingToCheck.WoodCost && _metal >= buildingToCheck.MetalCost)
            {
                _wood -= buildingToCheck.WoodCost;
                _metal -= buildingToCheck.MetalCost;
                return true;
            }
            return false;
        }
        public void AddConstruction(int constructionChoice)
        {
            /*
            Adds a building from reference list _buildingList. The Clone() method comes
            from the interface ICloneable. It makes it possible to copy an object
            and create a new instance in _underConstruction.
            */

            if (ResourceManager(_buildingList[constructionChoice]))
            {
                Building fromList = _buildingList[constructionChoice];
                Building newInstance = (Building)fromList.Clone();

                _underConstruction.Add(newInstance);
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
        public void WorkersDoWork()
        {
            /*
            DoWork() holds a delegate containing AddWood(), AddMetal(), AddFood(), Build()
            depending on the type of worker.
            */

            foreach (Worker worker in _workers.ToList())
            {
                worker.DoWork();
            }
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
            /*
            Checks total workerslots. Number of houses * 2 = total worker slots.
            */

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
        public void SaveProgress() 
        {
            _dbConnection.Save(this);
        }
        public void LoadProgress() 
        {
            var output = _dbConnection.Load();                        

            _food = output._food;
            _wood = output._wood;
            _metal = output._metal;
            _daysGone = output._daysGone;                                  
            _workers = output._workers;
            _graveyard = output._graveyard;            
            _underConstruction = output._underConstruction;
            _buildings = output._buildings;            
        }
        public void AddRandomWorker()
        {
            AddWorker(_randomizer.RandomInt());
        }        
    }
}

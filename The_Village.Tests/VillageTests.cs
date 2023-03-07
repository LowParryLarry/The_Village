using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Xml.Linq;
namespace The_Village.Tests
{
    /*
    MethodName_StateUnderTest_ExpectedBehavior
    There are arguments against this strategy that if method names
    change as part of code refactoring than test name like this should
    also change or it becomes difficult to comprehend at a later stage.
    
    isAdult_AgeLessThan18_False
    withdrawMoney_InvalidAccount_ExceptionThrown
    admitStudent_MissingMandatoryFields_FailToAdmit
    https://dzone.com/articles/7-popular-unit-test-naming
    https://xunit.net/docs/shared-context
    */

    /*
    Building indices:
    0 = House
    1 = Farm
    2 = Woodmill
    3 = Quarry
    4 = Castle
    
    Worker indices:
    0 = Lumberjack, AddWood
    1 = Miner, AddMetal
    2 = Farmer, AddFood
    3 = Builder, Build
    */
    public class VillageTests
    {
        private void AddWorkerIterator(int workerIndex, int iterations, Village village)
        {
            for (int i = 0; i < iterations; i++)
            {
                village.AddWorker(workerIndex);
            }
        }
        private void DayIterator(int iterations, Village village)
        {
            for (int i = 0; i < iterations; i++)
            {
                village.Day();
            }
        }
        
        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 3)]
        public void AddWorker_AddThreeWorkers_ListCountThree(int iterations, int expected)
        {
            //Arrange
            Village village = new Village();
            
            //Act
            AddWorkerIterator(0, iterations, village); //Lumberjack

            //Assert
            Assert.Equal(expected, village.Workers.Count);         
        }

        [Theory]
        [InlineData(10, 6)]
        [InlineData(20, 6)]        
        public void AddWorker_NotEnoughHouses_FailToAdd(int iterator, int expected)
        {
            //Arrange            
            Village village = new Village();

            //Act
            AddWorkerIterator(0, iterator, village);

            //Assert
            Assert.Equal(expected, village.Workers.Count);
        }
        
        [Theory]
        [InlineData(1, 10, 50)]
        [InlineData(4, 20, 330)]
        public void AddWorker_RunningDay_WorkerShouldAddResource(int workerIterations, int dayIterations, int expected)
        {
            //Arrange            
            Village village = new Village();            

            //Act
            AddWorkerIterator(2, workerIterations, village);
            DayIterator(dayIterations, village);

            //Assert
            Assert.Equal(expected, village.Food);
        }

        [Fact]
        public void Day_RunDayWithoutWorkers_DaysPass()
        {
            /*
            Win and loose conditions are held by Game class. Hence the village can 
            go to 10 days without workers.
             */

            //Arrange            
            Village village = new Village();
            int expected = 10;

            //Act
            DayIterator(10, village);

            //Assert
            Assert.Equal(expected, village.DaysGone);
        }

        [Theory]
        [InlineData(2, 2, 2, 2, 14)]
        [InlineData(10, 10, 10, 10, 30)]
        public void Day_RunDayWithWorkersAndWithFood_ShouldDoWorkAndFeed(int dayIterations, int expectedDays, int expectedWood, int expectedMetal, int expectedFood)
        {
            //Arrange            
            Village village = new Village();
            
            village.AddWorker(0);
            village.AddWorker(1);
            village.AddWorker(2);


            //Act
            DayIterator(dayIterations, village);

            //Assert
            Assert.Equal(expectedDays, village.DaysGone);
            Assert.Equal(expectedWood, village.Wood);
            Assert.Equal(expectedMetal, village.Metal);
            Assert.Equal(expectedFood, village.Food);
        }

        [Theory]
        [InlineData(41,6,2)]
        [InlineData(42, 6, 6)]
        public void Day_RunDayWithWorkersAndWithoutFood_WorkersDie(int dayIterations, int workerIterations, int expectedInGraveyard)
        {
            /*
            Day 1: All workers feed. Food 4.
            Day 2: Two workers become hungry.
            Day 3: Remaining workers become hungry.
            ...
            Day 41: Two workers die.
            Day 42: Remaining workers die.             
             */

            //Arrange            
            Village village = new Village();                       
            
            AddWorkerIterator(0, workerIterations, village);
                        
            //Act
            DayIterator(dayIterations, village);

            //Assert
            Assert.Equal(expectedInGraveyard, village.Graveyard.Count);
        }

        [Fact]
        public void AddConstruction_HavingEnoughResources_BuildingAdded()
        {
            //Arrange            
            Village village = new Village();
            village.Wood = 5;
            village.Metal = 2;            

            //Act
            village.AddConstruction(1); //Farm

            int expectedWood = 0;
            int expectedMetal = 0;
            int expectedUnderConstructionCount = 1;

            //Assert
            Assert.Equal(expectedWood, village.Wood);
            Assert.Equal(expectedMetal, village.Metal);
            Assert.Equal(expectedUnderConstructionCount, village.UnderConstruction.Count);

        }

        [Fact]
        public void AddConstruction_NotHavingEnoughResources_BuildingNotAdded()
        {
            //Arrange            
            Village village = new Village();

            //Act
            village.AddConstruction(4);
            
            int expectedWood = 0;
            int expectedMetal = 0;
            int expectedUnderConstructionCount = 0;

            //Assert
            Assert.Equal(expectedWood, village.Wood);
            Assert.Equal(expectedMetal, village.Metal);
            Assert.Equal(expectedUnderConstructionCount, village.UnderConstruction.Count);
        }

        [Fact]
        public void AddConstruction_WoodmillAdded_HigerWoodrate()
        {
            /*
            Workers work in the order they are added. If the builder is not first
            in the workers list, the new gathering rate will kick in one day later.
            */

            //Arrange
            Village village = new Village();

            //Act
            village.AddWorker(3); //Builder
            village.AddWorker(0); //Lumberjack
            village.AddWorker(1); //Miner
            village.AddWorker(2); //Farmer

            DayIterator(5, village); //5 Wood
            
            village.AddConstruction(2); //Woodmill, 0 Wood
            
            DayIterator(4, village); 

            int expetctedWoodPreWoodmill = 4;
            int actualWoodPreWoodmill = village.Wood;

            village.Day();

            int expectedWoodPostWoodmill = 7;
            int actualWoodPostWoodmill = village.Wood;
            
            //Assert
            Assert.Equal(expetctedWoodPreWoodmill, actualWoodPreWoodmill);
            Assert.Equal(expectedWoodPostWoodmill, actualWoodPostWoodmill);
        }

        [Fact]
        public void AddConstruction_Farm_HigerFoodRate()
        {
            /*
            Workers work in the order they are added. If the builder is not first
            in the workers list, the new gathering rate will kick in one day later.
            */

            //Arrange
            Village village = new Village();

            //Act
            village.AddWorker(3); //Builder
            village.AddWorker(0); //Lumberjack
            village.AddWorker(1); //Miner
            village.AddWorker(2); //Farmer

            DayIterator(5, village); // +5 food, 15 food

            village.AddConstruction(1); //Farm, 15 Food

            DayIterator(4, village); //+4 food, 19 food

            int expetctedFoodPreFarm = 19;
            int actualFoodPreFarm = village.Food;

            village.Day(); //Farm finnishes, 19 - 4 + 15 = 30 food

            int expetctedFoodPostFarm = 30;
            int actualFoodPostFarm = village.Food;

            //Assert
            Assert.Equal(expetctedFoodPreFarm, actualFoodPreFarm);
            Assert.Equal(expetctedFoodPostFarm, actualFoodPostFarm);
        }

        [Fact]
        public void AddConstruction_Quarry_HigerMetalRate()
        {
            /*
            Workers work in the order they are added. If the builder is not first
            in the workers list, the new gathering rate will kick in one day later.
            */

            //Arrange
            Village village = new Village();

            village.AddWorker(3); //Builder
            village.AddWorker(0); //Lumberjack
            village.AddWorker(1); //Miner
            village.AddWorker(2); //Farmer
            
            //Act

            DayIterator(5, village); //+5 Metal

            village.AddConstruction(3); //Quarry, -5 Metal

            DayIterator(6, village); // One miner, 6 days, +6 Metal

            int expetctedMetalPreQuarry = 6;
            int actualMetalPreQuarry = village.Metal;

            village.Day(); //Quarry finnishes, 6 + 3 = 9 metal 

            int expetctedMetalPostQuarry = 9;
            int actualMetalPostQuarry = village.Metal;

            //Assert
            Assert.Equal(expetctedMetalPreQuarry, actualMetalPreQuarry);
            Assert.Equal(expetctedMetalPostQuarry, actualMetalPostQuarry);
        }

        [Fact]
        public void AddConstruction_DoSomeWork_FinnishedByCorrectDay()
        {
            //Arrange
            Village village = new Village();

            //Act
            village.AddWorker(0); //Lumberjack
            village.AddWorker(1); //Miner
            village.AddWorker(2); //Farmer
            village.AddWorker(3); //Builder

            DayIterator(50, village);

            village.AddConstruction(4); //Castle

            DayIterator(49, village);

            int expectedBuildingsPre = 3; //Village starts with three houses
            int actualBuildingsPre = village.Buildings.Count();

            DayIterator(1, village); //Castle finnished

            int expectedBuildingsPost = 4;
            int actualBuildingsPost = village.Buildings.Count();
            var actualObject = village.Buildings.Last();

            //Assert
            Assert.Equal(expectedBuildingsPre, actualBuildingsPre);
            Assert.Equal(expectedBuildingsPost, actualBuildingsPost);
            Assert.Equal("Castle", actualObject.Name);
            Assert.True(actualObject.IsComplete);
        }

        [Fact]
        public void DoWork_DoNotFeedWorkers_ShouldNotWork()
        {
            //Arrange
            Village village = new Village();

            //Act
            AddWorkerIterator(0, 5, village); //5x Lumberjack

            DayIterator(2, village); //-10 food, +10 wood

            int expwctedFood = 0;
            int actualFood = village.Food;

            int expectedWoodPre = 10;
            int actualWoodPre = village.Wood;

            DayIterator(1, village); //All workers hungry, should not work

            int expectedWoodPost = 10;
            int actualWoodPost = village.Wood;

            //Assert
            Assert.Equal(expwctedFood, actualFood);
            Assert.Equal(expectedWoodPre, actualWoodPre);
            Assert.Equal(expectedWoodPost, actualWoodPost);
        }        

        [Fact]
        public void BuryDead_Passdays_WorkerDies()
        {
            //2.

            //Arrange
            Village village = new Village();

            //Act
            village.AddWorker(0); //Lumberjack

            DayIterator(49, village);

            int expectedWorkerCountPre = 1;
            int actualWorkerCountPre = village.Workers.Count;

            int expectedGraveyardCountPre = 0;
            int actualGraveyardCountPre = village.Graveyard.Count;

            DayIterator(1, village); //Lumberjack dies on day 50, should be moved to graveyard

            int expectedWorkerCountPost = 0;
            int actualWorkerCountPost = village.Workers.Count;

            int expectedGraveyardCountPost = 1;
            int actualGraveyardCountPost = village.Graveyard.Count;

            //Assert

            Assert.Equal(expectedWorkerCountPre, actualWorkerCountPre);
            Assert.Equal(expectedGraveyardCountPre, actualGraveyardCountPre);
            
            Assert.Equal(expectedWorkerCountPost, actualWorkerCountPost);
            Assert.Equal(expectedGraveyardCountPost, actualGraveyardCountPost);

        }

        //Kan kolla BuryDead
        
        /*         
            Direkt när en worker dör så flyttas den till graveyard och säts till false, efter 40 dagar som
            hungrig. Jag skrev koden så innan jag kommit till test-delen mest för jag tyckte det var roligt
            att ha en graveyard, inte för att underlätta testet.
        
            Dessa test svaras på i funktionen ovan.

            3.  Det skall inte gå att mata en arbetare med alive = false
                I min kod flyttas en worker direkt till graveyard när den dör.
            
            4.  Kolla att BuryDead() funktionen tar bort de som har alive = false
                I min kod flyttas en worker direkt till graveyard när den dör.
        */

        [Fact]
        public void WinGame_GatherResourcesAndBuildCastle_CastleExistsAndGameEnds()
        {
            //Arrange
            Village village = new Village();

            //Act
            village.AddWorker(0); //Lumberjack
            village.AddWorker(1); //Miner
            village.AddWorker(2); //Farmer
            village.AddWorker(3); //Builder

            DayIterator(50, village); //Generate resources for Castle

            village.AddConstruction(4); //Add Castle

            DayIterator(50, village); //Wait for Castle to be built

            int expectedDays = 100;
            int actualDays = village.DaysGone;
            
            var actualObject = village.Buildings.Last();

            //Assert
            Assert.Equal(expectedDays, actualDays);
            Assert.True(actualObject.IsComplete);
            Assert.Equal("Castle", actualObject.Name);
        }

        [Fact]
        public void LoadProgress_GetDataFromDB_ShouldInsertDataToCurrentGame()
        {            
            //Arrange    
            Mock<DatabaseConnection>dbcMock = new Mock<DatabaseConnection>();            
            Village village = new Village();
            village.DBConnection = dbcMock.Object;
            
            dbcMock
                .Setup(mock => mock.Load("select * from Village"))
                .Returns(MockVillage());
                                    
            //Act
            village.LoadProgress();

            int actual = village.Food;
            int expected = 33;

            //Assert            
            Assert.Equal(expected, actual);
        }
        private Village MockVillage()
        {
            Village village = new Village();

            village.Food = 33;

            return village;
        }

        [Fact]
        public void AddRandomWorker_RunDay_ShouldGatherWood()
        {
            //Arrange
            Mock<Randomizer>randMock = new Mock<Randomizer>();
            Village village = new Village();
            village.Randomizer = randMock.Object;

            randMock
                .Setup(mock => mock.RandomInt())
                .Returns(0); //Lumberjack

            //Act
            village.AddRandomWorker();
            village.Day();

            string expectedOccupation = "Lumberjack";
            string actualOccupation = village.Workers.First().Occupation;

            int expectedWood = 1;
            int actualWood = village.Wood;

            //Assert
            Assert.Equal(expectedWood, actualWood);
            Assert.Equal(expectedOccupation, actualOccupation);
        }
    }
}
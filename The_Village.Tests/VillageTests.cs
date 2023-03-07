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
    //MethodName_StateUnderTest_ExpectedBehavior
    //There are arguments against this strategy that if method names
    //change as part of code refactoring than test name like this should
    //also change or it becomes difficult to comprehend at a later stage.
    //
    //isAdult_AgeLessThan18_False
    //withdrawMoney_InvalidAccount_ExceptionThrown
    //admitStudent_MissingMandatoryFields_FailToAdmit
    //https://dzone.com/articles/7-popular-unit-test-naming


    //https://xunit.net/docs/shared-context

    //Building indices:
    //0 = House
    //1 = Farm
    //2 = Woodmill
    //3 = Quarry
    //4 = Castle
    //
    //Worker indices:
    //0 = Lumberjack, AddWood
    //1 = Miner, AddMetal
    //2 = Farmer, AddFood
    //3 = Builder, Build

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
            //AddWorker() 1.

            //Arrange
            Village village = new Village();
            
            //Act
            AddWorkerIterator(0, iterations, village);

            //Assert
            Assert.Equal(expected, village.Workers.Count);         
        }

        [Theory]
        [InlineData(10, 6)]
        [InlineData(20, 6)]        
        public void AddWorker_NotEnoughHouses_FailToAdd(int iterator, int expected)
        {
            //AddWorker() 2.

            //Arrange            
            Village village = new Village();
            //int expected = 6;

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
            //AddWorker() 3.

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
            //Day() 1.

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
            //Day() 2.

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
            //Day() 3.

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
            //AddConstruction() 1.

            //Arrange            
            Village village = new Village();
            village.Wood = 5;
            village.Metal = 2;

            int expectedWood = 0;
            int expectedMetal = 0;
            int expectedCount = 1;

            //Act
            village.AddConstruction(1);

            //Assert
            Assert.Equal(expectedWood, village.Wood);
            Assert.Equal(expectedMetal, village.Metal);
            Assert.Equal(expectedCount, village.UnderConstruction.Count);

        }

        [Fact]
        public void AddConstruction_NotHavingEnoughResources_BuildingNotAdded()
        {
            //AddConstruction() 2.

            //Arrange            
            Village village = new Village();

            int expectedWood = 0;
            int expectedMetal = 0;
            int expectedUnderConstructionCount = 0;

            //Act
            village.AddConstruction(4);

            //Assert
            Assert.Equal(expectedWood, village.Wood);
            Assert.Equal(expectedMetal, village.Metal);
            Assert.Equal(expectedUnderConstructionCount, village.UnderConstruction.Count);
        }

        [Fact]
        public void AddConstruction_WoodmillAdded_HigerWoodrate()
        {
            //Upgrades 1.

            /* Indices:
            Buildings:
            0 = House, 5, 0, 3
            1 = Farm, 5, 2, 5
            2 = Woodmill, 5, 1, 5
            3 = Quarry, 3, 5, 7
            4 = Castle, 50, 50, 50
            
            Workers:
            0 = Lumberjack, AddWood
            1 = Miner, AddMetal
            2 = Farmer, AddFood
            3 = Builder, Build
            */

            //Arrange
            Village village = new Village();

            //Act
            /*
             * Om jag ska följa uppgiften exakt så måste en builder läggas till först.
             * Detta eftersom workers arbetar i den ordning de ligger i listan. Om
             * builder ligger efter lumberjack kommer uppgraderingen gälla först
             * dagen efter.
             */

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
            //Upgrades 2.1

            /* Indices:
            Buildings:
            0 = House, 5, 0, 3
            1 = Farm, 5, 2, 5
            2 = Woodmill, 5, 1, 5
            3 = Quarry, 3, 5, 7
            4 = Castle, 50, 50, 50
            
            Workers:
            0 = Lumberjack, AddWood
            1 = Miner, AddMetal
            2 = Farmer, AddFood
            3 = Builder, Build
            */

            //Arrange
            Village village = new Village();

            //Act
            village.AddWorker(3); //Builder
            village.AddWorker(0); //Lumberjack
            village.AddWorker(1); //Miner
            village.AddWorker(2); //Farmer

            DayIterator(5, village); // 15 Food

            village.AddConstruction(1); //Farm, 15 Food

            DayIterator(4, village); 

            int expetctedFoodPreFarm = 19;
            int actualFoodPreFarm = village.Food;

            village.Day();

            int expetctedFoodPostFarm = 30;
            int actualFoodPostFarm = village.Food;

            //Assert
            Assert.Equal(expetctedFoodPreFarm, actualFoodPreFarm);
            Assert.Equal(expetctedFoodPostFarm, actualFoodPostFarm);
        }

        [Fact]
        public void AddConstruction_Quarry_HigerMetalRate()
        {
            //Upgrades 2.2

            /* Indices:
            Buildings:
            0 = House, 5, 0, 3
            1 = Farm, 5, 2, 5
            2 = Woodmill, 5, 1, 5
            3 = Quarry, 3, 5, 7
            4 = Castle, 50, 50, 50
            
            Workers:
            0 = Lumberjack, AddWood
            1 = Miner, AddMetal
            2 = Farmer, AddFood
            3 = Builder, Build
            */

            //Arrange
            Village village = new Village();

            village.AddWorker(3); //Builder
            village.AddWorker(0); //Lumberjack
            village.AddWorker(1); //Miner
            village.AddWorker(2); //Farmer
            
            //Act

            DayIterator(5, village); // 5 Metal

            village.AddConstruction(3); //Quarry, 0 Metal

            DayIterator(6, village); // 6 Metal

            int expetctedMetalPreQuarry = 6;
            int actualMetalPreQuarry = village.Metal;

            village.Day();

            int expetctedMetalPostQuarry = 9;
            int actualMetalPostQuarry = village.Metal;

            //Assert
            Assert.Equal(expetctedMetalPreQuarry, actualMetalPreQuarry);
            Assert.Equal(expetctedMetalPostQuarry, actualMetalPostQuarry);
        }

        [Fact]
        public void AddConstruction_DoSomeWork_FinnishedByCorrectDay()
        {
            //3.

            /* Indices:
            Buildings:
            0 = House, 5, 0, 3
            1 = Farm, 5, 2, 5
            2 = Woodmill, 5, 1, 5
            3 = Quarry, 3, 5, 7
            4 = Castle, 50, 50, 50
            
            Workers:
            0 = Lumberjack, AddWood
            1 = Miner, AddMetal
            2 = Farmer, AddFood
            3 = Builder, Build
            */

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

            int expectedCastleInBuildingsPre = 3;
            int actualCastleInBuildingsPre = village.Buildings.Count();

            DayIterator(1, village);

            int expectedCastleInBuildingsPost = 4;
            int actualCastleInBuildingsPost = village.Buildings.Count();
            var actualObject = village.Buildings.Last();

            //Assert
            Assert.Equal(expectedCastleInBuildingsPre, actualCastleInBuildingsPre);
            Assert.Equal(expectedCastleInBuildingsPost, actualCastleInBuildingsPost);
            Assert.Equal("Castle", actualObject.Name);
            Assert.True(actualObject.IsComplete);
        }

        [Fact]
        public void DoWork_DoNotFeedWorkers_ShouldNotWork()
        {
            //1.

            //Arrange
            Village village = new Village();

            //Act
            AddWorkerIterator(0, 5, village); //5x Lumberjack

            DayIterator(2, village);

            int expwctedFood = 0;
            int actualFood = village.Food;

            int expectedWoodPre = 10;
            int actualWoodPre = village.Wood;

            DayIterator(1, village);

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

            DayIterator(1, village);

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
            /* Indices:
            Buildings:
            0 = House, 5, 0, 3
            1 = Farm, 5, 2, 5
            2 = Woodmill, 5, 1, 5
            3 = Quarry, 3, 5, 7
            4 = Castle, 50, 50, 50
            
            Workers:
            0 = Lumberjack, AddWood
            1 = Miner, AddMetal
            2 = Farmer, AddFood
            3 = Builder, Build
            */

            //Arrange
            Village village = new Village();

            //Act
            village.AddWorker(0);
            village.AddWorker(1);
            village.AddWorker(2);
            village.AddWorker(3);

            DayIterator(50, village);

            village.AddConstruction(4);

            DayIterator(50, village);

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

            Village village = new Village(dbcMock.Object);

            dbcMock
                .Setup(mock => mock.Load("select * from Village"))
                .Returns(MockVillage());

            Village expected = MockVillage();

            //Act
            Village actual = village.LoadProgress();

            //Assert
            Assert.True(actual != null);
            Assert.Equal(expected.Food, actual.Food); //11 11
        }
        private Village MockVillage()
        {
            Village village = new Village();

            village.DaysGone = 11;
            village.Food = 11;

            return village;
        }

        [Fact]
        public void AddRandomWorker_DoWork_ShouldDoCorrectWork()
        {
            //Arrange
            Mock<Randomizer>randMock = new Mock<Randomizer>();            

            Village village = new Village();
            village.Randomizer = randMock.Object;



            randMock
                .Setup(mock => mock.RandomInt())
                .Returns(0);

            village.AddRandomWorker();
            village.Day();




            //Act



            //Assert
            Assert.Equal(1, village.Wood);
            Assert.Equal("Lumberjack", village.Workers.First().Occupation);



        }
















    }
}
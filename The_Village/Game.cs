using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace The_Village
{
    internal class Game
    {
        private Village _village;        
        public Game()
        {
            _village = new Village();
        }
        public Village Village
        {
            get { return _village; }
            set { _village = value; }
        }
        public void Run()
        {
            while (true)
            {
                MainMenu();

                if (YouWinCheck() || YouLooseCheck())
                {
                    break;
                }               
            }

            EndOfGame();
            
            PlayAgain();
        }
        public int ReadIntInstantly(int max)
        {
            /*
            Reads keyboardpresses only if it is int and instantly returns that int. 
            The max parameter sets the highest choice possible for the current menu.
            */

            while (true)
            {
                ConsoleKeyInfo input = Console.ReadKey(true);
                                
                if (char.IsDigit(input.KeyChar))
                {
                    int inputToInt = int.Parse(input.KeyChar.ToString());
                                        
                    if (inputToInt <= max && inputToInt != 0)
                    {
                        return inputToInt;
                    }
                }
            }            
        }
        public void MainMenu()
        {
            Console.CursorVisible = false;

            Console.Clear();
                        
            PrintVillageStats();            
            
            Console.SetCursorPosition(4,15);
            Console.WriteLine("1. Add a worker");
            
            Console.SetCursorPosition(4, 16);
            Console.WriteLine("2. Add a building");
            
            Console.SetCursorPosition(4, 17);
            Console.WriteLine("3. Pass time");

            Console.SetCursorPosition(4, 18);
            Console.WriteLine("4. Save game");

            Console.SetCursorPosition(4, 19);
            Console.WriteLine("5. Load game");

            switch (ReadIntInstantly(5) - 1)
            {
                case 0:
                    WorkerMenu();
                    break;
                case 1:
                    BuildingMenu();
                    break;
                case 2:
                    PassTimeMenu();
                    break;
                case 3:
                    Village.SaveProgress();
                    break;
                case 4:
                    Village.LoadProgress();
                    break;
            }
        }
        public void WorkerMenu()
        {
            Console.CursorVisible = false;

            ClearBottomBox();
            
            Console.SetCursorPosition(4, 15);
            Console.WriteLine("What worker to build:");
            
            Console.SetCursorPosition(4, 16);
            Console.WriteLine("1. Lumberjack");
            
            Console.SetCursorPosition(4, 17);
            Console.WriteLine("2. Miner");
            
            Console.SetCursorPosition(4, 18);
            Console.WriteLine("3. Farmer");
            
            Console.SetCursorPosition(4, 19);
            Console.WriteLine("4. Builder");

            Village.AddWorker(ReadIntInstantly(4) - 1);            
        }
        public void BuildingMenu()
        {
            Console.CursorVisible = false;

            ClearBottomBox();
            
            Console.SetCursorPosition(4, 15);
            Console.WriteLine("Choose a building:");
            
            Console.SetCursorPosition(4, 16);
            Console.WriteLine("1. House");
            
            Console.SetCursorPosition(4, 17);
            Console.WriteLine("2. Farm");
            
            Console.SetCursorPosition(4, 18);
            Console.WriteLine("3. Woodmill");
            
            Console.SetCursorPosition(4, 19);
            Console.WriteLine("4. Quarry");
            
            Console.SetCursorPosition(4, 20);
            Console.WriteLine("5. Castle");

            Village.AddConstruction(ReadIntInstantly(5) - 1);
        }        
        public void PassTimeMenu()
        {
            Console.CursorVisible = true;

            ClearBottomBox();

            Console.SetCursorPosition(4, 15);
            Console.Write("How many days? ");

            if (int.TryParse(Console.ReadLine(), out int days))
            {
                for (int i = 0; i < days; i++)
                {
                    Village.Day();
                }
            }
            else 
            {   ClearBottomBox();

                Console.SetCursorPosition(4, 15);
                Console.WriteLine("Enter a number!");
                
                Console.ReadKey(true);

                PassTimeMenu();
            }
        }
        public void PrintYouWin()
        {
            //Console.OutputEncoding = System.Text.Encoding.GetEncoding(28591);

            Console.CursorVisible = false;

            Console.Clear();

            Console.WriteLine(@$"
                                                                                              
                              Congratulations! You won the game!                                                               
                                     It took you {Village.DaysGone} days.

                   |>>>                                                      |>>>
                   |                     |>>>          |>>>                  |
                   *                     |             |                     *
                  / \                    *             *                    / \
                 /___\                 _/ \           / \_                 /___\
                 [   ]                |/   \_________/   \|                [   ]
                 [ I ]                /     \       /     \                [ I ]
                 [   ]_ _ _          /       \     /       \          _ _ _[   ]
                 [   ] U U |        [#########]   [#########]        | U U [   ]
                 [   ]====/          \=======/     \=======/          \====[   ]
                 [   ]    |           |   I |_ _ _ _| I   |           |    [   ]
                 [___]    |_ _ _ _ _ _|     | U U U |     |_ _ _ _ _ _|    [___]
                 \===/  I | U U U U U |     |=======|     | U U U U U | I  \===/
                  \=/     |===========| I   | + W + |   I |===========|     \=/
                   |  I   |           |     |_______|     |           |   I  |
                   |      |           |     |||||||||     |           |      |
                   |      |           |   I ||vvvvv|| I   |           |      |
               _-_-|______|-----------|_____||     ||_____|-----------|______|-_-_
                  /________\         /______||     ||______\         /________\
                 |__________|-------|________\_____/________|-------|__________|");           

        }
        public void PrintYouLoose()
        {
            Console.Clear();
            Console.WriteLine(@"

  ▄████  ▄▄▄       ███▄ ▄███▓▓█████  ▒█████   ██▒   █▓▓█████  ██▀███  
 ██▒ ▀█▒▒████▄    ▓██▒▀█▀ ██▒▓█   ▀ ▒██▒  ██▒▓██░   █▒▓█   ▀ ▓██ ▒ ██▒
▒██░▄▄▄░▒██  ▀█▄  ▓██    ▓██░▒███   ▒██░  ██▒ ▓██  █▒░▒███   ▓██ ░▄█ ▒
░▓█  ██▓░██▄▄▄▄██ ▒██    ▒██ ▒▓█  ▄ ▒██   ██░  ▒██ █░░▒▓█  ▄ ▒██▀▀█▄  
░▒▓███▀▒ ▓█   ▓██▒▒██▒   ░██▒░▒████▒░ ████▓▒░   ▒▀█░  ░▒████▒░██▓ ▒██▒
 ░▒   ▒  ▒▒   ▓▒█░░ ▒░   ░  ░░░ ▒░ ░░ ▒░▒░▒░    ░ ▐░  ░░ ▒░ ░░ ▒▓ ░▒▓░
  ░   ░   ▒   ▒▒ ░░  ░      ░ ░ ░  ░  ░ ▒ ▒░    ░ ░░   ░ ░  ░  ░▒ ░ ▒░
░ ░   ░   ░   ▒   ░      ░      ░   ░ ░ ░ ▒       ░░     ░     ░░   ░ 
      ░       ░  ░       ░      ░  ░    ░ ░        ░     ░  ░   ░     
                                                  ░");
        }
        public void PlayAgain()
        {            
            Console.WriteLine("\n\nDo you want to play again? (Y/N)");

            while (true)
            {
                ConsoleKeyInfo input = Console.ReadKey(true);

                if (input.Key == ConsoleKey.Y)
                {
                    Village = new Village();
                    Run();
                    break;
                }
                else if (input.Key == ConsoleKey.N)
                {
                    break;
                }                
                else if (input.Key != ConsoleKey.Y || input.Key != ConsoleKey.N)
                {
                    continue;
                }
            }
        }        
        public bool YouLooseCheck()
        {
            if (_village.DaysGone > 0 && _village.Workers.Count == 0)
            {
                return true;
            }
            return false;
        }
        public bool YouWinCheck()
        {
            if (Village.BuildingExists("Castle", Village.Buildings))
            {
                return true;
            }
            return false;
        }
        public void EndOfGame()
        {
            if (YouWinCheck())
            {
                PrintYouWin();
            }
            else
            {
                PrintYouLoose();
            }
        }
        public void PrintVillageStats()
        {
            int left = 4;

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;
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
            Console.WriteLine($"Days gone: {Village.DaysGone}");
            Console.SetCursorPosition(left, 4);
            Console.WriteLine($"Worker slots: {Village.Workers.Count()}/{Village.TotalWorkerSlots()}");
            Console.SetCursorPosition(left, 5);
            Console.WriteLine($"Under construction: {Village.UnderConstruction.Count}");
            Console.SetCursorPosition(left, 6);
            Console.WriteLine($"Buildings: {Village.Buildings.Count}");
            Console.SetCursorPosition(left, 7);
            Console.WriteLine($"Workers: {Village.Workers.Count}");
            Console.SetCursorPosition(left, 8);
            Console.WriteLine($"Food: {Village.Food}");
            Console.SetCursorPosition(left, 9);
            Console.WriteLine($"Wood: {Village.Wood}");
            Console.SetCursorPosition(left, 10);
            Console.WriteLine($"Metal: {Village.Metal}");
            Console.SetCursorPosition(left, 11);
            Console.WriteLine($"Graveyard: {Village.Graveyard.Count}");
            Console.SetCursorPosition(0, 0);

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
            Console.SetCursorPosition(1, 19);
            Console.WriteLine("                           ");

        }
    }
}

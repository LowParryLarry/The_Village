namespace The_Village
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Game game = new Game();
            //game.Run();

            Village village = new Village();
            village.Food = 199;
            Console.WriteLine(village.Food);

            village.SaveProgress();

            village.Food = 0;
            Console.WriteLine(village.Food);

            village.LoadProgress();

            Console.WriteLine(village.Food);







        }
    }
}


// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;
using TournamentManager3000.Models;

namespace TournamentManager3000 {
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Hello, World!");

            // Enum.GetValues(typeof(Testing)).Cast<Testing>().ToList().ForEach(x => Console.WriteLine(x));
            Console.WriteLine((int)Math.Pow(2, Math.Ceiling(Math.Log(1, 2))));
            /*
            IEntity entity = new Player();
            switch (entity.GetType())
            {
                case var type when type == typeof(Player):
                    Player player = (Player) entity;
                    break;
                case var type when type == typeof(Team):
                    Team team = (Team) entity;
                    break;
                case var type when type == typeof(Round):
                    Round round = (Round) entity;
                    break;
                default:
                    break;
            }

            IEntity entityTest = new Player();
            foreach (var f in entityTest.GetType().GetProperties().Where(prop => prop.CanWrite))
            {
                Console.WriteLine(
                    String.Format("Name: {0}", f.Name)
                    );
            }
            */
        }
    }
}
using System;

namespace Naves
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            
            using (Game game = new Game())
            {
                game.Run();
            }
            
            /*
            using (Pathfinding game = new Pathfinding())
            {
                game.Run();
            }
            */
        }
    }
#endif
}


/*  Program.cs
 *  Program class
 *  
 *  Project Sigma
 *  Michael Ou
 *  Wei Wei Huang
 */
using System;

namespace Sigma
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}


using System;

namespace Task15
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var hospital = new Hospital(5, 5, 100);
            try
            {
                hospital.StartSimulation();
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

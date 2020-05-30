using System;

namespace Task15
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var hospital = new Hospital(2, 10, 5); 
            hospital.StartSimulation();
            Console.ReadKey();
        }
    }
}

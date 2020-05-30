using System;


namespace Task15
{
    class Patient
    {
        public bool IsInfected { get; set; }
        public Patient()
        {
            if (new Random().Next(10) == 0) 
                IsInfected = true;
        }
    }
}

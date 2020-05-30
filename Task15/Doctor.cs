using System;
using System.Threading;
using System.Threading.Tasks;



namespace Task15
{
    class Doctor 
    {
        private Hospital _hospital;
        private int _number;
        private Random rnd;
        public bool IsWork { get; set; }
        public Doctor(Hospital hospital, int number)
        {
            _hospital = hospital;
            _number = number;
            rnd = new Random(_number);
        }

        public async void WorkAsync()
        {
            await Task.Run(Work);
        }
        private void Work()
        {
            while (true)
            {
                if (!_hospital.IsHasNewPatient || IsWork)
                {
                    Thread.Sleep(100);
                    continue;
                }
                _hospital.BeginInspection();
                Inspection();
                Consulting();
            }
        }

        private void Inspection()
        {
            Thread.Sleep(1000);
        }
        private void Consulting()
        {
            if (rnd.Next(10) >= 5) return;
            var docNum = _hospital.GetDoctorForConsulting();
            Thread.Sleep(rnd.Next(1, _hospital.time + 1) * 1000);
            _hospital.FreeDoctor(docNum);
        }
    }
}
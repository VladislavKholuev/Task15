using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Task15
{
    class Hospital
    {
        private ConcurrentDictionary<int, Doctor> _doctors;
        private ConcurrentQueue<Patient> smallRoom;
        private ConcurrentDictionary<int, Patient> _queue;
        private Random rnd = new Random();
        private int sizeRoom { get; }
        public int time { get; }
        private int DoctorsCount { get; }
        public bool IsHasNewPatient => smallRoom.Count != 0;
        private static int id = 0;
        public bool IsHasInfectionInRoom => smallRoom.Any(x => x.IsInfected);
        public Hospital(int doctorsCount, int timing, int sizeRoomSize = 10)
        {
            sizeRoom = Math.Abs(sizeRoomSize);
            time = timing;
            DoctorsCount = doctorsCount;
            _doctors = new ConcurrentDictionary<int, Doctor>();
            for (var i = 0; i < doctorsCount; i++) 
                _doctors.GetOrAdd(i, new Doctor(this, i));
            _queue = new ConcurrentDictionary<int, Patient>();
            smallRoom = new ConcurrentQueue<Patient>();
        }
        
        public void StartSimulation()
        {
            StartNewPatient();
            StartDemonAsync();
            StartQueue();
            StartInfectionDemonAsync();
            foreach (var doctor in _doctors) 
                doctor.Value.WorkAsync();
        }

        private async void StartDemonAsync()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    HospitalData();
                }
            });
        }

        private void HospitalData()
        {
            Console.WriteLine();
            Console.WriteLine("докторов " + DoctorsCount + " в смотровой " + smallRoom.Count + " в очереди " + _queue.Count);
            Console.WriteLine("Смотровая");
            foreach (var patient in smallRoom)
                if (patient.IsInfected)
                    Console.WriteLine("п(з), ");
                else
                    Console.WriteLine("п, ");
            Console.WriteLine();
            Console.WriteLine("Очередь");
            foreach (var patient in _queue)
            {
                if (patient.Value.IsInfected)
                    Console.Write("п-" + id + "(з), ");
                else
                    Console.Write("п-" + id + ", ");
            }
            id++;
            id = id % 10;
        }
        private async void StartNewPatient()
        {
            await Task.Run(NewPatDemon);
        }

        private void NewPatDemon()
        {
            while (true)
            {
                var newId = _queue.Count == 0 ? 0 : _queue.Keys.Max() + 1;
                _queue[newId] = new Patient();
                Thread.Sleep(rnd.Next(1000));
            }
        }

        public Patient BeginInspection()
        {
            while (true)
            {
                if (smallRoom.Count == 0) continue;
                try
                {
                    if (!smallRoom.TryDequeue(out var p)) continue;
                    return p;
                }
                catch (InvalidOperationException)
                {
                }
            }
        }

        private async void StartQueue()
        {
            await Task.Run(QueueDemon);
        }

        private void QueueDemon()
        {
            while (true)
            {
                Thread.Sleep(time);
                if (smallRoom.Count >= sizeRoom || _queue.Count == 0) continue;

                int queueId;
                try
                {
                    if (!IsHasNewPatient)
                        queueId = _queue.Keys.Min();
                    else if (IsHasInfectionInRoom)
                        queueId = _queue.Where(pair => pair.Value.IsInfected).Select(pair => pair.Key).Min();
                    else
                        queueId = _queue.Where(pair => !pair.Value.IsInfected).Select(pair => pair.Key).Min();
                }
                catch (InvalidOperationException)
                {
                    continue;
                }

                Patient p;
                while (!_queue.TryRemove(queueId, out p))
                {
                }
                smallRoom.Enqueue(p);
            }
        }

        private async void StartInfectionDemonAsync()
        {
            await Task.Run(InfectionDemon);
        }

        private void InfectionDemon()
        {
            while (true)
            {
                Thread.Sleep(1000);
                var keys = _queue.Keys.ToArray();
                for (var i = 0; i < keys.Length; i++)
                    try
                    {
                        if (!_queue[keys[i]].IsInfected) continue;
                        if (i > 0)
                        {
                            _queue[keys[i - 1]].IsInfected = true;
                        }

                        if (i < keys.Length - 1)
                        {
                            _queue[keys[i + 1]].IsInfected = true;
                        }
                        i++;
                    }
                    catch (KeyNotFoundException)
                    {
                    }
            }
        }

        public int GetDoctorForConsulting()
        {
            while (true)
            {
                foreach (var doctor in _doctors)
                {
                    if (!doctor.Value.IsWork)
                    {
                        doctor.Value.IsWork = true;
                        return doctor.Key;
                    }
                }
            }
        }
        public void FreeDoctor(int docNum)
        {
            _doctors[docNum].IsWork = false;
        }
    }
}
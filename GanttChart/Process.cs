using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanttChart
{
    class Process
    {
        private static int num = 0;
        public int arrivalTime { get; set; }
        public int burstTime { get; set; }
        public int priority { get; set; }
        public string name { get; set; }
        public bool completed { get; set; }
        public int start { get; set; }
        public int finish { get; set; }
        public int runTime { get; set; }
        public int waitTime { get; set; }
        public int turnaroundTime { get; set; }

        public Process(bool p=false)
        {
            int input;

            name = String.Format("P{0}", ++num);

            Console.WriteLine("Enter details for Process {0}:", name);

            //AT in
            Console.Write("Enter arrival time: ");
            while (!Int32.TryParse(Console.ReadLine(), out input) || input < 0)
            {
                Console.Write("Invalid Input! Please enter a valid arrival time: ");
            }
            arrivalTime = input;

            //BT in
            Console.Write("Enter burst time: ");
            while (!Int32.TryParse(Console.ReadLine(), out input) || input < 0)
            {
                Console.Write("Invalid Input! Please enter a valid burst time: ");
            }
            burstTime = input;

            //Priority in if needed
            if (p)
            {
                Console.Write("Enter priority: ");
                while (!Int32.TryParse(Console.ReadLine(), out input) || input < 0)
                {
                    Console.Write("Invalid Input! Please enter a non-negative priority: ");
                }
                priority = input;
            }
            else
            {
                priority = -1;
            }

            //init remaining vars
            completed = false;
            start = -1;
            finish = -1;
            runTime = 0;
            waitTime = 0;
            turnaroundTime = 0;
        }

        public bool isAvailable(int currentTime)
        {
            return arrivalTime <= currentTime;
        }

        //run process for given amount of time
        public void run(int currentTime, int length)
        {
            if(!completed && this.isAvailable(currentTime) && length <= this.timeLeft())
            {
                if (start == -1) //not started
                {
                    start = currentTime;
                    waitTime = currentTime - arrivalTime;
                }
                else if (finish != currentTime) //started with additional wait
                {
                    waitTime += currentTime - finish;
                }
                runTime += length;
                finish = currentTime + length;

                if (burstTime == runTime) //finished
                {
                    completed = true;
                    turnaroundTime = finish - arrivalTime;
                }
            }
            else
            {
                Console.WriteLine("{0} can't be run!", this.name);
            }
        }

        public int timeLeft()
        {
            return burstTime - runTime;
        }

        public void resetNum()
        {
            num = 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanttChart
{
    class Program
    {
        static void Main(string[] args)
        {
            welcome();
            while (true)
            {
                Console.Write("\nHow many processes would you like to enter: ");
                int numProcesses;

                //validate num of processses
                while (!Int32.TryParse(Console.ReadLine(), out numProcesses) || numProcesses < 1)
                {
                    Console.Write("Invalid Input! Please enter a valid number: ");
                }

                //process list
                List<Process> processes = new List<Process>();

                //pick algorithm
                char c = pickAlgorithm();

                //get processes
                for (int i = 0; i < numProcesses; i++)
                {
                    Process p = new Process(c == 'p');
                    processes.Add(p);
                }

                //create gantt chart
                Chart gantt = new Chart(processes, c);

                Console.WriteLine("");
                Console.WriteLine("Gantt Chart calculations for {0} successfull!", gantt.algo);
           
                //print segments to console
                gantt.print();

                Console.WriteLine("");

                //make html page for gantt chart visual
                gantt.html();

                //run again?
                Console.WriteLine("");
                Console.Write("Would you like to create another Gantt Chart? (y/n) ");
                c = Console.ReadKey().KeyChar;
                if (c != 'y')
                {
                    break;
                }

                //reset process numbers
                gantt.completed[0].resetNum();
            }
        }

        public static char pickAlgorithm()
        {
            Console.WriteLine("What scheduling algorithm would you like to use?");
            Console.WriteLine("[f] = FCFS");
            Console.WriteLine("[j] = SJF");
            Console.WriteLine("[p] = Priority");
            Console.WriteLine("[c] = SCF");
            Console.WriteLine("[r] = RR");

            char c = Console.ReadKey().KeyChar;
            Console.WriteLine("");

            //verify character
            switch (c)
            {
                case 'f':
                    return c;
                case 'j':
                    return c;
                case 'p':
                    return c;
                case 'c':
                    return c;
                case 'r':
                    return c;
                default:
                    Console.WriteLine("Incorrect character entered! Please try again!");
                    return pickAlgorithm();
            }
        }

        public static void welcome()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("#####################################################");
            Console.WriteLine("#    WELCOME TO THE SUPER COOL GANTT CHART MAKER    #");
            Console.WriteLine("# ------------------------------------------------- #");
            Console.WriteLine("# Author: Derek Cannistraro                         #");
            Console.WriteLine("# Class: Operating Systems                          #");
            Console.WriteLine("# Date: 7/31/2017                                   #");
            Console.WriteLine("# Description: Program that creates detailed Gantt  #");
            Console.WriteLine("#              charts for various algorithms.       #");
            Console.WriteLine("#####################################################");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}

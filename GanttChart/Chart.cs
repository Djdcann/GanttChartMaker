using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GanttChart
{
    class Chart
    {
        public List<Segment> segments { get; set; }
        public List<Process> completed { get; set; }
        public float averageWait { get; set; }
        public float averageTurnaround { get; set; }
        public int endTime { get; set; }
        public string algo { get; set; }

        public Chart(List<Process> p, char a)
        {
            //init
            segments = new List<Segment>();
            completed = new List<Process>();

            //sort p by AT
            p.Sort(delegate (Process x, Process y) { return x.arrivalTime.CompareTo(y.arrivalTime); });

            //run algorithm based on character
            switch (a)
            {
                case 'f':
                    algo = "FCFS";
                    fcfs(p);
                    break;
                case 'j':
                    algo = "SJF";
                    sjf(p);
                    break;
                case 'p':
                    algo = "Priority";
                    priority(p);
                    break;
                case 'c':
                    algo = "SCF";
                    scf(p);
                    break;
                case 'r':
                    //When Round Robin is selected, we must prompt for the quantum and ask if it is fixed
                    int quantum;
                    Console.Write("Enter quantum time for Round Robin Algorithm: ");
                    while (!Int32.TryParse(Console.ReadLine(), out quantum))
                    {
                        Console.WriteLine("Invalid Input! Please enter a number");
                    }

                    Console.Write("Is the quantum fixed? (y/n): ");
                    char c = Console.ReadKey().KeyChar;
                    Console.WriteLine("");
                    algo = c == 'y' ? "RR (Fixed)" : "RR (Not-Fixed)";

                    rr(p, quantum, (c == 'y'));
                    break;
                default:
                    Console.WriteLine("Invalid algorithm character!");
                    break;
            }

            //get end time, average times, and sort completed list by name
            endTime = segments.Last().finish;
            completed.Sort(delegate (Process x, Process y) { return x.name.CompareTo(y.name); });
            getAverageTimes();
        }

        //First Come First Serve Algorithm
        private void fcfs(List<Process> p)
        {
            int tick = 0; //currentTime
            int currentP = 0; //current Process index

            //Process list is already sorted by AT for FCFS

            //get chart segments
            while (currentP < p.Count)
            {
                //if process has arrived, complete it and add the segment
                if (p[currentP].isAvailable(tick))
                {
                    p[currentP].run(tick, p[currentP].burstTime);
                    Segment s = new Segment(p[currentP].name, tick, tick += p[currentP].burstTime);
                    segments.Add(s);
                    completed.Add(p[currentP]);

                    //increment current process
                    currentP++;
                }
                else //no available processes, add idle time
                {
                    Segment s = new Segment("Idle", tick, tick = p[currentP].arrivalTime);
                    segments.Add(s);
                }
            }
        }

        //Shortest Job First Algorithm
        private void sjf(List<Process> p)
        {
            int tick = 0; //currentTime

            //Queue
            List<Process> Q = new List<Process>();

            //get chart segments
            while (p.Count > 0 || Q.Count > 0)
            {
                List<Process> toRemove = new List<Process>();

                //add newly available processes to Queue
                foreach (Process i in p)
                {
                    if (i.isAvailable(tick))
                    {
                        Q.Add(i);
                        toRemove.Add(i);
                    }
                    else
                    {
                        break;
                    }
                }

                //remove available processes from p
                foreach (Process i in toRemove)
                {
                    p.Remove(i);
                }

                //sort Q by BT
                Q.Sort(delegate (Process x, Process y) { return x.burstTime.CompareTo(y.burstTime); });

                if (Q.Count > 0) //complete current process and add segment
                {
                    Q[0].run(tick, Q[0].burstTime);
                    Segment s = new Segment(Q[0].name, tick, tick += Q[0].burstTime);
                    segments.Add(s);
                    completed.Add(Q[0]);
                    Q.RemoveAt(0);
                }
                else //Q is empty, add idle time
                {
                    Segment s = new Segment("Idle", tick, tick = p[0].arrivalTime);
                    segments.Add(s);
                }
            }
        }

        //Priority Algorithm
        private void priority(List<Process> p)
        {
            int tick = 0; //currentTime
            Segment s = null;

            //Queue
            List<Process> Q = new List<Process>();

            //get chart segments
            while (p.Count > 0 || Q.Count > 0)
            {
                List<Process> toRemove = new List<Process>();

                //add newly available processes to Queue
                foreach (Process i in p)
                {
                    if (i.isAvailable(tick))
                    {
                        Q.Add(i);
                        toRemove.Add(i);
                    }
                    else
                    {
                        break;
                    }
                }

                //remove available processes from p
                foreach (Process i in toRemove)
                {
                    p.Remove(i);
                }

                //sort Q by priority
                Q.Sort(delegate (Process x, Process y) { return x.priority.CompareTo(y.priority); });

                if (Q.Count > 0)
                {
                    //run for a length of 1
                    Q[0].run(tick, 1);
                    if (s == null || s.name != Q[0].name) //if new process is starting or preempting
                    {
                        if (s != null) //mark segment finish and add to list if segment exists
                        {
                            s.markFinish(tick);
                            segments.Add(s);
                        }
                        s = new Segment(Q[0].name, tick); //otherwise create new segment
                    }

                    //current Process completed
                    if (Q[0].completed)
                    {
                        //mark finish, add segment, set s to null
                        s.markFinish(tick + 1);
                        segments.Add(s);
                        s = null;
                        completed.Add(Q[0]);
                        Q.RemoveAt(0);
                    }

                    //increment currentTime
                    tick++;
                }
                else //Q is empty, add idle time
                {
                    segments.Add(new Segment("Idle", tick, tick = p[0].arrivalTime));
                }
            }
        }

        //Shortest Job First w/ preemption
        private void scf(List<Process> p)
        {
            int tick = 0; //currentTime
            Segment s = null;

            //Queue
            List<Process> Q = new List<Process>();

            //get chart segments
            while (p.Count > 0 || Q.Count > 0)
            {
                List<Process> toRemove = new List<Process>();

                //add newly available processes to Queue
                foreach (Process i in p)
                {
                    if (i.isAvailable(tick))
                    {
                        Q.Add(i);
                        toRemove.Add(i);
                    }
                    else
                    {
                        break;
                    }
                }

                //remove available processes from p
                foreach (Process i in toRemove)
                {
                    p.Remove(i);
                }

                //sort Q by remaining time
                Q.Sort(delegate (Process x, Process y) { return x.timeLeft().CompareTo(y.timeLeft()); });

                if (Q.Count > 0)
                {
                    //run for a length of 1
                    Q[0].run(tick, 1);
                    if (s == null || s.name != Q[0].name) //if new process is starting or preempting
                    {
                        if (s != null) //mark segment finish and add to list if segment exists
                        {
                            s.markFinish(tick);
                            segments.Add(s);
                        }
                        s = new Segment(Q[0].name, tick); //otherwise create new segment
                    }

                    //current process has completed
                    if (Q[0].completed)
                    {
                        //mark finish, add segment, set new segment to null
                        s.markFinish(tick + 1);
                        segments.Add(s);
                        s = null;
                        completed.Add(Q[0]);
                        Q.RemoveAt(0);
                    }

                    //increment currentTime
                    tick++;
                }
                else //Q is empty, add idle time
                {
                    segments.Add(new Segment("Idle", tick, tick = p[0].arrivalTime));
                }
            }
        }

        //Round Robin method
        private void rr(List<Process> p, int q, bool isFixed)
        {
            int tick = 0; //currentTime
            bool rotate = false; //rotate Q flag

            //Queue
            List<Process> Q = new List<Process>();

            //get chart segments
            while (p.Count > 0 || Q.Count > 0)
            {
                List<Process> toRemove = new List<Process>();

                foreach (Process i in p)
                {
                    if (i.isAvailable(tick)) //process has arrived, add it to the Queue
                    {
                        Q.Add(i);
                        toRemove.Add(i);
                    }
                    else
                    {
                        break;
                    }
                }

                //remove newly available processes from p
                foreach (Process i in toRemove)
                {
                    p.Remove(i);
                }

                //rotate Q if needed
                if (rotate && Q.Count > 1)
                {
                    Q.Add(Q[0]);
                    Q.RemoveAt(0);
                }

                //if Q is not empty
                if (Q.Count > 0)
                {
                    if (Q[0].timeLeft() < q) //if time left is smaller than quantum (q)
                    {
                        //run for time left and set the remaining quantum to idle if fixed
                        int t = Q[0].timeLeft();
                        Q[0].run(tick, t);
                        segments.Add(new Segment(Q[0].name, tick, tick += t));
                        if (isFixed)
                        {
                            segments.Add(new Segment("Idle", tick, tick += q-t));
                        }
                    }
                    else
                    {
                        //run process for quantum (q)
                        Q[0].run(tick, q);
                        segments.Add(new Segment(Q[0].name, tick, tick += q));
                    }

                    if (Q[0].completed) //remove with no rotation
                    {
                        completed.Add(Q[0]);
                        Q.RemoveAt(0);
                        rotate = false;
                    }
                    else //set rotate flag
                    {
                        rotate = true;
                    }
                }
                else //Q is empty
                {
                    if (isFixed || p[0].arrivalTime > tick + q) //fixed or process arrives after next quantum
                    {
                        segments.Add(new Segment("Idle", tick, tick += q));
                    }
                    else //not fixed and process arrives before the next quantum
                    {
                        segments.Add(new Segment("Idle", tick, tick = p[0].arrivalTime));
                    }
                    
                }
            }
        }

        //calculate average wait and turnaround times
        private void getAverageTimes()
        {
            int waitSum = 0;
            int turnaroundSum = 0;

            foreach(Process i in completed)
            {
                waitSum += i.waitTime;
                turnaroundSum += i.turnaroundTime;
            }

            averageWait = (float) waitSum / completed.Count;
            averageTurnaround = (float) turnaroundSum / completed.Count;
        }

        //print method for chart to display segment information in the console
        public void print()
        {
            Console.WriteLine("Segments:");

            foreach(Segment s in segments)
            {
                Console.WriteLine("Name: {0}\tstart: {1}\tfinish: {2}", s.name, s.start, s.finish);
            }

            Console.WriteLine("\nAverage Wait Time: {0}", averageWait);
            Console.WriteLine("Average Turnaround Time: {0}", averageTurnaround);
        }

        //generate html page visual for gantt chart and processes
        public string html()
        {
            string s1 = "";
            string s2 = "";
            string s3 = "";

            //create segments in a width defined table
            foreach (Segment x in segments)
            {
                s1 += String.Format("<td class='{0}'width='{1}%'>{2}</td>", x.name, (float) 100.0*(x.finish - x.start) / endTime, x.name);
                s2 += String.Format("<td>{0}</td>", x.start);
            }
            s2 += String.Format("<td>{0}</td>", endTime);

            //create table of processes to display on html page
            foreach(Process p in completed)
            {
                s3 += String.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>", p.name, p.arrivalTime, p.burstTime, (p.priority == -1) ? "None" : p.priority.ToString(), p.waitTime, p.turnaroundTime);
            }

            //read template file and replace placeholder strings with generated strings
            var content = File.ReadAllText("output.html");
            content = content.Replace("[S1]", s1);
            content = content.Replace("[S2]", s2);
            content = content.Replace("[S3]", s3);
            content = content.Replace("[S4]", averageWait.ToString());
            content = content.Replace("[S5]", averageTurnaround.ToString());
            content = content.Replace("[S6]", algo);

            //write to result file
            File.WriteAllText("result.html", content);

            Console.WriteLine("HTML Gantt Chart created!");
            Console.Write("Press any key to view the generated document...");
            Console.ReadKey();

            //launch html page in default browser
            System.Diagnostics.Process.Start("result.html");
            return s1;
        }
    }

    //Segment class to represent a run sequence in a gantt chart
    class Segment
    {
        public int start { get; set; }
        public int finish { get; set; }
        public string name { get; set; }

        public Segment(string n, int s)
        {
            name = n;
            start = s;
            finish = -1;
        }

        public Segment(string n, int s, int f)
        {
            name = n;
            start = s;
            finish = f;
        }

        public void markFinish(int f)
        {
            finish = f;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;

class Assignment1
{
    // Simulation Initialisation
    private static int NUM_MACHINES = 50;   // Number of machines in the system that issue print requests
    private static int NUM_PRINTERS = 5;    // Number of printers in the system that print requests
    private static int SIMULATION_TIME = 30;
    private static int MAX_PRINTER_SLEEP = 3;
    private static int MAX_MACHINE_SLEEP = 5;
    private static volatile bool sim_active = true;

    // Create an empty list of print requests
    printList list = new printList();

    // Put any global variables here


    public void startSimulation()
    {
        // ArrayList to keep for machine and printer threads
        List<Thread> mThreads = new List<Thread>();
        List<Thread> pThreads = new List<Thread>();

        // Create Machine and Printer threads
        
        // start the machine threads
        
        //start the printer threads

        // let the simulation run for some time
        sleep(SIMULATION_TIME);

        // finish simulation
        sim_active = false;

        // Wait until all printer threads finish by using the joining them


    }

    // Printer class
    public class printerThread
    {
        private readonly Assignment1 outer;
        private int printerID;

        public printerThread(Assignment1 parent, int id)
        {
            outer = parent;
            printerID = id;
        }

        public void SetPrinter()
        {
            while (sim_active)
            {
                // Simulate printer taking some time to print the document
                printerSleep();

                // Grab the request at the head of the queue and print it
            }
        }

        public void printerSleep()
        {
            int sleepSeconds = 1 + (int)(new Random(Guid.NewGuid().GetHashCode()).NextDouble() * MAX_PRINTER_SLEEP);

            try
            {
                Thread.Sleep(sleepSeconds * 1000);
            }
            catch (ThreadInterruptedException)
            {
                Console.WriteLine("Sleep Interrupted");
            }
        }

        public void printDox(int printerID)
        {
            Console.WriteLine("Printer ID:" + printerID + " : now available");

            // Write code here

            // print from the queue
            list.queuePrint(list, printerID);

            // Write code here

        }

        private printList list
        {
            get { return outer.list; }
        }
    }

    // Machine class
    public class machineThread
    {
        private readonly Assignment1 outer;
        private int machineID;

        public machineThread(Assignment1 parent, int id)
        {
            outer = parent;
            machineID = id;
        }

        public void SetMachine()
        {
            while (sim_active)
            {
                // machine sleeps for a random amount of time
                machineSleep();

                // machine wakes up and sends a print request
            }
        }

        public void isRequestSafe(int id)
        {
            Console.WriteLine("Machine " + id + " Checking availability");

            // Write code here:

            Console.WriteLine("Machine " + id + " will proceed");
        }

        public void printRequest(int id)
        {
            Console.WriteLine("Machine " + id + " Sending a print request");

            // Build a print document
            printDoc doc = new printDoc("My name is machine " + id, id);

            // Insert it in print queue
            outer.list = outer.list.queueInsert(outer.list, doc);
        }

        public void postRequest(int id)
        {
            Console.WriteLine("Machine " + id + " Releasing binary semaphore");

            // Write code here
        }

        public void machineSleep()
        {
            int sleepSeconds = 1 + (int)(new Random(Guid.NewGuid().GetHashCode()).NextDouble() * MAX_MACHINE_SLEEP);

            try
            {
                Thread.Sleep(sleepSeconds * 1000);
            }
            catch (ThreadInterruptedException)
            {
                Console.WriteLine("Sleep Interrupted");
            }
        }
    }

    private static void sleep(int s)
    {
        try
        {
            Thread.Sleep(s * 1000);
        }
        catch (ThreadInterruptedException)
        {
            Console.WriteLine("Sleep Interrupted");
        }
    }

    public static void Main(string[] args)
    {
        new Assignment1().startSimulation();
    }
}
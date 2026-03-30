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
    private static Semaphore spaceAvaliable = new Semaphore(NUM_PRINTERS, NUM_PRINTERS);
    private static Mutex queueMutex = new Mutex();


    public void startSimulation()
    {
        // ArrayList to keep for machine and printer threads
        List<Thread> mThreads = new List<Thread>();
        List<Thread> pThreads = new List<Thread>();

        // Create Machine and Printer threads
        
        // start the machine threads
        for (int i = 0; i < NUM_MACHINES; i++){
            mThreads.Add(new Thread(new machineThread(this, i).SetMachine));
            mThreads[i].IsBackground = true;
            mThreads[i].Start();
        }
        
        //start the printer threads
        for (int i = 0; i < NUM_PRINTERS; i++){
            pThreads.Add(new Thread(new printerThread(this, i).SetPrinter));
            pThreads[i].Start();
        }

        // let the simulation run for some time
        sleep(SIMULATION_TIME);

        // finish simulation
        sim_active = false;

        // Wait until all printer threads finish by using the joining them
        foreach (Thread p in pThreads)
        {
            p.Join();
        }

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

                // Grab the request and print it
                if (sim_active)
                {
                    printDox(printerID);
                }
                
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
            queueMutex.WaitOne(); // Locks the queue
            
            try
            {
                // Grab the request at the head of the queue and print it
                if (list.head != null)
                {
                    list.queuePrint(list, printerID);
                    spaceAvaliable.Release();
                }
            }
            finally
            {
                 queueMutex.ReleaseMutex(); // Always releases, even if an exception occurs
            }

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
                if (sim_active){
                    isRequestSafe(machineID); // Calls this code to run
                    printRequest(machineID); // Calls this code to run
                    postRequest(machineID); // Calls this code to run
                }
            }
        }

        public void isRequestSafe(int id)
        {
            Console.WriteLine("Machine " + id + " Checking availability");

            // Write code here:
            spaceAvaliable.WaitOne(); // Waits until there is enough/avaliable space in the queue
            
            Console.WriteLine("Machine " + id + " will proceed");
        }

        public void printRequest(int id)
        {
            Console.WriteLine("Machine " + id + " Sending a print request");

            // Build a print document
            queueMutex.WaitOne(); // Locks the queue
            printDoc doc = new printDoc("My name is machine " + id, id);

            // Insert it in print queue
            outer.list = outer.list.queueInsert(outer.list, doc);
            queueMutex.ReleaseMutex();
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
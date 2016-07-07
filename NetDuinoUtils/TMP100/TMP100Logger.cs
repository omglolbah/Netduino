using System;
using System.Threading;
using System.Collections;
using Microsoft.SPOT;
using NetDuinoUtils.Utils;

namespace NetDuinoUtils.TMP100
{
    public class TMP100LoggerService
    {
        private static object _lockObject = new object();
        private static TMP100LoggerService _instance;

        private static Timer stateTimer;
        public Queue Temperatures{get;set;}
        public static int Interval { get; set; }
        public static int Count { get; set; }      
        public static TMP100LoggerService Instance
        {
            get
            {
                lock (_lockObject)
                {
                    return _instance ?? (_instance = new TMP100LoggerService(1000,60));
                }
            }
        }
        public Queue GetQueue(){
            return Temperatures;
        }
        public TMP100LoggerService(int interval, int count)
        {
            Temperatures = new Queue();

            Count = count;
            Interval = interval;

            // Create an event to signal the timeout count threshold in the timer callback.
            AutoResetEvent autoEvent = new AutoResetEvent(false);

            TMP100Logger tmpLogger = new TMP100Logger(10);

            // Create an inferred delegate that invokes methods for the timer.
            TimerCallback tcb = tmpLogger.LogTemperature;

            // Create a timer that signals the delegate to invoke 
            // CheckStatus after one second, and every 1/4 second 
            // thereafter.
            Debug.Print(DateTime.Now.ToString("h:mm:ss.fff") +" Creating timer.\n");
            stateTimer = new Timer(tcb, autoEvent, 1000, 1000);
        }
        public void LogTemperature(TempData td)
        {
            Temperatures.Enqueue(td);
            if (Temperatures.Count > TMP100LoggerService.Count)
            {
                while (Temperatures.Count > TMP100LoggerService.Count)
                { 
                    Temperatures.Dequeue();// get rid of stale data
                }
            }
        }
    }
    public class TempData
    {
        public Double Temperature { get; set; }
        public DateTime TimeStamp { get; set; }
        public TempData(double temperature, DateTime stamp)
        {
            Temperature = temperature;
            TimeStamp = stamp;
        }
    }
    class TMP100Logger
    {
        
        private int invokeCount;
        private int maxCount;

        public TMP100Logger(int count)
        {
            invokeCount = 0;
            maxCount = count;
        }

        // This method is called by the timer delegate.
        public void LogTemperature(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            Debug.Print(DateTime.Now.ToString("h:mm:ss.fff") + " Logging Temperature"  + (++invokeCount).ToString());
            TMP100LoggerService.Instance.LogTemperature(new TempData(TMP100Reader.Instance.GetTemperature(), DateTime.Now));
        }
    }
}

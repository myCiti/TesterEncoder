using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Globalization;

namespace Indexa
{
    class Program
    {
        static void Main(string[] args)
        {
            var gpio = new GpioController();
            // readPin function
            bool readPin(int p) => gpio.Read(p) == PinValue.High;

            int cycleNum = 2_500;
            int cycleCounter = 0;
            int pulseCounter = 0;
            int state = 0;
            double time = 0;
            double slowestTime = 0;
            double moy = 0;
            double[] timeTab = new double[10000];
            int i = 0;

            const int indexPin = 4;
            gpio.OpenPin(indexPin, PinMode.Input);
            const int pulsePin = 17;
            gpio.OpenPin(pulsePin, PinMode.Input);

            var watch = System.Diagnostics.Stopwatch.StartNew();

            while (cycleCounter <= 1000)
            {
                switch (state)
                {
                    case 0:
                        {
                            if (readPin(indexPin))
                            {
                                state = 1;
                                cycleCounter++;
                            }
                            break;
                        }
                    case 1:
                        {
                            if (!readPin(indexPin))
                            {
                                state = 2;
                            }
                            break;
                        }
                    case 2:
                        {
                            if (readPin(indexPin))
                            {
                                state = 1;
                                cycleCounter++;
                                for (int y = 0; y < i; y++)
                                {
                                    moy += timeTab[y];
                                }
                                moy /= i;
                                i = 0;
                                Console.WriteLine($"Index : {cycleCounter} | pulse : {pulseCounter} | temps moyen : {moy} ms | fréquence la plus lente obtenue dans ce cycle : {1 / slowestTime}");
                                pulseCounter = 0;
                            }
                            else if (readPin(pulsePin))
                            {
                                state = 3;
                                pulseCounter++;
                            }
                            break;
                        }
                    case 3:
                        {
                            if (readPin(indexPin))
                            {
                                state = 1;
                                cycleCounter++;
                                for (int y = 0; y < 10000; y++)
                                {
                                    moy += timeTab[y];
                                }
                                moy /= i;
                                i = 0;
                                Console.WriteLine($"Index : {cycleCounter} | pulse : {pulseCounter} | temps moyen : {moy} ms | fréquence la plus lente obtenue dans ce cycle : {1 / slowestTime}");
                                pulseCounter = 0;
                            }
                            else if (!readPin(pulsePin))
                            {
                                state = 2;
                            }
                            break;
                        }
                }
                if (time > slowestTime)
                {
                    slowestTime = time;
                    Console.WriteLine($"{1 / slowestTime / 1000}");
                }
                //saving elapsed time
                time = watch.Elapsed.TotalMilliseconds;
                //saving elapsed time in array
                if (i < 10000)
                {
                    timeTab[i++] = time;
                }
                
                watch.Stop();
                watch = System.Diagnostics.Stopwatch.StartNew();
            }
            
            
            
            /*
            while(cycleCounter <= 50)
            {
                switch (indexState)
                {
                    case 0:
                        {
                            if (readPin(indexPin)) //if indexPin == 1
                            {
                                indexState = 1;
                                ++cycleCounter;
                                while (!readPin(indexPin)) //while indexPin == 0
                                {
                                    var watch = System.Diagnostics.Stopwatch.StartNew();
                                    

                                    if (time > slowestTime)
                                    {
                                        slowestTime = time;
                                        //Console.WriteLine($"{1 / slowestTime / 1000}");
                                    }
                                    //saving elapsed time
                                    time = watch.Elapsed.TotalMilliseconds;
                                    //saving elapsed time in array
                                    timeTab[pulseCounter] = time;
                                    watch.Stop();
                                    watch = System.Diagnostics.Stopwatch.StartNew();
                                }

                                for (int y = 0; y < pulseCounter; y++)
                                {
                                    moy += timeTab[y];
                                }
                                moy /= pulseCounter;



                                Console.WriteLine($"Temps pour lire {cycleNum} index: "
                                                    + (stopWatch.Elapsed.TotalMilliseconds).ToString("#,##0.000", new CultureInfo("fr-CA"))
                                                    + " ms" + $" | temps moyen : {moy} ms | fréquence la plus lente obtenue dans ce cycle : {1 / slowestTime}");
                                cycleCounter = 0;
                                pulseCounter = 0;
                                stopWatch.Restart();
                                indexState = 0;
                            }
                            break;
                        }
                    case 1:
                        {
                            if (!readPin(pulsePin))
                            {
                                indexState = 0;
                            }
                            break;
                        }
                }
            }*/
        }
    }
}

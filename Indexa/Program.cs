using System;
using System.Device.Gpio;
using System.IO;
using System.Threading.Tasks;

namespace Indexa
{
    class Program
    {
        static void Main(string[] args)
        {
            var gpio = new GpioController();
            // readPin function
            //bool readPin(int p) => gpio.Read(p) == PinValue.High;
            var path = "Log";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            const int indexPin = 4;
            gpio.OpenPin(indexPin, PinMode.Input);
            const int pulsePin = 17;
            gpio.OpenPin(pulsePin, PinMode.Input);

            for (int i = 0; i < 20; i++)
            {
                StreamWriter sw = new StreamWriter($"{path}/ResultOfSimulation#{i}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

                sw.WriteLine($"SimulationStartAt = {DateTime.Now:yyyyMMdd_HHmmss}");
                sw.WriteLine("EachLineIsComposedOF:\nIndexCounter,Counter,SlowestSpeed");

                /*StreamWriter sw2 = new StreamWriter($"{path}/DeepIndexAnalysis_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

                sw2.WriteLine($"SimulationTestStartAt = {DateTime.Now:yyyyMMdd_HHmmss}");
                sw2.WriteLine("EachLineIsComposedOF:\nCounter,SlowestSpeed,Speed,State");*/

                int cycleCounter = 0;
                int pulseCounter = 0;
                int state = 0;
                //double time = 0;
                //double slowestTime = 0;
                //double[] timeTab = new double[100_000];
                //int x = 0;

                

                //var watch = System.Diagnostics.Stopwatch.StartNew();

                while (cycleCounter <= 1000)
                {
                    switch (state)
                    {
                        case 0:
                            {
                                if ((int)gpio.Read(indexPin) == 1)
                                {
                                    state = 1;
                                    cycleCounter++;
                                }
                                break;
                            }
                        case 1:
                            {
                                if ((int)gpio.Read(indexPin) == 0)
                                {
                                    state = 2;
                                }
                                break;
                            }
                        case 2:
                            {
                                if ((int)gpio.Read(indexPin) == 1)
                                {
                                    state = 1;
                                    cycleCounter++;
                                    //Console.WriteLine($"Index : {cycleCounter} | pulse : {pulseCounter} | fréquence la plus lente obtenue dans ce cycle (kHz) : {1 / slowestTime}");
                                    sw.WriteLineAsync($"{cycleCounter}, {pulseCounter}");
                                    //slowestTime = 0;
                                    pulseCounter = 0;
                                }
                                else if ((int)gpio.Read(pulsePin) == 1)
                                {
                                    state = 3;
                                    pulseCounter++;
                                }
                                break;
                            }
                        case 3:
                            {
                                if ((int)gpio.Read(indexPin) == 1)
                                {
                                    state = 1;
                                    cycleCounter++;
                                    //Console.WriteLine($"Index : {cycleCounter} | pulse : {pulseCounter} | fréquence la plus lente obtenue dans ce cycle (kHz) : {0:000.00}", (1 / slowestTime));
                                    sw.WriteLineAsync($"{cycleCounter}, {pulseCounter}");
                                    //slowestTime = 0;
                                    pulseCounter = 0;
                                }
                                else if ((int)gpio.Read(pulsePin) == 0)
                                {
                                    state = 2;
                                }
                                break;
                            }
                    }
                    /*if (time > slowestTime)
                    {
                        slowestTime = time;
                        //Console.WriteLine($"{1 / slowestTime / 1000}");
                    }
                    //saving elapsed time
                    time = watch.Elapsed.TotalMilliseconds;
                    sw2.WriteLine($"{cycleCounter}, {pulseCounter}, {state}, {time}");
                    watch.Stop();
                    watch = System.Diagnostics.Stopwatch.StartNew();*/
                }
                sw.WriteLine($"SimulationEndedAt = {DateTime.Now:yyyyMMdd_HHmmss}");
                sw.Close();
                /*sw2.WriteLine($"SimulationEndedAt = {DateTime.Now:yyyyMMdd_HHmmss}");
                sw2.Close();*/
                Console.WriteLine($"testEnded#{i}");
            }
        }
    }
}

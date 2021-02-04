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

            //const int indexPin = 4;
            const int pulsePin = 17;
            int cycleNum = 10_000;
            int pulseCounter = 0;
            int state = 0;

            gpio.OpenPin(pulsePin, PinMode.Input);

            var stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < 20; ++i)
            {
                while (pulseCounter < cycleNum)
                {
                    switch (state)
                    {
                        case 0: if (readPin(pulsePin)) { state = 1; ++pulseCounter; } break;
                        case 1: if (!readPin(pulsePin)) { state = 0; } break;
                    }
                }
                Console.WriteLine($"Temps pour lire {cycleNum} impulsions: "
                                    + (stopWatch.Elapsed.TotalMilliseconds).ToString("#,##0.000", new CultureInfo("fr-CA"))
                                    + " ms");
                pulseCounter = 0;
                stopWatch.Restart();
            }
        }
    }
}

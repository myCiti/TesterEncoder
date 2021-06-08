﻿using System;
using System.Device.Gpio;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TesterEncoder
{
    class Program
    {
        static int Main(string[] args)
        {
            //GPIO
            var gpio = new GpioController();
            const int indexPin = 13;
            gpio.OpenPin(indexPin, PinMode.Input);
            const int pulsePin = 19;
            gpio.OpenPin(pulsePin, PinMode.Input);
            const int BlackboxPin = 16;
            gpio.OpenPin(BlackboxPin, PinMode.Input);
            const int Open = 5;
            gpio.OpenPin(Open, PinMode.Input);
            const int Close = 6;
            gpio.OpenPin(Close, PinMode.Input);

            //start program
            #region startup
            
            //creating log folder
            var path = "Log";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //creating a file and saving his name in "date" variable
            DateTime date;
            StreamWriter swDA = new StreamWriter($"{path}/Data_TestEncoder_{date = DateTime.Now:yyyyMMdd_HHmmss}.csv");
            //writing to the file the start of the test
            swDA.WriteLine($"StartAt = {date}");
            //writing legend to know in each column what variable we saved  
            swDA.WriteLine("Index,Stop,IndexCounter,PulseCounter,Direction");
            //declaration of the variable for the state machine and logic
            int indexCounter = 0;
            int pulseCounter = 0;
            char direction = ' ';
            int totalPulseCounter = 0;
            int state = 0;

            int cycleCounter = 0;
            #endregion

            //start a new task to get logic working infinitely without freezing the ui
            Task task = new Task(() => Logic());
            task.Start();
            
            
            /*Task task2 = new Task(() => Debug2());
            task2.Start();*/

            //stoping message
            Console.WriteLine("Écrire 'st' pour arrêter le programme");
            //while to ask if the user doesn't write stop 
            while (Console.ReadLine() != "st")
            {
                Console.WriteLine("Commande inconnue");
                Console.WriteLine("Écrire 'st' pour arrêter le programme");
            }
            //telling when the test ended and closing file
            swDA.WriteLine($"TestEndedAt = {DateTime.Now:yyyyMMdd_HHmmss}");
            swDA.Close();
            //closing program
            return 0;

            void Logic()
            {
                while (true)
                {
                    #region switch logic (state machine)
                    switch (state)
                    {
                        case 0: //waiting for one of two limit to get on to start cycle
                            {
                                if (gpio.Read(Open) == PinValue.Low)
                                {
                                    direction = '^';
                                    Console.WriteLine($"#{cycleCounter} : Cycle d'ouverture débuté");
                                    state = 1;
                                }
                                else if (gpio.Read(Close) == PinValue.Low)
                                {
                                    direction = 'v';
                                    Console.WriteLine($"#{cycleCounter} : Cycle de fermeture débuté");
                                    state = 1;
                                }
                                break;
                            }
                        case 1: //waiting for the first index
                            {
                                if (gpio.Read(indexPin) == PinValue.High)
                                {
                                    state = 2;
                                }
                                break;
                            }
                        case 2: //waiting for the downing edge of the index input
                            {
                                if (gpio.Read(indexPin) == PinValue.Low)
                                {
                                    state = 3;
                                }
                                break;
                            }
                        case 3: //case 3 and 4 count number of index and pulse before blackbox
                            {
                                if (gpio.Read(indexPin) == PinValue.High)
                                {
                                    EndCycleWithIndex();
                                }
                                else if (gpio.Read(BlackboxPin) == PinValue.High)
                                {
                                    EndCycleWithBlackbox();
                                }
                                else if (gpio.Read(pulsePin) == PinValue.High)
                                {
                                    state = 4;
                                    pulseCounter++;
                                    totalPulseCounter++;
                                }
                                break;
                            }
                        case 4:
                            {
                                if (gpio.Read(indexPin) == PinValue.High)
                                {
                                    EndCycleWithIndex();
                                }
                                else if (gpio.Read(BlackboxPin) == PinValue.High)
                                {
                                    EndCycleWithBlackbox();
                                }
                                else if (gpio.Read(pulsePin) == PinValue.Low)
                                {
                                    state = 3;
                                }
                                break;
                            }
                        case 5: //case 5 and 6 get how many pulse we have between blackbox and next index
                            {
                                if (gpio.Read(indexPin) == PinValue.High)
                                {
                                    EndCycleOfIndex();
                                }
                                else if (gpio.Read(pulsePin) == PinValue.High)
                                {
                                    pulseCounter++;
                                    totalPulseCounter++;
                                    state = 6;
                                }
                                break;
                            }
                        case 6:
                            {
                                if (gpio.Read(indexPin) == PinValue.High)
                                {
                                    EndCycleOfIndex();
                                }
                                else if (gpio.Read(pulsePin) == PinValue.Low)
                                {
                                    state = 5;
                                }
                                break;
                            }
                    }
                    #endregion

                    //function called too many time to get save(sw.Fluch();) each time 
                    void EndCycleWithIndex()
                    {
                        indexCounter++;
                        Console.WriteLine($"Index : {indexCounter} | pulse : {pulseCounter} | direction : {direction}");
                        swDA.WriteLine($"1,0,{indexCounter},{pulseCounter},{direction}");
                        pulseCounter = 0;
                        state = 2;
                    }

                    void EndCycleWithBlackbox()
                    {
                        indexCounter++;
                        cycleCounter++;
                        Console.WriteLine($"Index : {indexCounter} | pulse : {pulseCounter} | direction : {direction}");
                        swDA.WriteLineAsync($"0,1,{indexCounter},{pulseCounter},{direction}");
                        pulseCounter = 0;
                        state = 5;
                    }

                    void EndCycleOfIndex()
                    {
                        swDA.WriteLine($"1,0,{indexCounter},{pulseCounter},{direction}");
                        Console.WriteLine($"Index : {indexCounter} | pulse : {pulseCounter} | direction : {direction}");
                        swDA.Flush();
                        pulseCounter = 0;
                        totalPulseCounter = 0;
                        indexCounter = 0;
                        state = 0;
                    }
                }
            }
        }
    }
}

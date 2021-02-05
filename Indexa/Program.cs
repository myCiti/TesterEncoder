using System;
using System.Device.Gpio;
using System.IO;

namespace Indexa
{
    class Program
    {
        static void Main(string[] args)
        {
            var gpio = new GpioController();
            const int indexPin = 4;
            gpio.OpenPin(indexPin, PinMode.Input);
            const int pulsePin = 17;
            gpio.OpenPin(pulsePin, PinMode.Input);
            const int BlackboxPin = 50;
            gpio.OpenPin(BlackboxPin, PinMode.Input);
             
            //start program
            #region 
            var path = "Log";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            int nbrCycles;
            Console.WriteLine("En attente de l'utilisateur concernant le nombre de cycles par fichier ainsi que le nombre de fichiers :");
            Console.WriteLine("Nbr Cycles : ");
            while ((nbrCycles = Convert.ToInt32(Console.ReadLine())) == 0)
            {
                Console.WriteLine("Merci d'écrire un nombre");
                Console.WriteLine("Nbr Cycles : ");
            }
            Console.WriteLine($"Le nombre de cycles à été défini à : {nbrCycles}");

            int nbrFichiers;
            Console.WriteLine("Nbr fichiers : ");
            while ((nbrFichiers = Convert.ToInt32(Console.ReadLine())) == 0)
            {
                Console.WriteLine("Merci d'écrire un nombre");
                Console.WriteLine("Nbr fichiers : ");
            }
            Console.WriteLine($"Le nombre de fichiers à été défini à : {nbrFichiers}");

            Console.WriteLine($"Le programme va éxécuter {nbrCycles} de cycles dans {nbrFichiers} de fichiers.");
            #endregion

            for (int i = 1; i <= nbrFichiers; i++)
            {
                StreamWriter sw = new StreamWriter($"{path}/ResultOfSimulation#{i}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");

                sw.WriteLine($"SimulationStartAt = {DateTime.Now:yyyyMMdd_HHmmss}");
                sw.WriteLine("EachLineIsComposedOF:\nIndexCounter,Counter,StopByIndex, StopByBlackBox");

                int cycleCounter = 0;
                int pulseCounter = 0;
                int state = 0;

                while (cycleCounter <= nbrCycles)
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
                                    EndCycleWithIndex();
                                }
                                else if ((int)gpio.Read(BlackboxPin) == 1)
                                {
                                    EndCycleWithBlackbox();
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
                                    EndCycleWithIndex();
                                }
                                else if ((int)gpio.Read(BlackboxPin) == 1)
                                {
                                    EndCycleWithBlackbox();
                                }
                                else if ((int)gpio.Read(pulsePin) == 0)
                                {
                                    state = 2;
                                }
                                break;
                            }
                    }
                }
                sw.WriteLine($"SimulationEndedAt = {DateTime.Now:yyyyMMdd_HH:mm:ss}");
                sw.Close();
                Console.WriteLine($"Test #{i} terminé");

                void EndCycleWithIndex()
                {
                    state = 1;
                    cycleCounter++;
                    Console.WriteLine($"Index : {cycleCounter} | pulse : {pulseCounter}");
                    sw.WriteLineAsync($"{cycleCounter}, {pulseCounter}, 1, 0");
                    pulseCounter = 0;
                }

                void EndCycleWithBlackbox()
                {
                    state = 1;
                    cycleCounter++;
                    Console.WriteLine($"Index : {cycleCounter} | pulse : {pulseCounter}");
                    sw.WriteLineAsync($"{cycleCounter}, {pulseCounter}, 0, 1");
                    pulseCounter = 0;
                }
            }
            Console.WriteLine("Simulation terminé");
        }
    }
}

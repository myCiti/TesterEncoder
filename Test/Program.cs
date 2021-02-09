using System;
using System.Threading;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

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

            Console.WriteLine($"Le programme va éxécuter {nbrCycles} de cycles dans {nbrFichiers} fichiers.");


            Thread.Sleep(10_000);
        }
    }
}

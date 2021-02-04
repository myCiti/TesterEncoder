using System;
using System.Globalization;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
           
            decimal t = 5215525152452552245.2645255m;
            int a = 9099;
            Console.WriteLine($"{a} is : " +
                t.ToString("#,##0.000", new CultureInfo("fr-CA")));
            Console.ReadLine();
        }
    }
}

using System;

class Program
{
    static void Main()
    {
        Console.WriteLine(new string('=', 50));
        Console.WriteLine("       KALKULATOR KREDYTOWY");
        Console.WriteLine(new string('=', 50));

        double kwota = WczytajDouble("Podaj kwotę kredytu (zł): ");
        int miesiace = WczytajInt("Podaj liczbę miesięcy spłaty: ");
        double oprocentowanie = WczytajDouble("Podaj oprocentowanie roczne (%): ");

        double stopa = oprocentowanie / 100 / 12;

        double rata;
        if (stopa > 0)
        {
            rata = kwota * (stopa * Math.Pow(1 + stopa, miesiace)) /
                   (Math.Pow(1 + stopa, miesiace) - 1);
        }
        else
        {
            rata = kwota / miesiace;
        }

        rata = Math.Round(rata, 2);

        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("       PODSUMOWANIE KREDYTU");
        Console.WriteLine(new string('=', 50));
        Console.WriteLine($"Kwota kredytu:         {kwota,12:N2} zł");
        Console.WriteLine($"Okres spłaty:          {miesiace,12} miesięcy");
        Console.WriteLine($"Oprocentowanie roczne: {oprocentowanie,12:N2} %");
        Console.WriteLine($"Rata miesięczna:       {rata,12:N2} zł");

        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine("       HARMONOGRAM SPŁAT");
        Console.WriteLine(new string('=', 70));
        Console.WriteLine($"{"Nr",-4} {"Rata",-12} {"Kapitał",-12} {"Odsetki",-12} {"Pozostało",-14}");
        Console.WriteLine(new string('-', 70));

        double saldo = kwota;
        double sumaOdsetek = 0;
        double sumaKapitalu = 0;

        for (int m = 1; m <= miesiace; m++)
        {
            double odsetki = Math.Round(saldo * stopa, 2);
            double kapital = Math.Round(rata - odsetki, 2);

            // korekta ostatniej raty
            if (m == miesiace)
            {
                kapital = saldo;
                rata = kapital + odsetki;
            }

            saldo = Math.Round(saldo - kapital, 2);
            if (saldo < 0) saldo = 0;

            sumaOdsetek += odsetki;
            sumaKapitalu += kapital;

            Console.WriteLine($"{m,-4} {rata,-12:N2} {kapital,-12:N2} {odsetki,-12:N2} {saldo,-14:N2}");
        }

        double calkowityKoszt = kwota + sumaOdsetek;

        Console.WriteLine(new string('-', 70));
        Console.WriteLine($"\n{"PODSUMOWANIE KOŃCOWE",70}");
        Console.WriteLine(new string('=', 70));
        Console.WriteLine($"Suma spłaconego kapitału:  {sumaKapitalu,12:N2} zł");
        Console.WriteLine($"Suma zapłaconych odsetek:  {sumaOdsetek,12:N2} zł");
        Console.WriteLine($"Całkowity koszt kredytu:   {calkowityKoszt,12:N2} zł");
        Console.WriteLine($"Nadpłata (odsetki):        {sumaOdsetek,12:N2} zł");
        Console.WriteLine(new string('=', 70));

        Console.WriteLine("\nNaciśnij dowolny klawisz, aby zakończyć...");
        Console.ReadKey();
    }

    static double WczytajDouble(string komunikat)
    {
        while (true)
        {
            Console.Write(komunikat);
            if (double.TryParse(Console.ReadLine(), out double wynik))
                return wynik;

            Console.WriteLine("Błąd: wpisz poprawną liczbę.");
        }
    }

    static int WczytajInt(string komunikat)
    {
        while (true)
        {
            Console.Write(komunikat);
            if (int.TryParse(Console.ReadLine(), out int wynik))
                return wynik;

            Console.WriteLine("Błąd: wpisz poprawną liczbę całkowitą.");
        }
    }
}

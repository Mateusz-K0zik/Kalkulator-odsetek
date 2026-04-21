using System;
using System.Collections.Generic;

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

        Loan loan = new Loan(kwota, miesiace, oprocentowanie);
        AmortizationSchedule schedule = new AmortizationSchedule(loan);

        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("       PODSUMOWANIE KREDYTU");
        Console.WriteLine(new string('=', 50));
        Console.WriteLine($"Kwota kredytu:         {loan.Amount,12:N2} zł");
        Console.WriteLine($"Okres spłaty:          {loan.Months,12} miesięcy");
        Console.WriteLine($"Oprocentowanie roczne: {loan.AnnualInterestRate,12:N2} %");
        Console.WriteLine($"Rata miesięczna:       {loan.MonthlyInstallment,12:N2} zł");

        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine("       HARMONOGRAM SPŁAT");
        Console.WriteLine(new string('=', 70));
        Console.WriteLine($"{"Nr",-4} {"Rata",-12} {"Kapitał",-12} {"Odsetki",-12} {"Pozostało",-14}");
        Console.WriteLine(new string('-', 70));

        foreach (Payment payment in schedule.Payments)
        {
            Console.WriteLine(
                $"{payment.Number,-4} {payment.Installment,-12:N2} {payment.Capital,-12:N2} {payment.Interest,-12:N2} {payment.RemainingBalance,-14:N2}");
        }

        Console.WriteLine(new string('-', 70));
        Console.WriteLine($"\n{"PODSUMOWANIE KOŃCOWE",70}");
        Console.WriteLine(new string('=', 70));
        Console.WriteLine($"Suma spłaconego kapitału:  {schedule.TotalCapital,12:N2} zł");
        Console.WriteLine($"Suma zapłaconych odsetek:  {schedule.TotalInterest,12:N2} zł");
        Console.WriteLine($"Całkowity koszt kredytu:   {schedule.TotalCost,12:N2} zł");
        Console.WriteLine($"Nadpłata (odsetki):        {schedule.TotalInterest,12:N2} zł");
        Console.WriteLine(new string('=', 70));

        Console.WriteLine("\nNaciśnij dowolny klawisz, aby zakończyć...");
        Console.ReadKey();
    }

    static double WczytajDouble(string komunikat)
    {
        while (true)
        {
            Console.Write(komunikat);
            if (double.TryParse(Console.ReadLine(), out double wynik) && wynik >= 0)
                return wynik;

            Console.WriteLine("Błąd: wpisz poprawną liczbę.");
        }
    }

    static int WczytajInt(string komunikat)
    {
        while (true)
        {
            Console.Write(komunikat);
            if (int.TryParse(Console.ReadLine(), out int wynik) && wynik > 0)
                return wynik;

            Console.WriteLine("Błąd: wpisz poprawną liczbę całkowitą większą od zera.");
        }
    }
}

class Loan
{
    public double Amount { get; }
    public int Months { get; }
    public double AnnualInterestRate { get; }
    public double MonthlyInterestRate => AnnualInterestRate / 100 / 12;
    public double MonthlyInstallment { get; }

    public Loan(double amount, int months, double annualInterestRate)
    {
        Amount = amount;
        Months = months;
        AnnualInterestRate = annualInterestRate;
        MonthlyInstallment = CalculateMonthlyInstallment();
    }

    private double CalculateMonthlyInstallment()
    {
        double stopa = MonthlyInterestRate;

        if (stopa > 0)
        {
            double rata = Amount * (stopa * Math.Pow(1 + stopa, Months)) /
                          (Math.Pow(1 + stopa, Months) - 1);

            return Math.Round(rata, 2);
        }

        return Math.Round(Amount / Months, 2);
    }
}

class Payment
{
    public int Number { get; set; }
    public double Installment { get; set; }
    public double Capital { get; set; }
    public double Interest { get; set; }
    public double RemainingBalance { get; set; }
}

class AmortizationSchedule
{
    public Loan Loan { get; }
    public List<Payment> Payments { get; }
    public double TotalInterest { get; private set; }
    public double TotalCapital { get; private set; }
    public double TotalCost => Loan.Amount + TotalInterest;

    public AmortizationSchedule(Loan loan)
    {
        Loan = loan;
        Payments = GenerateSchedule();
    }

    private List<Payment> GenerateSchedule()
    {
        List<Payment> payments = new List<Payment>();

        double saldo = Loan.Amount;
        double rata = Loan.MonthlyInstallment;
        double stopa = Loan.MonthlyInterestRate;

        for (int m = 1; m <= Loan.Months; m++)
        {
            double odsetki = Math.Round(saldo * stopa, 2);
            double kapital = Math.Round(rata - odsetki, 2);
            double aktualnaRata = rata;

            // Korekta ostatniej raty
            if (m == Loan.Months)
            {
                kapital = saldo;
                aktualnaRata = kapital + odsetki;
            }

            saldo = Math.Round(saldo - kapital, 2);
            if (saldo < 0) saldo = 0;

            TotalInterest += odsetki;
            TotalCapital += kapital;

            payments.Add(new Payment
            {
                Number = m,
                Installment = aktualnaRata,
                Capital = kapital,
                Interest = odsetki,
                RemainingBalance = saldo
            });
        }

        TotalInterest = Math.Round(TotalInterest, 2);
        TotalCapital = Math.Round(TotalCapital, 2);

        return payments;
    }
}

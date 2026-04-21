import tkinter as tk
from tkinter import messagebox, ttk

class Payment:
    def __init__(self, number, installment, principal, interest, balance):
        self.number = number
        self.installment = installment
        self.principal = principal
        self.interest = interest
        self.balance = balance

class Loan:
    def __init__(self, amount, months, annual_interest_rate):
        self.amount = amount
        self.months = months
        self.annual_interest_rate = annual_interest_rate

    @property
    def monthly_interest_rate(self):
        return self.annual_interest_rate / 100 / 12

    def calculate_installment(self):
        rate = self.monthly_interest_rate

        if rate > 0:
            installment = self.amount * (
                rate * (1 + rate) ** self.months
            ) / ((1 + rate) ** self.months - 1)
        else:
            installment = self.amount / self.months

        return round(installment, 2)

class AmortizationSchedule:
    def __init__(self, loan):
        self.loan = loan
        self.payments = []

    def generate(self):
        self.payments.clear()

        balance = self.loan.amount
        installment = self.loan.calculate_installment()
        rate = self.loan.monthly_interest_rate

        for month in range(1, self.loan.months + 1):
            interest = round(balance * rate, 2)
            principal = round(installment - interest, 2)

            if month == self.loan.months:
                principal = round(balance, 2)
                installment = round(principal + interest, 2)

            balance = round(balance - principal, 2)

            if balance < 0:
                balance = 0

            payment = Payment(
                number=month,
                installment=installment,
                principal=principal,
                interest=interest,
                balance=balance
            )
            self.payments.append(payment)

        return self.payments

def oblicz():
    try:
        kwota = float(entry_kwota.get())
        miesiace = int(entry_miesiace.get())
        oprocentowanie = float(entry_oprocentowanie.get())

        if kwota <= 0:
            raise ValueError("Kwota kredytu musi być większa od zera.")
        if miesiace <= 0:
            raise ValueError("Liczba miesięcy musi być większa od zera.")
        if oprocentowanie < 0:
            raise ValueError("Oprocentowanie nie może być ujemne.")

        loan = Loan(kwota, miesiace, oprocentowanie)
        schedule = AmortizationSchedule(loan)
        payments = schedule.generate()

        rata = loan.calculate_installment()
        wynik_var.set(f"Rata miesięczna: {rata:,.2f} zł")

        for row in tree.get_children():
            tree.delete(row)

        for payment in payments:
            tag = "even" if payment.number % 2 == 0 else "odd"

            tree.insert("", "end", values=(
                payment.number,
                f"{payment.installment:,.2f}",
                f"{payment.principal:,.2f}",
                f"{payment.interest:,.2f}",
                f"{payment.balance:,.2f}"
            ), tags=(tag,))

    except ValueError as e:
        messagebox.showerror("Błąd", str(e))

root = tk.Tk()
root.title("Kalkulator kredytowy")
root.geometry("700x500")

frame_input = tk.Frame(root)
frame_input.pack(pady=10)

tk.Label(frame_input, text="Kwota kredytu (zł)").grid(row=0, column=0, padx=5, pady=5)
entry_kwota = tk.Entry(frame_input)
entry_kwota.grid(row=0, column=1, padx=5)

tk.Label(frame_input, text="Liczba miesięcy").grid(row=1, column=0, padx=5, pady=5)
entry_miesiace = tk.Entry(frame_input)
entry_miesiace.grid(row=1, column=1, padx=5)

tk.Label(frame_input, text="Oprocentowanie (%)").grid(row=2, column=0, padx=5, pady=5)
entry_oprocentowanie = tk.Entry(frame_input)
entry_oprocentowanie.grid(row=2, column=1, padx=5)

tk.Button(frame_input, text="Oblicz", command=oblicz).grid(row=3, column=0, columnspan=2, pady=10)

wynik_var = tk.StringVar()
tk.Label(root, textvariable=wynik_var, font=("Arial", 12, "bold")).pack()

frame_table = tk.Frame(root)
frame_table.pack(fill="both", expand=True, padx=10, pady=10)

tree = ttk.Treeview(
    frame_table,
    columns=("nr", "rata", "kapital", "odsetki", "saldo"),
    show="headings"
)
tree.pack(side="left", fill="both", expand=True)

tree.heading("nr", text="Nr")
tree.heading("rata", text="Rata")
tree.heading("kapital", text="Kapitał")
tree.heading("odsetki", text="Odsetki")
tree.heading("saldo", text="Pozostało")

tree.column("nr", width=50, anchor="center")
tree.column("rata", anchor="e", width=100)
tree.column("kapital", anchor="e", width=100)
tree.column("odsetki", anchor="e", width=100)
tree.column("saldo", anchor="e", width=120)

scrollbar = ttk.Scrollbar(frame_table, orient="vertical", command=tree.yview)
tree.configure(yscroll=scrollbar.set)
scrollbar.pack(side="right", fill="y")

tree.tag_configure("odd", background="#f2f2f2")
tree.tag_configure("even", background="white")

root.mainloop()

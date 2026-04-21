import tkinter as tk
from tkinter import messagebox, ttk

def oblicz():
    try:
        kwota = float(entry_kwota.get())
        miesiace = int(entry_miesiace.get())
        oprocentowanie = float(entry_oprocentowanie.get())

        stopa = oprocentowanie / 100 / 12

        if stopa > 0:
            rata = kwota * (stopa * (1 + stopa)**miesiace) / ((1 + stopa)**miesiace - 1)
        else:
            rata = kwota / miesiace

        rata = round(rata, 2)

        wynik_var.set(f"Rata miesięczna: {rata:,.2f} zł")

        for row in tree.get_children():
            tree.delete(row)

        saldo = kwota

        for m in range(1, miesiace + 1):
            odsetki = round(saldo * stopa, 2)
            kapital = round(rata - odsetki, 2)
            saldo = round(saldo - kapital, 2)

            if saldo < 0:
                saldo = 0

            tag = 'even' if m % 2 == 0 else 'odd'

            tree.insert("", "end", values=(
                m,
                f"{rata:,.2f}",
                f"{kapital:,.2f}",
                f"{odsetki:,.2f}",
                f"{saldo:,.2f}"
            ), tags=(tag,))

    except ValueError:
        messagebox.showerror("Błąd", "Podaj poprawne dane!")

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

tree = ttk.Treeview(frame_table, columns=("nr", "rata", "kapital", "odsetki", "saldo"), show="headings")
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

tree.tag_configure('odd', background='#f2f2f2')
tree.tag_configure('even', background='white')

root.mainloop()
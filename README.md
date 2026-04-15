# FinanceTracker

Applicazione WPF per tracciare entrate, spese e abbonamenti con supporto per entrate ricorrenti (stipendio) e un grafico mensile semplice.

Requisiti
- .NET 10 SDK
- Windows (WPF)

Build e esecuzione
- Da CLI: `dotnet build` e `dotnet run --project FinanceTracker.csproj`
- In Visual Studio: apri la soluzione e avvia il progetto.

Struttura principale
- `MainWindow.xaml` / `MainWindow.xaml.cs` — UI e logica principale.
- `Item.cs`, `Income.cs`, `Expense.cs`, `Subscription.cs` — modelli dati.
- `Data/` — file di persistenza (testo):
  - `UserData.txt` — `Name;Surname;Balance`
  - `SubscriptionData.txt` — `Subscription;Name;Description;Date;Price;Payed`
  - `IncomeData.txt` — righe `Transaction;...` o `Income;...`
  - `StipendioData.txt` — `Name;Description;Date;Price;Recurring`

Funzionalità
- Aggiunta e visualizzazione di: abbonamenti, entrate, spese.
- Abbonamenti pagati vengono registrati come spese immediate e aggiornano il saldo.
- Entrate ricorrenti (stipendio): caricate da `StipendioData.txt`, aggiunte automaticamente ogni mese se non già presenti.
- Grafico "Andamento Mensile": mostra riepilogo entrate vs uscite del mese corrente.

Note importanti
- I file dati usano `;` come separatore e le date devono essere in un formato parsabile da `DateTime.Parse` (consigliato ISO: `yyyy-MM-dd`).
- Se non visualizzi voci previste, verifica i file in `Data/` e riavvia l'app.
- Il loop di background aggiorna i dati periodicamente (1s di default).

Suggerimenti per sviluppatori
- Miglioramenti consigliati: usare formati di data ISO, centralizzare I/O in una classe di repository, deduplicare righe prima di salvare.

The MIT License (MIT)

Copyright (c) 2011-2026 The Bootstrap Authors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.


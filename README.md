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

Licenza
- Aggiungi qui la licenza che preferisci (es. MIT).


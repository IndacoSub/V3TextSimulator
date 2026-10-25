# Organizzazione della riscrittura v25

La base è il ramo `v25`, commit `6601709c85e48cc17376fd700f40f58a47f02e4c`. La finestra principale mantiene il nome `Operations`: la riscrittura comprende il suo ciclo di vita, il layout, l'inizializzazione, i comandi e le due finestre secondarie.

Le classi parziali distribuiscono le responsabilità della stessa finestra in file distinti. Non rappresentano finestre o programmi diversi. `Program.cs` inizializza Windows Forms e avvia `Operations`.

## Dalla struttura originale alla nuova

| Responsabilità nella v25 originale | Nuova collocazione |
| --- | --- |
| `Operations.cs`: costruzione e stato della finestra principale | `Operations.cs`: stato condiviso e ciclo di vita della form |
| `Operations.Designer.cs`: definizione dei controlli | `Operations.Designer.cs`: campi dei controlli e `InitializeComponent`, separati dai comandi di traduzione |
| `Init.cs`: creazione dei manager, impostazioni iniziali, eventi | `Init.cs`: inizializzazione del banco di lavoro, collegamento degli eventi e apertura iniziale |
| `Misc.cs`: navigazione, testo selezionato, salvataggio delle modifiche in memoria, autoplay | `Misc.cs`: selezione della battuta, sincronizzazione dell'editor, conteggi, ricerca e scorrimento automatico |
| `Operations.cs`: apertura e aggiornamento delle finestre secondarie | `WindowCoordinator.cs`: creazione, riapertura, sincronizzazione e chiusura delle finestre secondarie |
| `GUIFunctionsButton.cs` e `FileManager.cs`: apertura, salvataggio, conversione ed esportazioni | `DocumentCommands.cs`: interazione con i dialoghi di file; `DGRV3TS.Core`: lettura, scrittura e conservazione dei dati |
| `VariableManager.cs`, `Listboxes.cs`, `GUIFunctionsListbox.cs`: categorie e uso delle variabili | `VariableCommands.cs`: pannello e comandi; `VariableCatalog` nel core: lettura e risoluzione del dizionario esterno |
| `TLDictionary.cs` e scelta delle traduzioni esistenti | `TranslationCommands.cs`, `SuggestionDialog` in `EditorDialogs.cs` e `TranslationMemory` nel core |
| `HalfBoxTester.cs`, `DisplayImage.cs`, `TextRender.cs`, `FontManager.cs`, `SoundManager.cs` | `DialoguePreviewWindow.cs` e `MediaAssets.cs`: anteprima grafica, risorse, font, WAV, TTS e screenshot |
| `VerticalView.cs` e `Subforms/VerticalSummary` | `VerticalView.cs`: riepilogo secondario dell'intero file con schede disegnate in base alla parte visibile |

`Ui.cs` raccoglie gli elementi grafici comuni. `EditorDialogs.cs` contiene le finestre modali di inserimento, sostituzione e scelta dei suggerimenti. I codec e i modelli del core rimangono indipendenti da Windows Forms, così da poter essere verificati anche su Linux.

La vista verticale rimane **una finestra secondaria**: non è una colonna dell'editor principale. L'altra finestra secondaria è l'anteprima grafica orizzontale. Nessuna delle due deve diventare un requisito per salvare una traduzione.

## Comandi e flussi da preservare

- **Apertura:** il nuovo documento sostituisce quello corrente solo dopo una lettura riuscita. L'argomento iniziale di `Program.cs` consente l'apertura di un file dalla riga di comando.
- **Modifica:** la textbox aggiorna la traduzione della battuta e della lingua selezionate in memoria; l'originale rimane separato e di sola lettura.
- **Navigazione:** frecce, numero di battuta, ricerca, reset, fast-read e clic sulle schede agiscono sulla stessa selezione. Cambiare lingua aggiorna anche il riepilogo.
- **Salvataggio:** `Ctrl+S` salva il documento corrente; **Salva con nome / converti** permette di scegliere il formato. JSON conserva il progetto completo. Backup e rilevamento di modifiche esterne appartengono al core.
- **Esportazione degli originali:** produce una copia dei testi sorgenti senza sostituire il documento aperto. L'esportazione delle battute con voce non deve alterare l'elenco di lavoro.
- **Variabili:** caricamento e ricarica di `VARS_BAK`/`VARS_AI`, categorie, ricerca, note, inserimento del token e valori alternativi. La risoluzione per l'anteprima non cambia il testo da salvare.
- **Memoria:** la scansione legge traduzioni esistenti; i suggerimenti in conflitto richiedono una scelta. Il completamento multiplo applica solo candidati univoci alle battute non completate.
- **Autoplay:** pausa, ripresa e stop non devono bloccare l'interfaccia. Prima di un'operazione di modifica occorre fermare la navigazione, per evitare di applicare il testo a un'altra battuta.
- **Finestre secondarie:** chiuderle e riaprirle non deve perdere dati o lasciare riferimenti a controlli eliminati. Copia/incolla nel riepilogo modifica la battuta corretta nella lingua corrente.

## Verifica della consegna

Dalla cartella che contiene `DGRV3TS.sln`:

```sh
dotnet build DGRV3TS.sln -c Release --warnaserror
dotnet run --project DGRV3TS.Core.Tests -c Release --no-build
```

Il secondo comando è il runner di regressione del repository. Una compilazione riuscita non dimostra che l'interfaccia sia stata eseguita su Windows.

Su Windows, con SDK .NET 8:

```sh
dotnet run --project DGRV3TS -- examples/dialoghi.po
```

Controlla almeno questi passaggi prima di considerare verificata l'interfaccia:

1. Avvio, apertura dell'esempio e presenza della finestra principale con originale, textbox e variabili.
2. Apertura della seconda finestra verticale: tutte le battute devono mostrare originale e traduzione insieme. Clic, copia e incolla devono agire sulla battuta corretta senza perdere lo scorrimento.
3. Modifica, cambio battuta e cambio lingua; salvataggio e riapertura del progetto JSON. Prova anche una traduzione intenzionalmente vuota.
4. Annullamento di apertura o salvataggio e chiusura con modifiche: il documento deve rimanere aperto se l'operazione viene annullata.
5. Esportazione degli originali in una copia, poi verifica che il documento e le traduzioni correnti siano invariati.
6. Autoplay attivo seguito da scrittura, sostituzione, scelta di un suggerimento o incolla da una scheda: nessuna modifica deve finire su una battuta diversa.
7. Ridimensionamento della finestra e scala Windows del 150%: editor e comandi devono restare utilizzabili.
8. Chiusura e riapertura di entrambe le finestre secondarie. Per anteprima, audio e screenshot usa risorse proprie e segui la checklist completa in [VERIFICA-WINDOWS.md](VERIFICA-WINDOWS.md).

Questa sezione descrive verifiche manuali da svolgere; non afferma che siano già state eseguite nell'ambiente Linux.

## Archivio ZIP dei sorgenti

Un archivio dei sorgenti deve contenere soluzione, progetti, codice, test, documentazione, esempi e licenze. Non deve contenere `.git`, `bin`, `obj`, cache, backup, file temporanei o risorse personali del gioco.

Dopo l'estrazione, apri `DGRV3TS.sln` oppure esegui i comandi precedenti dalla cartella estratta. Lo ZIP dei sorgenti non è un'applicazione Windows già installata: per compilarlo serve l'SDK .NET 8. Una consegna eseguibile è un archivio distinto, con tutti i file prodotti dalla pubblicazione e con i relativi requisiti di runtime dichiarati.

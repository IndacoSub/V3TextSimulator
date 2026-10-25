# V3 Text — riscrittura della versione v25

Questa riscrittura parte **esclusivamente dal ramo `v25`, commit `6601709c85e48cc17376fd700f40f58a47f02e4c`**. Sostituisce interfaccia, gestione dei documenti e funzioni accessorie, mantenendo il flusso di lavoro di quella versione. Il programma serve prima di tutto per tradurre e revisionare testi.

## Le tre finestre

| Finestra | Funzione |
| --- | --- |
| **Editor principale (`Operations`)** | Originale di sola lettura, textbox della traduzione, navigazione, lingue, ricerca e variabili esterne. Non contiene un elenco verticale dei dialoghi. |
| **Vista verticale separata** (`F6`) | Riepilogo scorrevole di tutte le battute del file: ID, personaggio, originale e traduzione insieme. Il clic seleziona la battuta nell'editor; il menu contestuale copia o incolla la traduzione. Le schede si aggiornano mentre scrivi, senza ricreare migliaia di controlli. |
| **Anteprima grafica separata** (`F7`) | Finestra orizzontale con sfondo, sprite, testo con tag CLT, font, audio e screenshot. È accessoria: nessuna risorsa del gioco è necessaria per tradurre. |

La vista verticale si apre quando viene caricato un documento. Puoi chiuderla e riaprirla dal menu. Mantiene lo scorrimento mentre modifichi una battuta, si sincronizza con la lingua selezionata e distingue una traduzione intenzionalmente vuota da una battuta ancora da tradurre.

## Avvio e compilazione

Serve **SDK .NET 8, versione 8.0.400 o successiva della stessa serie**. L'interfaccia richiede Windows; il core e le regressioni possono essere eseguiti anche su Linux. Il progetto non richiede pacchetti NuGet esterni all'SDK e ai framework Microsoft.

```sh
dotnet build DGRV3TS.sln -c Release --warnaserror
dotnet run --project DGRV3TS.Core.Tests -c Release --no-build
```

Il secondo comando è il runner di regressione: termina con codice diverso da zero se un controllo fallisce. Non usare `dotnet test` per questa suite.

Su Windows:

```sh
dotnet run --project DGRV3TS
dotnet run --project DGRV3TS -- examples/dialoghi.po
```

L'argomento iniziale conserva il flusso “Apri con” della v25. Il file di esempio ha un dizionario `VARS_BAK` accanto e metadati personaggio/espressione/voce. Non distribuisce immagini o audio del gioco.

## Tradurre

Apri PO, VO, TXT, STX, XLSX o un progetto JSON. Scrivi nella textbox **Traduzione**; cambiando battuta o lingua le modifiche rimangono in memoria. L'originale resta separato. Le variabili inserite restano token nel file; solo l'anteprima ne risolve il valore.

- `Ctrl+O`, `Ctrl+S`, `Ctrl+Maiusc+S`: apri, salva, salva con nome/conversione.
- `Ctrl+↑/↓`: battuta precedente/successiva; numero di riga + **Vai** per saltare direttamente.
- `Ctrl+F`, `Ctrl+H`: ricerca e sostituzione. La sostituzione multipla riguarda tutte le battute della lingua selezionata e richiede conferma.
- `F8`: prossima battuta non tradotta; **Segna traduzione intenzionalmente vuota** serve per esportare una stringa vuota deliberatamente.
- `Ctrl+Z` / `Ctrl+Y`: annulla/ripristina nella textbox corrente. La cronologia della textbox viene azzerata cambiando battuta.
- **Riparti dalla prima battuta** e **Fast-read** conservano rispettivamente reset e salto rapido all'ultima battuta della v25.

Gli avvisi confrontano il numero di tag, variabili e placeholder .NET/printf tra originale e traduzione. Non verificano la qualità linguistica. Lo scorrimento automatico usa un timer dell'interfaccia, con pausa/ripresa/stop; si ferma quando inizi a modificare o apri operazioni modali, evitando scritture sulla battuta sbagliata. Le voci WAV e il TTS al cambio battuta sono attivabili dal menu; per i WAV l'intervallo è temporizzato, non basato sulla durata dell'audio.

### Memoria di traduzione

**Indicizza memoria da cartella** legge ricorsivamente traduzioni PO/VO/XLSX/JSON senza modificarle, escludendo il documento corrente, cartelle di build, risorse e collegamenti simbolici. Cerca corrispondenze esatte per originale e lingua, preferendo lo stesso personaggio.

**Suggerimenti** mostra le alternative in conflitto e la provenienza. **Completa vuoti dalla memoria** applica solo candidati univoci e non sovrascrive traduzioni già completate. È il riuso locale della v25, non un servizio di traduzione automatica: non richiede rete o chiavi API.

**Stop** annulla anche un'indicizzazione in corso, mantenendo la memoria precedente. Durante la scansione i comandi che cambiano documento o nome del file sono disabilitati; puoi continuare a modificare il testo. La chiusura annulla la scansione e poi gestisce normalmente le modifiche non salvate.

## File e salvataggi

**JSON è il formato di lavoro completo**: conserva originali, tutte le traduzioni, completamenti intenzionalmente vuoti e il modello del file importato, così da mantenere commenti, metadati e strutture quando viene riesportato. Salva con nome propone JSON.

| Formato | Supporto e limiti |
| --- | --- |
| PO | `msgid`, `msgstr`, commenti, contesti v25 `ID \| origine \| personaggio \| espressione \| voce`, stringhe multilinea ed escape. Plurali, charset dichiarati non UTF-8 ed escape sconosciuti sono rifiutati esplicitamente. Esporta una sola lingua. |
| VO | Metadati legacy, commenti, ultima battuta senza riga vuota finale, più lingue, escape reversibili. Il sentinel legacy `"(blank)"` viene letto come vuoto; quella stringa letterale non è esportabile senza ambiguità. |
| TXT | Una battuta per riga, compreso il contenitore facoltativo `{`/`}`. Gli a capo interni diventano `\n`. |
| STX | Tabella singola, UTF-16LE, ID non sequenziali, stringhe vuote e intestazione conservati. Controllo di puntatori e terminatori; le tabelle multiple sono rifiutate. |
| XLSX | Primo foglio: intestazione, A=ID/OFFSET, B=originale, C e successive=traduzioni. Conserva gli altri fogli e le parti del file importato; non sostituisce formule modificate. Non supporta `.xls`. |

TXT/STX contengono solo il testo finale: le righe ancora da tradurre mantengono il sorgente, quelle segnate intenzionalmente vuote vengono svuotate. Mantieni anche JSON per conservare il confronto originale/traduzione. Il comando di esportazione degli originali non cambia il documento aperto.

Il salvataggio scrive prima un file temporaneo e mantiene la versione precedente in `<file>.bak`. Rileva modifiche esterne al documento aperto e rifiuta di sovrascriverle: salva con un altro nome e confronta. Non c'è autosalvataggio sul disco; `.bak` conserva una sola versione precedente. Aprire/chiudere un documento con modifiche chiede se salvarle, scartarle o annullare.

### STX e WRD

Aprendo uno STX, un WRD omonimo accanto al file viene caricato se presente. Puoi sceglierlo anche dal menu. I metadati vengono associati tramite gli ID LOC, con controlli sui limiti e sugli opcode supportati, e alimentano personaggio/espressione/voce, riepilogo e conversione PO. L'importazione dei metadati segna il progetto come modificato; salva JSON per conservarli insieme al lavoro.

## VARS_BAK e VARS_AI

La selezione V3/AI sceglie rispettivamente `VARS_BAK`/`vars_bak.txt` e `vars_ai.txt`. La ricerca automatica considera cartella del documento, risorse, eseguibile e cartella di lavoro. Puoi caricare un file diverso o ricaricarlo dopo una modifica esterna.

I dizionari individuati automaticamente vengono ricercati aprendo un altro documento, dando priorità alla sua cartella. Un dizionario scelto manualmente viene mantenuto finché non ne scegli un altro o cambi gioco.

```text
// Personaggi
<HERO>, Kaede Akamatsu | Nota mostrata nell'editor
// Glossario
<ACADEMY>, Accademia per talenti : Ultimate Academy | Principale e alternativo
<ACADEMY_MN>, accademia per talenti
```

`//` apre una categoria; le righe senza categoria sono accettate. Il primo `,` separa token e valore, ` : ` l'alternativa, ` | ` il commento. Categorie, ricerca e note sono disponibili nel pannello principale. Doppio clic inserisce il token alla posizione del cursore; il menu contestuale copia token/valore o inserisce una variante `_MN`/`_MS` realmente definita nel file.

Le sostituzioni annidate sono limitate a tre livelli, rispettano i confini e segnalano cicli. Token duplicati o righe malformate producono avvisi; non fanno bloccare l'avvio. Il programma non riscrive il dizionario esterno.

## Risorse e anteprima

Scegli la cartella che contiene `Graphics` e `Sound` da **Finestre e risorse**. I percorsi conservano la struttura v25:

```text
Graphics/Backgrounds/DR/*.png
Graphics/Backgrounds/Danganronpa/*.png    (anche convenzione README precedente)
Graphics/Backgrounds/AI/*.png
Graphics/Sprites/DR/C015_Akama/anim000.png
Graphics/Sprites/DR/C015_Akama/default.png
Sound/Voices/DR/vic_Akama/vic_Akama_01_001.wav
font.ttf oppure font.otf
```

L'anteprima offre sfondo, sprite facoltativi, selezione V3/AI, font installati e TTF/OTF esterni, dimensioni e tag CLT, interlinea fissa/adattiva, WAV, TTS Windows con lingua, copia immagine e PNG. Il rendering è indicativo, non una garanzia di impaginazione identica al gioco. File mancanti o audio non disponibile non impediscono di tradurre.

L'importazione degli sprite mantiene la mappa personaggi v25 e non sovrascrive file esistenti. L'esportazione delle battute con voce comprende tutti i personaggi, senza il filtro Monokuma hardcoded della v25. Il vecchio comando “Setup Voicelines” era esplicitamente non implementato: le voci si usano nelle cartelle documentate.

## Verifiche e struttura

- `DGRV3TS`: `Operations.cs` per stato e ciclo di vita, `Operations.Designer.cs` per il layout, `Init.cs` per eventi e inizializzazione, `Misc.cs` per navigazione e stato dell'editor; comandi e finestre secondarie separati per responsabilità.
- `DGRV3TS.Core`: documenti, formati, memoria di traduzione, variabili, WRD e controlli.
- `DGRV3TS.Core.Tests`: regressioni eseguibili anche su Linux, compresi metadati v25 e conflitti di salvataggio.
- [Verifica Windows](docs/VERIFICA-WINDOWS.md): checklist visiva e audio. Queste verifiche richiedono Windows e non sono eseguite dal runner Linux.
- [Mappa della riscrittura](docs/RISCRITTURA-V25.md): corrispondenza con i file originali v25 e contenuto dello ZIP dei sorgenti.

Per generare un archivio dei sorgenti senza cache, output di compilazione o risorse personali:

```sh
python3 tools/package_source.py
```

L'archivio viene scritto nella cartella `artifacts` accanto al checkout; il comando stampa il percorso. Lo ZIP include soluzione, progetti, test, esempi e licenze e richiede SDK .NET 8 per essere compilato.

La CI configura build Release con avvisi trattati come errori e runner di regressione. Una build riuscita su Linux non dimostra il funzionamento di GUI, SAPI o dispositivi audio Windows.

## Licenza

GPL v3.0, vedi [LICENSE](LICENSE). Conservati i crediti storici FEITS/SciresM e DRV3-STX-TOOL/Liquid S in `ThirdParty` e il changelog originale. La riscrittura non dipende più da EPPlus, ExcelDataReader o System.Speech; per il TTS usa SAPI di Windows. I marchi e le risorse dei giochi appartengono ai rispettivi titolari; questo progetto non è affiliato e non distribuisce tali risorse.

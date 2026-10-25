# Verifica su Windows della riscrittura v25

Questa è una checklist da eseguire su Windows, non l'elenco di test già svolti. Il runner Linux verifica formati e logica, non l'interfaccia né l'audio.

1. Avvia `dotnet run --project DGRV3TS -- examples/dialoghi.po`. L'editor principale deve avere le textbox e le variabili; una seconda finestra verticale deve mostrare tre schede con originale e traduzione insieme.
2. Seleziona una scheda e traduci nella finestra principale. La scheda deve aggiornarsi senza perdere lo scorrimento; cambiando battuta e lingua le modifiche devono rimanere.
3. Prova copia originale/copia traduzione/incolla dal menu di una scheda non selezionata. L'incolla deve modificare quella scheda e selezionarla nell'editor, nella lingua attiva.
4. Chiudi la vista verticale, continua a scrivere e riaprila con F6. Nessuna eccezione; tutti i dati devono essere aggiornati. Prova testi più lunghi dell'altezza della finestra e scala Windows al 150%.
5. Verifica `VARS_BAK`: note `|`, alternative, categorie, ricerca, inserimento nel cursore e ricarica dopo modifica esterna. Cambia V3/AI per `vars_ai.txt`.
6. Elimina un token nella traduzione e controlla l'avviso; ripristinalo. Segna una traduzione intenzionalmente vuota e verifica il riepilogo e il salvataggio JSON.
7. Indicizza una cartella di traduzioni con candidati identici e in conflitto. Suggerimenti deve consentire la scelta; completamento multiplo deve lasciare irrisolti i conflitti e conservare testi già tradotti.
8. Avvia autoplay e poi scrivi, apri Sostituisci o un suggerimento, oppure apri il menu di una scheda: la navigazione deve fermarsi prima della modifica. Verifica pausa/ripresa/stop e fine file.
9. Apri F7. Carica risorse proprie, scegli sfondo e sprite, prova font TTF/OTF, tag CLT e dimensioni. Copia/salva PNG, verifica il file salvato e che le risorse originali restino modificabili esternamente.
10. Prova WAV e TTS con voce installata, lingua, stop e riapertura dell'anteprima. Il testo deve rimanere modificabile anche in assenza di dispositivi o risorse audio.
11. Prova STX+WRD con ID non sequenziali: personaggio, espressione e voce devono corrispondere; salva JSON e poi PO per verificarne la conservazione.
12. Salva e riapri progetti PO/VO/XLSX con più lingue. Verifica `.bak`, annullamento del salvataggio e conflitto quando un editor esterno cambia il documento aperto. Non usare i file originali di lavoro per queste prove.
13. Avvia con un percorso inesistente: la textbox deve restare disabilitata fino all'apertura di un documento valido. Seleziona due volte la stessa scheda: cursore, selezione e annullamento devono rimanere intatti.
14. Indicizza una cartella estesa e premi Stop: deve rimanere disponibile la memoria precedente. Ripeti chiudendo la finestra principale: prima si annulla la scansione, poi viene gestito il salvataggio delle modifiche.
15. Apri due documenti con `VARS_BAK` diversi accanto ai file: ciascuno deve caricare il proprio dizionario automatico. Se scegli un dizionario esplicitamente, deve rimanere selezionato passando all'altro documento.

Limiti espliciti: rendering indicativo; PO senza plurali/non-UTF8; STX a tabella singola; `.xls` non supportato; formule di traduzione XLSX protette. Opcode WRD non riconosciuti vengono rifiutati senza applicare metadati parziali.

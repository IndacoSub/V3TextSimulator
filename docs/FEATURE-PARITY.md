# Feature parity against v25

The minimum acceptance target for the rewrite is **all implemented features of v25**, plus the rewrite's improvements. The reference is commit `6601709c85e48cc17376fd700f40f58a47f02e4c` on branch `v25`, not `main`.

This is a tracked acceptance requirement, not a claim that 100% parity has already been demonstrated. Automated core checks and a successful Windows-targeted build do not establish visual, audio or complete workflow equivalence. No percentage is assigned until the outstanding comparison and Windows checks are complete.

| Original v25 capability | Rewrite evidence and current status |
| --- | --- |
| Main translation editor, source view, editable textbox, line/language navigation | Implemented in `Operations`, its designer, `Init` and `Misc`. Original and translation are kept separate. Windows interaction still needs the manual checks below. |
| A separate tall full-file summary with original/translation cards, speaker colours, selection and copy/paste | Implemented in `VerticalView`; it remains a secondary window, not an inline list. Card scrolling, clipboard interaction and DPI behaviour still require Windows verification. |
| Separate graphical preview, backgrounds, sprites, installed/custom fonts and screenshots | Implemented in `DialoguePreviewWindow` and `MediaAssets`. This is currently a functional preview, not verified rendering parity; see the specific differences below. |
| PO, VO, TXT, STX and XLSX import/export and conversions | Implemented in the core. Regression checks cover EOF records, metadata, language selection, Unicode, escaping, arbitrary STX IDs and round trips. The exact corpus of real v25 project files still needs comparison. |
| Legacy `{` / `}` TXT output from structured formats | Preserved for PO/VO/STX/XLSX and new native projects. Imported unwrapped TXT keeps its layout, including after saving and reopening JSON. Regression checks cover wrappers, blank rows and intentional empty translations. |
| External `vars_bak.txt` / `vars_ai.txt`, categories, comments, alternatives and bounded nested substitutions | Implemented in `VariableCatalog` and the variable panel. This pass restores missing context-menu and clipboard workflows, including session case variants, without rewriting the external dictionary. |
| Local translation-memory reuse and explicit choice between conflicting translations | Implemented in `TranslationMemory` and the suggestion dialog. Exact originals, languages and speaker preference are tested. The rewrite also supports cancellation and preserves files during indexing. Remaining legacy automatic-trigger and marker behaviour is listed below. |
| STX plus WRD character, expression and voice metadata | Implemented with bounds/opcode validation and LOC-ID association; tested fixtures cover the mapping. Real project WRD files should be included in Windows acceptance checks. |
| WAV/TTS playback, language selection, autoplay/pause/resume, reset and fast-read | Implemented. Windows audio output and timing remain unverified. Autoplay currently uses a configured interval for WAV playback rather than waiting for the WAV to finish. |
| Sprite-folder import and voiced-dialogue export | Implemented. Import preserves existing files; voiced export supports all speakers instead of the original hardcoded Monokuma-only filter. |
| Opening a document through the initial command-line argument | Implemented by `Program`/`Operations`; invalid startup paths leave an initialized empty editor. File-association installation itself is not performed automatically. |

## Remaining comparisons and known differences

These are open parity items, not features to silently remove:

- **Exact game rendering:** v25 uses character-specific sprite scaling/cropping, including special cases for Monokubs, Maki and flashback sprites. The rewrite currently uses a general fit. Compare both versions with the same assets before accepting placement parity.
- **AI layouts:** v25 applies a ToWitter-specific text selection rule and an 11-pixel fixed newline height. The rewrite has ToWitter bounds but does not yet reproduce all of those text-processing rules.
- **Control markers and colours:** v25 marks automatically reused text with `<SIGNAL_AutoTL>`, truncates that marker's suffix in its preview and paints the result pink. Its TTS strips signal-delimited suffixes. The rewrite's readable preview and speech currently handle those markers differently. The `cltSYSTEM` colour has been restored to v25's `Color.Green`; full visual comparison still requires Windows.
- **Translation-memory workflow:** v25 can index/apply suggestions automatically while opening supported files, appends an AutoTL marker, and uses legacy candidate heuristics. The rewrite exposes indexing, suggestions and unambiguous completion explicitly. Equivalent access exists, but the trigger/marker workflow still needs a compatibility option or an agreed replacement.
- **Audio timing and voice choice:** test the original and rewritten WAV/TTS paths with the same installed Windows voices, speech language and autoplay settings. A headless Linux build cannot validate these behaviours.
- **Parser restrictions:** PO plurals, non-UTF-8 declarations, unsupported PO escapes and multi-table STX files are rejected rather than partially interpreted. Changing translation formula cells in XLSX is also rejected. These restrictions must be checked against the actual v25 input corpus; a safe rejection is not by itself proof of parity.

## Original placeholders are not completed features

The v25 **Setup Voicelines from Folder** command explicitly displays “Currently unimplemented!”. It is not evidence of a working original importer. The original `OpenFile` extension check accepts `.xlsx` but not `.xls`; the underlying Excel library's broader capability is not a separate accepted-file workflow.

The original `MAKE_(...)` branch is not counted as demonstrated functionality: `AddVariables` filters out `MAKE_` definitions before the replacement code can use them. A future implementation may add it, but parity should be based on a reproducible original workflow rather than unreachable code or TODO labels.

## Acceptance evidence

Run the core regression runner and build described in the [README](../README.md). Then execute the [Windows checklist](VERIFICA-WINDOWS.md) and compare both versions using the same real translation files, dictionaries, fonts, backgrounds, sprites and voices. Record unresolved differences here; do not label the rewrite feature-complete solely because its build passes.

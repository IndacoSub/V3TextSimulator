# Feature parity against v25

The minimum acceptance target for the rewrite is **all implemented features of v25**, plus the rewrite's improvements. The reference is commit `6601709c85e48cc17376fd700f40f58a47f02e4c` on branch `v25`, not `main`.

This is a tracked acceptance requirement, not a claim that 100% parity has already been demonstrated. Automated core checks and a successful Windows-targeted build do not establish visual, audio or complete workflow equivalence. No percentage is assigned until the outstanding comparison and Windows checks are complete.

| Original v25 capability | Rewrite evidence and current status |
| --- | --- |
| Main translation editor, source view, editable textbox, line/language navigation | Implemented in `Operations`, its designer, `Init` and `Misc`. Original and translation are kept separate. Windows interaction still needs the manual checks below. |
| A separate tall full-file summary with original/translation cards, speaker colours, selection and copy/paste | Implemented in `VerticalView`; it remains a secondary window, not an inline list. Card scrolling, clipboard interaction and DPI behaviour still require Windows verification. |
| Separate graphical preview, backgrounds, sprites, installed/custom fonts and screenshots | Implemented in `DialoguePreviewWindow`, `MediaAssets`, `PreviewLayout` and `PreviewTextRules`. v25 geometry, clipping offsets, draw order, point-sized fonts and explicit-line-break defaults are restored and regression-tested. Actual Windows pixel output still needs comparison. |
| PO, VO, TXT, STX and XLSX import/export and conversions | Implemented in the core. Regression checks cover EOF records, optional PO metadata, context-free entries, language selection, Unicode, escaping, arbitrary STX IDs and round trips. `PoOptionalMetadataTests` checks that absent metadata remains absent through translation edits and PO/JSON export. The exact corpus of real v25 project files still needs comparison. |
| Legacy `{` / `}` TXT output from structured formats | Preserved for PO/VO/STX/XLSX and new native projects. Imported unwrapped TXT keeps its layout, including after saving and reopening JSON. Regression checks cover wrappers, blank rows and intentional empty translations. |
| External `vars_bak.txt` / `vars_ai.txt`, categories, comments, alternatives and bounded nested substitutions | Implemented in `VariableCatalog` and the variable panel. This pass restores missing context-menu and clipboard workflows, including session case variants, without rewriting the external dictionary. |
| Local translation-memory reuse and explicit choice between conflicting translations | Implemented in `TranslationMemory`/`TranslationReuse` and the suggestion dialog. Opening PO/VO/XLSX automatically scans the parent directory, supports explicit conflict selection and marks reused strings with AutoTL. Manual text and intentional blanks are protected; cancellation, placeholders and markers have regressions. |
| STX plus WRD character, expression and voice metadata | Implemented with bounds/opcode validation and LOC-ID association; tested fixtures cover the mapping. Real project WRD files should be included in Windows acceptance checks. |
| WAV/TTS playback, language selection, autoplay/pause/resume, reset and fast-read | Implemented with nonblocking WAV completion tracking and SAPI state checks. Autoplay waits for playback; age/gender/chapter hints and hidden speech suffix handling are restored. Windows audio-device and installed-voice behavior still require verification. |
| Sprite-folder import and voiced-dialogue export | Implemented. Import preserves existing files; voiced export supports all speakers instead of the original hardcoded Monokuma-only filter. |
| Opening a document through the initial command-line argument | Implemented by `Program`/`Operations`; invalid startup paths leave an initialized empty editor. File-association installation itself is not performed automatically. |

## Remaining comparisons and known differences

These are open parity items, not features to silently remove:

- **Exact game rendering:** the general-fit regression is fixed. Numeric regression cases now cover regular full bodies, all original mascot IDs, Maki, `_kak` flashbacks, placeholders, default portraits and integer rounding. Compare both versions with identical assets/font files on Windows before accepting pixel parity; geometry tests cannot validate native GDI font rasterization or asset-specific DPI.
- **AI layouts:** ToWitter's message selection, 11-pixel fixed line height, 10-point font and black text are restored, as are AI/tutorial font sizes and positions. Native screenshots still need comparison.
- **Control markers and colours:** AutoTL suffix truncation/pink text and speech truncation at NDR/ALT/AutoTL are restored and covered by pure text tests. CLT colors and size adjustments follow v25; compare native rendering on Windows.
- **Translation-memory candidate heuristics:** the automatic trigger and AutoTL workflow are restored. Automatic candidates deliberately require compatible protected placeholders; v25 used looser heuristics, including variable preference. Candidates needing review remain available through Suggestions with an explicit warning. Compare this behavior with real translation folders rather than silently changing completed strings.
- **Audio timing and voice choice:** test the restored completion tracking, original-language fallback and speaker hints with the same Windows voices and audio devices. A headless Linux build cannot validate SAPI output or WinMM behavior.
- **Parser restrictions:** PO plurals, non-UTF-8 declarations, unsupported PO escapes and multi-table STX files are rejected rather than partially interpreted. Changing translation formula cells in XLSX is also rejected. These restrictions must be checked against the actual v25 input corpus; a safe rejection is not by itself proof of parity.

## Original placeholders are not completed features

The v25 **Setup Voicelines from Folder** command explicitly displays “Currently unimplemented!”. It is not evidence of a working original importer. The original `OpenFile` extension check accepts `.xlsx` but not `.xls`; the underlying Excel library's broader capability is not a separate accepted-file workflow.

The original `MAKE_(...)` branch was unreachable because `AddVariables` filtered out `MAKE_` definitions. The rewrite now implements single-argument MAKE templates, balanced nested calls (including the same template inside itself), recursive variable expansion and alternate values. `VariableExpansionTests.Run` verifies these additions using invented dictionary entries, including cycles, token boundaries and expansion limits. This is implemented functionality beyond that unreachable original branch.

## Acceptance evidence

Run the core regression runner and build described in the [README](../README.md). Then execute the [Windows checklist](WINDOWS-VERIFICATION.md) and compare both versions using the same real translation files, dictionaries, fonts, backgrounds, sprites and voices. Record unresolved differences here; do not label the rewrite feature-complete solely because its build passes.

# V3TextSimulator / DGRV3TS

Danganronpa V3 Text Simulator and translation editor. The program previews Danganronpa V3 conversations and helps translate and review the game's text. Translation is its primary purpose; graphics and audio are optional aids.

**AI: The Somnium Files support remains a work in progress.** The project was originally based on the Fire Emblem If / Fates Text Simulator ("FEFTS" or "FEITS") by [SciresM](https://github.com/SciresM): [SciresM/FEITS](https://github.com/SciresM/FEITS).

This rewrite is based exclusively on **`v25`, commit `6601709c85e48cc17376fd700f40f58a47f02e4c`**. At least 100% feature parity with that version is a requirement, not a claim that every feature has already been verified. See the [feature-parity checklist](docs/FEATURE-PARITY.md) for coverage, remaining gaps and validation status. In particular, Windows UI, audio and exact preview rendering still need Windows validation.

The project is licensed under the **GNU General Public License v3.0 (GPL v3.0)**. Contributions, bug reports and issue reports are welcome and appreciated.

## The three windows

| Window | Purpose |
| --- | --- |
| **Main editor (`Operations`)** | A compact simple view by default, with the translation textbox, navigation, languages and visible variable list. Expand it for the read-only original, search and extra actions. The dialogue list is not embedded in this window. |
| **Separate vertical summary (`F6`)** | A scrollable overview of every dialogue in the file, showing its ID, speaker, original and translation together. Click a card to select that dialogue in the editor; right-click to copy or paste a translation. |
| **Separate graphics preview (`F7`)** | A landscape window with backgrounds, sprites, text styling, fonts, audio and screenshots. Game assets are not required to translate. |

The vertical summary opens when a document is loaded. It can be closed and reopened, follows the selected translation language, updates while you type and preserves the scroll position. Cards distinguish an intentionally empty translation from an untranslated dialogue.

### Simple and extended views

`Operations` starts in **simple view**, designed around a 240-pixel client height at 96 DPI so the three independent windows can be placed together. Use **⏬ Extended view** (`Vista estesa`) at the bottom to expand downward; **⏫ Simple view** (`Vista semplice`) collapses it again. Each mode remembers its own height, while the width, document, selection and textbox undo state are retained.

Simple view hides the original pane, variable Insert/Load/Reload buttons, Copy Original, Suggestions and the search/autoplay toolbar (query, Find Next, untranslated filter, Replace, Play/Resume, Pause, Stop and seconds). The variable list, its filter and categories remain visible; double-click and its context menu remain available. Hidden commands are still accessible through menus or extended view. `Ctrl+F` opens extended view before focusing text search. Placeholder warnings remain available in the status bar; clicking the warning count expands their details.

The main window's **Dialogues** (`Dialoghi`) and **Preview** (`Anteprima`) buttons reopen the respective secondary windows. The preview opens smaller and can be resized down to 480×300; its zoom does not change the 1280×720 rendering/screenshot canvas.

Some Windows UI labels are currently in Italian; the commands below describe their function.

## Build and run

Install the **.NET 8 SDK, version 8.0.400 or a later .NET 8 SDK feature band**. The desktop application requires Windows. The format-handling core and regression runner can also run on Linux. The rewrite does not require third-party NuGet packages beyond the Microsoft SDK and framework components.

From the directory containing `DGRV3TS.sln`:

```sh
dotnet build DGRV3TS.sln -c Release --warnaserror
dotnet run --project DGRV3TS.Core.Tests -c Release --no-build
```

The second command runs the repository's executable regression suite and returns a nonzero exit code on failure. `dotnet test` is not the runner for this suite.

On Windows:

```sh
dotnet run --project DGRV3TS
dotnet run --project DGRV3TS -- examples/dialoghi.po
```

The initial file argument preserves v25's command-line / "Open with" workflow. The sample has an adjacent `VARS_BAK` dictionary and speaker, expression and voice metadata; no game images or sounds are included.

A Windows build is produced under `DGRV3TS/bin/Release/net8.0-windows/win-x64/`. Running that build requires the .NET 8 Windows Desktop Runtime x64 and its adjacent application files; using `dotnet run` requires the SDK.

## Translation workflow

Open PO, VO, TXT, STX, XLSX or a JSON project. Write in the **Translation** textbox (`Traduzione`). Edits stay in memory when changing dialogue or translation language, while the original remains separate. Inserted variable tokens remain raw tokens in saved files; resolving them affects only the preview.

| Shortcut or command | Action |
| --- | --- |
| `Ctrl+O`, `Ctrl+S`, `Ctrl+Shift+S` | Open, save, save as / convert format. |
| `Ctrl+Up` / `Ctrl+Down` | Previous / next dialogue. Use the dialogue number and **Go** (`Vai`) to jump directly. |
| `Ctrl+F`, `Ctrl+H` | Search and replace. Replace-all affects the selected language and requires confirmation. |
| `F8` | Next untranslated dialogue. |
| **Mark intentionally empty** | Explicitly export an empty string rather than leaving a dialogue untranslated. |
| `Ctrl+Z` / `Ctrl+Y` | Undo / redo in the current textbox. Its undo history is cleared when switching to another dialogue or language. |
| **Reset reading progress** / **Fast-read** | Return to the first dialogue / jump to the final dialogue. |

Warnings compare tag, variable and .NET/printf placeholder counts between the original and translation; they do not assess linguistic quality. Re-selecting the same dialogue preserves the textbox's caret, selection and undo history.

Autoplay uses a UI timer with pause, resume and stop. Manual editing, navigation and modal operations stop autoplay to keep edits attached to the intended dialogue. WAV and text-to-speech playback on dialogue changes can be enabled from the menu. Navigation waits for active WAV/TTS playback to finish; WAV playback runs outside the UI thread. TTS restores v25's speaker age/gender preferences and chapter-dependent narrator hints, within the voices installed on Windows.

### Translation memory

Opening PO, VO or XLSX automatically searches the document's folder and subfolders for reusable translations, matching v25's default workflow. Turn **Reuse translations automatically on open** (`Riusa traduzioni automaticamente in apertura`) off to use only explicit indexing. Existing manual translations and intentionally empty rows are never replaced.

**Index memory from folder** (`Indicizza memoria da cartella`) explicitly selects a different folder. Both paths recursively read PO/VO/XLSX/JSON translations without modifying their files. They exclude the current document, build directories, game assets and symbolic links. Suggestions match the exact original and translation-language index, preferring the same speaker. Automatic reuse checks protected placeholders before applying a candidate; manually reviewed suggestions can be accepted after a warning.

**Suggestions** shows conflicting candidates and their source files. **Fill untranslated from memory** applies only unambiguous candidates and does not overwrite completed translations. Reused translations receive one `<SIGNAL_AutoTL>` marker: the preview hides the marker and provenance suffix and shows the reused text in pink, as in v25. This reuses existing local translations; it does not call an online machine-translation service or require API keys.

**Stop** also cancels indexing and retains the previous memory. Commands that change the document or its filename are disabled during indexing, while text editing remains available. Closing the editor cancels the scan before handling unsaved changes normally.

## Documents and saving

**JSON is the complete working-project format.** It preserves separate originals, all translation languages, intentionally empty completion states and the imported source template so metadata and structures can survive re-export. Save As proposes JSON.

| Format | Support and current limits |
| --- | --- |
| PO | `msgid`, `msgstr`, comments, multiline strings, escapes and v25 contexts: `ID \| origin \| speaker \| expression \| voice`. Plurals, declared non-UTF-8 charsets and unknown escapes are rejected explicitly. Exports one translation language. |
| VO | Legacy metadata, comments, the final record without a trailing blank line, multiple languages and reversible escapes. Legacy `"(blank)"` means empty; that exact literal cannot be exported unambiguously. |
| TXT | One dialogue per line, with optional enclosing `{` / `}`. Newlines inside a dialogue are exported as `\n`. |
| STX | Single-table UTF-16LE files, nonsequential IDs, empty source strings and preserved headers. Pointers and string terminators are validated; multiple tables are rejected. |
| XLSX | First worksheet: headers in row 1, A = ID/OFFSET, B = original, C onward = translations. Languages defined only in headers remain selectable even if every translation is blank; older JSON projects recover these columns from their embedded workbook. Other worksheets and imported workbook parts are preserved. Changed translation formula cells cannot be overwritten. Legacy `.xls` is not supported. |

TXT and STX store only final text: untranslated rows retain the source; intentionally empty rows are cleared. Keep a JSON project to preserve the original/translation comparison. Exporting originals produces a copy without changing the open document.

Saving writes a temporary file first and keeps the previous version as `<filename>.bak`. If another program changes or removes the open file, saving refuses to overwrite it: save under another name and compare the versions. There is no automatic disk save, and `.bak` retains only one previous version. Opening or closing a modified document offers save, discard or cancel.

### STX and WRD

Opening an STX also loads an adjacent WRD with the same basename when present. A WRD can be selected manually from the menu. Metadata is associated by numeric LOC IDs, with bounds and supported-opcode checks, and supplies speaker, expression and voice information to the summaries, graphics preview and PO conversion. Importing metadata marks the project as modified; save JSON to retain it with the translation work.

## External variables: VARS_BAK and VARS_AI

The V3/AI selector uses `VARS_BAK` / `vars_bak.txt` and `vars_ai.txt`, respectively. Automatic lookup checks the document directory first, followed by the asset directory, executable directory and working directory. A dictionary can also be selected explicitly or reloaded after an external edit.

Automatically discovered dictionaries are looked up again when opening another document. An explicitly chosen dictionary stays selected until another is chosen or the game is changed.

```text
// Characters
<HERO>, Kaede Akamatsu | Note displayed in the editor
// Glossary
<ACADEMY>, Accademia per talenti : Ultimate Academy | Main and alternate values
<ACADEMY_MN>, accademia per talenti
```

`//` starts a category; entries before the first category are accepted. The first `,` separates token and value, ` : ` separates an alternate value and ` | ` introduces a comment. The main editor provides categories, search, hover notes and entry information. A single left-click copies the raw token for V3 or the displayed value for AI; double-click inserts the corresponding token or AI value at the caret.

The context menu can copy tokens and values, show alternate values and entry information, and copy or insert lowercase (`_MN`) and uppercase (`_MS`) variants. Existing external definitions take precedence. When no definition exists, a variant can be generated for the current session, preserving the original v25 convenience. Generated variants do not change the external dictionary and are cleared on reload. **Remove from list** hides an entry from the current list without deleting its file definition.

Nested substitutions are limited to three levels, respect token boundaries and report cycles. Duplicate tokens and malformed lines produce warnings instead of preventing startup. The external dictionary is not rewritten automatically.

## Folders, files and preview assets

**This repository does not contain game image or sound files. You must provide those yourself.**

By default, all paths below are relative to the executable. To use another asset location, choose the folder containing `Graphics` and `Sound` from the windows/assets menu.

| Asset | Folder and example |
| --- | --- |
| AI backgrounds | `Graphics/Backgrounds/AI`, e.g. `Graphics/Backgrounds/AI/AIBlank.png`. |
| AI sprites | `Graphics/Sprites/AI`. AI support was marked TODO/WIP in the original; complete game-specific behavior is not yet verified. |
| AI voice lines | `Sound/Voices/AI`. AI support was marked TODO/WIP in the original; complete game-specific behavior is not yet verified. |
| DR backgrounds | `Graphics/Backgrounds/Danganronpa`, e.g. `Graphics/Backgrounds/Danganronpa/default_background.png`. The v25 `Graphics/Backgrounds/DR` convention is also supported. |
| DR sprites | `Graphics/Sprites/DR`, e.g. `Graphics/Sprites/DR/C000_Saiha/anim000.png`. |
| DR voice lines | `Sound/Voices/DR`, e.g. `Sound/Voices/DR/vic_Akama/vic_Akama_01_001.wav`. |

Each character's sprite folder should also contain a **`default.png`** for use when no matching animation or expression is available. Background images should ideally be **1280×720**; sprite images should ideally be **no wider than 1024 pixels**. A default custom font can be supplied as `font.ttf` or `font.otf` in the asset root.

The separate graphics preview provides backgrounds, optional sprites, V3/AI selection, installed fonts and external TTF/OTF files, font sizes, CLT tags, fixed/adaptive line spacing, WAV playback, Windows text-to-speech with language selection, image copying and PNG screenshots. Composition stays in 1280×720 coordinates regardless of window zoom. Sprite scale, negative offsets and clipping follow v25's separate rules for regular characters, Monokubs, Maki, flashbacks and default portraits. Text is drawn before sprites, as in v25.

The default font is Calibri when available, measured in points. The game/background profiles restore DR, AI, tutorial and ToWitter origins, font defaults and line spacing; ToWitter also restores its message-only text selection. Automatic word wrapping is off by default, so existing explicit game line breaks retain their layout; wrapping remains an optional preview setting. AutoTL and signal-delimited hidden suffixes are not read aloud. Missing assets or unavailable audio do not prevent translation. These rules are regression-tested, but actual pixel output, installed fonts, audio devices and UI still need comparison on Windows with the same game assets.

Sprite import retains the v25 character mapping and does not overwrite existing files. Exporting voiced dialogues supports all speakers rather than only the original hardcoded Monokuma filter. The original **Setup Voicelines** menu item was explicitly unimplemented; voice files use the documented directories.

## Project structure and validation

- `DGRV3TS`: `Operations.cs` owns the main window's state and lifecycle; `Operations.Designer.cs` defines its layout; `Init.cs` connects events and initialization; `Misc.cs` handles navigation and editor state. File, variable and translation commands and secondary windows have separate files.
- `DGRV3TS.Core`: documents, file formats, translation memory, variables, WRD metadata and text checks.
- `DGRV3TS.Core.Tests`: regression runner that also works on Linux.
- [Feature parity](docs/FEATURE-PARITY.md): comparison with v25 and remaining validation requirements.
- [Actual bug fixes](docs/BUG-FIXES.md): defects with triggers and evidence, distinguished from restored features and defects introduced during the rewrite.
- [Rewrite map](docs/RISCRITTURA-V25.md): correspondence with original v25 files and source ZIP contents; currently in Italian.
- [Windows verification](docs/VERIFICA-WINDOWS.md): manual visual/audio checklist; currently in Italian. These checks are not run by the Linux test runner.

CI is configured to build Release with warnings treated as errors and run the regression suite. A successful Linux build does not establish Windows GUI, SAPI or audio-device behavior.

To create a source ZIP without caches, build outputs or personal game assets:

```sh
python3 tools/package_source.py
```

The script writes the archive to an `artifacts` directory beside the checkout and prints its path. The ZIP contains the solution, projects, tests, examples, documentation and licenses; the .NET 8 SDK is needed to build it.

## Project history

The internal name **DGRV3TS** was chosen early in development, in **early 2021**, and was never changed. The original README described the older code as "very messy and sometimes just... confusing" and announced further documentation. The historical changelog is retained in `CHANGELOG.txt`.

## License and credits

This tool is licensed under the GNU General Public License v3.0: see [LICENSE](LICENSE).

“DANGANRONPA” is a registered trademark of **Spike Chunsoft Co., Ltd., Too Kyo Games, LLC and NIS America Inc.** “AI: THE SOMNIUM FILES” is a registered trademark of **Spike Chunsoft Co., Ltd.** We are not affiliated or associated with these companies.

Thanks to:

- **[SciresM](https://github.com/SciresM)** for creating [FEITS](https://github.com/SciresM/FEITS), the original basis of this project. FEITS is licensed under [GPL v3.0](https://github.com/SciresM/FEITS/blob/master/LICENSE.txt).
- **[Liquid S!](https://github.com/Liquid-S)** for [DRV3-STX-TOOL](https://github.com/Liquid-S/DRV3-STX-TOOL). The original `StxInternal.cs` and `WRDInternal.cs` incorporated code from that MIT-licensed project; its license remains in `ThirdParty`.
- Members of the **SPIRAL Discord server** and the **[Spiral Framework project](https://spiralframework.info/)**, whose source is at [SpiralFramework/Spiral](https://github.com/SpiralFramework/Spiral).
- All collaborators and reviewers.

### Historical dependencies

The original application used the following NuGet packages, which were useful during its development. Their licenses remain in `ThirdParty` for attribution and project history. **They are not active dependencies of this rewrite**; the current TTS implementation uses Windows SAPI.

| Original package | Version | License |
| --- | --- | --- |
| [EPPlus](https://www.epplussoftware.com) | 6.1.3 | [PolyForm Noncommercial License 1.0.0](https://polyformproject.org/licenses/noncommercial/1.0.0/). |
| [ExcelDataReader](https://github.com/ExcelDataReader/ExcelDataReader) | 3.6.0 | [MIT](https://licenses.nuget.org/MIT). |
| ExcelDataReader.DataSet | 3.6.0 | [MIT](https://licenses.nuget.org/MIT). |
| [System.Speech](https://dot.net) | 7.0.0 | [MIT](https://licenses.nuget.org/MIT). |

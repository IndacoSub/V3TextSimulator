# Organization of the v25 rewrite

The baseline is branch `v25`, commit `6601709c85e48cc17376fd700f40f58a47f02e4c`. The main window retains the name `Operations`: the rewrite covers its lifecycle, layout, initialization, commands and both secondary windows.

Partial classes distribute the responsibilities of the same window across separate files. `Program.cs` initializes Windows Forms and starts `Operations`.

## Mapping the original structure to the rewrite

| Responsibility in the original v25 | New location |
| --- | --- |
| `Operations.cs`: main-window construction and state | `Operations.cs`: shared state and form lifecycle |
| `Operations.Designer.cs`: control definitions | `Operations.Designer.cs`: control fields and `InitializeComponent`, separate from translation commands |
| `Init.cs`: manager creation, initial settings and events | `Init.cs`: workspace initialization, event bindings and startup file opening |
| `Misc.cs`: navigation, selected text, in-memory updates and autoplay | `Misc.cs`: dialogue selection, editor synchronization, counts, search and autoplay |
| `Operations.cs`: opening and updating secondary windows | `WindowCoordinator.cs`: creating, reopening, synchronizing and closing secondary windows |
| `GUIFunctionsButton.cs` and `FileManager.cs`: opening, saving, conversion and exports | `DocumentCommands.cs`: file-dialog interaction; `DGRV3TS.Core`: parsing, writing and data preservation |
| `VariableManager.cs`, `Listboxes.cs`, `GUIFunctionsListbox.cs`: variable categories and usage | `VariableCommands.cs`: panel and commands; `VariableCatalog` and `VariableExpansion` in the core: external dictionary parsing, recursive variables and nested MAKE calls |
| `TLDictionary.cs` and selection of existing translations | `TranslationCommands.cs`, `SuggestionDialog` in `EditorDialogs.cs`, and `TranslationMemory`/`TranslationReuse` in the core |
| `HalfBoxTester.cs`, `DisplayImage.cs`, `TextRender.cs`, `FontManager.cs`, `SoundManager.cs` | `DialoguePreviewWindow.cs` and `MediaAssets.cs`: graphical preview, assets, fonts, WAV, TTS and screenshots |
| `VerticalView.cs` and `Subforms/VerticalSummary` | `VerticalView.cs`: separate full-file summary with cards drawn for the visible area |

`Ui.cs` provides shared visual elements. `EditorDialogs.cs` contains modal text-entry, replacement and suggestion dialogs. `PreviewImageBox.cs` displays the composed image at the bottom of the preview window, preserving its aspect ratio while the controls and status stay above it. Core codecs and models remain independent of Windows Forms so they can also be tested on Linux.

The vertical summary remains **a secondary window**. The other secondary window is the landscape graphical preview. Translations can be saved independently of either window.

`ViewModes.cs` switches the main editor between simple and extended views without recreating the controls or clearing text, selection or undo history. The simple view keeps the variable list visible and reduces the window height so the three windows can be arranged together.

## Commands and workflows to preserve

- **Opening:** a document replaces the current one only after parsing succeeds. The initial `Program.cs` argument supports opening a file from the command line.
- **Editing:** the textbox updates the selected dialogue and language in memory; the original stays separate and read-only.
- **Navigation:** arrows, line number, search, reset, fast-read and summary-card clicks use the same selection. Changing language also updates the summary.
- **Saving:** `Ctrl+S` saves the current document; **Save as / convert** selects the format. JSON preserves the complete project. Backup and external-change detection belong to the core.
- **Original-text export:** creates a copy of the source text while keeping the current document open. Exporting voiced dialogue must preserve the working list.
- **Variables:** load and reload `VARS_BAK`/`VARS_AI`, browse categories, search, view notes, insert tokens and select alternate values. Recursive variable and MAKE expansion affects the preview while preserving raw text for saving.
- **Translation memory:** scanning reads existing translations; conflicting suggestions require a choice. Bulk completion applies only unique candidates to unfinished rows. Placeholder checks include whole-token case variants and MAKE names.
- **Autoplay:** pause, resume and stop must keep the interface responsive. Editing commands stop navigation before applying changes so text reaches the intended dialogue.
- **Secondary windows:** closing and reopening them must preserve data and avoid references to disposed controls. Summary copy/paste acts on the correct dialogue in the current language.

## Delivery verification

From the directory containing `DGRV3TS.sln`:

```sh
dotnet build DGRV3TS.sln -c Release --warnaserror
dotnet run --project DGRV3TS.Core.Tests -c Release --no-build
```

The second command runs the repository's regression suite. A successful build does not demonstrate that the interface has run on Windows.

On Windows, with the .NET 8 SDK:

```sh
dotnet run --project DGRV3TS -- examples/dialoghi.po
```

Check at least these workflows before considering the interface verified:

1. Start the application, open the example and confirm the main window has the source view, translation textbox and variables. Expand the simple view to access the source view.
2. Open the separate vertical summary: all dialogues must show original and translation together. Clicking, copying and pasting must affect the correct dialogue while preserving scroll position.
3. Edit text, change dialogue and language, then save and reopen the JSON project. Include an intentionally empty translation.
4. Cancel opening or saving, and attempt to close with unsaved changes: the document must remain open when the operation is canceled.
5. Export originals to a separate file, then confirm the current document and translations are unchanged.
6. With autoplay active, type, replace text, choose a suggestion or paste from a summary card: each edit must affect its intended dialogue.
7. Resize the windows and use 150% Windows scaling: the editor and commands must remain usable.
8. Close and reopen both secondary windows. Supply your own assets for preview, audio and screenshot checks, and follow the full [Windows verification checklist](WINDOWS-VERIFICATION.md).

These are manual checks to perform; they have not been executed in the Linux environment.

## Source ZIP contents

A source archive must contain the solution, projects, code, tests, documentation, examples and licenses. Exclude `.git`, `bin`, `obj`, caches, backups, temporary files and personal game assets or dictionaries.

After extraction, open `DGRV3TS.sln` or run the commands above from the extracted directory. Building the source ZIP requires the .NET 8 SDK. A runnable Windows distribution is a separate archive containing all published output files and stating its runtime requirements.

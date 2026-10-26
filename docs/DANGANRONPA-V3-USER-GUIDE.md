# Danganronpa V3 user guide

This guide is for translators, reviewers and anyone who wants to create dialogue screenshots with V3TextSimulator. You can use it as a text editor, a translation workspace or an image composer. This guide focuses on **Danganronpa V3**; select **DR** in the graphical preview.

**PO (`.po`) is the recommended format for regular V3 translation work.** VO (`.vo`) is deprecated for new work, although it remains useful for opening older projects, recovering translations and reusing translation memory. JSON is an optional complete project snapshot, especially useful for multiple languages and intentionally empty translations.

**Variables are optional.** You can translate and create screenshots using ordinary text without any dictionary. Game images, sprites, fonts and audio are also optional for text editing. Supply your own assets when you want a game-style screenshot.

All dialogue and glossary examples written for this guide are invented. The examples do not reproduce a private working dictionary or game dialogue.

## Contents

1. [Choose your starting point](#1-choose-your-starting-point)
2. [Install and start the program](#2-install-and-start-the-program)
3. [Understand the three windows](#3-understand-the-three-windows)
4. [Make your first screenshot](#4-make-your-first-screenshot)
5. [Start a translation in PO](#5-start-a-translation-in-po)
6. [Work through a file](#6-work-through-a-file)
7. [Review, search and replace](#7-review-search-and-replace)
8. [Save, convert and exchange files](#8-save-convert-and-exchange-files)
9. [Set up backgrounds, sprites and fonts](#9-set-up-backgrounds-sprites-and-fonts)
10. [Control dialogue metadata and expressions](#10-control-dialogue-metadata-and-expressions)
11. [Format text and check line breaks](#11-format-text-and-check-line-breaks)
12. [Use optional variables and MAKE templates](#12-use-optional-variables-and-make-templates)
13. [Reuse translations with AutoTL](#13-reuse-translations-with-autotl)
14. [Use voices, text-to-speech and autoplay](#14-use-voices-text-to-speech-and-autoplay)
15. [Prepare a review or screenshot session](#15-prepare-a-review-or-screenshot-session)
16. [Troubleshoot common problems](#16-troubleshoot-common-problems)
17. [Shortcut reference](#17-shortcut-reference)
18. [Before handing off your work](#18-before-handing-off-your-work)

## 1. Choose your starting point

| Your goal | Start here | What you need |
| --- | --- | --- |
| Translate an existing V3 script | Open its PO, then follow sections 5–8. | A translation file; images and dictionaries are optional. |
| Continue an older VO project | Open the VO, check the selected language, then save a PO copy for ongoing single-language work. | The legacy file; keep JSON too if you need all its languages. |
| Review someone else's translation | Open the PO, expand the editor and open the vertical summary. | The source/translation file; optional matching assets. |
| Create a single screenshot | Follow section 4. | A line of text; optional background, font and character assets. |
| Produce a set of screenshots | Put each dialogue in a project, use the summary to navigate, then save each PNG. | A project and any desired assets; image export is manual. |
| Inspect extracted game text | Open STX, with its matching WRD alongside when available. | Extracted files from your own workflow. Save a PO copy for translation. |

You do not need to learn variable syntax, obtain voice files or configure a translation memory before starting.

## 2. Install and start the program

The desktop application runs on Windows. If you have a Windows build, keep its application files together and install the **.NET 8 Windows Desktop Runtime x64**. Copying only the executable out of its build folder is insufficient.

If you downloaded source code, install the **.NET 8 SDK** and open a terminal in the folder containing `DGRV3TS.sln`:

```sh
dotnet build DGRV3TS.sln -c Release
dotnet run --project DGRV3TS
```

To start with a file already open:

```sh
dotnet run --project DGRV3TS -- examples/dialoghi.po
```

You can also use **Open** or `Ctrl+O` after startup. The bundled example is useful for learning navigation; the separate examples below are intended for making your own small projects.

Some interface labels are currently Italian. This English guide describes controls by their function; keyboard shortcuts provide another way to reach common commands.

## 3. Understand the three windows

### Main editor: where you translate

The main window contains the translation textbox, dialogue navigation, language selection and optional variable list. This is where you write and save your work.

It starts in **simple view**, which keeps the window short. Use the downward expansion control at the bottom to open **extended view**. Extended view reveals the original text, search, suggestions and additional actions. Collapse it again when you want more room for the other windows. Switching views preserves your text and selection.

The original is read-only. The translation textbox edits the currently selected dialogue and language. Typing changes the project in memory; use Save to write those changes to disk.

### Vertical summary: the whole file at a glance

Press `F6`, or use the **Dialogues** button, to open the separate tall summary. Each card shows original and translation together, with identifying information. Click a card to select its dialogue in the main editor.

Right-click a card to copy text or paste a translation. Pasting acts on that card in the active language, including when the card was not previously selected. Closing this window does not discard the document; reopen it with `F6`.

### Graphical preview: what the text looks like

Press `F7`, or use **Preview**, to open the separate landscape window. It shows the current dialogue, optional background and optional character sprite. Its controls and status information are above the image, and the image is the lowest part of the window.

The image keeps its proportions and stays aligned to the bottom as the window changes size. Unused space may appear above or beside it. Resizing the window changes the viewing scale; copied and saved images still use the **1280×720 canvas**.

Arrange the three windows beside one another to suit your screen. They remain independent windows: you can close either secondary window and continue editing.

## 4. Make your first screenshot

You can do this without a game script or a variable dictionary.

1. Choose **New project** (`Ctrl+N`). In the new-dialogue prompt, enter an invented line such as `The lantern is still on.` and confirm.
2. Type the text you want to display into the translation textbox. You can use the same sentence, a translation or a completely different creative line.
3. Press `F7` and select **DR**. A basic preview works even without assets.
4. For a game-style background, use the preview's asset-folder control to select the folder containing `Graphics` and `Sound`, then choose a background. Section 9 explains the layout.
5. Choose an installed font with **Font**, or load a font file with **TTF/OTF**. Adjust the font size and line breaks until the line fits.
6. Toggle the character display if desired. A character requires matching speaker metadata and sprite files; see section 10. You can make a background-and-text screenshot without either.
7. Use **Save PNG** to save the finished image, or **Copy image** to paste it into another application. These commands export the composed image, without window borders or controls.
8. Save the text project as well if you want to return to it later. Saving a PNG does not save the dialogue project.

A new JSON project is convenient for an invented screenshot collection because you can add dialogues. Use **Add dialogue** for each new entry, then select it from the summary. For an existing non-JSON document, the program asks you to save a JSON copy before adding entries.

There is no manual speaker/expression editor in the new-dialogue prompt. For a screenshot with a specific character, open an existing dialogue with the right metadata or create a small PO as described in section 10.

## 5. Start a translation in PO

### Open an existing PO

1. Open your `.po` with `Ctrl+O`.
2. Open extended view to see the original above the editable translation.
3. Select the dialogue in the summary or use previous/next navigation.
4. Translate in the translation textbox. Keep control tags and placeholders intact unless your translation pipeline explicitly requires a change.
5. Press `Ctrl+S` regularly.

Opening PO can trigger automatic reuse of existing translations from its folder and subfolders. Section 13 explains how to review or disable this behavior.

A PO separates the source text (`msgid`) from its translation (`msgstr`). The context (`msgctxt`) identifies the dialogue and can also contain character, expression and voice metadata. Preserve those identifiers when exchanging files with your team.

### Create a small PO yourself

Save the following invented example as UTF-8 text with a `.po` extension, then open it:

```po
msgid ""
msgstr ""
"Content-Type: text/plain; charset=UTF-8\n"

msgctxt "1 | scene_demo"
msgid "The lantern is still on."
msgstr ""

msgctxt "2 | scene_demo"
msgid "Someone must be nearby."
msgstr ""
```

Translate the empty entries in the application. Blank `msgstr` fields normally represent unfinished translations. Until a translation is supplied, the preview can display the original.

Use UTF-8. This editor rejects PO plurals, declared non-UTF-8 charsets and unsupported escapes rather than silently discarding their contents. Use a copy when testing a file from a different pipeline.

### Start from STX and WRD

Open an extracted `.stx`. If a `.wrd` with the same basename is beside it, the editor attempts to load its metadata too. You can also load a matching WRD manually.

Save a PO copy using **Save as / convert** and choose the PO filter. Translate that copy as your normal working file. The tool does not install a translation into the game: any required extraction, packing and game integration remain part of your team's separate toolchain.

## 6. Work through a file

### Move between dialogues

Use `Ctrl+Up` and `Ctrl+Down`, the previous/next buttons, or a summary card. To jump directly, enter the dialogue number and use Go. The displayed row number is its position in this file; a stored dialogue ID can be different or nonsequential.

`F8` moves to the next untranslated dialogue. Reset returns to the first entry; Fast-read jumps to the last. Fast-read is navigation, not a command that marks every dialogue translated.

Edits stay in memory when you change dialogue. A language selector appears for formats with multiple translations; make sure you are editing the intended language before a long session. A normal PO contains one translation language.

### Use the original as a reference

Extended view shows the original separately. Copy Original replaces the current translation after confirmation when needed; use it when you want to preserve the structure and translate around tags. A copied original is still text you need to review.

The preview can show the original instead of the translation when **Preview original** is enabled. Turn that option off to inspect your edited translation again.

### Understand empty translations

An untouched empty row is unfinished. An **intentionally empty** row means you want the final text to be empty. Use the explicit Mark Intentionally Empty command for that case. Existing text, including an intentionally empty completed row, is protected from automatic memory filling.

JSON preserves explicit completion flags. PO's empty `msgstr` alone cannot distinguish an intentional blank from an untranslated entry after reopening, so retain a JSON snapshot when that distinction matters. TXT/STX final-text exports use the intentional-empty state currently held in the document.

### Undo and redo

Use `Ctrl+Z` and `Ctrl+Y` in the current textbox. Changing to another dialogue or language clears that textbox's undo history. Re-selecting the same unchanged dialogue or expanding/collapsing the editor preserves the current caret, selection and undo history.

Undo is local to the textbox; it is not a project-wide history for bulk replacement or memory filling. Keep a saved copy before a large edit you may want to reverse.

## 7. Review, search and replace

Press `Ctrl+F` to expand the editor and focus search. Search can match the original, translation, speaker, context or ID. Find Next wraps through the file. Enable the untranslated filter to restrict the results to unfinished rows, or use `F8` directly.

Press `Ctrl+H` for literal text replacement. Choose whether case matters and whether to replace in the current dialogue or all translations of the selected language. Replace All requires confirmation. It works on translation text, including tags and token names, so inspect the resulting placeholder warnings after changing repeated strings.

The warning indicator compares protected tags and placeholders with the original. It checks counts of game tags, known variable tokens, MAKE names and supported formatting placeholders such as `{0}` or `%02d`. Click the warning count to open the extended details.

A clean placeholder check means the counted tokens agree. It does not judge meaning, grammar, character voice, translation accuracy or whether text fits the game's dialogue box. Review the source and the preview as well.

For a team review, use the summary to compare surrounding dialogues rather than checking isolated sentences only. Preserve a list of the file and dialogue IDs that need decisions.

## 8. Save, convert and exchange files

### Recommended formats

| Format | When to use it | What to remember |
| --- | --- | --- |
| **PO — recommended** | Regular single-language V3 translation, review and exchange. | Keeps original/translation pairs and context metadata. Exports the selected language. |
| **JSON — optional full snapshot** | A complete editor project, multiple languages, added dialogues or explicit blank completion states. | Retains originals, languages and imported source structures; useful alongside your working PO. |
| **VO — deprecated** | Legacy projects, recovering previous work and translation-memory donors. | Still readable/writable, but choose PO for new single-language work. Preserve a JSON copy before converting a multilingual VO to PO. |
| STX | Final-string output when your external V3 toolchain expects STX. | Stores final text rather than editable source/translation pairs. Supports one table. |
| TXT | Quick plain-text input or readable export. | One dialogue per physical line; not a full translation-project backup. |
| XLSX | Spreadsheet exchange when your team already uses it. | First worksheet uses ID, original and translation columns. Translation formula cells cannot be overwritten. `.xls` is unsupported. |

**Save as / convert** currently proposes JSON. For your normal V3 PO workflow, explicitly choose **Gettext PO** and a `.po` filename. The recommendation to work in PO does not require removing JSON support.

After saving under another name or format, that file becomes the current document. Subsequent `Ctrl+S` saves to that new path. If you keep both a JSON snapshot and a PO deliverable, save each deliberately when needed.

### Protect your work

Saving keeps the previous destination as `<filename>.bak`. That is one previous version, not a full backup history. There is no automatic disk save.

If another application changes or removes the open file, the editor refuses to overwrite it. Save under a different name and compare the versions. If you intentionally edit a PO externally, save or close your current work and reopen the external version before continuing.

Export Originals writes a separate readable text copy without changing the open project. Export Voiced Dialogue exports entries that have voice metadata. Neither replaces your source/translation working file.

## 9. Set up backgrounds, sprites and fonts

You can skip this section for text-only translation. The application does not include game graphics or audio.

Choose an asset root with the following structure. `my-assets` is an invented folder name; the directories beneath it are the supported layout:

```text
my-assets/
  Graphics/
    Backgrounds/
      DR/
        dialogue-background.png
    Sprites/
      DR/
        C000_Saiha/
          default.png
          anim001.png
  Sound/
    Voices/
      DR/
        voice_demo.wav
  font.ttf
```

Select **`my-assets`**, the parent containing `Graphics` and `Sound`. The alternative background directory `Graphics/Backgrounds/Danganronpa` is supported too. Background PNGs can be selected from the preview's dropdown.

Choosing the asset folder in the preview also updates the main editor's asset folder, so sprite imports and automatic dictionary discovery use the same location.

For backgrounds, **1280×720** is the natural canvas size. Other dimensions are drawn into that canvas, so use the correct proportions to avoid distortion. Sprite images should ideally be no wider than 1024 pixels. Keep their original transparency and canvas; the preview can frame them without changing the files.

### Character files

A dialogue's speaker selects its sprite folder. Its expression selects a PNG such as `anim001.png`. If that file is missing, the preview tries `default.png`, then the supported unknown-character placeholder if available.

The `chara_Blank` and `chara_Hatena` icons use `image.png`. Both layouts are supported: `Graphics/Sprites/chara_Hatena/image.png` and `Graphics/Sprites/DR/chara_Hatena/image.png`, with the corresponding paths for `chara_Blank`. The shared location takes precedence when both exist. If a sprite archive contains character directories directly at its root, extract those directories into `Graphics/Sprites/DR`; select the asset root containing `Graphics` in the application.

A `default.png` is useful for dialogues without matching animation metadata. Automatic framing recognizes that fallback as an existing portrait and preserves its complete visible artwork.

**Automatic Framing** is on by default. It ignores transparent margins for placement, shows tall full-body sprites as busts and fits them to a portrait area left of the dialogue. This prevents a wide pose from covering text and gives images with different padding a consistent position. Icons and existing portraits remain complete. Framing affects only the preview and exported screenshot; the source PNG is never cropped or rewritten.

Disable Automatic Framing when you want the historical v25 scale and coordinates. That mode uses the complete PNG canvas, including transparent padding, so keep the original files intact. Text remains in the foreground in both modes. Use original alpha channels rather than flattening sprites against a background.

For Monotaro, both `C021_Mtaro` and `C021_Monot` are accepted: older rendering metadata and the sprite importer used different names. The loader checks the named folder first and can use the equivalent folder when needed. Both identifiers use the same mascot placement; the dialogue's stored metadata is unchanged.

The sprite-folder import command recognizes supported extracted `stand_*.png` naming and character-folder conventions. It copies recognized images into the asset layout and preserves existing destination files. For a small custom setup, placing the desired PNGs directly is often simpler.

### Fonts

The V3 reference is **FOT-HummingStd at size 22**, measured in points. Its Light (L), Medium (M) and Bold (B) variants are separate faces. Medium is the default preference; use the Humming weight dropdown to select another available face. Selecting Bold uses the B file/family rather than relying on a simulated bold style.

You can place `FOT-HummingStd-L.otf`, `FOT-HummingStd-M.otf` and `FOT-HummingStd-B.otf` in the asset root or its `Fonts` directory. Alternatively, load one through **TTF/OTF**; the weight dropdown can then find the other faces beside it. Matching installed Windows families are also supported. The program does not include these font files.

Use **Font** to select another installed Windows font and size, or **TTF/OTF** for another custom file. A root-level `font.otf` or `font.ttf` still overrides automatic discovery; OTF takes precedence when both are present. Without a matching custom or installed Humming font, Calibri is used when available. The status line shows the actual family and point size, so check it if the text looks different from your reference. An unavailable weight leaves the current font in place and reports the missing face.

Choose a font with the accented characters and punctuation your translation requires. Reload checks whether the active external font file changed and rereads it when needed; you can also select it again through the font-file control. Exact font loading and rasterization must be checked on Windows.

Use Reload after adding or replacing images, then check the selected background and dialogue. A basic-background selection stays selected even when background PNGs are available. The image can be displayed at a smaller scale while its exported size remains 1280×720. If rendering fails, PNG/clipboard export stops and reports the error rather than exporting the previous frame.

## 10. Control dialogue metadata and expressions

Sprites and recorded voices are selected from dialogue metadata, not from words typed into the translation textbox.

V3 PO contexts use this structure:

```text
ID | origin | speaker | expression | voice
```

The speaker and expression identify asset names. The origin identifies the source scene/file; the optional voice identifies a WAV. Preserve the IDs and metadata supplied by your translation pipeline.

**Character, expression and recorded-voice metadata are all optional.** A PO can contain only some of them, or none at all; a standard PO without `msgctxt` is also valid input. Unavailable information stays unknown. Editing or saving a translation does not invent these fields or copy them from the preceding dialogue.

These invented contexts illustrate different combinations:

```text
1 | demo_scene
2 | demo_scene | ACTOR_DEMO
3 | demo_scene | ACTOR_DEMO | anim_demo
4 | demo_scene | ACTOR_DEMO | vic_demo
5 | demo_scene |  |  | custom_demo_voice
```

The fourth example uses the legacy `vic` voice-name convention in the fourth field, with no animation. For another voice naming convention, retain an empty expression field and put the voice in the fifth field, as in the last example. Empty intermediate fields preserve the meaning of later positions.

With no speaker, the preview has no character sprite to request. With a speaker but no expression, an available default portrait may be shown; that visual fallback does not add animation metadata to the PO. With no voice, recorded playback is unavailable, while optional text-to-speech can still be used. These are normal cases and do not prevent translation, saving or text/background screenshots.

For a small character screenshot, create a UTF-8 PO like this invented dialogue:

```po
msgid ""
msgstr "Content-Type: text/plain; charset=UTF-8\n"

msgctxt "1 | scene_demo | C000_Saiha | anim001 | voice_demo"
msgid "The lantern is still on."
msgstr "The lantern is still on."
```

With the asset layout from section 9, this selects `C000_Saiha/anim001.png` and `voice_demo.wav`. The character code is an application asset identifier; the sentence and scene/voice names are invented examples. Replace the expression with an actual filename from your own sprite set.

You can omit voice metadata when making a screenshot. If you only want background and text, leave speaker/expression fields out and disable the character checkbox.

For real extracted scenes, the matching WRD can supply speaker, expression and voice automatically. Loading WRD updates metadata and marks the document modified. Use the WRD belonging to that STX; numeric LOC IDs determine the associations.

An STX can contain strings that the WRD never references. Those strings remain editable, but their character, expression and voice stay unknown. The editor does not copy the preceding dialogue's metadata onto them or assume that the first WRD parameter is a character name.

The program does not provide arbitrary sprite X/Y dragging. Use Automatic Framing for a readable portrait beside the dialogue, or disable it for historical V3 coordinates. Check metadata and the selected mode before modifying source assets to compensate for placement.

## 11. Format text and check line breaks

### Line breaks and fit

In the editor, use line breaks where you want the dialogue to continue on the next line. The preview also recognizes literal game-style `\n` escapes. In a PO file, newlines inside quoted fields are escaped as `\n`; the editor handles the file quoting when saving.

Automatic wrapping is off by default. This makes explicit line breaks easier to compare with V3's layout. You can enable wrapping for an experiment or screenshot, but a preview wrapping option does not insert line breaks into the saved translation.

If text is cut off, shorten the line, add an intentional break or adjust the screenshot font size. Check punctuation and long words at the right edge. The preview clips to its dialogue area; a successful save alone does not mean every glyph is visible.

### CLT styling

A common color span is:

```text
Follow the <CLT=cltSTRONG>blue lantern<CLT=cltNORMAL> tonight.
```

The sentence is invented. In the preview, `cltSTRONG` is orange. Supported preview styles also include `cltMIND`, `cltSYSTEM` and `cltWEAK`; normal/reset tags restore the base style. Supported `size` tags adjust font size relative to the selected size.

The CLT checkbox enables or disables interpretation of these styles for the preview. It does not remove the tags from the file. Closing a styled span should not introduce extra spaces: type spaces only where the sentence needs them.

When translating existing scripts, preserve the original controls and their multiplicity unless a team decision requires a change. A visual experiment is different from changing the game's control data.

### Other markers

Signal and platform controls are handled specially by the preview and should be retained in translation data as required by your pipeline. AutoTL is a provenance/review marker: its suffix is hidden and reused text is shown in pink. Section 13 explains that workflow.

The graphical preview is an editing aid. Compare important scenes in your actual game build as part of your external translation workflow; installed fonts and asset details affect the result.

## 12. Use optional variables and MAKE templates

### You can work without them

A variable dictionary is a convenience for reusable names, terminology and templates. **It is not required to open, translate, save or make a screenshot.** If you prefer ordinary text, type ordinary text.

If an existing script already contains a token, preserve it or coordinate its replacement with your team. Without its dictionary, the editor can still open the file, but it may show that token literally instead of its intended display value.

### Load a dictionary

For V3, an optional dictionary can be named `VARS_BAK` or `vars_bak.txt`. Automatic lookup checks the document directory, asset root, executable directory and working directory in that order. You can select another file explicitly through Load Variables.

An automatically found dictionary follows the document when you open another file. An explicitly chosen dictionary remains selected until you choose a different one or change the game. Reload rereads the external file and clears session-only generated variants.

This invented dictionary illustrates the syntax:

```text
// Imaginary glossary
VAR_ORCHARD_ITEM, apricot : plum | An invented glossary item
VAR_ORCHARD_LABEL, Sweet VAR_ORCHARD_ITEM : Fresh VAR_ORCHARD_ITEM
// Imaginary templates
MAKE_BORDER(MY_ARG), [MY_ARG]
MAKE_REPEAT(MY_ARG), MY_ARG / MY_ARG
```

- `//` introduces a category.
- The first comma separates a token from its value.
- ` : ` separates the main value from an alternate value.
- ` | ` introduces a note shown in the editor.

`VAR_ORCHARD_LABEL` displays `Sweet apricot` normally and `Fresh plum` with alternate values enabled. Variables inside values are resolved recursively. A shorter name does not replace part of a longer token.

### Insert and inspect variables

Filter the variable list by category, token, displayed value or note. In V3 mode, a single click copies the raw token; double-click inserts it at the caret. Right-click for copy, information, alternate-value and case-variant commands.

**Resolve variables in preview** changes how the preview displays tokens; it does not replace them in the saved translation. **Alternate values** switches the convention used for display. To write a resolved word permanently instead, use Copy Value and paste it deliberately into the translation.

Lowercase `_MN` and uppercase `_MS` variants use an existing external definition when present. Otherwise, the editor can generate a session-only variant. To keep a generated variant across reloads and share it with a team, add the desired definition to your own dictionary manually. The application does not rewrite the dictionary automatically.

### MAKE templates

A MAKE defines a template with one text argument. In the invented dictionary above:

```text
MAKE_BORDER(VAR_ORCHARD_ITEM)
```

shows `[apricot]`. Calls can be nested, including calls to the same template:

```text
MAKE_BORDER(MAKE_REPEAT(VAR_ORCHARD_LABEL))
```

shows `[Sweet apricot / Sweet apricot]`. Templates can contain variables or other MAKE calls too. Arguments can include commas and balanced parentheses; the call's closing parenthesis must match its opening one.

Pass a case-variant variable as the argument when needed. Unknown tokens remain visible, and incomplete known MAKE calls remain unexpanded. Cycles generate dictionary warnings; expansion guards stop runaway definitions from freezing the editor.

Keep the optional dictionary alongside the project when sharing work that depends on it, subject to your team's own sharing rules. A PO or JSON save does not embed the external dictionary.

## 13. Reuse translations with AutoTL

AutoTL reuses existing local translations. It does **not** contact an online translation service, require an API key or produce a new translation from a language model.

When you open PO, VO or XLSX, automatic reuse can scan the document's folder and subfolders. Turn off **Reuse translations automatically on open** if you want to translate from scratch or control indexing manually. Otherwise, review any changes applied during opening before treating the file as untouched.

To choose another source folder, use **Index memory from folder**. The scanner reads supported PO/VO/XLSX/JSON documents without modifying them. Build folders, application assets, symbolic links and the current document are excluded.

Candidates match the exact original and translation-language index. Eligible same-speaker translations are preferred. Automatic filling requires compatible protected placeholders; a source-text copy is not treated as a new translation. Different valid texts remain a conflict instead of being silently chosen.

Use **Suggestions** to inspect candidates and source files for the current dialogue. Manually selecting an incompatible suggestion requires reviewing its placeholder warning. **Fill untranslated from memory** fills only unique candidates and leaves conflicts for manual review.

Reused text receives one `<SIGNAL_AutoTL>` marker. The preview hides that marker and any provenance tail, and draws the text in pink. That pink is a review signal, not evidence that the wording is correct. Read the full sentence in context and check tags before accepting it into your deliverable.

The marker remains in the raw saved translation unless you edit it. Follow your team's export policy before feeding tagged text into another tool. Do not remove unrelated game signals while cleaning review markers.

Existing translations and intentionally empty completed rows are protected from automatic filling. Stop cancels indexing while keeping the previous memory available. If opening-time reuse has already applied some rows before cancellation, those edits remain in memory for review.

## 14. Use voices, text-to-speech and autoplay

These are optional review aids; they are unnecessary for text translation or screenshot export.

**Voice WAV** plays the recorded voice named in dialogue metadata. Place it under `Sound/Voices/DR`; nested character folders are supported. The filename must match the metadata, with `.wav` added when omitted. A fallback search can also find a matching WAV elsewhere under the asset root.

**Read text** uses Windows text-to-speech. Choose a language such as `en-US` in the preview. Voice availability depends on your Windows installation; the application can fall back to its default installed voice. TTS is a synthetic reading of displayed text, not a replacement for the game's recorded performance.

Enable navigation voice/TTS options if you want playback when moving between dialogues. Stop Audio ends playback. Autoplay uses the selected interval to move through the file and waits for active audio to finish. Pause/Resume suspends and restarts navigation; Stop ends playback/navigation and can cancel memory indexing.

Typing, manual navigation and editing dialogs stop autoplay so a replacement or paste stays attached to the intended dialogue. For careful proofreading, navigate manually and compare the original, translation and preview before moving on.

## 15. Prepare a review or screenshot session

### Translator's session

1. Open the working PO and confirm whether automatic reuse is enabled.
2. Expand the source view and keep the summary beside it.
3. Use `F8` to find unfinished rows, then translate in context.
4. Resolve placeholder warnings and check line lengths in the preview when assets are available.
5. Save the PO regularly. Keep an optional JSON snapshot when completion flags or multiple languages matter.
6. Reopen the saved deliverable and spot-check dialogue IDs, metadata and text.

### Screenshot session

1. Create a small project or open a PO with the desired character metadata.
2. Select DR, the asset root, background and font.
3. Choose whether to show the character and whether the preview should use the original or translation.
4. Set line breaks and styling, then inspect the entire image.
5. Save each PNG with a useful filename such as `scene-demo-001.png`.
6. Save the text project separately so the image can be reproduced later.

Changing the preview window size does not increase export resolution. The image stays at 1280×720, and controls, status text and window borders are not part of it. If you want a differently sized final graphic, resize the exported PNG in an image editor.

## 16. Troubleshoot common problems

| Symptom | What to check |
| --- | --- |
| I cannot see the original text or search controls. | Expand the main editor; `Ctrl+F` also expands it and focuses search. |
| The dialogue list disappeared. | Press `F6`; it is a separate window. |
| The preview disappeared. | Press `F7`; closing it does not close the translation project. |
| The preview still shows the source after I translated it. | Disable Preview Original and check the selected dialogue/language. |
| No background appears. | Select the parent containing `Graphics`, choose a background from the dropdown and Reload after adding files. |
| No character appears, or the wrong expression is shown. | Check the character checkbox, speaker/expression metadata, folder name and matching PNG; a missing expression can fall back to `default.png`. |
| The sprite looks cropped or too large. | Automatic Framing intentionally shows tall full-body artwork as a bust beside the text. Disable it for historical coordinates, or supply a prepared `default.png` portrait. Keep source PNGs intact while comparing modes. |
| A token is visible instead of a word. | Either leave it as intentional raw text, or load its optional dictionary and enable preview resolution. Check spelling and whole-token boundaries. |
| A MAKE stays visible. | Check that its template is defined, spelling matches, parentheses balance and no cycle is involved. |
| The text is pink. | It has an AutoTL marker; review the reused translation and your team's marker-cleanup policy. |
| Text is clipped or wraps differently from the source. | Check explicit line breaks, font, size and the optional wrapping setting. Preview wrapping does not edit saved text. |
| Letters or punctuation are missing. | Choose a font that contains those glyphs. |
| A saved PNG did not save my dialogue edits. | Image export and project save are separate; save the PO/JSON too. |
| Save As produced JSON when I wanted PO. | Select the Gettext PO filter explicitly; JSON is the current dialog's initial proposal. |
| An older VO contains several languages. | Select the intended one before exporting PO and retain a JSON snapshot for the others. |
| Saving reports that the file changed externally. | Save under another name, compare versions and reopen the intended file. |
| WAV playback finds nothing. | Check the voice metadata, exact filename, asset root and `Sound/Voices/DR` directory. |
| TTS uses an unexpected voice. | Check the language field and installed Windows voices; a fallback may be in use. |
| The preview has empty space above or beside the image. | This preserves its proportions. The image is aligned to the bottom; exported PNGs contain the canvas itself. |

## 17. Shortcut reference

| Shortcut | Action |
| --- | --- |
| `Ctrl+N` | New project. |
| `Ctrl+O` | Open a document. |
| `Ctrl+S` | Save the current path. |
| `Ctrl+Shift+S` | Save as / convert format. |
| `Ctrl+Up` / `Ctrl+Down` | Previous / next dialogue. |
| `Ctrl+F` | Search, expanding the editor if necessary. |
| `Ctrl+H` | Replace translation text. |
| `Ctrl+Z` / `Ctrl+Y` | Undo / redo in the current textbox. |
| `F6` | Open/reopen the vertical summary. |
| `F7` | Open/reopen the graphical preview. |
| `F8` | Next untranslated dialogue. |
| `F1` | Show the in-application workflow help. |

## 18. Before handing off your work

For a translation handoff:

- Save the intended PO language and reopen it to confirm the file is readable.
- Review unfinished entries, intentionally empty rows and remaining AutoTL markers.
- Check placeholder warnings, line breaks, character metadata and expressions.
- Include a JSON snapshot if your team needs multiple languages or explicit completion states.
- Include an optional dictionary only when the project uses one and your team needs it; it is not a prerequisite for ordinary text.
- Tell the recipient which external conversion or game-integration step comes next. A PO or screenshot is not an installed game patch.

For screenshot sharing, open the exported PNG and inspect it at full resolution. Save the editable project too if future revisions matter.

For technical support, describe the format, dialogue ID, chosen font/assets and the action that failed. Use a small invented reproduction when the working text or dictionary should remain private.

See the [README](../README.md) for build details and credits, the [format and feature comparison](FEATURE-PARITY.md) for current limits, and the [Windows checklist](WINDOWS-VERIFICATION.md) for native UI/audio verification.

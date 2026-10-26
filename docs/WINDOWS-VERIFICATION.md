# Windows verification of the v25 rewrite

This checklist describes checks to perform on Windows. The Linux test runner verifies formats and logic; it does not execute the native interface or audio checks below.

1. Run `dotnet run --project DGRV3TS -- examples/dialoghi.po`. The main editor must provide textboxes and variables; a separate vertical window must show three cards with original and translation together. Expand the simple view to access the source textbox.
2. Select a card and translate in the main window. The card must update without losing scroll position; edits must survive dialogue and language changes.
3. Copy the original, copy the translation and paste from an unselected card's context menu. Pasting must update that card and select it in the editor, in the active language.
4. Close the vertical summary, continue typing and reopen it with F6. All data must be current and no exception should occur. Try text longer than the window height and 150% Windows scaling.
5. Check `VARS_BAK`: `|` notes, alternatives, categories, search, insertion at the caret and reload after an external edit. Switch V3/AI to check `vars_ai.txt`.
6. Delete a token from a translation and check the warning, then restore it. Mark a translation intentionally empty and verify the summary and JSON save.
7. Index a translation folder with identical and conflicting candidates. Suggestions must allow a choice; bulk completion must leave conflicts for review and preserve existing translations.
8. Start autoplay, then type, open Replace or Suggestions, or open a summary-card menu: navigation must stop before editing. Check pause, resume, stop and end of file.
9. Open F7. Load your own assets, choose a background and sprite, and try TTF/OTF fonts, CLT tags and sizes. Copy/save a PNG, inspect the saved file and confirm the original assets remain externally editable.
10. Check WAV and TTS playback with an installed voice, language selection, stop and preview reopening. Translation text must remain editable when audio devices or assets are unavailable.
11. Open STX+WRD files with nonsequential IDs: character, expression and voice must match. Save JSON and then PO to verify metadata preservation.
12. Save and reopen multilingual PO/VO/XLSX projects. Check `.bak`, canceled saves and conflicts when another editor modifies the open file. Use copies of working files for these checks.
13. Start with a nonexistent path: the translation textbox must remain disabled until a valid document opens. Select the same card twice: caret, selection and undo history must remain intact.
14. Index a large folder and press Stop: the previous memory index must remain available. Repeat by closing the main window: scanning must stop before unsaved changes are handled.
15. Open two documents with different adjacent `VARS_BAK` dictionaries: each must load its own automatic dictionary. An explicitly chosen dictionary must remain selected when switching documents.
16. Compare `v25` and `Update` using the same PNGs: a regular character, each Monokub, Maki, a `_kak` sprite, a portrait without an expression and a missing animation. Scale and clipping must follow the original rules independently of window size.
17. Check AIBlank, AITutorial and ToWitter: position, point-sized fonts, line height and selection of the message text. Disable automatic wrapping for comparison with `v25`.
18. Open a PO with untranslated rows and a matching translation in its folder: it must be offered/reused automatically, with an AutoTL marker and pink preview text. Conflicts require a choice; cancellation or typing during indexing must preserve manual edits.
19. Use a WAV longer than the autoplay interval: navigation must wait for playback to finish while keeping the interface responsive. Try Stop before and during playback, preview closing/reopening and TTS with different character age/gender hints.
20. Start in simple view: the source view, variable action buttons and search/autoplay bar must be hidden without reserved blank space. The variable list must remain visible and usable. Arrange the three windows together and resize the preview: its exported canvas must remain 1280×720.
21. Press ⏬ to expand downward and ⏫ to collapse. Edit text, retain a selection and try Ctrl+Z after each view change: text, caret and undo history must be preserved. The two view heights must be remembered separately.
22. Press Ctrl+F in simple view: it must open the extended view and focus search. Clicking the warning counter must open the details in extended view.
23. Try both views at 100%, 150% and 200% DPI, move the window between monitors and expand near the bottom screen edge. Splitters must remain valid, and the variable list and main commands must remain accessible when the toolbar wraps to two rows.
24. Create a dictionary with invented recursive-variable and MAKE examples like those in the README. Check nested MAKE calls, variables in arguments, alternate values and expansion in the list and preview. Save and reopen: raw tokens and MAKE calls must remain intact. Try a cycle and a missing parenthesis: the editor must remain usable and show unresolved expressions.
25. Resize the preview to wide and tall proportions: controls and status must remain above the image, with nothing below it inside the client area. The complete canvas must remain proportional, horizontally centered and aligned to the bottom. Copy/save a PNG and verify it still measures 1280×720 and contains no controls or extra window space.
26. Open an invented PO that alternates fully specified contexts with missing speaker, animation or voice fields, including entries without context. Translation and saving must remain available. Missing speaker/voice metadata must not reuse the preceding dialogue's sprite or WAV; a known speaker without an expression may use its default portrait without adding metadata to the saved file.

Known limits: rendering still requires native comparison; PO plurals and non-UTF-8 declarations are unsupported; STX supports one table; `.xls` is unsupported; XLSX translation formula cells are protected. Unknown WRD opcodes are rejected before partial metadata can be applied.

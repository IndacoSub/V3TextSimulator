using System.ComponentModel;
using DGRV3TS.Core;

namespace DGRV3TS;

/// <summary>The main v25 translation window. Layout and application behavior are separate partials.</summary>
internal sealed partial class Operations : Form
{
    private TranslationDocument _document = TranslationDocument.Create();
    private VariableCatalog _catalog = VariableCatalog.Empty;
    private TranslationMemory? _memory;
    private int _index = -1;
    private TranslationEntry? _displayedEntry;
    private int _displayedLanguage = -1;
    private bool _binding;
    private bool _busy;
    private CancellationTokenSource? _memoryScan;
    private bool _closeWhenScanStops;
    private bool _catalogIsExplicit;
    private readonly HashSet<string> _hiddenVariableTokens = new(StringComparer.Ordinal);
    private string _assetRoot = AppContext.BaseDirectory;
    private readonly System.Windows.Forms.Timer _autoplay = new() { Interval = 3000 };
    private VerticalView? _vertical;
    private DialoguePreviewWindow? _preview;
    private bool _autoplayPaused;
    private bool _voiceOnNavigate;
    private bool _ttsOnNavigate;
    private bool _reuseOnOpen = true;
    private bool _automaticMemoryScan;
    private TranslationEntry? Current => _index >= 0 && _index < _document.Entries.Count ? _document.Entries[_index] : null;

    public Operations() : this(null) { }

    public Operations(string? initialPath)
    {
        InitializeComponent();
        if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            InitializeWorkspace(initialPath);
    }

    private bool Safe(Action action)
    {
        try { action(); return true; }
        catch (Exception e) when (e is not OutOfMemoryException and not StackOverflowException)
        { MessageBox.Show(this, e.Message, "Operazione non completata", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _memoryScan?.Cancel();
            _autoplay.Dispose();
            _vertical?.Dispose();
            _preview?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}

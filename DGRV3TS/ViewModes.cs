namespace DGRV3TS;

internal sealed partial class Operations
{
    // Logical client heights at 96 DPI. Width, text and controls are retained between modes.
    private int _simpleHeight = 240;
    private int _extendedHeight = 721;
    private int _sourceDivider = 140;
    private bool _extendedView = true;
    private bool _viewModeInitialized;
    private bool _changingView;

    private int ViewPixels(int logical) => (int)Math.Round(logical * DeviceDpi / 96.0);
    private int ViewLogical(int pixels) => (int)Math.Round(pixels * 96.0 / DeviceDpi);

    private void InitializeViewModes()
    {
        // Load runs after the initial DPI autoscale and before the form becomes visible.
        Load += (_, _) => SetExtendedView(false);
        SizeChanged += (_, _) => UpdateSimpleMinimum();
        _contentSplit.SplitterMoved += (_, _) => UpdateSimpleMinimum();
        DpiChanged += (_, _) =>
        {
            if (_viewModeInitialized && IsHandleCreated)
                BeginInvoke(new Action(() => { if (!IsDisposed) SetExtendedView(_extendedView, resizeWindow: false); }));
        };
        _validationStatus.Click += (_, _) => SetExtendedView(true);
        _variableToolTip.SetToolTip(_refreshPreviewButton, "Aggiorna anteprima");
    }

    private void SetExtendedView(bool extended, bool resizeWindow = true)
    {
        if (_changingView) return;
        if (_viewModeInitialized && extended == _extendedView && resizeWindow) return;
        _changingView = true;
        try
        {
            if (resizeWindow) StopAutoplay();
            if (resizeWindow && WindowState != FormWindowState.Normal) WindowState = FormWindowState.Normal;
            if (resizeWindow && _viewModeInitialized)
            {
                if (_extendedView) _extendedHeight = ViewLogical(ClientSize.Height);
                else _simpleHeight = ViewLogical(ClientSize.Height);
            }
            if (_extendedView && !_textSplit.Panel1Collapsed)
                _sourceDivider = ViewLogical(_textSplit.SplitterDistance);

            MinimumSize = Size.Empty;
            _contentSplit.Panel1MinSize = 0;
            _contentSplit.Panel2MinSize = 0;
            // Grow first: uncollapsing the source panel into a short splitter can throw.
            if (extended && resizeWindow) ResizeView(_extendedHeight);
            SuspendLayout();
            _rootLayout.SuspendLayout();
            try
            {
                _extendedView = extended;
                _searchBar.Visible = extended;
                _metadata.Visible = extended;
                SetRow(_rootLayout, 2, extended ? SizeType.AutoSize : SizeType.Absolute, 0);
                SetRow(_rootLayout, 3, SizeType.Absolute, extended ? 34 : 0);

                _textSplit.Panel1MinSize = 0;
                _textSplit.Panel2MinSize = ViewPixels(32);
                _textSplit.Panel1Collapsed = !extended;
                _sourceLayout.Visible = extended;
                _copySourceButton.Visible = extended;
                _suggestButton.Visible = extended;
                _textActions.Visible = extended;
                _checks.Visible = extended;
                SetRow(_editorLayout, 1, extended ? SizeType.AutoSize : SizeType.Absolute, 0);
                SetRow(_editorLayout, 2, SizeType.Absolute, extended ? 48 : 0);

                _variableTitle.Visible = extended;
                _variableActions.Visible = extended;
                _insertVariableButton.Visible = extended;
                _loadVariablesButton.Visible = extended;
                _reloadVariablesButton.Visible = extended;
                _variableNote.Visible = extended;
                _variableHint.Visible = extended;
                SetRow(_variableLayout, 0, SizeType.Absolute, extended ? 30 : 0);
                SetRow(_variableLayout, 4, extended ? SizeType.AutoSize : SizeType.Absolute, 0);
                SetRow(_variableLayout, 5, SizeType.Absolute, extended ? 58 : 0);
                SetRow(_variableLayout, 6, SizeType.Absolute, extended ? 38 : 0);

                _viewToggleButton.Text = extended ? "⏫ Vista semplice" : "⏬ Vista estesa";
                _viewToggleButton.AccessibleName = extended ? "Apri vista semplice" : "Apri vista estesa";
                _viewToggleButton.ToolTipText = extended ? "Riduci la finestra alla vista semplice" : "Apri la vista estesa verso il basso";
                _viewModeInitialized = true;
            }
            finally { _rootLayout.ResumeLayout(true); ResumeLayout(true); }

            if (!extended && resizeWindow) ResizeView(Math.Max(_simpleHeight, ViewLogical(SimpleMinimumClientHeight())));
            if (extended)
            {
                var available = Math.Max(0, _textSplit.Height - _textSplit.SplitterWidth);
                var sourceMinimum = Math.Min(ViewPixels(70), available / 3);
                var translationMinimum = Math.Min(ViewPixels(100), available / 3);
                _textSplit.SplitterDistance = Math.Clamp(ViewPixels(_sourceDivider), sourceMinimum, Math.Max(sourceMinimum, available - translationMinimum));
                _textSplit.Panel1MinSize = sourceMinimum;
                _textSplit.Panel2MinSize = translationMinimum;
                var area = Screen.FromControl(this).WorkingArea;
                MinimumSize = new Size(Math.Min(ViewPixels(960), area.Width), Math.Min(ViewPixels(600), area.Height));
            }
            else ApplySimpleMinimum();
            var workingArea = Screen.FromControl(this).WorkingArea;
            if (Width > workingArea.Width || Height > workingArea.Height) ResizeView(ViewLogical(ClientSize.Height));
            SetContentMinimums();
        }
        finally { _changingView = false; }
    }

    private void SetRow(TableLayoutPanel panel, int row, SizeType type, int logicalHeight)
    {
        panel.RowStyles[row].SizeType = type;
        panel.RowStyles[row].Height = ViewPixels(logicalHeight);
    }

    private int SimpleMinimumClientHeight()
    {
        var width = Math.Max(1, ClientSize.Width - _rootLayout.Padding.Horizontal);
        var navigation = _navigation.GetPreferredSize(new Size(width, 0)).Height;
        var options = _previewOptions.GetPreferredSize(new Size(Math.Max(1, _contentSplit.Panel1.Width), 0)).Height;
        var body = Math.Max(ViewPixels(72 + 48), ViewPixels(26 + 48) + options);
        return Math.Max(ViewPixels(200), _mainMenu.Height + navigation + _statusStrip.Height + _rootLayout.Padding.Vertical + body);
    }

    private void ApplySimpleMinimum()
    {
        var frameHeight = SizeFromClientSize(new Size(0, SimpleMinimumClientHeight())).Height;
        var area = Screen.FromControl(this).WorkingArea;
        MinimumSize = new Size(Math.Min(ViewPixels(960), area.Width), Math.Min(frameHeight, area.Height));
    }

    private void UpdateSimpleMinimum()
    {
        if (_viewModeInitialized && !_extendedView && !_changingView && WindowState == FormWindowState.Normal)
        {
            _changingView = true;
            try { SetContentMinimums(); ApplySimpleMinimum(); }
            finally { _changingView = false; }
        }
    }

    private void SetContentMinimums()
    {
        var available = Math.Max(0, _contentSplit.Width - _contentSplit.SplitterWidth);
        var editor = Math.Min(ViewPixels(400), available * 2 / 3);
        var variables = Math.Min(ViewPixels(260), available / 3);
        _contentSplit.Panel1MinSize = 0;
        _contentSplit.Panel2MinSize = 0;
        _contentSplit.SplitterDistance = Math.Clamp(_contentSplit.SplitterDistance, editor, Math.Max(editor, available - variables));
        _contentSplit.Panel1MinSize = editor;
        _contentSplit.Panel2MinSize = variables;
    }

    private void ResizeView(int logicalHeight)
    {
        var area = Screen.FromControl(this).WorkingArea;
        var target = SizeFromClientSize(new Size(ClientSize.Width, ViewPixels(logicalHeight)));
        var height = Math.Min(target.Height, area.Height);
        var width = Math.Min(Width, area.Width);
        var top = Math.Clamp(Top, area.Top, Math.Max(area.Top, area.Bottom - height));
        var left = Math.Clamp(Left, area.Left, Math.Max(area.Left, area.Right - width));
        SetBounds(left, top, width, height);
    }
}

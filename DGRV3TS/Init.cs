namespace DGRV3TS
{
    public enum GameIndex
    {
        V3,
        AI,

        Count,
    };

    partial class Operations
    {
        GameIndex CurrentGameIndex = GameIndex.V3;

        private bool AutoPlayOn;
        private bool Convention;
        public bool DEBUG_ON = false;
        private bool FastReading;

        private FileManager fi;
        private FontManager fm = new FontManager("Arial");
        private ImageManager im;

        private bool LoadedFile;
        private SoundManager sm;

        private TranslationManager tm = new TranslationManager();
        private VariableManager vm = new VariableManager(false);

        private void InitWindow()
        {
            // DR is default
            if (im.BackgroundImagesDR.Count <= 0)
            {
                return;
            }
            if (dialogue_window.DisplayedImage.Image != null) dialogue_window.DisplayedImage.Image.Dispose();
            dialogue_window.DisplayedImage.Image = new Bitmap(new Bitmap(im.BackgroundImagesDR[CB_TB.SelectedIndex]),
                new Size(dialogue_window.DisplayedImage.Width, dialogue_window.DisplayedImage.Height));
            dialogue_window.DisplayedImage.Refresh();
        }
        private void Init()
        {
            CreateListBoxMenu();

            LoadedFile = new bool();
            LoadedFile = false;

            AutoPlayOn = new bool();
            AutoPlayOn = false;

            FastReading = new bool();
            FastReading = false;

            Convention = new bool();
            Convention = false;

            CB_Game.Items.Clear();
            CB_Game.Items.Add("V3");
            CB_Game.Items.Add("AI");

            CB_Game.SelectedIndex = 0;

            // Check if the executable name is DGRV3TEST
            // If so, enable some debug functionalities
            // TODO: Such as...?
            if (AppDomain.CurrentDomain.FriendlyName.Contains("DGRV3TEST"))
            {
                DEBUG_ON = true;
            }
#if DEBUG
            DEBUG_ON = true;
#endif

            im = new ImageManager(CurrentGameIndex);

            InitCBTB(CurrentGameIndex);

            vm = new VariableManager(Convention);
            foreach (string ms in vm.Menu.Items)
            {
                ListBoxMenuIndex.Items.Add(ms);
            }
            bool has_vars = ListBoxMenuIndex.Items.Count > 0;
            ListBoxMenuIndex.Visible = has_vars;
            ListBoxMenuElements.Visible = has_vars;

            tm = new TranslationManager();

            fm = new FontManager("");

            NumericUpDownFontSize.Value = (int)fm.CurrentFontSize;

            NumericUpDownFontSize.ValueChanged += NumericUpDownFontSize_ValueChanged;

            LabelFontName.Text = fm.DefaultFontName;

            fi = new FileManager();

            fi.GameIndex = CurrentGameIndex;

            CB_TB.SelectedIndexChanged += CB_TB_SelectedIndexChanged;

            TextboxCurrentLanguage.Text = "en-US";

            sm = new SoundManager();

            ButtonReloadText_Click(null, null);

            // Create the ToolTip and associate with the Form container.
            toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Collect garbage from initialization?
            GC.Collect();
        }

        private void InitCBTB(GameIndex index)
        {
            // Add the backgrounds from /Graphics/Backgrounds/ and assign them a name

            List<string> backgrounds = new List<string>();

            switch (index)
            {
                case GameIndex.V3:
                    backgrounds = im.BackgroundImagesDR;
                    break;
                case GameIndex.AI:
                    backgrounds = im.BackgroundImagesAI;
                    break;
            }

            CB_TB.Items.Clear();

            if (backgrounds.Count <= 0)
            {
                return;
            }

            foreach (string file in backgrounds)
            {
                string fn = Path.GetFileNameWithoutExtension(file);
                CB_TB.Items.Add(fn);
            }

            CB_TB.SelectedIndex = 0;
        }
    }
}
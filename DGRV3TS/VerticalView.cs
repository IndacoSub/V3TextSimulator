using DGRV3TS.Subforms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinRT;

namespace DGRV3TS
{
    public partial class VerticalView : Form
    {

        Point LastScrollPosition = new Point(0, 0);
        Point LastScrollOffset = new Point(0, 0);
        int ScrollValue = 0;

        public VerticalView(List<string> original, List<string> translated, List<string> speakers)
        {
            InitializeComponent();
            InitSummaries(original, translated, speakers);
        }

        Color GetColorBySpeaker(string speaker)
        {
            if(speaker == null || speaker.Length == 0)
            {
                return Color.Black;
            }

            switch(speaker)
            {
                case "C000_Saiha":
                    return Color.MediumBlue;
                case "C001_Momot":
                    return Color.Violet;
                case "C002_Hoshi":
                    return Color.Navy;
                case "C003_Amami":
                    return Color.DarkGreen;
                case "C004_Gokuh":
                    return Color.ForestGreen;
                case "C005_Oma__":
                    return Color.Purple;
                case "C006_Shing":
                    return Color.Brown;
                case "C007_Ki-Bo":
                    return Color.SteelBlue;
                case "C008_Tojo_":
                    return Color.DarkCyan;
                case "C009_Yumen":
                    return Color.DarkRed;
                case "C010_Haruk":
                    return Color.IndianRed;
                case "C011_Chaba":
                    return Color.DodgerBlue;
                case "C012_Shiro":
                    return Color.MidnightBlue;
                case "C013_Yonag":
                    return Color.Orange;
                case "C014_Iruma":
                    return Color.DeepPink;
                case "C015_Akama":
                    return Color.HotPink;
                case "C020_Monok":
                    return Color.Black;
                case "C021_Mtaro":
                    return Color.Red;
                case "C022_Msuke":
                    return Color.DarkOrange;
                case "C024_Mdam_":
                    return Color.Green;
                case "C025_Mkid_":
                    return Color.Blue;
                case "C026_Eguis":
                    return Color.Gray;
                case "C027_Mono5":
                    return Color.Gray;
                case "C035_Exred":
                    return Color.Red;
                case "C036_Exyel":
                    return Color.DarkOrange;
                case "C038_Exgre":
                    return Color.Green;
                case "C039_Exblu":
                    return Color.Blue;
                case "C023_Mfunn":
                case "C037_Expin":
                    return Color.DeepPink;

                default:
                    return Color.Black;
            }
        }

        public void InitSummaries(List<string> original, List<string> translated, List<string> speakers)
        {
            if(original == null || original.Count == 0 || translated == null || translated.Count == 0)
            {
                return;
            }

            this.panel1.Controls.Clear();

            this.panel1.AutoScroll = true;
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            int last_size_y = 0;
            int last_location_y = 0;
            int gap = 10;

            for (int i = 0; i < original.Count; i++) {

                string speaker = speakers.Count > 0 ? speakers[i] : "";
                int newy = (last_location_y + last_size_y + (i == 0 ? 0 : gap));

                VerticalSummary summary = new VerticalSummary();

                //summary.AutoScroll = true;
                summary.AutoSize = true;
                summary.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                summary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                summary.Location = new System.Drawing.Point(0, newy);
                summary.BackColor = Color.White;
                summary.ForeColor = GetColorBySpeaker(speaker);
                //summary.MaximumSize = new System.Drawing.Size(0, 0);
                summary.MaximumSize = new System.Drawing.Size(230, 200);
                summary.MinimumSize = new System.Drawing.Size(230, 40);
                summary.Name = "verticalSummary1";
                summary.TabIndex = 1;

                summary.SetID(i);
                summary.SetString(original[i], translated[i]);

                summary.Update();
                summary.Refresh();

                last_location_y = newy;
                //last_size_y = summary.ClientSize.Height;
                //last_size_y = summary.Size.Height;
                //last_size_y = summary.Size.Height * 2;
                //last_size_y = summary.MaximumSize.Height;
                //last_size_y = summary.Height;
                //last_size_y = summary.Bounds.Height;
                last_size_y = summary.PreferredSize.Height;

                this.panel1.Controls.Add(summary);
                //this.vScrollBar1.Controls.Add(summary);
            }

            //this.panel1.Controls.Add(this.vScrollBar1);

            panel1.VerticalScroll.Value = ScrollValue;

            this.Refresh();
        }

        public void UpdateSummaries(List<string> original, List<string> translated, int index)
        {

            int last_size_y = 0;
            int last_location_y = 0;
            int gap = 10;
            int valid_index = -1;

            this.panel1.AutoScrollOffset = new Point(0, 0);
            this.panel1.AutoScrollPosition = new Point(0, 0);

            if(index >= original.Count)
            {
                InputManager.Print("Invalid ID " + index + "!");
            }

            for (int i = 0; i < original.Count; i++)
            {

                if(i < 0)
                {
                    continue;
                }

                var panel = this.panel1;
                if(panel == null)
                {
                    break;
                }

                Control.ControlCollection controls = panel.Controls;
                if(controls == null)
                {
                    break;
                }

                valid_index++;

                if(controls.Count <= valid_index)
                {
                    InputManager.Print("Invalid control index: " + valid_index + ", arg index: " + index);
                    break;
                }

                var single_control = controls[valid_index];

                if(single_control == null)
                {
                    InputManager.Print("Null control index: " + valid_index + ", arg index: " + index);
                    break;
                }

                VerticalSummary summary = single_control as VerticalSummary;

                if (summary == null)
                {
                    continue;
                }

                int id = summary.ID;

                if (id < index)
                {
                    last_location_y = summary.Location.Y;
                    last_size_y = summary.PreferredSize.Height;
                    continue;
                }

                int newy = (last_location_y + last_size_y + (id == 0 ? 0 : gap));

                summary.Location = new System.Drawing.Point(0, newy);

                summary.SetString(original[i], translated[i]);

                last_location_y = newy;
                last_size_y = summary.PreferredSize.Height;
            }

            panel1.VerticalScroll.Value = ScrollValue;

            this.Refresh();
        }

        public void OnClick(int ID)
        {
            ScrollValue = panel1.VerticalScroll.Value;

            var owner = this.Owner;
            if(owner == null)
            {
                return;
            }
            //InputManager.Print(this.Owner.Name);
            (this.Owner as Operations).OnVerticalViewClick(ID);
        }
    }
}

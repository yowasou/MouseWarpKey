using KeyboardHookManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IniParser;
using IniParser.Model;
using System.IO;

namespace MouseWarpKey
{
    public partial class frmMouseWarpKey : Form
    {
        int leftX = 0;
        int leftY = 0;
        int rightX = 0;
        int rightY = 0;
        string shortCutKey = "Q";
        Keys shortCutKeys = Keys.Q;
        string iniFileName = "Configuration.ini";
        int setMode = 0;

        public frmMouseWarpKey()
        {
            InitializeComponent();
            HookManager.KeyDown += HookManager_KeyDown;
            var parser = new FileIniDataParser();
            IniData data = null;
            if (!File.Exists(iniFileName))
            {
                data = new IniData();
                data["LEFT"]["X"] = "0";
                data["LEFT"]["Y"] = "0";
                data["RIGHT"]["X"] = "0";
                data["RIGHT"]["Y"] = "0";
                data["KEYS"]["SHORTCUTKEY"] = "Q";
                parser.WriteFile(iniFileName, data);
            }
            data = parser.ReadFile(iniFileName);
            leftX = Convert.ToInt32(data["LEFT"]["X"]);
            leftY = Convert.ToInt32(data["LEFT"]["Y"]);
            rightX = Convert.ToInt32(data["RIGHT"]["X"]);
            rightY = Convert.ToInt32(data["RIGHT"]["Y"]);
            shortCutKey = data["KEYS"]["SHORTCUTKEY"];
            shortCutKeys = (Keys)Enum.Parse(typeof(Keys), shortCutKey, true);
            ShowMousePosition();
            label3.Visible = false;
        }
        private void SaveIni()
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(iniFileName);
            data["LEFT"]["X"] = leftX.ToString();
            data["LEFT"]["Y"] = leftY.ToString();
            data["RIGHT"]["X"] = rightX.ToString();
            data["RIGHT"]["Y"] = rightY.ToString();
            data["KEYS"]["SHORTCUTKEY"] = shortCutKey;
            parser.WriteFile(iniFileName, data);
        }

        private void ShowMousePosition()
        {
            label1.Text = leftX.ToString() + "," + leftY.ToString();
            label2.Text = rightX.ToString() + "," + rightY.ToString();
        }

        private void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == shortCutKeys)
            {
                if (setMode == 1)
                {
                    leftX = Cursor.Position.X;
                    leftY = Cursor.Position.Y;
                    setMode = 0;
                    label3.Visible = false;
                    ShowMousePosition();
                    SaveIni();
                }
                else if (setMode == 2)
                {
                    rightX = Cursor.Position.X;
                    rightY = Cursor.Position.Y;
                    setMode = 0;
                    label3.Visible = false;
                    ShowMousePosition();
                    SaveIni();
                }
                else
                {
                    Point p = new Point(leftX, leftY);
                    if (Cursor.Position.X < Screen.PrimaryScreen.Bounds.Width / 2)
                    {
                        p = new Point(rightX, rightY);
                    }
                    Cursor.Position = p;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label3.Visible = true;
            label3.Text = "Please down " + shortCutKey + " key on position";
            if (((Button)sender).Name == "button1")
            {
                setMode = 1;
            }
            else
            {
                setMode = 2;
            }

        }
    }
}

using System;
using System.Data;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace Ikarus
{
    /// <summary>
    /// Interaction logic for Monitor10x24.xaml
    /// </summary>
    public partial class Monitor04x08 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private static string newline = Environment.NewLine;

        private int[] codeChar = new int[19];
        private string[] replaceString = new string[19];
        private string[] character = new string[19];

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        public Monitor04x08()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            Line1.Text = "";
            Line2.Text = "";
            Line3.Text = "";
            Line4.Text = "";

            codeChar[0] = 0x00BB;
            codeChar[1] = 0x00AB;
            codeChar[2] = 0x00AE;
            codeChar[3] = 0x00A1;
            codeChar[4] = 0x00A9;
            codeChar[5] = 0x00B0;
            codeChar[6] = 0x00B6;
            codeChar[7] = 0x00B1;
            codeChar[8] = 0x003A;
            codeChar[9] = 0x2192;
            codeChar[10] = 0x0030;
            codeChar[11] = 0x0031;
            codeChar[12] = 0x2299;
            codeChar[13] = 0x2190;
            codeChar[14] = 0x2337;
            codeChar[15] = 0x2195;
            codeChar[16] = 0x2588;
            codeChar[17] = 0x003A;
            codeChar[18] = 0x002A;


            replaceString[0] = "0x00BB";
            replaceString[1] = "0x00AB";
            replaceString[2] = "0x00AE";
            replaceString[3] = "0x00A1";
            replaceString[4] = "0x00A9";
            replaceString[5] = "0x00B0";
            replaceString[6] = "0x00B6";
            replaceString[7] = "0x00B1";
            replaceString[8] = "0x003A";
            replaceString[9] = "0x2192";
            replaceString[10] = "0x0030";
            replaceString[11] = "0x0031";
            replaceString[12] = "0x2299";
            replaceString[13] = "0x2190";
            replaceString[14] = "0x2337";
            replaceString[15] = "0x2195";
            replaceString[16] = "0x2588";
            replaceString[17] = "0x003A";
            replaceString[18] = "0x002A";

            for (int n = 0; n < codeChar.Length; n++)
            {
                //character[n] = ((char)codeChar[n]).ToString();
                character[n] = char.ConvertFromUtf32(codeChar[n]);
            }
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
        }

        public string GetID() { return dataImportID; }

        private void LoadBmaps()
        {
            DataRow[] dataRows = MainWindow.dtInstruments.Select("IDInst=" + dataImportID);

            if (dataRows.Length > 0)
            {
                string frame = dataRows[0]["ImageFrame"].ToString();
                string light = dataRows[0]["ImageLight"].ToString();

                try
                {
                    if (frame.Length > 4)
                    {
                        if (File.Exists(Environment.CurrentDirectory + "\\Images\\Frames\\" + frame))
                            Frame.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + frame));
                    }
                    if (light.Length > 4)
                    {
                        if (File.Exists(Environment.CurrentDirectory + "\\Images\\Frames\\" + light))
                            Light.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + light));
                    }
                    SwitchLight(false);
                }
                catch { }
            }
        }

        public void SwitchLight(bool _on)
        {
            //Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void SetInput(string _input)
        {
            string[] vals = _input.Split(';');

            if (vals.Length < 3) { return; }

            try
            {
                SolidColorBrush color = new SolidColorBrush(Color.FromRgb(Convert.ToByte(vals[0]), Convert.ToByte(vals[1]), Convert.ToByte(vals[2])));

                Line1.Foreground = color;
                Line2.Foreground = color;
                Line3.Foreground = color;
                Line4.Foreground = color;
            }
            catch { }
        }

        public void SetOutput(string _output)
        {
            if (_output != "-")
            {
                Line1.Text = _output;
                Line2.Text = _output;
                Line3.Text = _output;
                Line4.Text = _output;
            }
        }

        public double GetSize()
        {
            return 290.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');

            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           string line1 = "", line2 = "", line3 = "", line4 = "";

                           try
                           {
                               if (vals.Length > 0) { line1 = vals[0]; }
                               if (vals.Length > 1) { line2 = vals[1]; }
                               if (vals.Length > 2) { line3 = vals[2]; }
                               if (vals.Length > 3) { line4 = vals[3]; }

                               line1 = ReplaceStrings(line1);
                               line2 = ReplaceStrings(line2);
                               line3 = ReplaceStrings(line3);
                               line4 = ReplaceStrings(line4);

                               Line1.Text = line1.ToString();
                               Line2.Text = line2.ToString();
                               Line3.Text = line3.ToString();
                               Line4.Text = line4.ToString();
                           }
                           catch { return; };
                       }));
        }

        private string ReplaceStrings(string line)
        {
            for (int n = 0; n < replaceString.Length; n++)
            {
                try
                {
                    line = line.Replace(replaceString[n], character[n]);
                }
                catch { }
            }
            return line;
        }

        private void MakeDraggable(System.Windows.UIElement moveThisElement, System.Windows.UIElement movedByElement)
        {
            System.Windows.Point originalPoint = new System.Windows.Point(0, 0), currentPoint;
            TranslateTransform trUsercontrol = new TranslateTransform(0, 0);
            bool isMousePressed = false;

            movedByElement.MouseLeftButtonDown += (a, b) =>
            {
                isMousePressed = true;
                originalPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
            };

            movedByElement.MouseLeftButtonUp += (a, b) =>
            {
                isMousePressed = false;
                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID);
            };
            movedByElement.MouseLeave += (a, b) => isMousePressed = false;

            movedByElement.MouseMove += (a, b) =>
            {
                if (!isMousePressed || !MainWindow.editmode) return;

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                trUsercontrol.X += currentPoint.X - originalPoint.X;
                trUsercontrol.Y += currentPoint.Y - originalPoint.Y;
                moveThisElement.RenderTransform = trUsercontrol;


            };
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}

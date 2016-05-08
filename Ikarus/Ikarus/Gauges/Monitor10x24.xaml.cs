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
    public partial class Monitor10x24 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private static string newline = Environment.NewLine;

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        public Monitor10x24()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            Line1.Text = "";
            Line2.Text = "";
            Line3.Text = "";
            Line4.Text = "";
            Line5.Text = "";
            Line6.Text = "";
            Line7.Text = "";
            Line8.Text = "";
            Line9.Text = "";
            Line10.Text = "";
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
                Line5.Foreground = color;
                Line6.Foreground = color;
                Line7.Foreground = color;
                Line8.Foreground = color;
                Line9.Foreground = color;
                Line10.Foreground = color;
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
                Line5.Text = _output;
                Line6.Text = _output;
                Line7.Text = _output;
                Line8.Text = _output;
                Line9.Text = _output;
                Line10.Text = _output;
            }
        }

        public double GetSize()
        {
            return 290.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            //strData.Replace(newline, ";");

            string[] vals = strData.Split(';');

            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           string line1 = "", line2 = "", line3 = "", line4 = "", line5 = "", line6 = "", line7 = "", line8 = "", line9 = "", line10 = "";

                           try
                           {
                               if (vals.Length > 0) { line1 = vals[0]; }
                               if (vals.Length > 1) { line2 = vals[1]; }
                               if (vals.Length > 2) { line3 = vals[2]; }
                               if (vals.Length > 3) { line4 = vals[3]; }
                               if (vals.Length > 4) { line5 = vals[4]; }
                               if (vals.Length > 5) { line6 = vals[5]; }
                               if (vals.Length > 6) { line7 = vals[6]; }
                               if (vals.Length > 7) { line8 = vals[7]; }
                               if (vals.Length > 8) { line9 = vals[8]; }
                               if (vals.Length > 9) { line10 = vals[9]; }

                               Line1.Text = line1.ToString();
                               Line2.Text = line2.ToString();
                               Line3.Text = line3.ToString();
                               Line4.Text = line4.ToString();
                               Line5.Text = line5.ToString();
                               Line6.Text = line6.ToString();
                               Line7.Text = line7.ToString();
                               Line8.Text = line8.ToString();
                               Line9.Text = line9.ToString();
                               Line10.Text = line10.ToString();
                           }
                           catch { return; };
                       }));
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

            movedByElement.MouseLeftButtonUp += (a, b) => isMousePressed = false;
            movedByElement.MouseLeave += (a, b) => isMousePressed = false;

            movedByElement.MouseMove += (a, b) =>
            {
                if (!isMousePressed || !MainWindow.editmode) return;

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                trUsercontrol.X += currentPoint.X - originalPoint.X;
                trUsercontrol.Y += currentPoint.Y - originalPoint.Y;
                moveThisElement.RenderTransform = trUsercontrol;

                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID);
            };
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}

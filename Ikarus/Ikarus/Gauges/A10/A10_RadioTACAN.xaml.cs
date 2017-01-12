using System;
using System.Windows.Controls;
using System.Data;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for A10_RadioTACAN.xaml
    /// </summary>
    public partial class A10_RadioTACAN : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        public A10_RadioTACAN()
        {
            InitializeComponent();
            if (MainWindow.editmode) MakeDraggable(this, this);

            Frequenz.Text = "";
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
            Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void SetInput(string _input)
        {

        }

        public void SetOutput(string _output)
        {
            string[] vals = _output.Split(',');

        }

        public double GetSize()
        {
            return 178.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');

            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           // 263, TACAN_digit_pos.hundreds
                           // 264, TACAN_digit_pos.tens
                           // 265, TACAN_digit_pos.ones
                           string TACAN_hundreds = "";
                           string TACAN_tens = "";
                           string TACAN_ones = "";

                           string tacanFreq = "";

                           try
                           {
                               if (vals.Length > 0) { TACAN_hundreds = vals[0]; }
                               if (vals.Length > 1) { TACAN_tens = vals[1]; }
                               if (vals.Length > 2) { TACAN_ones = vals[2]; }
                           }
                           catch { return; }

                           if (TACAN_hundreds.Length == 0) { TACAN_hundreds = " "; }
                           if (TACAN_tens.Length == 0) { TACAN_tens = " "; }
                           if (TACAN_ones.Length == 0) { TACAN_ones = " "; }

                           tacanFreq = TACAN_hundreds.ToString().Substring(0, 1) + TACAN_tens.ToString().Substring(0, 1) + TACAN_ones.ToString().Substring(0, 1);

                           if (tacanFreq == "000")
                           {
                               Frequenz.Text = "";
                           }
                           else
                           {
                               Frequenz.Text = TACAN_hundreds.ToString().Substring(0, 1) + TACAN_tens.ToString().Substring(0, 1) + TACAN_ones.ToString().Substring(0, 1) + "X";
                           }
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

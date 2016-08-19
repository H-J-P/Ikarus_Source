using System;
using System.Windows.Controls;
using System.Data;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for F15EngRPM.xaml
    /// </summary>
    public partial class F15EngRPM : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        RotateTransform rtRPM = new RotateTransform();
        TranslateTransform ttRPM100 = new TranslateTransform();
        TranslateTransform ttRPM10 = new TranslateTransform();
        TranslateTransform ttRPM1 = new TranslateTransform();

        double rpm = 0.0;
        double rpm100 = 0.0;
        double rpm10 = 0.0;
        double rpm1 = 0.0;

        double lrpm = 0.0;
        double lrpm100 = 0.0;
        double lrpm10 = 0.0;
        double lrpm1 = 0.0;

        public F15EngRPM()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            RPMValue.Visibility = System.Windows.Visibility.Hidden;
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
            return 255.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           vals = strData.Split(';');

                           string rpmString = "";

                           try
                           {
                               if (vals.Length > 0) { rpm = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               //if (vals.Length > 1) { rpm100 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               //if (vals.Length > 2) { rpm10 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               //if (vals.Length > 3) { rpm1 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (rpm < 0.0) rpm = 0.0;

                               rpmString = Convert.ToInt16(rpm * 1100).ToString();
                               //RPMValue.Text = rpmString;

                               if (rpmString.Length == 0) { rpmString = "0000" + rpmString; }
                               if (rpmString.Length == 1) { rpmString = "000" + rpmString; }
                               if (rpmString.Length == 2) { rpmString = "00" + rpmString; }
                               if (rpmString.Length == 3) { rpmString = "0" + rpmString; }

                               rpm100 = Convert.ToDouble((rpmString[0]).ToString(), CultureInfo.InvariantCulture);
                               rpm10 = Convert.ToDouble((rpmString[1] + ((rpm100 > 0) ? "." + rpmString[2] : "")).ToString(), CultureInfo.InvariantCulture);
                               rpm1 = Convert.ToDouble((rpmString[2] + "." + rpmString[3]).ToString(), CultureInfo.InvariantCulture);

                               if (lrpm != rpm)
                               {
                                   rtRPM.Angle = rpm * 242;
                                   EngineRPM.RenderTransform = rtRPM;
                               }
                               if (lrpm100 != rpm100)
                               {
                                   ttRPM100.Y = rpm100 * -29.2;
                                   EngineRPM_100.RenderTransform = ttRPM100;
                               }
                               if (lrpm10 != rpm10)
                               {
                                   ttRPM10.Y = rpm10 * -29.2;
                                   EngineRPM_10.RenderTransform = ttRPM10;
                               }
                               if (lrpm1 != rpm1)
                               {
                                   ttRPM1.Y = rpm1 * -29.2;
                                   EngineRPM_1.RenderTransform = ttRPM1;
                               }
                               lrpm = rpm;
                               lrpm100 = rpm100;
                               lrpm10 = rpm10;
                               lrpm1 = rpm1;
                           }
                           catch { return; }
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

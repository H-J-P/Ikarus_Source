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
    /// Interaction logic for WPALTr.xaml
    /// </summary>
    public partial class WPALTr10 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        const int valueScaleIndex = 6;

        double rAltimeter = 0.0;
        double dangerAltimeter = 0.0;
        double warningFlag = 0.0;
        double dangerAltimeterLamp = 0.0;

        double lrAltimeter = 0.0;
        double ldangerAltimeter = 0.0;
        double lwarningFlag = 0.0;
        double ldangerAltimeterLamp = 0.0;

        RotateTransform rtrAltimeter = new RotateTransform();
        RotateTransform rtDangerAltimeter = new RotateTransform();
        RotateTransform rtWarningFlag = new RotateTransform();

        public WPALTr10()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            knob_Marker_Alarm.Visibility = System.Windows.Visibility.Hidden;

            RotateTransform rtWarningFlag = new RotateTransform();
            rtWarningFlag.Angle = 10;
            Off_Flagg.RenderTransform = rtWarningFlag;
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
        }

        public double GetSize()
        {
            return 167.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { rAltimeter = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { dangerAltimeter = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { warningFlag = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { dangerAltimeterLamp = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               // WPALTr10                      		     =   { 0,    20,    50,   100,   500,  1000}
                               double[] valueScale = new double[valueScaleIndex] { 0, 0.020, 0.050, 0.100, 0.500, 1.0 };
                               double[] degreeDial = new double[valueScaleIndex] { 0, 32, 79, 157, 225, 313 };

                               if (lrAltimeter != rAltimeter)
                               {
                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (rAltimeter > valueScale[n] && rAltimeter <= valueScale[n + 1])
                                       {
                                           rtrAltimeter.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (rAltimeter - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   AltRad.RenderTransform = rtrAltimeter;
                               }
                               if (ldangerAltimeter != dangerAltimeter)
                               {
                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (dangerAltimeter > valueScale[n] && dangerAltimeter <= valueScale[n + 1])
                                       {
                                           rtDangerAltimeter.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (dangerAltimeter - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   Alarm_Marker.RenderTransform = rtDangerAltimeter;
                               }
                               if (lwarningFlag != warningFlag)
                               {
                                   rtWarningFlag.Angle = warningFlag * 10;
                                   Off_Flagg.RenderTransform = rtWarningFlag;
                               }
                               knob_Marker_Alarm.Visibility = dangerAltimeterLamp > 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lrAltimeter = rAltimeter;
                               ldangerAltimeter = dangerAltimeter;
                               lwarningFlag = warningFlag;
                               ldangerAltimeterLamp = dangerAltimeterLamp;
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

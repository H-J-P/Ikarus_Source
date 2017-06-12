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
    /// Interaction logic for KA50ADI.xaml
    /// </summary>
    public partial class KA50ADI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        RotateTransform rtSideslip = new RotateTransform();
        RotateTransform rtRoll = new RotateTransform();
        TranslateTransform ttPitch = new TranslateTransform();
        RotateTransform rtBank = new RotateTransform();
        TranslateTransform ttPitchSteering = new TranslateTransform();
        TranslateTransform ttAirspeedDeviation = new TranslateTransform();
        TranslateTransform ttTrackDeviation = new TranslateTransform();
        TranslateTransform ttHeightDeviation = new TranslateTransform();

        double roll = 0.0;
        double pitch = 0.0;
        double steeringWarningFlag = 0.0;
        double attitudeWarningFlag = 0.0;
        double bankSteering = 0.0;
        double pitchSteering = 0.0;
        double airspeedDeviation = 0.0;
        double trackDeviation = 0.0;
        double heightDeviation = 0.0;
        double sideslip = 0.0;

        double lroll = 0.0;
        double lpitch = 0.0;
        double lsteeringWarningFlag = 0.0;
        double lattitudeWarningFlag = 0.0;
        double lbankSteering = 0.0;
        double lpitchSteering = 0.0;
        double lairspeedDeviation = 0.0;
        double ltrackDeviation = 0.0;
        double lheightDeviation = 0.0;
        double lsideslip = 0.0;

        public KA50ADI()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            KA50_needleSTRG_ADI.Visibility = System.Windows.Visibility.Visible;
            KA50_needleOFF_ADI.Visibility = System.Windows.Visibility.Visible;
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
            return 206.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { roll = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { pitch = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { steeringWarningFlag = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { attitudeWarningFlag = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { bankSteering = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { pitchSteering = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { airspeedDeviation = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { trackDeviation = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { heightDeviation = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }
                               if (vals.Length > 9) { sideslip = Convert.ToDouble(vals[9], CultureInfo.InvariantCulture); }

                               if (lsideslip != sideslip)
                               {
                                   rtSideslip.Angle = sideslip * -10;
                                   KA50_needleSLIP_ADI.RenderTransform = rtSideslip;
                               }
                               if (lroll != roll)
                               {
                                   rtRoll.Angle = roll * 180;
                                   KA50_plane_ADI.RenderTransform = rtRoll;
                               }
                               if (lpitch != pitch)
                               {
                                   ttPitch.Y = pitch * -200;
                                   KA50_dialPitch_ADI.RenderTransform = ttPitch;
                               }

                               KA50_needleSTRG_ADI.Visibility = (steeringWarningFlag > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               KA50_needleOFF_ADI.Visibility = (attitudeWarningFlag > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               if (lbankSteering != bankSteering)
                               {
                                   rtBank.Angle = bankSteering * 30;
                                   KA50_needleBank_ADI.RenderTransform = rtBank;
                               }
                               if (lpitchSteering != pitchSteering)
                               {
                                   ttPitchSteering.Y = pitchSteering * -70;
                                   KA50_needlePitch_ADI.RenderTransform = ttPitchSteering;
                               }
                               if (lairspeedDeviation != airspeedDeviation)
                               {
                                   ttAirspeedDeviation.Y = airspeedDeviation * -20;
                                   KA50_needleLegSpd_ADI.RenderTransform = ttAirspeedDeviation;
                               }
                               if (ltrackDeviation != trackDeviation)
                               {
                                   ttTrackDeviation.X = trackDeviation * 25;
                                   KA50_needleLegHead_ADI.RenderTransform = ttTrackDeviation;
                               }
                               if (lheightDeviation != heightDeviation)
                               {
                                   ttHeightDeviation.Y = heightDeviation * -25;
                                   KA50_needleLegAlt_ADI.RenderTransform = ttHeightDeviation;
                               }

                               lroll = roll;
                               lpitch = pitch;
                               lsteeringWarningFlag = steeringWarningFlag;
                               lattitudeWarningFlag = attitudeWarningFlag;
                               lbankSteering = bankSteering;
                               lpitchSteering = pitchSteering;
                               lairspeedDeviation = airspeedDeviation;
                               ltrackDeviation = trackDeviation;
                               lheightDeviation = heightDeviation;
                               lsideslip = sideslip;
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

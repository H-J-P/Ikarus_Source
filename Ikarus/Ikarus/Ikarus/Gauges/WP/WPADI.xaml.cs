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
    /// Interaction logic for WPADI.xaml
    /// </summary>
    public partial class WPADI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double bank = 0.0;
        double pitch = 0.0;
        double sideslip = 0.0;
        double steeringWarningFlag = 1.0; // Flag_L
        double attitudeWarningFlag = 1.0; // Flag_R
        double desiredBank = 0.0;
        double desirePitch = 0.0;
        double trackDeviation = 0.0;
        double heightDeviation = 0.0;

        double lbank = 0.0;
        double lpitch = 0.0;
        double lsideslip = 0.0;
        double lsteeringWarningFlag = 1.0; // Flag_L
        double lattitudeWarningFlag = 1.0; // Flag_R
        double ldesiredBank = 0.0;
        double ldesirePitch = 0.0;
        double ltrackDeviation = 0.0;
        double lheightDeviation = 0.0;

        RotateTransform rtBank = new RotateTransform();
        TranslateTransform ttPitch = new TranslateTransform();
        RotateTransform rtDesired_bank = new RotateTransform();
        TranslateTransform ttDesirePitch = new TranslateTransform();
        TranslateTransform ttTrackDeviation = new TranslateTransform();
        TranslateTransform ttHeightDeviation = new TranslateTransform();
        RotateTransform rtSideslip = new RotateTransform();

        public WPADI()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            Flag_L.Visibility = System.Windows.Visibility.Visible;
            Flag_R.Visibility = System.Windows.Visibility.Visible;
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
             Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { bank = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { pitch = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { sideslip = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { steeringWarningFlag = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { attitudeWarningFlag = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { desiredBank = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { desirePitch = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { trackDeviation = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { heightDeviation = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }

                               if (lbank != bank)
                               {
                                   rtBank.Angle = bank * 180;
                                   Bank.RenderTransform = rtBank;
                               }
                               if (lpitch != pitch)
                               {
                                   ttPitch.Y = pitch * 205;
                                   Pitch.RenderTransform = ttPitch;
                               }
                               if (lsideslip != sideslip)
                               {
                                   rtSideslip.Angle = sideslip * 12;
                                   SlipBallPosition.RenderTransform = rtSideslip;
                               }

                               if (lsteeringWarningFlag != steeringWarningFlag)
                                    Flag_L.Visibility = (steeringWarningFlag > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lattitudeWarningFlag != attitudeWarningFlag)
                                    Flag_R.Visibility = (attitudeWarningFlag > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               if (ldesiredBank != desiredBank)
                               {
                                   rtDesired_bank.Angle = desiredBank * 30;
                                   Desired_bank.RenderTransform = rtDesired_bank;
                               }
                               if (ldesirePitch != desirePitch)
                               {
                                   ttDesirePitch.Y = desirePitch * -40;
                                   Desired_pitch.RenderTransform = ttDesirePitch;
                               }
                               if (ltrackDeviation != trackDeviation)
                               {
                                   ttTrackDeviation.X = trackDeviation * 25;
                                   Side.RenderTransform = ttTrackDeviation;
                               }
                               if (lheightDeviation != heightDeviation)
                               {
                                   ttHeightDeviation.Y = heightDeviation * -25;
                                   Glide.RenderTransform = ttHeightDeviation;
                               }
                               lbank = bank;
                               lpitch = pitch;
                               lsideslip = sideslip;
                               lsteeringWarningFlag = steeringWarningFlag; // Flag_L
                               lattitudeWarningFlag = attitudeWarningFlag; // Flag_R
                               ldesiredBank = desiredBank;
                               ldesirePitch = desirePitch;
                               ltrackDeviation = trackDeviation;
                               lheightDeviation = heightDeviation;
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

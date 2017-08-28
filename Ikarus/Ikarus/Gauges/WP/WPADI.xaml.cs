using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

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
        GaugesHelper helper = null;

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

            Flag_L.Visibility = System.Windows.Visibility.Visible;
            Flag_R.Visibility = System.Windows.Visibility.Visible;
            Flag_K.Visibility = System.Windows.Visibility.Hidden;
            Flag_T.Visibility = System.Windows.Visibility.Hidden;
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;

            helper = new GaugesHelper(dataImportID, windowID, "Instruments");
            helper.LoadBmaps(Frame, Light);

            SwitchLight(false);

            if (MainWindow.editmode) { helper.MakeDraggable(this, this); }
        }

        public string GetID() { return dataImportID; }

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
            return Frame.Width;
        }

        public double GetSizeY()
        {
            return Frame.Height;
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

                               //Flag_K.Visibility = System.Windows.Visibility.Hidden;
                               //Flag_T.Visibility = System.Windows.Visibility.Hidden;

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

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}

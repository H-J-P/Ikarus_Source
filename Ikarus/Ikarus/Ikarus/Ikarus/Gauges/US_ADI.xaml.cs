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
    /// Interaction logic for USADI.xaml
    /// </summary>
    public partial class US_ADI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double pitch = 0.0;
        double bank = 0.0;
        double slipBall = 0.0;
        double bankSteering = 0.0;
        double pitchSteering = 0.0;
        double turnNeedle = 0.0;
        double glideSlopeIndicator = 0.0;
        double glideSlopeWarningFlag = 0.0;
        double attitudeWarningFlag = 0.0;
        double courceWarningFlag = 0.0;

        double lpitch = 0.0;
        double lbank = 0.0;
        double lslipBall = 0.0;
        double lbankSteering = 0.0;
        double lpitchSteering = 0.0;
        double lturnNeedle = 0.0;
        double lglideSlopeIndicator = 0.0;
        double lglideSlopeWarningFlag = 1.0;
        double lattitudeWarningFlag = 1.0;
        double lcourceWarningFlag = 1.0;

        RotateTransform rtSlipball = new RotateTransform();
        TranslateTransform ttTurnIndicator = new TranslateTransform();
        TranslateTransform rtpitchSteering = new TranslateTransform();
        TranslateTransform rtbanksteering = new TranslateTransform();
        TranslateTransform rtglideSlopeIndicator = new TranslateTransform();
        TransformGroup grp = new TransformGroup();
        RotateTransform rt = new RotateTransform();
        TranslateTransform tt = new TranslateTransform();

        public US_ADI()
        {
            InitializeComponent();
            if (MainWindow.editmode) MakeDraggable(this, this);

            //Flagg_course_off.Visibility = System.Windows.Visibility.Hidden;
            //Flagg_glide_off.Visibility = System.Windows.Visibility.Hidden;
            //Flagg_course_off.Visibility = System.Windows.Visibility.Hidden;
            Flagg_off.Visibility = System.Windows.Visibility.Visible;
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
            return 314; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { pitch = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { bank = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { slipBall = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { bankSteering = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { pitchSteering = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { turnNeedle = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { glideSlopeIndicator = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { glideSlopeWarningFlag = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { attitudeWarningFlag = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }
                               if (vals.Length > 9) { courceWarningFlag = Convert.ToDouble(vals[9], CultureInfo.InvariantCulture); }

                               if (lpitch != pitch || lbank != bank)
                               {
                                   grp = new TransformGroup();
                                   rt = new RotateTransform();
                                   tt = new TranslateTransform();

                                   tt.Y = pitch * -270;
                                   rt.Angle = bank * -180;
                                   grp.Children.Add(tt);
                                   grp.Children.Add(rt);

                                   Pitch.RenderTransform = grp;
                                   Bank.RenderTransform = rt;

                                   lpitch = pitch;
                                   lbank = bank;
                               }
                               if (lslipBall != slipBall)
                               {
                                   rtSlipball.Angle = slipBall * -9;
                                   SlipBallPosition.RenderTransform = rtSlipball;
                               }
                               if (lturnNeedle != turnNeedle)
                               {
                                   ttTurnIndicator.X = turnNeedle * 40;
                                   Turnindicator.RenderTransform = ttTurnIndicator;
                               }
                               if (lpitchSteering != pitchSteering)
                               {
                                   rtpitchSteering.Y = pitchSteering * -100;
                                   pichsteering.RenderTransform = rtpitchSteering;
                               }
                               if (lbankSteering != bankSteering)
                               {
                                   rtbanksteering.X = bankSteering * 120;
                                   banksteering.RenderTransform = rtbanksteering;
                               }
                               if (lglideSlopeIndicator != glideSlopeIndicator)
                               {
                                   rtglideSlopeIndicator.Y = glideSlopeIndicator * -55;
                                   GlideSlopeIndicator.RenderTransform = rtglideSlopeIndicator;
                               }

                               if (lcourceWarningFlag != courceWarningFlag)
                                    Flagg_course_off.Visibility = (courceWarningFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lglideSlopeWarningFlag != glideSlopeWarningFlag)
                                    Flagg_glide_off.Visibility = (glideSlopeWarningFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lattitudeWarningFlag != attitudeWarningFlag)
                                    Flagg_off.Visibility = (attitudeWarningFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lpitchSteering != pitchSteering)
                                    pichsteering.Visibility = pitchSteering < -0.9 ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;

                               lpitch = pitch;
                               lbank = bank;
                               lslipBall = slipBall;
                               lbankSteering = bankSteering;
                               lpitchSteering = pitchSteering;
                               lturnNeedle = turnNeedle;
                               lglideSlopeIndicator = glideSlopeIndicator;
                               lglideSlopeWarningFlag = glideSlopeWarningFlag;
                               lattitudeWarningFlag = attitudeWarningFlag;
                               lcourceWarningFlag = courceWarningFlag;
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

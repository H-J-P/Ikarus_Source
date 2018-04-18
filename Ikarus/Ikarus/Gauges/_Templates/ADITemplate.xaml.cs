using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for AJS37_ADI.xaml
    /// </summary>
    public partial class ADITemplate : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }
        private string lightColor = "#FFFFFF"; // white

        private double pitch = 0.0;
        private double bank = 0.0;
        private double bankNeedle = 0.0;
        private double heading = 0.0;
        private double slipBall = 0.0;
        private double vvi = 0.0;
        private double flag_vvi_off = 0.0;
        private double flag_off = 0.0;
        private double courceWarningFlag = 0.0;
        private double pitchangle = 0.0;
        private double bankSteering = 0.0;
        private double glideSlope = 0.0;

        private double lpitch = 0.0;
        private double lbank = 0.0;
        private double lbankNeedle = 0.0;
        private double lheading = 0.0;
        private double lslipBall = 0.0;
        private double lvvi = 0.0;
        private double lFlag_vvi_off = 1.0;
        private double lFlag_off = 1.0;
        private double lcourceWarningFlag = 1.0;
        private double lbankSteering = 0.0;
        private double lglideSlope = 0.0;

        private RotateTransform rtSlipball = new RotateTransform();
        private TranslateTransform ttTurnIndicator = new TranslateTransform();
        private TranslateTransform rtpitchSteering = new TranslateTransform();
        private RotateTransform rtbankNeedle = new RotateTransform();
        private TranslateTransform rtvvi = new TranslateTransform();
        private TranslateTransform ttglideSlope = new TranslateTransform();
        private TranslateTransform ttbanksteering = new TranslateTransform();
        Sphere3D sphere3D;

        public ADITemplate()
        {
            InitializeComponent();

            Flagg_course_off.Visibility = System.Windows.Visibility.Hidden;
            Flagg_VVI_off.Visibility = System.Windows.Visibility.Hidden;
            Flagg_off.Visibility = System.Windows.Visibility.Visible;
            //Pitchsteering.Visibility = System.Windows.Visibility.Hidden;
            //Banksteering.Visibility = System.Windows.Visibility.Hidden;

            InitialSphere();

            //sphere3D.Rotate(0, 0,-90);
            sphere3D.xRotation = 0.0;
            sphere3D.yRotation = 0.0;
            sphere3D.zRotation = -90;
            sphere3D.Rotate();

            directionalLight.Color = (Color)ColorConverter.ConvertFromString(lightColor);
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
            directionalLight.Color = _on ? (Color)ColorConverter.ConvertFromString("#" + MainWindow.lightOnColor) : (Color)ColorConverter.ConvertFromString(lightColor);
        }

        public void SetInput(string _input)
        {
            helper.SetInput(ref _input, ref valueScale, ref valueScaleIndex, 3);
        }

        public void SetOutput(string _output)
        {
            helper.SetOutput(ref _output, ref degreeDial, 3);
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

                               if (vals.Length > 0) { pitch = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { heading = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { bank = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { slipBall = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { vvi = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { bankSteering = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { glideSlope = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { flag_vvi_off = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { flag_off = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }
                               if (vals.Length > 9) { courceWarningFlag = Convert.ToDouble(vals[9], CultureInfo.InvariantCulture); }

                               bankNeedle = bank;

                               if (lpitch != pitch)
                               {
                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (pitch >= valueScale[n] && heading <= valueScale[n + 1])
                                       {
                                           pitchangle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (pitch - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "2", heading.ToString(), Convert.ToInt32(pitchangle).ToString());
                                   }
                               }

                               if (lpitch != pitch || lheading != heading || lbank != bank)
                               {
                                   //sphere3D.Rotate(0, pitchangle * -1, (bank * 180) - 90);

                                   sphere3D.xRotation = 0;
                                   sphere3D.yRotation = pitchangle * -1;
                                   sphere3D.zRotation = ((bank * 180) - 90);
                                   sphere3D.Rotate();
                               }
                               if (glideSlope > 0.5) glideSlope = 0.5;
                               if (bankSteering > 0.5) bankSteering = 0.5;

                               if (lslipBall != slipBall)
                               {
                                   rtSlipball.Angle = slipBall * -12;
                                   SlipBallPosition.RenderTransform = rtSlipball;
                               }

                               if (lbankNeedle != bankNeedle)
                               {
                                   rtbankNeedle.Angle = bankNeedle * -180;
                                   Bank.RenderTransform = rtbankNeedle;
                               }
                               if (lvvi != vvi)
                               {
                                   rtvvi.Y = vvi * -46;
                                   VVI.RenderTransform = rtvvi;
                               }

                               if (lbankSteering != bankSteering)
                               {
                                   ttbanksteering.X = bankSteering * 140;
                                   Banksteering.RenderTransform = ttbanksteering;
                               }
                               if (lglideSlope != glideSlope)
                               {
                                   ttglideSlope.Y = glideSlope * -125;
                                   Pitchsteering.RenderTransform = ttglideSlope;
                               }

                               if (lcourceWarningFlag != courceWarningFlag)
                                   Flagg_course_off.Visibility = (courceWarningFlag > 0.8) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Hidden;
                               if (lFlag_vvi_off != flag_vvi_off)
                                   Flagg_VVI_off.Visibility = (flag_vvi_off > 0.8) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Hidden;
                               if (lFlag_off != flag_off)
                                   Flagg_off.Visibility = (flag_off > 0.8) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;

                               lpitch = pitch;
                               lheading = heading;
                               lbank = bank;
                               lslipBall = slipBall;
                               lvvi = vvi;
                               lFlag_vvi_off = flag_vvi_off;
                               lFlag_off = flag_off;
                               lcourceWarningFlag = courceWarningFlag;
                               lbankNeedle = bankNeedle;
                           }
                           catch { return; };
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }

        private void InitialSphere()
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();

            if (File.Exists(Environment.CurrentDirectory + "\\Images\\Textures3D\\US_ADI.png"))
                bitmapImage.UriSource = new Uri(Environment.CurrentDirectory + "\\Images\\Textures3D\\US_ADI.png");
            else
                bitmapImage.UriSource = new Uri(Environment.CurrentDirectory + "\\Images\\Textures3D\\CheckerTest.jpg");

            bitmapImage.DecodePixelWidth = 1024;
            bitmapImage.EndInit();

            // declaration Sphere Object with model3Dgroup Name from XAML file and Sphere texture
            sphere3D = new Sphere3D(model3DGroup, bitmapImage);
        }
    }
}

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
    /// Interaction logic for M2KC_ADI.xaml
    /// </summary>
    public partial class M2KC_ADI : UserControl, I_Ikarus
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
        private double headingAngle = 0.0;
        private double slipBall = 0.0;
        private double flag_off = 0.0;
        private double courseDeviation = 0.0;
        private double glideSlopeDeviation = 0.0;

        private double lpitch = 0.0;
        private double lbank = 0.0;
        private double lbankNeedle = 0.0;
        private double lheading = 0.0;
        private double lslipBall = 0.0;
        private double lFlag_off = 1.0;
        private double lcourseDeviation = 0.0;
        private double lglideSlopeDeviation = 0.0;

        private RotateTransform rtSlipball = new RotateTransform();
        private TranslateTransform rtpitchSteering = new TranslateTransform();
        private RotateTransform rtbankNeedle = new RotateTransform();
        private TranslateTransform ttglideSlope = new TranslateTransform();
        private TranslateTransform ttbanksteering = new TranslateTransform();
        Sphere3D sphere3D;

        public M2KC_ADI()
        {
            InitializeComponent();

            Flagg_off.Visibility = System.Windows.Visibility.Hidden;

            InitialSphere();

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
                               if (vals.Length > 1) { bank = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { heading = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { slipBall = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { flag_off = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { courseDeviation = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { glideSlopeDeviation = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }

                               bankNeedle = bank;

                               if (lheading != heading)
                               {
                                   //    -1.0, -0.5, 0.0, 0.5, 1.0,
                                   //     0.0,   90, 180, 270, 360,
                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (heading >= valueScale[n] && heading <= valueScale[n + 1])
                                       {
                                           headingAngle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (heading - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "3", heading.ToString(), Convert.ToInt32(headingAngle).ToString());
                                   }
                               }

                               if (lpitch != pitch || lheading != heading || lbank != bank)
                               {
                                   sphere3D.xRotation = pitch * 90;
                                   sphere3D.yRotation = headingAngle * -1;
                                   sphere3D.zRotation = bank * -180;
                                   sphere3D.Rotate();
                               }

                               if (lslipBall != slipBall)
                               {
                                   rtSlipball.Angle = slipBall * -15;
                                   SLIP_BALL.RenderTransform = rtSlipball;
                               }

                               if (lbankNeedle != bankNeedle)
                               {
                                   rtbankNeedle.Angle = bankNeedle * 180;
                                   BANK.RenderTransform = rtbankNeedle;
                               }

                               if (lcourseDeviation != courseDeviation)
                               {
                                   ttbanksteering.X = courseDeviation * 90;
                                   COURSE_deviation.RenderTransform = ttbanksteering;
                               }
                               if (lglideSlopeDeviation != glideSlopeDeviation)
                               {
                                   ttglideSlope.Y = glideSlopeDeviation * -90;
                                   GLIDESLOPE_deviation.RenderTransform = ttglideSlope;
                               }

                               if (lFlag_off != flag_off)
                                   Flagg_off.Visibility = (flag_off > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lpitch = pitch;
                               lheading = heading;
                               lbank = bank;
                               lslipBall = slipBall;
                               lcourseDeviation = courseDeviation;
                               lglideSlopeDeviation = glideSlopeDeviation;
                               lFlag_off = flag_off;
                               lbankNeedle = bankNeedle;
                           }
                           catch (Exception e) { ImportExport.LogMessage(GetType().Name + " got data and failed with exception: " + e.ToString()); }
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

            if (File.Exists(Environment.CurrentDirectory + "\\Images\\Textures3D\\M2KC_ADI.jpg"))
                bitmapImage.UriSource = new Uri(Environment.CurrentDirectory + "\\Images\\Textures3D\\M2KC_ADI.jpg");
            else
                bitmapImage.UriSource = new Uri(Environment.CurrentDirectory + "\\Images\\Textures3D\\CheckerTest.jpg");

            bitmapImage.DecodePixelWidth = 1024;
            bitmapImage.EndInit();

            // declaration Sphere Object with model3Dgroup Name from XAML file and Sphere texture
            sphere3D = new Sphere3D(model3DGroup, bitmapImage);
        }
    }
}

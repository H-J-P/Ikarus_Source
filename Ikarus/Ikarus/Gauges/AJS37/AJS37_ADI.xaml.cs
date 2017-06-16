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
    /// Interaction logic for AJS37_ADI.xaml
    /// </summary>
    public partial class AJS37_ADI : UserControl, I_Ikarus
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
        private double headingAngle = 0.0;
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
        private RotateTransform rtbank = new RotateTransform();
        private TranslateTransform rtvvi = new TranslateTransform();
        private TranslateTransform ttglideSlope = new TranslateTransform();
        private TranslateTransform ttbanksteering = new TranslateTransform();
        Sphere3D sphere3D;

        public AJS37_ADI()
        {
            InitializeComponent();

            Flagg_course_off.Visibility = System.Windows.Visibility.Hidden;
            Flagg_VVI_off.Visibility = System.Windows.Visibility.Hidden;
            Flagg_off.Visibility = System.Windows.Visibility.Visible;

            InitialSphere();

            directionalLight.Color = (Color)ColorConverter.ConvertFromString(lightColor);
        }
        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;
            helper = new GaugesHelper(dataImportID, windowID, "Instruments");
            if (MainWindow.editmode) { helper.MakeDraggable(this, this); }
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
            directionalLight.Color = _on ? (Color)ColorConverter.ConvertFromString("#" + MainWindow.lightOnColor) : (Color)ColorConverter.ConvertFromString(lightColor);
        }

        public void SetInput(string _input)
        {
            string[] vals = _input.Split(',');

            if (vals.Length < 3) return;

            valueScale = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                valueScale[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
            valueScaleIndex = vals.Length;
        }

        public void SetOutput(string _output)
        {
            string[] vals = _output.Split(',');

            if (vals.Length < 3) return;

            degreeDial = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                degreeDial[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
        }

        public double GetSize()
        {
            return 255; // Width
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

                               if (lheading != heading)
                               {
                                   //    -1.0, -0.5, 0.0, 0.5, 1.0,
                                   //     0.0,   90, 180, 270, 360,
                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (heading > valueScale[n] && heading <= valueScale[n + 1])
                                       {
                                           headingAngle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (heading - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "2", heading.ToString(), Convert.ToInt32(headingAngle).ToString());
                                   }
                               }

                               if (lpitch != pitch || lheading != heading || lbank != bank)
                                   sphere3D.Rotate(pitch * -180, headingAngle * -1, bank * -180);

                               if (glideSlope > 0.5) glideSlope = 0.5;
                               if (bankSteering > 0.5) bankSteering = 0.5;

                               if (lslipBall != slipBall)
                               {
                                   rtSlipball.Angle = slipBall * -12;
                                   SlipBallPosition.RenderTransform = rtSlipball;
                               }

                               if (lbankNeedle != bankNeedle)
                               {
                                   rtbank.Angle = bankNeedle * -180;
                                   Bank.RenderTransform = rtbank;
                               }
                               if (lvvi != vvi)
                               {
                                   rtvvi.Y = vvi * -46;
                                   VVI.RenderTransform = rtvvi;
                               }

                               if (lbankSteering != bankSteering)
                               {
                                   ttbanksteering.X = bankSteering * 140;
                                   banksteering.RenderTransform = ttbanksteering;
                               }
                               if (lglideSlope != glideSlope)
                               {
                                   ttglideSlope.Y = glideSlope * -125;
                                   pichsteering.RenderTransform = ttglideSlope;
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
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }

        private void InitialSphere()
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();

            if (File.Exists(Environment.CurrentDirectory + "\\Images\\Textures3D\\AJS37_ADI.jpg"))
                bitmapImage.UriSource = new Uri(Environment.CurrentDirectory + "\\Images\\Textures3D\\AJS37_ADI.jpg");
            else
                bitmapImage.UriSource = new Uri(Environment.CurrentDirectory + "\\Images\\Textures3D\\CheckerTest.jpg");

            bitmapImage.DecodePixelWidth = 1024;
            bitmapImage.EndInit();

            // declaration Sphere Object with model3Dgroup Name from XAML file and Sphere texture
            sphere3D = new Sphere3D(model3DGroup, bitmapImage);
        }
    }
}

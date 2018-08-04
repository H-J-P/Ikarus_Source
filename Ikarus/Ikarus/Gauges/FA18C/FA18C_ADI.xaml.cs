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
    public partial class FA18C_ADI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }
        private string lightColor = "#FFFFFF"; // white

        private const double dNull = 0.0;

        private double pitch = 0.0;
        private double bank = 0.0;

        private double bankNeedle = 0.0;
        private double slipBall = 0.0;
        private double turn = 0.0;
        private double flag_off = 0.0;
        private double side = 0.0;
        private double glide = 0.0;

        private double lpitch = 0.0;
        private double lbank = 0.0;
        private double lbankNeedle = 0.0;
        private double lslipBall = 0.0;
        private double lTurn = 0.0;
        private double lFlag_off = 1.0;
        private double lside = 0.0;
        private double lglide = 0.0;

        private RotateTransform rtSlipball = new RotateTransform();
        private TranslateTransform ttTurnIndicator = new TranslateTransform();
        private TranslateTransform rtpitchSteering = new TranslateTransform();
        private RotateTransform rtbankNeedle = new RotateTransform();
        private TranslateTransform ttTurn = new TranslateTransform();
        private TranslateTransform ttglide = new TranslateTransform();
        private TranslateTransform ttSide = new TranslateTransform();
        Sphere3D sphere3D;

        public FA18C_ADI()
        {
            InitializeComponent();

            Shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            Flag_Off.Visibility = System.Windows.Visibility.Visible;

            InitialSphere();

            sphere3D.xRotation = dNull;
            sphere3D.yRotation = dNull;
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
            //helper.SetInput(ref _input, ref valueScale, ref valueScaleIndex, 3);
        }

        public void SetOutput(string _output)
        {
            //helper.SetOutput(ref _output, ref degreeDial, 3);
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
                               if (vals.Length > 2) { slipBall = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { turn = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { side = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { glide = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { flag_off = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }

                               bankNeedle = bank;

                               if (lpitch != pitch || lbank != bank)
                               {
                                   sphere3D.xRotation = dNull;
                                   sphere3D.yRotation = pitch * 126;
                                   sphere3D.zRotation = ((bank * -180) - 90);

                                   sphere3D.Rotate();
                               }
                               if (glide > 0.5) glide = 0.5;
                               if (side > 0.5) side = 0.5;

                               if (lslipBall != slipBall)
                               {
                                   rtSlipball.Angle = slipBall * -17;
                                   Slipball.RenderTransform = rtSlipball;
                               }

                               if (lbankNeedle != bankNeedle)
                               {
                                   rtbankNeedle.Angle = bankNeedle * 180;
                                   Bank.RenderTransform = rtbankNeedle;
                               }
                               if (lTurn != turn)
                               {
                                   ttTurn.X = turn * -60;
                                   Turnindicator.RenderTransform = ttTurn;
                               }

                               if (lside != side)
                               {
                                   ttSide.X = side * 140;
                                   Side.RenderTransform = ttSide;
                               }
                               if (lglide != glide)
                               {
                                   ttglide.Y = glide * -160;
                                   Glide.RenderTransform = ttglide;
                               }

                               if (lFlag_off != flag_off)
                                   Flag_Off.Visibility = (flag_off > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lpitch = pitch;
                               lbank = bank;
                               lslipBall = slipBall;
                               lTurn = turn;
                               lFlag_off = flag_off;
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

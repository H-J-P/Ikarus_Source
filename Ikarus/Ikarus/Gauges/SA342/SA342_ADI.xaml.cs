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
    /// Interaktionslogik für SA342_ADI.xaml
    /// </summary>
    public partial class SA342_ADI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        double pitch = 0.0;
        double bank = 0.0;
        double slipBall = 0.0;
        double glide = 0.0;
        double side = 0.0;
        double bankNeedle = 0.0;
        double flagg_OFF = 0.0;
        double flagg_GS = 0.0;
        double flagg_LOC = 0.0;
        double silhouette = 0.0;

        private const double dNull = 0.0;

        double lpitch = 0.0;
        double lbank = 0.0;
        double lslipBall = 0.0;
        double lglide = 0.0;
        double lside = 0.0;
        double lflagg_OFF = 1.0;
        double lflagg_GS = 1.0;
        double lflagg_LOC = 1.0;
        double lsilhouette = 0.0;

        private string lightColor = "#FFFFFF"; // white
        RotateTransform rt = new RotateTransform();
        RotateTransform rtSlipBall = new RotateTransform();
        TranslateTransform ttglide = new TranslateTransform();
        TranslateTransform ttside = new TranslateTransform();
        TranslateTransform ttSilhouette = new TranslateTransform();

        Sphere3D sphere3D;

        public int GetWindowID() { return windowID; }

        public SA342_ADI()
        {
            InitializeComponent();

            Flagg_OFF.Visibility = System.Windows.Visibility.Visible;
            Flagg_GS.Visibility = System.Windows.Visibility.Visible;
            Flagg_LOC.Visibility = System.Windows.Visibility.Visible;

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
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { pitch = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { bank = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { slipBall = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { glide = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { side = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { flagg_OFF = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { flagg_GS = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { flagg_LOC = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { silhouette = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }

                               bankNeedle = bank;

                               if (glide > 1.0) { glide = 1.0; }
                               if (glide < -1.0) { glide = -1.0; }
                               if (side > 1.0) { side = 1.0; }
                               if (side < -1.0) { side = -1.0; }

                               if (lpitch != pitch || lbank != bank)
                               {
                                   sphere3D.xRotation = dNull;
                                   sphere3D.yRotation = pitch * 180; // 126;
                                   sphere3D.zRotation = ((bank * 180) - 90);

                                   sphere3D.Rotate();
                               }
                               if (lbank != bank)
                               {
                                   rt = new RotateTransform()
                                   {
                                       Angle = bankNeedle * -180
                                   };
                                   Bank.RenderTransform = rt;
                               }

                               if (lslipBall != slipBall)
                               {
                                   rtSlipBall = new RotateTransform()
                                   {
                                       Angle = slipBall * -13
                                   };
                                   SlipBall.RenderTransform = rtSlipBall;
                               }

                               if (lglide != glide)
                               {
                                   ttglide.Y = glide * -50;
                                   Glide.RenderTransform = ttglide;
                               }

                               if (lside != side)
                               {
                                   ttside.X = side * 45;
                                   Side.RenderTransform = ttside;
                               }

                               if (lsilhouette != silhouette)
                               {
                                   ttSilhouette.Y = silhouette * -30;
                                   Silhouette.RenderTransform = ttSilhouette;
                               }

                               if (lflagg_OFF != flagg_OFF)
                                   Flagg_OFF.Visibility = (flagg_OFF > 0.5) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               if (lflagg_GS != flagg_GS)
                                   Flagg_GS.Visibility = (flagg_GS > 0.5) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               if (lflagg_LOC != flagg_LOC)
                                   Flagg_LOC.Visibility = (flagg_LOC > 0.5) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lpitch = pitch;
                               lbank = bank;
                               lslipBall = slipBall;
                               lglide = glide;
                               lside = side;
                               lflagg_OFF = flagg_OFF;
                               lflagg_GS = flagg_GS;
                               lflagg_LOC = flagg_LOC;
                               lsilhouette = silhouette;
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

            if (File.Exists(Environment.CurrentDirectory + "\\Images\\Textures3D\\SA342_ADI.jpg"))
                bitmapImage.UriSource = new Uri(Environment.CurrentDirectory + "\\Images\\Textures3D\\SA342_ADI.jpg");
            else
                bitmapImage.UriSource = new Uri(Environment.CurrentDirectory + "\\Images\\Textures3D\\CheckerTest.jpg");

            bitmapImage.DecodePixelWidth = 1024;
            bitmapImage.EndInit();

            // declaration Sphere Object with model3Dgroup Name from XAML file and Sphere texture
            sphere3D = new Sphere3D(model3DGroup, bitmapImage);
        }
    }
}

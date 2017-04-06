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
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }
        private string lightColor = "#FFFFFF"; // white

        double pitch = 0.0;
        double bank = 0.0;
        double heading = 0.0;
        double slipBall = 0.0;
        double vvi = 0.0;
        double flag_vvi_off = 0.0;
        double flag_off = 0.0;
        double courceWarningFlag = 0.0;

        double lpitch = 0.0;
        double lbank = 0.0;
        double lheading = 0.0;
        double lslipBall = 0.0;
        double lvvi = 0.0;
        double lFlag_vvi_off = 1.0;
        double lFlag_off = 1.0;
        double lcourceWarningFlag = 1.0;

        RotateTransform rtSlipball = new RotateTransform();
        TranslateTransform ttTurnIndicator = new TranslateTransform();
        TranslateTransform rtpitchSteering = new TranslateTransform();
        RotateTransform rtbank = new RotateTransform();
        TranslateTransform rtvvi = new TranslateTransform();
        Sphere3D sphere3D;

        public AJS37_ADI()
        {
            InitializeComponent();
            if (MainWindow.editmode) MakeDraggable(this, this);

            //Flagg_course_off.Visibility = System.Windows.Visibility.Hidden;
            //lFlag_vvi_off.Visibility = System.Windows.Visibility.Hidden;
            Flagg_off.Visibility = System.Windows.Visibility.Visible;

            InitialSphere();
            directionalLight.Color = (Color)ColorConverter.ConvertFromString(lightColor);
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
            directionalLight.Color = _on ? (Color)ColorConverter.ConvertFromString(MainWindow.lightOnColor) : (Color)ColorConverter.ConvertFromString(lightColor);
        }

        public void SetInput(string _input)
        {
        }

        public void SetOutput(string _output)
        {
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
                               if (vals.Length > 5) { flag_vvi_off = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { flag_off = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { courceWarningFlag = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }

                               if (lpitch != pitch || lheading != heading || lbank != bank)
                                   sphere3D.Rotate(pitch * 180, heading * 360, bank * 180);

                               if (lslipBall != slipBall)
                               {
                                   rtSlipball.Angle = slipBall * -12;
                                   SlipBallPosition.RenderTransform = rtSlipball;
                               }
                               if (lbank != bank)
                               {
                                   rtbank.Angle = bank * -180;
                                   Bank.RenderTransform = rtbank;
                               }
                               if (lvvi != vvi)
                               {
                                   rtvvi.Y = vvi * -46;
                                   VVI.RenderTransform = rtvvi;
                               }

                               if (lcourceWarningFlag != courceWarningFlag)
                                   Flagg_course_off.Visibility = (courceWarningFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lFlag_vvi_off != flag_vvi_off)
                                   Flagg_VVI_off.Visibility = (flag_vvi_off > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lFlag_off != flag_off)
                                   Flagg_off.Visibility = (flag_off > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lpitch = pitch;
                               lheading = heading;
                               lbank = bank;
                               lslipBall = slipBall;
                               lvvi = vvi;
                               lFlag_vvi_off = flag_vvi_off;
                               lFlag_off = flag_off;
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

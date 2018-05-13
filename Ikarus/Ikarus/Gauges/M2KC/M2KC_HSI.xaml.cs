using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for M2KC_HSI.xaml
    /// </summary>
    public partial class M2KC_HSI : UserControl, I_Ikarus
    {
        private string dataImportID = "";

        //private double[] valueScale = new double[] { };
        //private double[] degreeDial = new double[] { };
        //int valueScaleIndex = 0;
        GaugesHelper helper = null;
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }

        TranslateTransform ttDME_1000 = new TranslateTransform();
        TranslateTransform ttDME_100 = new TranslateTransform();
        TranslateTransform ttDME_10 = new TranslateTransform();
        TranslateTransform ttDME_1 = new TranslateTransform();

        RotateTransform rtHeadingInd = new RotateTransform();
        RotateTransform rtNeedleLarge = new RotateTransform();
        RotateTransform rtNeedleSmall = new RotateTransform();
        RotateTransform rtCompassRose = new RotateTransform();

        double headingInd = 0.0;
        double needleLarge = 0.0;
        double needleSmall = 0.0;
        double dmeCounter_1000 = 0.0;
        double dmeCounter_100 = 0.0;
        double dmeCounter_10 = 0.0;
        double dmeCounter_1 = 0.0;
        double compassRose = 0.0;
        double hsiFlag1 = 0.0;
        double hsiFlag2 = 0.0;
        double hsiFlag_cap = 0.0;

        double lheadingInd = 0.0;
        double lneedleLarge = 0.0;
        double lneedleSmall = 0.0;
        double ldmeCounter_1000 = 0.0;
        double ldmeCounter_100 = 0.0;
        double ldmeCounter_10 = 0.0;
        double ldmeCounter_1 = 0.0;
        double lcompassRose = 0.0;
        double lhsiFlag1 = 1.0;
        double lhsiFlag2 = 1.0;
        double lhsiFlag_cap = 1.0;

        public M2KC_HSI()
        {
            InitializeComponent();

            DME_OFF_Flag.Visibility = System.Windows.Visibility.Hidden;
            NEEDLE_1_Flag.Visibility = System.Windows.Visibility.Visible;
            NEEDLE_2_Flag.Visibility = System.Windows.Visibility.Visible;
            CAP_Flag.Visibility = System.Windows.Visibility.Visible;

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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
            //helper.SetInput(ref _input, ref valueScale, ref valueScaleIndex, 2);
        }

        public void SetOutput(string _output)
        {
            //helper.SetOutput(ref _output, ref degreeDial, 2);
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

                               if (vals.Length > 0) { headingInd = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { needleLarge = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { needleSmall = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { dmeCounter_1000 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { dmeCounter_100 = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { dmeCounter_10 = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { dmeCounter_1 = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { compassRose = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { hsiFlag1 = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }
                               if (vals.Length > 9) { hsiFlag2 = Convert.ToDouble(vals[9], CultureInfo.InvariantCulture); }
                               if (vals.Length > 10) { hsiFlag_cap = Convert.ToDouble(vals[10], CultureInfo.InvariantCulture); }

                               if (dmeCounter_1000 < 0) dmeCounter_1000 = 0;
                               if (dmeCounter_100 < 0) dmeCounter_100 = 0;
                               if (dmeCounter_10 < 0) dmeCounter_10 = 0;
                               if (dmeCounter_1 < 0) dmeCounter_1 = 0;

                               if (lheadingInd != headingInd)
                               {
                                   rtHeadingInd.Angle = headingInd * 360;
                                   COURSE.RenderTransform = rtHeadingInd;
                               }
                               if (lneedleLarge != needleLarge)
                               {
                                   rtNeedleLarge.Angle = needleLarge * 360;
                                   BEARING.RenderTransform = rtNeedleLarge;
                               }
                               if (lneedleSmall != needleSmall)
                               {
                                   rtNeedleSmall.Angle = needleSmall * 360;
                                   VOR_BEARING.RenderTransform = rtNeedleSmall;
                               }
                               if (lcompassRose != compassRose)
                               {
                                   rtCompassRose.Angle = compassRose * -360;
                                   CompassRose.RenderTransform = rtCompassRose;
                               }

                               if (ldmeCounter_1000 != dmeCounter_1000)
                               {
                                   ttDME_1000.Y = dmeCounter_1000 * -333;
                                   DME_1000.RenderTransform = ttDME_1000;
                               }
                               if (ldmeCounter_100 != dmeCounter_100)
                               {
                                   ttDME_100.Y = dmeCounter_100 * -333;
                                   DME_100.RenderTransform = ttDME_100;
                               }
                               if (ldmeCounter_10 != dmeCounter_10)
                               {
                                   ttDME_10.Y = dmeCounter_10 * -333;
                                   DME_10.RenderTransform = ttDME_10;
                               }
                               if (ldmeCounter_1 != dmeCounter_1)
                               {
                                   ttDME_1.Y = dmeCounter_1 * -333;
                                   DME_1.RenderTransform = ttDME_1;
                               }

                               if (lhsiFlag1 != hsiFlag1)
                                   NEEDLE_1_Flag.Visibility = (hsiFlag1 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lhsiFlag2 != hsiFlag2)
                                   NEEDLE_2_Flag.Visibility = (hsiFlag2 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lhsiFlag_cap != hsiFlag_cap)
                                   CAP_Flag.Visibility = (hsiFlag_cap > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;


                               lheadingInd = headingInd;
                               lneedleLarge = needleLarge;
                               lneedleSmall = needleSmall;
                               lcompassRose = compassRose;

                               ldmeCounter_1000 = dmeCounter_1000;
                               ldmeCounter_100 = dmeCounter_100;
                               ldmeCounter_10 = dmeCounter_10;
                               ldmeCounter_1 = dmeCounter_1;

                               lhsiFlag1 = hsiFlag1;
                               lhsiFlag2 = hsiFlag2;
                               lhsiFlag_cap = hsiFlag_cap;
                           }
                           catch { return; }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}

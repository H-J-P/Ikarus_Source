using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für AJS37_ASI.xaml
    /// </summary>
    public partial class AJS37_ASI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double ias = 0.0;
        double mach1 = 0.0;
        double mach10 = 0.0;
        double mach100 = 0.0;
        double asiOff = 0.0;
        double machOff = 0.0;

        double lias = 0.0;
        double lmach1 = 0.0;
        double lmach10 = 0.0;
        double lmach100 = 0.0;
        double lasiOff = 0.0;
        double lmachOff = 0.0;

        RotateTransform rtIas = new RotateTransform();
        TranslateTransform ttMach1 = new TranslateTransform();
        TranslateTransform ttMach10 = new TranslateTransform();
        TranslateTransform ttMach100 = new TranslateTransform();

        public AJS37_ASI()
        {
            InitializeComponent();

            ASI_OFF_flag.Visibility = System.Windows.Visibility.Visible;
            Mach_OFF_flag.Visibility = System.Windows.Visibility.Visible;
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
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { ias = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { mach1 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { mach10 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { mach100 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { asiOff = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               // km/h  { 0,   150,   200,   231,   265,   300,   355,   385,    440,   490,    555,    700,    860,    950,   1040,   1150,   1260,   1370,   1500}
                               // m/s   { 0, 41.66, 55.56, 64.17, 73.61, 83.33, 98.61, 106.9, 122.22, 136.1, 154.16, 194.44, 239.00, 263.89, 288.89, 319.44, 350.00, 380.55, 416.66}
                               // input { 0,  0.01,  0.10,  0.15,  0.20,  0.25,  0.30,   0.35,  0.40,   0.45,   0.50,   0.60,   0.70,   0.75,   0.80,   0.85,   0.90,   0.95,     1}
                               // °     { 0,    21,    32,    41,    66,    85,   107,   118,    135,   150,    169,    205,    236,    252,    270,    285,    303,    317,    340}

                               if (ias < 0.0) { ias = 0.0; }

                               if (lias != ias)
                               {
                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (ias >= valueScale[n] && ias <= valueScale[n + 1])
                                       {
                                           rtIas.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (ias - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", ias.ToString(), Convert.ToInt32(rtIas.Angle).ToString());
                                   }
                                   ASI1.RenderTransform = rtIas;
                               }

                               if (mach1 != lmach1)
                               {
                                   ttMach1.Y = mach1 * -315;
                                   MACH1.RenderTransform = ttMach1;
                               }
                               if (mach10 != lmach10)
                               {
                                   ttMach10.Y = mach10 * -315;
                                   MACH10.RenderTransform = ttMach10;

                                   Mach_OFF_flag.Visibility = (mach10 >= 0.4) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
                               }
                               if (mach1 != lmach1)
                               {
                                   ttMach100.Y = mach100 * -315;
                                   MACH100.RenderTransform = ttMach100;
                               }

                               asiOff = (asiOff > 0.9) ? 0.0 : 1.0;

                               if (lasiOff != asiOff)
                                    ASI_OFF_flag.Visibility = (asiOff > 0.9) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;

                               lias = ias;
                               lmach1 = mach1;
                               lmach10 = mach10;
                               lmach100 = mach100;
                               lasiOff = asiOff;
                               lmachOff = machOff;
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

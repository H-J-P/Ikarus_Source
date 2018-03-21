using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MI8_DriftIndicator.xaml
    /// </summary>
    public partial class MI8_DriftIndicator : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double drift_angle = 0.0;
        double shutter = 0.0;
        double memory_lamp = 0.0;
        double hundreds = 0.0;
        double tens = 0.0;
        double ones = 0.0;

        double ldrift_angle = 0.0;
        double lshutter = 0.0;
        double lmemory_lamp = 0.0;
        double lhundreds = 0.0;
        double ltens = 0.0;
        double lones = 0.0;

        RotateTransform rtdrift_angle = new RotateTransform();
        TranslateTransform ttHundreds = new TranslateTransform();
        TranslateTransform ttTens = new TranslateTransform();
        TranslateTransform ttOnes = new TranslateTransform();

        public MI8_DriftIndicator()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            diss15_W_shutter.Visibility = System.Windows.Visibility.Visible;
            diss15_W_memory_lamp.Visibility = System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { drift_angle = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { shutter = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { memory_lamp = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { hundreds = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { tens = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { ones = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }

                               if (ldrift_angle != drift_angle)
                               {
                                   rtdrift_angle.Angle = drift_angle * 135;
                                   diss15_drift_angle.RenderTransform = rtdrift_angle;
                               }
                               
                               diss15_W_shutter.Visibility = (hundreds > 0.01 || tens > 0.01 || ones > 0.01) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
                               diss15_W_memory_lamp.Visibility = (memory_lamp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               if (lhundreds != hundreds)
                               {
                                   ttHundreds.Y = hundreds * -166;
                                   diss15_W_hundreds.RenderTransform = ttHundreds;
                               }
                               if (ltens != tens)
                               {
                                   ttTens.Y = tens * -166;
                                   diss15_W_tens.RenderTransform = ttTens;
                               }
                               if (lones != ones)
                               {
                                   ttOnes.Y = ones * -166;
                                   diss15_W_ones.RenderTransform = ttOnes;
                               }
                               ldrift_angle = drift_angle;
                               lshutter = shutter;
                               lmemory_lamp = memory_lamp;
                               lhundreds = hundreds;
                               ltens = tens;
                               lones = ones;
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

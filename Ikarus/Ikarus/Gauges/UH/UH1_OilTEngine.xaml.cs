using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for UH1_OilTEngine.xaml
    /// </summary>
    public partial class UH1_OilTEngine : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double needle = 0.0;
        double lneedle = 0.0;

        RotateTransform rtrNeedle = new RotateTransform();

        public UH1_OilTEngine()
        {
            InitializeComponent();
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
                           const int valueScaleIndex = 5;

                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { needle = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               if (lneedle != needle)
                               {
                                   // 114			                                 = {-70.0, -50.0, 0.0, 100.0, 150.0}
                                   //double[] valueScale = new double[valueScaleIndex] { 0.0, 0.13, 0.38, 0.71, 1.0 };
                                   //double[] degreeDial = new double[valueScaleIndex] { -50, -34, 0, 50, 90 };

                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (needle >= valueScale[n] && needle <= valueScale[n + 1])
                                       {
                                           rtrNeedle.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (needle - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", needle.ToString(), Convert.ToInt32(rtrNeedle.Angle).ToString());
                                   }
                                   EngOilTemp.RenderTransform = rtrNeedle;
                               }
                               lneedle = needle;
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

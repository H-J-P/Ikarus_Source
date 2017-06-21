using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für UH1_AirTemp.xaml
    /// </summary>
    public partial class UH1_AirTemp : UserControl
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double needle = 0.0;
        double lneedle = 0.0;

        RotateTransform rtrNeedle = new RotateTransform();

        public UH1_AirTemp()
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
        }

        public void SetOutput(string _output)
        {
        }

        public double GetSize()
        {
            return 255.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           //const int valueScaleIndex = 3;

                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { needle = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               if (lneedle != needle)
                               {
                                   //// 115			                               = {-3.0,   0.0, 100}
                                   //double[] valueScale = new double[valueScaleIndex] { 0.0, 0.029, 1.0 };
                                   //double[] degreeDial = new double[valueScaleIndex] { -5, 0, 250 };

                                   //if (needle < 0.0) needle = 0.0;

                                   //for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   //{
                                   //    if (needle >= valueScale[n] && needle <= valueScale[n + 1])
                                   //    {
                                   //        rtrNeedle.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (needle - valueScale[n]) + degreeDial[n];
                                   //        break;
                                   //    }
                                   //}
                                   rtrNeedle.Angle = (needle * 150 + 190) - 190;
                                   AirT.RenderTransform = rtrNeedle;
                               }
                               lneedle = needle;
                           }
                           catch { return; }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}

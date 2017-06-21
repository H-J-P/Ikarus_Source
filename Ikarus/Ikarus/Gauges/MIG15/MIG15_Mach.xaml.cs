using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG15_Mach.xaml
    /// </summary>
    public partial class MIG15_Mach : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        const int valueScaleIndex = 3;

        public int GetWindowID() { return windowID; }
        double mach = 0.0;
        double lmach = 0.0;

        RotateTransform rtMach = new RotateTransform();

        public MIG15_Mach()
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
            return 170.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { mach = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               if (lmach != mach)
                               {
                                   double[] valueScale = new double[valueScaleIndex] { 0.0, 0.215, 1.0 };
                                   double[] degreeDial = new double[valueScaleIndex] { 0, 24, 271 };

                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (mach >= valueScale[n] && mach <= valueScale[n + 1])
                                       {
                                           rtMach.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (mach - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   MACH.RenderTransform = rtMach;
                               }
                               lmach = mach;
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

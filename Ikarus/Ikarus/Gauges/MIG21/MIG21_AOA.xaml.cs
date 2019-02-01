using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG21_AOA.xaml
    /// </summary>
    public partial class MIG21_AOA : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double uuaIndicator = 0.0;
        double luuaIndicator = 0.0;

        public MIG21_AOA()
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
                           const int valueScaleIndex = 3;

                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { uuaIndicator = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               if (luuaIndicator != uuaIndicator)
                               {
                                   RotateTransform rtuuaIndicator = new RotateTransform();

                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (uuaIndicator >= valueScale[n] && uuaIndicator <= valueScale[n + 1])
                                       {
                                           rtuuaIndicator.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (uuaIndicator - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", uuaIndicator.ToString(), Convert.ToInt32(uuaIndicator).ToString());
                                   }
                                   UUA_indicator.RenderTransform = rtuuaIndicator;
                               }
                               luuaIndicator = uuaIndicator;
                           }
                           catch (Exception e) { ImportExport.LogMessage(GetType().Name + " got data and failed with exception: " + e.ToString()); }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}

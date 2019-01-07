using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für LwAlt.xaml   Author: HJP
    /// </summary>
    public partial class LwAlt : UserControl, I_Ikarus
    {
        private string dataImportID = "";

        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double valueKm = 0.0;
        double valueMeter = 0.0;
        double valuePressure = 0.0;

        double lvalueKm = 0.0;
        double lvalueMeter = 0.0;
        double lvaluePressure = 0.0;

        RotateTransform rtKm = new RotateTransform();
        RotateTransform rtMeter = new RotateTransform();
        RotateTransform rtPressure = new RotateTransform();

        public LwAlt()
        {
            InitializeComponent();

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
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { valueKm = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { valueMeter = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { valuePressure = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (lvalueKm != valueKm)
                               {
                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (valueKm >= valueScale[n] && valueKm <= valueScale[n + 1])
                                       {
                                           rtKm.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (valueKm - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", valueKm.ToString(), Convert.ToInt32(rtKm.Angle).ToString());
                                   }
                                   Lw_ALT_k.RenderTransform = rtKm;
                               }
                               if (lvalueMeter != valueMeter)
                               {
                                   rtMeter.Angle = valueMeter * 360;
                                   Lw_ALT_Needle.RenderTransform = rtMeter;
                               }
                               if (lvaluePressure != valuePressure)
                               {
                                   rtPressure.Angle = (valuePressure * 360) + 42;
                                   Lw_ALT_P.RenderTransform = rtPressure;
                               }
                               lvalueKm = valueKm;
                               lvalueMeter = valueMeter;
                               lvaluePressure = valuePressure;
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

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for FWD9H20Temp.xaml   Author: HJP
    /// </summary>
    public partial class LwH20Temp : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        public LwH20Temp()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        double h2oTemp = 0.0;
        double lh2oTemp = 0.0;

        RotateTransform rtH20Temp = new RotateTransform();

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

                               if (vals.Length > 0) { h2oTemp = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               if (lh2oTemp != h2oTemp)
                               {
                                   if (h2oTemp > 0.0 && h2oTemp <= 0.070)
                                   {
                                       rtH20Temp.Angle = (degreeDial[0]) / (valueScale[0]) * (h2oTemp);
                                   }
                                   else
                                   {
                                       for (int n = 0; n < valueScaleIndex - 1; n++)
                                       {
                                           if (h2oTemp >= valueScale[n] && h2oTemp <= valueScale[n + 1])
                                           {
                                               rtH20Temp.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (h2oTemp - valueScale[n]) + degreeDial[n];
                                               break;
                                           }
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", h2oTemp.ToString(), Convert.ToInt32(rtH20Temp.Angle).ToString());
                                   }
                                   FWD9_H2oT_Needle.RenderTransform = rtH20Temp;
                               }
                               lh2oTemp = h2oTemp;
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

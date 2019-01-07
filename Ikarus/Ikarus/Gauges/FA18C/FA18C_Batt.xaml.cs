using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for AV8BNA_Nozzle.xaml
    /// </summary>
    public partial class FA18C_Batt : UserControl, I_Ikarus
    {
        private string dataImportID = "";

        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        GaugesHelper helper = null;
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }

        double battE = 0.0;
        double lbattE = 0.0;
        double battU = 0.0;
        double lbattU = 0.0;

        RotateTransform rtValueE = new RotateTransform();
        RotateTransform rtValueU = new RotateTransform();

        public FA18C_Batt()
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
            helper.SetInput(ref _input, ref valueScale, ref valueScaleIndex, 2);
        }

        public void SetOutput(string _output)
        {
            helper.SetOutput(ref _output, ref degreeDial, 2);
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

                               if (vals.Length > 0) { battE = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { battU = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (lbattE != battE)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (battE >= valueScale[n] && battE <= valueScale[n + 1])
                                       {
                                           rtValueE.Angle = -1 * (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (battE - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", battE.ToString(), Convert.ToInt32(rtValueE.Angle).ToString());
                                   }
                                   BATT_E.RenderTransform = rtValueE;
                               }
                               if (lbattU != battU)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (battU >= valueScale[n] && battU <= valueScale[n + 1])
                                       {
                                           rtValueU.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (battU - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "2", battU.ToString(), Convert.ToInt32(rtValueU.Angle).ToString());
                                   }
                                   BATT_U.RenderTransform = rtValueU;
                               }
                               lbattE = battE;
                               lbattU = battU;
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

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for AV8BNA_ASI.xaml
    /// </summary>
    public partial class AV8BNA_ASI : UserControl, I_Ikarus
    {
        private string dataImportID = "";

        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        GaugesHelper helper = null;
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }

        double ASI_1000 = 0.0;
        double lASI_1000 = 0.0;
        double ASI_100 = 0.0;
        double lASI_100 = 0.0;

        RotateTransform rtASI_1000 = new RotateTransform();
        RotateTransform rtASI_100 = new RotateTransform();

        public AV8BNA_ASI()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            rtASI_100.Angle = -10;
            IAS_0_100_kts.RenderTransform = rtASI_100;
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

                               if (vals.Length > 0) { ASI_1000 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { ASI_100 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (lASI_1000 != ASI_1000)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (ASI_1000 >= valueScale[n] && ASI_1000 <= valueScale[n + 1])
                                       {
                                           rtASI_1000.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (ASI_1000 - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", ASI_1000.ToString(), Convert.ToInt32(rtASI_1000.Angle).ToString());
                                   }
                                   IAS_100_1000_kts.RenderTransform = rtASI_1000;
                               }

                               if (lASI_100 != ASI_100)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (ASI_100 >= valueScale[n] && ASI_100 <= valueScale[n + 1])
                                       {
                                           rtASI_100.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (ASI_100 - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "2", ASI_1000.ToString(), Convert.ToInt32(rtASI_1000.Angle).ToString());
                                   }
                                   IAS_0_100_kts.RenderTransform = rtASI_100;
                               }
                               lASI_1000 = ASI_1000;
                               lASI_100 = ASI_100;
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

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for WPEngRPM.xaml
    /// </summary>
    public partial class WPEngRPM : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        GaugesHelper helper = null;

        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }

        double engineRPMleft = 0.0;
        double engineRPMright = 0.0;

        double lengineRPMleft = 0.0;
        double lengineRPMright = 0.0;

        RotateTransform rtEngineRPMleft = new RotateTransform();
        RotateTransform rtEngineRPMright = new RotateTransform();

        public WPEngRPM()
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
            return 167.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { engineRPMleft = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { engineRPMright = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (lengineRPMleft != engineRPMleft)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (engineRPMleft >= valueScale[n] && engineRPMleft <= valueScale[n + 1])
                                       {
                                           rtEngineRPMleft.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (engineRPMleft - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", engineRPMleft.ToString(), Convert.ToInt32(rtEngineRPMleft.Angle).ToString());
                                   }
                                   EngineRPMleft.RenderTransform = rtEngineRPMleft;
                               }

                               if (lengineRPMright != engineRPMright)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (engineRPMright >= valueScale[n] && engineRPMright <= valueScale[n + 1])
                                       {
                                           rtEngineRPMright.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (engineRPMright - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   EngineRPMright.RenderTransform = rtEngineRPMright;
                               }
                               lengineRPMleft = engineRPMleft;
                               lengineRPMright = engineRPMright;
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

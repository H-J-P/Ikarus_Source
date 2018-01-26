using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für SA342_RPM.xaml
    /// </summary>
    public partial class SA342_RPM : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        GaugesHelper helper = null;

        private double turbine_RPM = 0.0;
        private double rotor_RPM = 0.0;

        private double lturbine_RPM = 0.0;
        private double lrotor_RPM = 0.0;

        RotateTransform rtTurbine_RPM = new RotateTransform();
        RotateTransform rtRotor_RPM = new RotateTransform();

        public int GetWindowID() { return windowID; }

        public SA342_RPM()
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
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { turbine_RPM = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { rotor_RPM = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (lturbine_RPM != turbine_RPM)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (turbine_RPM >= valueScale[n] && turbine_RPM <= valueScale[n + 1])
                                       {
                                           rtTurbine_RPM.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (turbine_RPM - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   RPM_Turb.RenderTransform = rtTurbine_RPM;
                               }

                               if (lrotor_RPM != rotor_RPM)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (rotor_RPM >= valueScale[n] && rotor_RPM <= valueScale[n + 1])
                                       {
                                           rtRotor_RPM.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (rotor_RPM - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   RPM_Rotor.RenderTransform = rtRotor_RPM;
                               }

                               //RPM.Text = rotor_RPM.ToString();

                               lturbine_RPM = turbine_RPM;
                               lrotor_RPM = rotor_RPM;
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

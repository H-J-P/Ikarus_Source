using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for AV8BNA_AOA.xaml
    /// </summary>
    public partial class AV8BNA_AOA : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double angleOfAttack = 0.0;
        double powerOffFlag = 0.0;

        double langleOfAttack = 0.0;
        double lpowerOffFlag = 1.0;

        RotateTransform rtAOA = new RotateTransform();

        public AV8BNA_AOA()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            Flag_Off.Visibility = System.Windows.Visibility.Hidden;
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
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { angleOfAttack = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { powerOffFlag = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               //if (angleOfAttack > 1.0) { angleOfAttack = 1.0; }
                               //if (angleOfAttack < 0.0) { angleOfAttack = 0.0; }

                               if (langleOfAttack != angleOfAttack)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (angleOfAttack >= valueScale[n] && angleOfAttack <= valueScale[n + 1])
                                       {
                                           rtAOA.Angle = -1.0 * ((degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (angleOfAttack - valueScale[n]) + degreeDial[n]);
                                           break;
                                       }
                                   }
                                   AOA.RenderTransform = rtAOA;

                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", angleOfAttack.ToString(), Convert.ToInt32(rtAOA.Angle).ToString());
                                   }
                               }

                               if(lpowerOffFlag != powerOffFlag)
                                    Flag_Off.Visibility = (powerOffFlag > 0.8) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;

                               langleOfAttack = angleOfAttack;
                               lpowerOffFlag = powerOffFlag;
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

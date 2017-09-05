using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für AJS37_FUEL_Q.xaml
    /// </summary>
    public partial class AJS37_FUEL_Q : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        private double fuelQ = 0.0;
        private double joker= 0.0;
        private double lfuelQ = 0.0;
        private double ljoker = 0.0;

        RotateTransform rtFuelQ = new RotateTransform();
        RotateTransform rtJoker = new RotateTransform();

        public AJS37_FUEL_Q()
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

                               if (vals.Length > 0) { fuelQ = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { joker = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (lfuelQ != fuelQ)
                               {
                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (fuelQ >= valueScale[n] && fuelQ <= valueScale[n + 1])
                                       {
                                           rtFuelQ.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (fuelQ - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   FUEL_Q1.RenderTransform = rtFuelQ;
                               }
                               if (ljoker != joker)
                               {
                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (joker >= valueScale[n] && joker <= valueScale[n + 1])
                                       {
                                           rtJoker.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (joker - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   JOKER_TIE.RenderTransform = rtJoker;

                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", fuelQ.ToString(), Convert.ToInt32(rtFuelQ.Angle).ToString());
                                       Cockpit.UpdateInOut(dataImportID, "2", joker.ToString(), Convert.ToInt32(rtJoker.Angle).ToString());
                                   }
                               }
                               lfuelQ = fuelQ;
                               ljoker = joker;
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

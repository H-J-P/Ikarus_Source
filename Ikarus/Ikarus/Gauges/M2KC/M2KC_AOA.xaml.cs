using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for M2KC_AOA.xaml
    /// </summary>
    public partial class M2KC_AOA : UserControl, I_Ikarus
    {
        private string dataImportID = "";

        GaugesHelper helper = null;
        private int windowID = 0;
        private string[] vals = new string[] { };
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;

        public int GetWindowID() { return windowID; }

        double aoa = 0.0;
        double laoa = 0.0;

        TranslateTransform ttAOA = new TranslateTransform();

        public M2KC_AOA()
        {
            InitializeComponent();

            //shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { aoa = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               if (laoa != aoa)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (aoa >= valueScale[n] && aoa <= valueScale[n + 1])
                                       {
                                           ttAOA.Y = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (aoa - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   AOA.RenderTransform = ttAOA;
                               }
                               if (MainWindow.editmode)
                               {
                                   Cockpit.UpdateInOut(dataImportID, "1", aoa.ToString(), Convert.ToInt32(ttAOA.Y).ToString());
                               }
                               laoa = aoa;
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

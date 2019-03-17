using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MI8_AOA.xaml
    /// </summary>
    public partial class MI8_RotorPitch : UserControl, I_Ikarus
    {
        private string dataImportID = "";

        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        public MI8_RotorPitch()
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
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');
                               double rotorPitch = 0.0;

                               if (vals.Length > 0) { rotorPitch = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               RotateTransform rtrotorPitch = new RotateTransform();
                               //RotorPitch.input =  { 1.0,  2.0,  3.0,  4.0,  5.0,  6.0,  7.0, 8.0,  9.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0}
                               //RotorPitch.output = { 0.0, 0.07, 0.14, 0.21, 0.29, 0.35, 0.43, 0.5, 0.57, 0.64, 0.71, 0.79, 0.86, 0.93,  1.0}
                               //                        0,   15,   29,   44,   59,   74,   89, 104,  120,  134,  150,  165,  180,  195,  210 

                               for (int n = 0; n < (valueScaleIndex - 1); n++)
                               {
                                   if (rotorPitch >= valueScale[n] && rotorPitch <= valueScale[n + 1])
                                   {
                                       rtrotorPitch.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (rotorPitch - valueScale[n]) + degreeDial[n];
                                       break;
                                   }
                               }
                               if (MainWindow.editmode)
                               {
                                   Cockpit.UpdateInOut(dataImportID, "1", rotorPitch.ToString(), Convert.ToInt32(rtrotorPitch.Angle).ToString());
                               }
                               RotorPitch.RenderTransform = rtrotorPitch;
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

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MI8_ALTR.xaml
    /// </summary>
    public partial class MI8_ALTR : UserControl, I_Ikarus
    {
        private string dataImportID = "";

        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double rAlt = 0.0;
        double dangerRAlt = 0.0;
        double dangerRAltLamp = 0.0;
        double warningFlag = 0.0;

        double lrAlt = 0.0;
        double ldangerRAlt = 0.0;
        double ldangerRAltLamp = 0.0;
        double lwarningFlag = 0.0;

        RotateTransform rtRAlt = new RotateTransform();
        RotateTransform rtdangerRAlt = new RotateTransform();
        RotateTransform rtwarningFlag = new RotateTransform();

        public MI8_ALTR()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            A_036_DangerRALT_lamp.Visibility = System.Windows.Visibility.Hidden;

            RotateTransform rtwarningFlag = new RotateTransform();
            rtwarningFlag.Angle = 11;
            Off_Flagg.RenderTransform = rtwarningFlag;
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

                               if (vals.Length > 0) { rAlt = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { dangerRAlt = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { dangerRAltLamp = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { warningFlag = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (lrAlt != rAlt)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (rAlt >= valueScale[n] && rAlt <= valueScale[n + 1])
                                       {
                                           rtRAlt.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (rAlt - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", rAlt.ToString(), Convert.ToInt32(rtRAlt.Angle).ToString());
                                   }
                                   A_036_RALT.RenderTransform = rtRAlt;
                               }
                               if (ldangerRAlt != dangerRAlt)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (dangerRAlt >= valueScale[n] && dangerRAlt <= valueScale[n + 1])
                                       {
                                           rtdangerRAlt.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (dangerRAlt - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   A_036_DangerRALT_index.RenderTransform = rtdangerRAlt;
                               }
                               if (lwarningFlag != warningFlag)
                               {
                                   rtwarningFlag.Angle = warningFlag * 11;
                                   Off_Flagg.RenderTransform = rtwarningFlag;
                               }
                               A_036_DangerRALT_lamp.Visibility = (dangerRAltLamp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lrAlt = rAlt;
                               ldangerRAlt = dangerRAlt;
                               ldangerRAltLamp = dangerRAltLamp;
                               lwarningFlag = warningFlag;
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

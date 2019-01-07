using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für SA342_ALT_R.xaml
    /// </summary>
    public partial class SA342_ALT_R : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        private double radarAltimeter = 0.0;
        private double dangerRALTindex = 0.0;
        private double rAltlamp = 0.0;
        private double flagOff = 0.0;
        private double flagTest = 0.0;

        private double lradarAltimeter = 0.0;
        private double ldangerRALTindex = 0.0;
        private double lrAltlamp = 0.0;
        private double lflagOff = 1.0;
        private double lflagTest = 0.0;

        RotateTransform rtRadarAltimeter = new RotateTransform();
        RotateTransform rtDangerRALTindex = new RotateTransform();

        public int GetWindowID() { return windowID; }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
        }

        public SA342_ALT_R()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            RAltlamp.Visibility = System.Windows.Visibility.Hidden;
            Flagg_off.Visibility = System.Windows.Visibility.Visible;
            flagg_A.Visibility = System.Windows.Visibility.Hidden;
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
        }

        public void SetOutput(string _output)
        {
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

                               if (vals.Length > 0) { radarAltimeter = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { dangerRALTindex = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { rAltlamp = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { flagOff = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { flagTest = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               if (lradarAltimeter != radarAltimeter)
                               {
                                   rtRadarAltimeter.Angle = radarAltimeter * 360;
                                   Radar_Altimeter.RenderTransform = rtRadarAltimeter;
                               }
                               if (ldangerRALTindex != dangerRALTindex)
                               {
                                   rtDangerRALTindex.Angle = dangerRALTindex * 360;
                                   DangerRALT_index.RenderTransform = rtDangerRALTindex;
                               }

                               if (lrAltlamp != rAltlamp)
                                   RAltlamp.Visibility = (rAltlamp > 0.5) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lflagOff != flagOff)
                                   Flagg_off.Visibility = (flagOff > 0.5) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lflagTest != flagTest)
                                   flagg_A.Visibility = (flagTest > 0.5) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lradarAltimeter = radarAltimeter;
                               ldangerRALTindex = dangerRALTindex;
                               lrAltlamp = rAltlamp;
                               lflagOff = flagOff;
                               lflagTest = flagTest;
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

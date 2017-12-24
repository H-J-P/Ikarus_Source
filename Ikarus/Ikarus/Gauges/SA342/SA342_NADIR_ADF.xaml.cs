using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für SA342_HSI.xaml
    /// </summary>
    public partial class SA342_NADIR_ADF : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        private double heading = 0.0;
        private double steadybug = 0.0;
        private double nadir = 0.0;
        private double adf = 0.0;
        private double pxFlag = 0.0;
        private double butFlag = 0.0;
        private double capFlag = 0.0;
        private double range001 = 0.0;
        private double range010 = 0.0;
        private double range100 = 0.0;

        private double lheading = 0.0;
        private double lsteadybug = 0.0;
        private double lnadir = 0.0;
        private double ladf = 0.0;
        private double lpxFlag = 0.0;
        private double lbutFlag = 0.0;
        private double lcapFlag = 0.0;
        private double lrange001 = 0.0;
        private double lrange010 = 0.0;
        private double lrange100 = 0.0;

        public SA342_NADIR_ADF()
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

                               if (vals.Length > 0) { heading = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { nadir = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { adf = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { capFlag = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { butFlag = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { pxFlag = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { range100 = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { range010 = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { range001 = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }
                               if (vals.Length > 9) { steadybug = Convert.ToDouble(vals[9], CultureInfo.InvariantCulture); }



                               lheading = heading;
                               lnadir = nadir;
                               ladf = adf;
                               lcapFlag = capFlag;
                               lbutFlag = butFlag;
                               lpxFlag = pxFlag;
                               lrange100 = range100;
                               lrange010 = range010;
                               lrange001 = range001;
                               lsteadybug = steadybug;
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

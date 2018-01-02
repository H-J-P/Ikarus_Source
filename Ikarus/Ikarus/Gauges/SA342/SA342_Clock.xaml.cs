using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für SA342_Clock.xaml
    /// </summary>
    public partial class SA342_Clock : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double clockHours = 0.0;
        double clockMinutes = 0.0;
        double cronoMinutes = 0.0;
        double clockSeconds = 0.0;

        double lclockHours = 0.0;
        double lclockMinutes = 0.0;
        double lcronoMinutes = 0.0;
        double lclockSeconds = 0.0;

        RotateTransform rtClockHours = new RotateTransform();
        RotateTransform rtClockMinutes = new RotateTransform();
        RotateTransform rtClockSeconds = new RotateTransform();
        RotateTransform rtCronoMinutes = new RotateTransform();

        public SA342_Clock()
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

                               if (vals.Length > 0) { clockHours = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { clockMinutes = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { clockSeconds = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { cronoMinutes = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (lclockHours != clockHours)
                               {
                                   rtClockHours.Angle = clockHours * 360;
                                   Time_Hour.RenderTransform = rtClockHours;
                               }
                               if (lclockMinutes != clockMinutes)
                               {
                                   rtClockMinutes.Angle = clockMinutes * 360;
                                   Time_Minute.RenderTransform = rtClockMinutes;
                               }
                               if (lclockSeconds != clockSeconds)
                               {
                                   rtClockSeconds.Angle = clockSeconds * 360;
                                   Crono_Second.RenderTransform = rtClockSeconds;
                               }
                               if (lcronoMinutes != cronoMinutes)
                               {
                                   rtCronoMinutes.Angle = cronoMinutes * 360;
                                   Crono_Minutes.RenderTransform = rtCronoMinutes;
                               }
                               lclockHours = clockHours;
                               lclockMinutes = clockMinutes;
                               lcronoMinutes = cronoMinutes;
                               lclockSeconds = clockSeconds;
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

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for USClock.xaml
    /// </summary>
    public partial class F86_Clock : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }
        GaugesHelper helper = null;

        double currtimeHours = 0.0;
        double currtimeMinutes = 0.0;
        double currtimeSeconds = 0.0;
        double cronoMinutes = 0.0;
        double cronoSeconds = 0.0;

        double lcurrtimeHours = 0.0;
        double lcurrtimeMinutes = 0.0;
        double lcurrtimeSeconds = 0.0;
        double lcronoMinutes = 0.0;
        double lcronoSeconds = 0.0;

        RotateTransform rtCurrtimeHours = new RotateTransform();
        RotateTransform rtCurrtimeMinutes = new RotateTransform();
        RotateTransform rtcurrtimeSeconds = new RotateTransform();
        RotateTransform rtCronoMinutes = new RotateTransform();
        RotateTransform rtCronoSeconds = new RotateTransform();

        public F86_Clock()
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

                               if (vals.Length > 0) { currtimeHours = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { currtimeMinutes = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { currtimeSeconds = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { cronoMinutes = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { cronoSeconds = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               if (currtimeHours < 0.0) currtimeHours = 0.0;
                               if (currtimeMinutes < 0.0) currtimeMinutes = 0.0;
                               if (currtimeSeconds < 0.0) currtimeSeconds = 0.0;
                               if (cronoMinutes < 0.0) cronoMinutes = 0.0;
                               if (cronoSeconds < 0.0) cronoSeconds = 0.0;


                               if (lcurrtimeHours != currtimeHours)
                               {
                                   rtCurrtimeHours.Angle = currtimeHours * 360;
                                   Time_Hour.RenderTransform = rtCurrtimeHours;
                               }
                               if (lcurrtimeMinutes != currtimeMinutes)
                               {
                                   rtCurrtimeMinutes.Angle = currtimeMinutes * 360;
                                   Time_Minute.RenderTransform = rtCurrtimeMinutes;
                               }
                               if (lcurrtimeSeconds != currtimeSeconds)
                               {
                                   rtcurrtimeSeconds.Angle = currtimeSeconds * 360;
                                   Time_Second.RenderTransform = rtcurrtimeSeconds;
                               }
                               if (lcronoMinutes != cronoMinutes)
                               {
                                   rtCronoMinutes.Angle = cronoMinutes * 360;
                                   Crono_Minutes.RenderTransform = rtCronoMinutes;
                               }
                               if (lcronoSeconds != cronoSeconds)
                               {
                                   rtCronoSeconds.Angle = cronoSeconds * 360;
                                   Crono_Second.RenderTransform = rtCronoSeconds;
                               }
                               lcurrtimeHours = currtimeHours;
                               lcurrtimeMinutes = currtimeMinutes;
                               lcurrtimeSeconds = currtimeSeconds;
                               lcronoMinutes = cronoMinutes;
                               lcronoSeconds = cronoSeconds;
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

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for LwClock.xaml   Author: HJP
    /// </summary>
    public partial class LwClock : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double valueHour = 0.0;
        double valueMinutes = 0.0;
        double valueSeconds = 0.0;
        double valueChronoMinutes = 0.0;
        double turnBrezel = 0.0;

        double lvalueHour = 0.0;
        double lvalueMinutes = 0.0;
        double lvalueSeconds = 0.0;
        double lvalueChronoMinutes = 0.0;
        double lturnBrezel = 0.0;

        RotateTransform rtHour = new RotateTransform();
        RotateTransform rtMin = new RotateTransform();
        RotateTransform rtCronoMinutes = new RotateTransform();
        RotateTransform rtBrezel = new RotateTransform();

        public LwClock()
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

                               if (vals.Length > 0) { valueHour = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { valueMinutes = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { valueSeconds = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { valueChronoMinutes = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { turnBrezel = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               if (lvalueHour != valueHour)
                               {
                                   rtHour.Angle = valueHour * 360;
                                   Lw_Clock_NeedleHrs.RenderTransform = rtHour;
                               }

                               if (lvalueMinutes != valueMinutes)
                               {
                                   rtMin.Angle = valueMinutes * 360;
                                   Lw_Clock_NeedleMin.RenderTransform = rtMin;
                               }
                               if (lvalueChronoMinutes != valueChronoMinutes)
                               {
                                   rtCronoMinutes.Angle = valueChronoMinutes * 360;
                                   Lw_Clock_NeedleStop.RenderTransform = rtCronoMinutes;
                               }
                               if (lturnBrezel != turnBrezel)
                               {
                                   rtBrezel.Angle = turnBrezel * 360;
                                   Lw_Clock_BezelMin.RenderTransform = rtBrezel;
                               }
                               lvalueHour = valueHour;
                               lvalueMinutes = valueMinutes;
                               lvalueSeconds = valueSeconds;
                               lvalueChronoMinutes = valueChronoMinutes;
                               lturnBrezel = turnBrezel;
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

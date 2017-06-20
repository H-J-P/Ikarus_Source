using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for Spit_Clock.xaml
    /// </summary>
    public partial class Spit_Clock : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        double valueHour = 0.0;
        double valueMinutes = 0.0;
        double valueSeconds = 0.0;

        double lvalueHour = 0.0;
        double lvalueMinutes = 0.0;
        double lvalueSeconds = 0.0;

        RotateTransform rtHour = new RotateTransform();
        RotateTransform rtMin = new RotateTransform();
        RotateTransform rtseconds = new RotateTransform();

        public int GetWindowID() { return windowID; }

        public Spit_Clock()
		{
			this.InitializeComponent();
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;
            helper = new GaugesHelper(dataImportID, windowID, "Instruments");
            if (MainWindow.editmode) { helper.MakeDraggable(this, this); }
            LoadBmaps();
        }

        public string GetID() { return dataImportID; }

        private void LoadBmaps()
        {
            string frame = "";
            string light = "";

            helper.LoadBmaps(ref frame, ref light);

            try
            {
                if (frame.Length > 4)
                    Frame.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + frame));

                if (light.Length > 4)
                    Light.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + light));

                SwitchLight(false);
            }
            catch { }
        }

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
            return 255; // Width
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

                               if (lvalueHour != valueHour)
                               {
                                   rtHour.Angle = valueHour * 360;
                                   hour.RenderTransform = rtHour;
                               }

                               if (lvalueMinutes != valueMinutes)
                               {
                                   rtMin.Angle = valueMinutes * 360;
                                   min.RenderTransform = rtMin;
                               }
                               if (lvalueSeconds != valueSeconds)
                               {
                                   rtseconds.Angle = valueSeconds * 360;
                                   sec.RenderTransform = rtseconds;
                               }
                               lvalueHour = valueHour;
                               lvalueMinutes = valueMinutes;
                               lvalueSeconds = valueSeconds;
                           }
                           catch { return; }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
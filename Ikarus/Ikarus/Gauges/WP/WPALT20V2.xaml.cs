using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für WPALT20V2.xaml
    /// </summary>
    public partial class WPALT20V2 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double altituteKmNeedle = 0.0;
        double altituteMeterNeedle = 0.0;
        double baroPressure = 0.0;

        double laltituteKmNeedle = 0.0;
        double laltituteMeterNeedle = 0.0;
        double lbaroPressure = 0.0;

        RotateTransform rtAltituteKmNeedle = new RotateTransform();
        RotateTransform rtAltituteMeterNeedle = new RotateTransform();
        RotateTransform rtBaroPressure = new RotateTransform();

        public WPALT20V2()
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
            return 170.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { altituteKmNeedle = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { altituteMeterNeedle = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { baroPressure = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (baroPressure > 0.962) baroPressure = 0.962;
                               if (baroPressure < 0.0) baroPressure = 0.0;

                               if (laltituteKmNeedle != altituteKmNeedle)
                               {
                                   rtAltituteKmNeedle.Angle = altituteKmNeedle * 360;
                                   Altimeter_km.RenderTransform = rtAltituteKmNeedle;
                               }
                               if (laltituteMeterNeedle != altituteMeterNeedle)
                               {
                                   rtAltituteMeterNeedle.Angle = altituteMeterNeedle * 360;
                                   Altimeter_m.RenderTransform = rtAltituteMeterNeedle;
                               }
                               if (lbaroPressure != baroPressure)
                               {
                                   rtBaroPressure.Angle = baroPressure * -300;
                                   Altimeter_Pressure.RenderTransform = rtBaroPressure;
                               }
                               laltituteKmNeedle = altituteKmNeedle;
                               laltituteMeterNeedle = altituteMeterNeedle;
                               lbaroPressure = baroPressure;
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

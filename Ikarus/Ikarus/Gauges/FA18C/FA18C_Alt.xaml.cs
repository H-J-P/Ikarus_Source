using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for AV8BNA_Nozzle.xaml
    /// </summary>
    public partial class FA18C_Alt : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        RotateTransform rtalt100FP = new RotateTransform();
        TranslateTransform ttalt10000 = new TranslateTransform();
        TranslateTransform ttalt1000 = new TranslateTransform();
        TranslateTransform ttalt100 = new TranslateTransform();
        TranslateTransform ttpressure_0 = new TranslateTransform();
        TranslateTransform ttpressure_1 = new TranslateTransform();
        TranslateTransform ttpressure_2 = new TranslateTransform();
        TranslateTransform ttpressure_3 = new TranslateTransform();

        double alt100FP = 0.0;
        double alt10000 = 0.0;
        double alt1000 = 0.0;
        double pressure_0 = 0.0;
        double pressure_1 = 0.0;
        double pressure_2 = 0.0;
        double pressure_3 = 0.0;

        double lalt100FP = 0.0;
        double lalt10000 = 0.0;
        double lalt1000 = 0.0;
        double lpressure_0 = 0.0;
        double lpressure_1 = 0.0;
        double lpressure_2 = 0.0;
        double lpressure_3 = 0.0;

        string pressure_1000_100 = "";

        public FA18C_Alt()
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
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           vals = strData.Split(';');

                           try
                           {
                               if (vals.Length > 0) { alt100FP = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { alt10000 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { alt1000 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { pressure_0 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { pressure_1 = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { pressure_2 = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }


                               if (alt10000 < 0) { alt10000 = 0.0; }
                               if (alt1000 < 0) { alt1000 = 0.0; }
                               if (pressure_0 < 0) { pressure_0 = 0.0; }
                               if (pressure_1 < 0) { pressure_1 = 0.0; }
                               if (pressure_2 < 0) { pressure_2 = 0.0; }

                               if (alt100FP != lalt100FP)
                               {
                                   rtalt100FP.Angle = alt100FP * 360;
                                   ALT.RenderTransform = rtalt100FP;
                               }
                               if (alt10000 != lalt10000)
                               {
                                   ttalt10000.Y = alt10000 * -332;
                                   ALT_10000.RenderTransform = ttalt10000;
                               }
                               if (alt1000 != lalt1000)
                               {
                                   ttalt1000.Y = alt1000 * -332;
                                   ALT_1000.RenderTransform = ttalt1000;
                               }

                               // pressure_2:  0.0 = 26, 0.2 = 27, 0.4 = 28, 0.6 = 29, 0.8 = 30 und 1.0 = 31
                               if (pressure_2 == 0.0) { pressure_1000_100 = "26"; }
                               else if (pressure_2 == 0.2) { pressure_1000_100 = "27"; }
                               else if (pressure_2 == 0.4) { pressure_1000_100 = "28"; }
                               else if (pressure_2 == 0.6) { pressure_1000_100 = "29"; }
                               else if (pressure_2 == 0.8) { pressure_1000_100 = "30"; }
                               else if (pressure_2 == 1.0) { pressure_1000_100 = "31"; }

                               pressure_3 = Convert.ToDouble(pressure_1000_100[0].ToString(), CultureInfo.InvariantCulture);
                               pressure_2 = Convert.ToDouble(pressure_1000_100[1].ToString(), CultureInfo.InvariantCulture);


                               if (pressure_0 != lpressure_0)
                               {
                                   ttpressure_0.Y = pressure_0 * -332;
                                   MB_1.RenderTransform = ttpressure_0;
                               }
                               if (pressure_1 != lpressure_1)
                               {
                                   ttpressure_1.Y = pressure_1 * -332;
                                   MB_10.RenderTransform = ttpressure_1;
                               }
                               if (pressure_2 != lpressure_2)
                               {
                                   ttpressure_2.Y = pressure_2 * -33.2;
                                   MB_100.RenderTransform = ttpressure_2;
                               }
                               if (pressure_3 != lpressure_3)
                               {
                                   ttpressure_3.Y = pressure_3 * -33.2;
                                   MB_1000.RenderTransform = ttpressure_3;
                               }

                               lalt100FP = alt100FP;
                               lalt10000 = alt10000;
                               lalt1000 = alt1000;
                               lpressure_0 = pressure_0;
                               lpressure_1 = pressure_1;
                               lpressure_2 = pressure_2;
                               lpressure_3 = pressure_3;
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

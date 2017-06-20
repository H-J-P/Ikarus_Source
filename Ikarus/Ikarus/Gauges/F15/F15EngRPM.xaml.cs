using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for F15EngRPM.xaml
    /// </summary>
    public partial class F15EngRPM : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        RotateTransform rtRPM = new RotateTransform();
        TranslateTransform ttRPM100 = new TranslateTransform();
        TranslateTransform ttRPM10 = new TranslateTransform();
        TranslateTransform ttRPM1 = new TranslateTransform();

        double rpm = 0.0;
        double rpm100 = 0.0;
        double rpm10 = 0.0;
        double rpm1 = 0.0;

        double lrpm = 0.0;
        double lrpm100 = 0.0;
        double lrpm10 = 0.0;
        double lrpm1 = 0.0;

        public F15EngRPM()
        {
            InitializeComponent();
            RPMValue.Visibility = System.Windows.Visibility.Hidden;
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
            return 255.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           vals = strData.Split(';');

                           string rpmString = "";

                           try
                           {
                               if (vals.Length > 0) { rpm = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               //if (vals.Length > 1) { rpm100 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               //if (vals.Length > 2) { rpm10 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               //if (vals.Length > 3) { rpm1 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (rpm < 0.0) rpm = 0.0;

                               rpmString = Convert.ToInt16(rpm * 1100).ToString();
                               //RPMValue.Text = rpmString;

                               if (rpmString.Length == 0) { rpmString = "0000" + rpmString; }
                               if (rpmString.Length == 1) { rpmString = "000" + rpmString; }
                               if (rpmString.Length == 2) { rpmString = "00" + rpmString; }
                               if (rpmString.Length == 3) { rpmString = "0" + rpmString; }

                               rpm100 = Convert.ToDouble((rpmString[0]).ToString(), CultureInfo.InvariantCulture);
                               rpm10 = Convert.ToDouble((rpmString[1] + ((rpm100 > 0) ? "." + rpmString[2] : "")).ToString(), CultureInfo.InvariantCulture);
                               rpm1 = Convert.ToDouble((rpmString[2] + "." + rpmString[3]).ToString(), CultureInfo.InvariantCulture);

                               if (lrpm != rpm)
                               {
                                   rtRPM.Angle = rpm * 242;
                                   EngineRPM.RenderTransform = rtRPM;
                               }
                               if (lrpm100 != rpm100)
                               {
                                   ttRPM100.Y = rpm100 * -29.2;
                                   EngineRPM_100.RenderTransform = ttRPM100;
                               }
                               if (lrpm10 != rpm10)
                               {
                                   ttRPM10.Y = rpm10 * -29.2;
                                   EngineRPM_10.RenderTransform = ttRPM10;
                               }
                               if (lrpm1 != rpm1)
                               {
                                   ttRPM1.Y = rpm1 * -29.2;
                                   EngineRPM_1.RenderTransform = ttRPM1;
                               }
                               lrpm = rpm;
                               lrpm100 = rpm100;
                               lrpm10 = rpm10;
                               lrpm1 = rpm1;
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

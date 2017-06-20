using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for F15EGT.xaml
    /// </summary>
    public partial class F15EGT : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        RotateTransform rtEGT = new RotateTransform();
        TranslateTransform ttEGT1000 = new TranslateTransform();
        TranslateTransform ttEGT100 = new TranslateTransform();
        TranslateTransform ttEGT10 = new TranslateTransform();

        double egt = 0.0;
        double egt1000 = 0.0;
        double egt100 = 0.0;
        double egt10 = 0.0;

        double legt = 0.0;
        double legt1000 = 0.0;
        double legt100 = 0.0;
        double legt10 = 0.0;

        public F15EGT()
        {
            InitializeComponent();
            rtEGT.Angle = -10;
            EngineTemp.RenderTransform = rtEGT;
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
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { egt = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { egt1000 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { egt100 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { egt10 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (egt < 0.0) egt = 0.0;
                               if (egt1000 < 0.0) egt1000 = 0.0;
                               if (egt100 < 0.0) egt100 = 0.0;
                               if (egt10 < 0.0) egt10 = 0.0;

                               if (legt != egt)
                               {
                                   rtEGT.Angle = egt * 262 - 10;
                                   EngineTemp.RenderTransform = rtEGT;
                               }
                               if (legt1000 != egt1000)
                               {
                                   ttEGT1000.Y = egt1000 * -271;
                                   EngineTemp_1000.RenderTransform = ttEGT1000;
                               }
                               if (legt100 != egt100)
                               {
                                   ttEGT100.Y = egt100 * -271;
                                   EngineTemp_100.RenderTransform = ttEGT100;
                               }
                               if (legt10 != egt10)
                               {
                                   ttEGT10.Y = egt10 * -271;
                                   EngineTemp_10.RenderTransform = ttEGT10;
                               }
                               legt = egt;
                               legt1000 = egt1000;
                               legt100 = egt100;
                               legt10 = egt10;
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

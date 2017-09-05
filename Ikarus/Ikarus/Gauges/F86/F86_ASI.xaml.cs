using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for F86_ASI.xaml
    /// </summary>
    public partial class F86_ASI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }
        GaugesHelper helper = null;

        TranslateTransform ttAirspeeedDrum = new TranslateTransform();
        RotateTransform rtAirspeeed = new RotateTransform();
        RotateTransform rtAirspeeedM1 = new RotateTransform();

        double airspeeed = 0.0;
        double airspeeedDrum = 0.0;
        double airspeeedM1 = 0.0;

        double lairspeeed = 0.0;
        double lairspeeedDrum = 0.0;
        double lairspeeedM1 = 0.0;

        public F86_ASI()
        {
            InitializeComponent();

            AirSpeedBox.Visibility = System.Windows.Visibility.Hidden;
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
                           const int valueScaleIndex = 4;
                           const int valueScaleIndex2 = 5;

                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { airspeeed = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { airspeeedDrum = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { airspeeedM1 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (airspeeed < 0.0) airspeeed = 0.0;
                               if (airspeeedDrum < 0.0) airspeeedDrum = 0.0;
                               if (airspeeedM1 < 0.0) airspeeedM1 = 0.0;

                               Double[] valueScale = new double[valueScaleIndex] { 0.0, 0.263, 0.414, 1 };
                               Double[] degreeDial = new double[valueScaleIndex] { 0.0, 102, 153, 345 };

                               if (lairspeeed != airspeeed)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (airspeeed >= valueScale[n] && airspeeed <= valueScale[n + 1])
                                       {
                                           rtAirspeeed.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (airspeeed - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   Airspeeed.RenderTransform = rtAirspeeed;
                               }
                               if (lairspeeedDrum != airspeeedDrum)
                               {
                                   ttAirspeeedDrum.X = airspeeedDrum * -316;
                                   AirspeeedDrum.RenderTransform = ttAirspeeedDrum;
                               }
                               // AirspeeedM1.input		               = { 0.0, 25.7,51.444, 308.67, 334.4 } ???
                               valueScale = new double[valueScaleIndex2] { 0.0, 0.02, 0.1, 0.6, 1.0 };
                               degreeDial = new double[valueScaleIndex2] { 0.0, 27, 56, 320, 345 };

                               if (lairspeeedM1 != airspeeedM1)
                               {
                                   for (int n = 0; n < (valueScaleIndex2 - 1); n++)
                                   {
                                       if (airspeeedM1 >= valueScale[n] && airspeeedM1 <= valueScale[n + 1])
                                       {
                                           rtAirspeeedM1.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (airspeeedM1 - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   AirspeeedM1.RenderTransform = rtAirspeeedM1;
                               }
                               AirSpeedBox.Text = rtAirspeeedM1.ToString();

                               lairspeeed = airspeeed;
                               lairspeeedDrum = airspeeedDrum;
                               lairspeeedM1 = airspeeedM1;
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

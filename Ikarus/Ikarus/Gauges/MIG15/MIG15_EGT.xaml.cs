using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG15_EGT.xaml
    /// </summary>
    public partial class MIG15_EGT : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

       public int GetWindowID() { return windowID; }

        double engineTemperature = 0.0;
        double lengineTemperature = 0.0;

        RotateTransform rtengineTemperature = new RotateTransform();

        public MIG15_EGT()
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

                               if (vals.Length > 0) { engineTemperature = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               if (engineTemperature < 0.0) engineTemperature = 0.0;

                               if (lengineTemperature != engineTemperature)
                               {
                                   rtengineTemperature.Angle = engineTemperature * 103;
                                   EngineTemperature.RenderTransform = rtengineTemperature;
                               }
                               lengineTemperature = engineTemperature;
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

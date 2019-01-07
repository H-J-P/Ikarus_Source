using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for WPEngT12.xaml
    /// </summary>
    public partial class WPEngT12 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double engineTemp1200 = 0.0;
        double engineTemp100 = 0.0;

        double lengineTemp1200 = 0.0;
        double lengineTemp100 = 0.0;

        RotateTransform rtEngineTemp1200 = new RotateTransform();
        RotateTransform rtEngineTemp100 = new RotateTransform();

        public WPEngT12()
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

                               if (vals.Length > 0) { engineTemp1200 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { engineTemp100 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (lengineTemp1200 != engineTemp1200)
                               {
                                   rtEngineTemp1200.Angle = engineTemp1200 * 270;
                                   rtEngineTemp100.Angle = engineTemp100 * 360;
                               }
                               if (lengineTemp100 != engineTemp100)
                               {
                                   EngineTemp1200.RenderTransform = rtEngineTemp1200;
                                   EngineTemp100.RenderTransform = rtEngineTemp100;
                               }
                               lengineTemp1200 = engineTemp1200;
                               lengineTemp100 = engineTemp100;
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

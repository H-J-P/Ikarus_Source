using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for WPDistance.xaml
    /// </summary>
    public partial class MIG21_Distance : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double distance100 = 0.0;
        double distance10 = 0.0;
        double distance1 = 0.0;

        double ldistance100 = 0.0;
        double ldistance10 = 0.0;
        double ldistance1 = 0.0;

        TranslateTransform ttDistance_I = new TranslateTransform();
        TranslateTransform ttDistance_X = new TranslateTransform();
        TranslateTransform ttDistance_C = new TranslateTransform();

        public MIG21_Distance()
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

                               if (vals.Length > 0) { distance100 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { distance10 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { distance1 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (ldistance1 != distance1)
                               {
                                   ttDistance_I.Y = distance1 * -238;
                                   DistanceToWaypoint_I.RenderTransform = ttDistance_I;
                               }
                               if (ldistance10 != distance10)
                               {
                                   ttDistance_X.Y = distance10 * -238;
                                   DistanceToWaypoint_X.RenderTransform = ttDistance_X;
                               }
                               if (ldistance100 != distance100)
                               {
                                   ttDistance_C.Y = distance100 * -238;
                                   DistanceToWaypoint_C.RenderTransform = ttDistance_C;
                               }
                               ldistance100 = distance100;
                               ldistance10 = distance10;
                               ldistance1 = distance1;
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

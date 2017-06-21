using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG15_AltR.xaml
    /// </summary>
    public partial class MIG15_ALTR : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }
        GaugesHelper helper = null;

        double rAltitute = 0.0;
        double range = 0.0;

        double lrAltitute = 0.0;
        double lrange = 0.0;

        RotateTransform rtrAltitute = new RotateTransform();
        RotateTransform rtRange = new RotateTransform();

        public MIG15_ALTR()
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

                               if (vals.Length > 0) { rAltitute = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { range = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (lrAltitute != rAltitute)
                               {
                                   if (rAltitute < 0.0)
                                   {
                                       rtrAltitute.Angle = rAltitute * 10;
                                       PRV_46_RAlt.RenderTransform = rtrAltitute;
                                   }
                                   else
                                   {
                                       rtrAltitute.Angle = rAltitute * 243;
                                       PRV_46_RAlt.RenderTransform = rtrAltitute;
                                   }
                               }
                               if (lrange != range)
                               {
                                   rtRange.Angle = range * -30;
                                   Range.RenderTransform = rtRange;
                               }
                               lrAltitute = rAltitute;
                               lrange = range;
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

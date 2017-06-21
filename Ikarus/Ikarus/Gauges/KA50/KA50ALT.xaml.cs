using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for KA50ALT.xaml
    /// </summary>
    public partial class KA50ALT : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double alt10000 = 0.0;
        double alt1000 = 0.0;
        double baroPressure = 0.0;
        double commandedAlt = 0.0;

        double lalt10000 = 0.0;
        double lalt1000 = 0.0;
        double lbaroPressure = 0.0;
        double lcommandedAlt = 0.0;

        RotateTransform rtAlt10000 = new RotateTransform();
        RotateTransform rtAlt1000 = new RotateTransform();
        RotateTransform rtBaroPressure = new RotateTransform();
        RotateTransform rtCommandedAlt = new RotateTransform();

        public KA50ALT()
        {
            InitializeComponent();

            rtBaroPressure.Angle = 240;
            WP_needleP_ALT.RenderTransform = rtBaroPressure;

            rtCommandedAlt.Angle = -203;
            WP_needleAl_ALT.RenderTransform = rtCommandedAlt;
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
            return 167.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { alt10000 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { alt1000 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { baroPressure = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { commandedAlt = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (lalt10000 != alt10000)
                               {
                                   rtAlt10000.Angle = alt10000 * 360;
                                   WP_needleM_ALT.RenderTransform = rtAlt10000;
                               }
                               if (lalt1000 != alt1000)
                               {
                                   rtAlt1000.Angle = alt1000 * 360;
                                   WP_needleC_ALT.RenderTransform = rtAlt1000;
                               }
                               if (lbaroPressure != baroPressure)
                               {
                                   rtBaroPressure.Angle = (baroPressure * -300) + 240;
                                   WP_needleP_ALT.RenderTransform = rtBaroPressure;
                               }
                               if (lcommandedAlt != commandedAlt)
                               {
                                   rtCommandedAlt.Angle = (commandedAlt * 360) - 203;
                                   WP_needleAl_ALT.RenderTransform = rtCommandedAlt;
                               }
                               lalt10000 = alt10000;
                               lalt1000 = alt1000;
                               lbaroPressure = baroPressure;
                               lcommandedAlt = commandedAlt;
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

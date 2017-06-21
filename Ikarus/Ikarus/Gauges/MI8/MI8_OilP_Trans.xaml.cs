using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MI8_OilP_Trans.xaml
    /// </summary>
    public partial class MI8_OilP_Trans : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double pressureMain = 0.0;
        double tempMain = 0.0;
        double tempTail = 0.0;

        double lpressureMain = 0.0;
        double ltempMain = 0.0;
        double ltempTail = 0.0;

        RotateTransform rttempMain = new RotateTransform();
        RotateTransform rtpressureMain = new RotateTransform();
        RotateTransform rtTempTail = new RotateTransform();

        public MI8_OilP_Trans()
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

                               if (vals.Length > 0) { pressureMain = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { tempMain = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { tempTail = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (pressureMain < 0.0) pressureMain = 0.0;
                               if (tempMain < 0.0) tempMain = 0.0;
                               if (tempTail < 0.0) tempTail = 0.0;

                               if (lpressureMain != pressureMain)
                               {
                                   rtpressureMain.Angle = pressureMain * 120;
                                   oils_p_main_reductor.RenderTransform = rtpressureMain;
                               }
                               if (ltempMain != tempMain)
                               {
                                   rttempMain.Angle = tempMain * -90;
                                   oils_temp_intermediate_reductor.RenderTransform = rttempMain;
                               }
                               if (ltempTail != tempTail)
                               {
                                   rtTempTail.Angle = tempTail * 90;
                                   oils_temp_tail_reductor.RenderTransform = rtTempTail;
                               }
                               lpressureMain = pressureMain;
                               ltempMain = tempMain;
                               ltempTail = tempTail;
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

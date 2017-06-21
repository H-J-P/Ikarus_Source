using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG21_SPO10.xaml
    /// </summary>
    public partial class MIG21_SPO10 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double sPO_L_F = 0.0;
        double sPO_R_F = 0.0;
        double sPO_R_B = 0.0;
        double sPO_L_B = 0.0;
        double sPO_MUTED = 0.0;

        public MIG21_SPO10()
        {
            InitializeComponent();

            SPO_L_F.Visibility = System.Windows.Visibility.Hidden;
            SPO_R_F.Visibility = System.Windows.Visibility.Hidden;
            SPO_R_B.Visibility = System.Windows.Visibility.Hidden;
            SPO_L_B.Visibility = System.Windows.Visibility.Hidden;
            SPO_MUTED.Visibility = System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { sPO_L_F = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { sPO_R_F = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { sPO_R_B = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { sPO_L_B = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { sPO_MUTED = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               SPO_L_F.Visibility = (sPO_L_F > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SPO_R_F.Visibility = (sPO_R_F > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SPO_R_B.Visibility = (sPO_R_B > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SPO_L_B.Visibility = (sPO_L_B > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SPO_MUTED.Visibility = (sPO_MUTED > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for A10_RadioVHFAM.xaml
    /// </summary>
    public partial class F5E_UHFPresetChannel : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        private double  value10 = 0.0;
        private int valueInt10 = 0;
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        public F5E_UHFPresetChannel()
        {
            InitializeComponent();
            Display.Text = "1";
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
            return 114.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { value10 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                                                                            //   1    2   3    4   5    6    7     8    9    10   11    12   13
                               valueInt10 = Convert.ToUInt16(value10 * 20); // 0.0;0.05;0.1;0.15;0.2;0.25; 0.3; 0.35; 0.4; 0.45; 0.5; 0.55; 0.6......

                               Display.Text = (valueInt10 + 1).ToString();
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

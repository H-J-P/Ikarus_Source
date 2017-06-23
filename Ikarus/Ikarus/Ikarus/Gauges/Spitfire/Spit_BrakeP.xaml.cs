using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for Spit_BrakeP.xaml
    /// </summary>
    public partial class Spit_BrakeP : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        double port = 0.0;
        double starb = 0.0;
        double supply = 0.0;

        double lport = 0.0;
        double lstarb = 0.0;
        double lsupply = 0.0;

        RotateTransform rtport = new RotateTransform();
        RotateTransform rtstaff = new RotateTransform();
        RotateTransform rtsupply = new RotateTransform();

        public int GetWindowID() { return windowID; }

        public Spit_BrakeP()
		{
			this.InitializeComponent();
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
            return 255; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { supply = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { port = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { starb = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (port < 0.0) { port = 0.0; }
                               if (starb < 0.0) { starb = 0.0; }
                               if (supply < 0.0) { supply = 0.0; }

                               if (lport != port)
                               {
                                   rtport.Angle = port * 143;
                                   Port.RenderTransform = rtport;
                               }
                               if (lstarb != starb)
                               {
                                   rtstaff.Angle = starb * -143;
                                   Starb.RenderTransform = rtstaff;
                               }
                               if (lsupply != supply)
                               {
                                   rtsupply.Angle = supply * 188;
                                   Supply.RenderTransform = rtsupply;
                               }
                               lport = port;
                               lstarb = starb;
                               lsupply = supply;
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
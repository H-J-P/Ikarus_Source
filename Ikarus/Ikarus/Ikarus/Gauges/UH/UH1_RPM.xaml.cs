using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for UH1_RPM.xaml
    /// </summary>
    public partial class UH1_RPM : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double engineTach = 0.0;
        double rotorTach = 0.0;

        double lengineTach = 0.0;
        double lrotorTach = 0.0;

        RotateTransform rtEngineTach = new RotateTransform();
        RotateTransform rtRotorTach = new RotateTransform();

        public UH1_RPM()
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
            return 255.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { engineTach = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { rotorTach = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (engineTach < 0.0) engineTach = 0.0;
                               if (rotorTach < 0.0) rotorTach = 0.0;

                               if (lengineTach != engineTach)
                               {
                                   rtEngineTach.Angle = engineTach * 302;
                                   EngineTach.RenderTransform = rtEngineTach;
                               }
                               if (lrotorTach != rotorTach)
                               {
                                   rtRotorTach.Angle = rotorTach * 302;
                                   RotorTach.RenderTransform = rtRotorTach;
                               }
                               lengineTach = engineTach;
                               lrotorTach = rotorTach;
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

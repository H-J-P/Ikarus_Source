using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for L39_BrakeP.xaml
    /// </summary>
    public partial class L39_BrakeP : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        private double brakeL = 0.0;
        private double brakeR = 0.0;

        private double lbrakeL = 0.0;
        private double lbrakeR = 0.0;

        RotateTransform rtBrakeL = new RotateTransform();
        RotateTransform rtBrakeR = new RotateTransform();

        public L39_BrakeP()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { brakeL = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { brakeR = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (brakeL < 0.0) brakeL = 0.0;
                               if (brakeR < 0.0) brakeR = 0.0;

                               if (lbrakeL != brakeL)
                               {
                                   rtBrakeL.Angle = brakeL * 172;
                                   Brake_Left.RenderTransform = rtBrakeL;
                               }
                               if (lbrakeR != brakeR)
                               {
                                   rtBrakeR.Angle = brakeR * -172;
                                   Brake_Right.RenderTransform = rtBrakeR;
                               }
                               lbrakeL = brakeL;
                               lbrakeR = brakeR;
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

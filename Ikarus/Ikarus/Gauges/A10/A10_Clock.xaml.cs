using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for A10_Clock.xaml
    /// </summary>
    public partial class A10_Clock : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }
        GaugesHelper helper = null;

        string valueHour = "";
        string state = "";

        string hours = "";
        string minutes = "";
        string seconds = "";
        double lseconds = 0.0;

        RotateTransform rtSec = new RotateTransform();

        public A10_Clock()
        {
            InitializeComponent();

            HourMinutesDigits.Text = "";
            SecondsDigits.Text = "";
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

                               if (vals.Length > 0)
                               {
                                   valueHour = vals[0];
                                   hours = (valueHour[0].ToString() + valueHour[1].ToString()).ToString();
                                   minutes = (valueHour[2].ToString() + valueHour[3].ToString()).ToString();
                                   seconds = (valueHour[4].ToString() + valueHour[5].ToString()).ToString();
                               }
                               if (vals.Length > 1) { state = vals[1]; }

                               if (lseconds != Convert.ToDouble(seconds, CultureInfo.InvariantCulture))
                               {
                                   rtSec.Angle = Convert.ToDouble(seconds, CultureInfo.InvariantCulture) / 60 * 360;
                                   Second.RenderTransform = rtSec;
                               }
                               SecondsDigits.Text = seconds;
                               HourMinutesDigits.Text = (hours + ":" + minutes);
                               State.Text = state;

                               lseconds = Convert.ToDouble(seconds, CultureInfo.InvariantCulture);
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

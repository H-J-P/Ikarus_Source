using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for F86_Labs.xaml
    /// </summary>
    public partial class F86_Labs : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public int GetWindowID() { return windowID; }
        GaugesHelper helper = null;

        double roll_needle = 0.0;
        double pitch_needle = 0.0;

        double lroll_needle = 0.0;
        double lpitch_needle = 0.0;

        RotateTransform rtLABS_roll_needle = new RotateTransform();
        RotateTransform rtLABS_pitch_needle = new RotateTransform();

        public F86_Labs()
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

                               if (vals.Length > 0) { roll_needle = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { pitch_needle = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (lroll_needle != roll_needle)
                               {
                                   rtLABS_roll_needle.Angle = roll_needle * 18;
                                   LABS_roll_needle.RenderTransform = rtLABS_roll_needle;
                               }
                               if (lpitch_needle != pitch_needle)
                               {
                                   rtLABS_pitch_needle.Angle = pitch_needle * 20;
                                   LABS_pitch_needle.RenderTransform = rtLABS_pitch_needle;
                               }
                               lroll_needle = roll_needle;
                               lpitch_needle = pitch_needle;
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

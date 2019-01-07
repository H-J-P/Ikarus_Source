using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for AJS37_Afterburner.xaml
    /// </summary>
    public partial class AJS37_Afterburner : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        private double readValue = 0.0;
        private double lreadValue = 0.0;

        public AJS37_Afterburner()
        {
            InitializeComponent();

            Burner1.Visibility = System.Windows.Visibility.Hidden;
            Burner2.Visibility = System.Windows.Visibility.Hidden;
            Burner3.Visibility = System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { readValue = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               if (lreadValue != readValue)
                               {
                                   Burner1.Visibility = (readValue >= 0.3 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
                                   Burner2.Visibility = (readValue >= 0.6 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
                                   Burner3.Visibility = (readValue >= 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
                               }
                               lreadValue = readValue;
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

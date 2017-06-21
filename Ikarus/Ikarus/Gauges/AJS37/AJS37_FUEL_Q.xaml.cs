using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für AJS37_FUEL_Q.xaml
    /// </summary>
    public partial class AJS37_FUEL_Q : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        private double fuelQ = 0.0;
        private double joker= 0.0;
        private double lfuelQ = 0.0;
        private double ljoker = 0.0;

        RotateTransform rtFuelQ = new RotateTransform();
        RotateTransform rtJoker = new RotateTransform();

        public AJS37_FUEL_Q()
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

                               if (vals.Length > 0) { fuelQ = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { joker = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (fuelQ < 0.0) { fuelQ = 0.0; }
                               if (joker < 0.0) { joker = 0.0; }

                               if (lfuelQ != fuelQ)
                               {
                                   rtFuelQ.Angle = fuelQ * 324;
                                   FUEL_Q1.RenderTransform = rtFuelQ;
                               }
                               if (ljoker != joker)
                               {
                                   rtJoker.Angle = joker * 324;
                                   JOKER_TIE.RenderTransform = rtJoker;
                               }
                               lfuelQ = fuelQ;
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

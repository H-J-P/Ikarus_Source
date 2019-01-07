using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for L39_HydraulikP.xaml
    /// </summary>
    public partial class L39_HydraulikP : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double hydrP = 0.0;
        double hydrPE = 0.0;

        double lhydrP = 0.0;
        double lhydrPE = 0.0;

        RotateTransform rtHydrP = new RotateTransform();
        RotateTransform rtHydrPE = new RotateTransform();

        public L39_HydraulikP()
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

                               if (vals.Length > 0) { hydrP = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { hydrPE = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (hydrP < 0.0) hydrP = 0.0;
                               if (hydrPE < 0.0) hydrPE = 0.0;

                               if (lhydrP != hydrP)
                               {
                                   rtHydrP.Angle = hydrP * 172;
                                   HydraulikP.RenderTransform = rtHydrP;
                               }
                               if (lhydrPE != hydrPE)
                               {
                                   rtHydrPE.Angle = hydrPE * -172;
                                   HydraulikEmergP.RenderTransform = rtHydrPE;
                               }
                               lhydrP = hydrP;
                               lhydrPE = hydrPE;
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

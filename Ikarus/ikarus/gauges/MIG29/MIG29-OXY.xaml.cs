using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MIG29OXY.xaml
    /// </summary>
    public partial class MIG29OXY : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double diffPressure = 0.0;
        double oxyQuantity = 0.0;
        double hkab = 0.0;

        double ldiffPressure = 0.0;
        double loxyQuantity  = 0.0;
        double lhkab = 0.0;

        TranslateTransform ttdiffPressure = new TranslateTransform();
        TranslateTransform ttoxyQuantity = new TranslateTransform();
        TranslateTransform tthkab = new TranslateTransform();


        public MIG29OXY()
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

                               if (vals.Length > 0) { hkab = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { oxyQuantity = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { diffPressure = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (hkab < 0.0) hkab = 0.0;
                               if (oxyQuantity < 0.0) oxyQuantity = 0.0;
                               if (diffPressure < 0.0) diffPressure = 0.0;

                               if (lhkab != hkab)
                               {
                                   tthkab.Y = hkab * -134;
                                   H_KAB.RenderTransform = tthkab;
                               }
                               if (loxyQuantity != oxyQuantity)
                               {
                                   ttoxyQuantity.Y = oxyQuantity * -113;
                                   OXY_Quantity.RenderTransform = ttoxyQuantity;
                                   Abstraction.RenderTransform = ttoxyQuantity;
                               }
                               if (ldiffPressure != diffPressure)
                               {
                                   ttdiffPressure.Y = diffPressure * -131;
                                   Diff_Pressure.RenderTransform = ttdiffPressure;
                               }
                               lhkab = hkab;
                               loxyQuantity = oxyQuantity;
                               ldiffPressure = diffPressure;
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

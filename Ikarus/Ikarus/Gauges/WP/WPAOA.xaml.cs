using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for WPAOA.xaml
    /// </summary>
    public partial class WPAOA : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double aoa = 0.0;
        double gLoad = 0.0;
        double gLoadMax = 0.0;
        double gLoadMin = 0.0;
        double marker = 0.0;

        double laoa = 0.0;
        double lgLoad = 0.0;
        double lgLoadMax = 0.0;
        double lgLoadMin = 0.0;
        double lmarker = 0.0;

        RotateTransform rtgLoad = new RotateTransform();
        RotateTransform rtgLoadMax = new RotateTransform();
        RotateTransform rtgLoadMin = new RotateTransform();
        RotateTransform rtAoa = new RotateTransform();

        public WPAOA()
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

                               if (vals.Length > 0) { aoa = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { gLoad = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { gLoadMax = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { gLoadMin = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { marker = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               if (laoa != aoa)
                               {
                                   rtAoa.Angle = aoa * 130;
                                   AOA.RenderTransform = rtAoa;
                               }
                               if (lgLoad != gLoad)
                               {
                                   rtgLoad.Angle = gLoad * -120;
                                   GLoad.RenderTransform = rtgLoad;
                               }
                               if (lgLoadMax != gLoadMax)
                               {
                                   rtgLoadMax.Angle = gLoadMax * -120;
                                   GLmax.RenderTransform = rtgLoadMax;
                               }
                               if (lgLoadMin != gLoadMin)
                               {
                                   rtgLoadMin.Angle = gLoadMin * -120;
                                   GLmin.RenderTransform = rtgLoadMin;
                               }
                               Marks_1.Visibility = marker > 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Marks_2.Visibility = marker > 0.9 ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;

                               laoa = aoa;
                               lgLoad = gLoad;
                               lgLoadMax = gLoadMax;
                               lgLoadMin = gLoadMin;
                               lmarker = marker;
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

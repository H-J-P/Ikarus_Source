using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;


namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für MIG_29_AOA.xaml
    /// </summary>
    public partial class MIG29AOA : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;
        public int GetWindowID() { return windowID; }

        double aoa = 0.0;
        double glmin = 0.0;
        double glmax = 0.0;
        double gload = 0.0;

        double laoa = 0.0;
        double lglmin = 0.0;
        double lglmax = 0.0;
        double lgload = 0.0;

        RotateTransform rtaoa = new RotateTransform();
        RotateTransform rtglmin = new RotateTransform();
        RotateTransform rtglmax = new RotateTransform();
        RotateTransform rtgload = new RotateTransform();

        public MIG29AOA()
        {
            InitializeComponent();

            rtgload.Angle = 12 + (gload * -120);
            GLoad.RenderTransform = rtgload;

            rtglmin.Angle = 12 + (glmin * -120);
            GLmin.RenderTransform = rtglmin;

            rtglmax.Angle = 12 + (glmax * -120);
            GLmax.RenderTransform = rtglmax;
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
                               if (vals.Length > 1) { gload = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { glmin = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { glmax = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               //if (aoa < 0.0) aoa = 0.0;
                               //if (glmin < 0.0) glmin = 0.0;
                               //if (glmax < 0.0) glmax = 0.0;
                               //if (gload < 0.0) gload = 0.0;

                               if (laoa != aoa)
                               {
                                   rtaoa.Angle = aoa * 123;
                                   AOA.RenderTransform = rtaoa;
                               }
                               if (lglmin != glmin)
                               {
                                   rtglmin.Angle = 12 + (glmin * -120);
                                   GLmin.RenderTransform = rtglmin;
                               }
                               if (lglmax != glmax)
                               {
                                   rtglmax.Angle = 12 + (glmax * -120);
                                   GLmax.RenderTransform = rtglmax;
                               }
                               if (lgload != gload)
                               {
                                   rtgload.Angle = 12 + (gload * -120);
                                   GLoad.RenderTransform = rtgload;
                               }
                               laoa = aoa;
                               lglmin = glmin;
                               lglmax = glmax;
                               lgload = gload;
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

using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for P51ACC.xaml
    /// </summary>
    public partial class P51ACC : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double GLmain = 0.0;
        double GLmin = 0.0;
        double GLmax = 0.0;

        double lGLmain = 0.0;
        double lGLmin = 0.0;
        double lGLmax = 0.0;

        RotateTransform rtGLmain = new RotateTransform();
        RotateTransform rtGLmax = new RotateTransform();
        RotateTransform rtGLmin = new RotateTransform();

        public P51ACC()
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
            string[] vals = _output.Split(',');
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
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { GLmain = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { GLmin = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { GLmax = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (lGLmain != GLmain)
                               {
                                   rtGLmain.Angle = (GLmain * 339) - 100;
                                   Accelerometer_main.RenderTransform = rtGLmain;
                               }
                               if (lGLmin != GLmax)
                               {
                                   rtGLmax.Angle = (GLmax * 339) - 100;
                                   Accelerometer_max.RenderTransform = rtGLmax;
                               }
                               if (lGLmax != GLmin)
                               {
                                   rtGLmin.Angle = (GLmin * 339) - 100;
                                   Accelerometer_min.RenderTransform = rtGLmin;
                               }
                               lGLmain = GLmain;
                               lGLmin = GLmax;
                               lGLmax = GLmin;
                           }
                           catch { return; }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}

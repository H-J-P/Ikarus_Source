using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für LwHeadCourse.xaml   Author: HJP
    /// </summary>
    public partial class LwHeadCourse : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double compassHeading = 0.0;
        double commandedCourse = 0.0;

        double lcompassHeading = 0.0;
        double lcommandedCourse = 0.0;

        RotateTransform rtCompassHeading = new RotateTransform();
        RotateTransform rtCommandedCourse = new RotateTransform();

        public LwHeadCourse()
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
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { compassHeading = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { commandedCourse = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (lcompassHeading != compassHeading)
                               {
                                   rtCompassHeading.Angle = compassHeading * 360 + commandedCourse * 360;
                                   CompassHeading.RenderTransform = rtCompassHeading;
                               }

                               if (lcommandedCourse != commandedCourse)
                               {
                                   rtCommandedCourse.Angle = commandedCourse * 360;
                                   CompassCourse.RenderTransform = rtCommandedCourse;

                                   rtCompassHeading.Angle = compassHeading * 360 + commandedCourse * 360;
                                   CompassHeading.RenderTransform = rtCompassHeading;
                               }

                               lcompassHeading = compassHeading;
                               lcommandedCourse = commandedCourse;
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

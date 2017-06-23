using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for UH1_RadioCompassIndicatorCopilot.xaml Copilot
    /// </summary>
    public partial class UH1_RadioCompassIndicatorCopilot : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double coursePointer1 = 0.0;
        double coursePointer2 = 0.0;
        double heading = 0.0;

        double lcoursePointer1 = 0.0;
        double lcoursePointer2 = 0.0;
        double lheading = 0.0;

        RotateTransform rtHeading = new RotateTransform();
        RotateTransform rtCoursePointer1 = new RotateTransform();
        RotateTransform rtCoursePointer2 = new RotateTransform();

        public UH1_RadioCompassIndicatorCopilot()
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

                               if (vals.Length > 0) { coursePointer1 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { coursePointer2 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { heading = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (lheading != heading)
                               {
                                   rtHeading.Angle = heading * -360;
                                   RMI_Heading.RenderTransform = rtHeading;
                               }

                               if (lcoursePointer1 != coursePointer1)
                               {
                                   rtCoursePointer1.Angle = coursePointer1 * 360;
                                   RMI_CoursePointer1.RenderTransform = rtCoursePointer1;
                               }

                               if (lcoursePointer2 != coursePointer2)
                               {
                                   rtCoursePointer2.Angle = coursePointer2 * 360;
                                   RMI_CoursePointer2.RenderTransform = rtCoursePointer2;
                               }

                               lcoursePointer1 = coursePointer1;
                               lcoursePointer2 = coursePointer2;
                               lheading = heading;
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

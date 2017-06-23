using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;


namespace Ikarus
{
    /// <summary>
    /// Interaction logic for F5E_HSI.xaml
    /// </summary>
    public partial class F5E_HSI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        TransformGroup grp = new TransformGroup();
        RotateTransform rtCourse = new RotateTransform();
        TranslateTransform ttdeviation = new TranslateTransform();

        TranslateTransform ttrangeCounter_100 = new TranslateTransform();
        TranslateTransform ttrangeCounter_10 = new TranslateTransform();
        TranslateTransform ttrangeCounter_1 = new TranslateTransform();
        TranslateTransform ttcourseCounter_100 = new TranslateTransform();
        TranslateTransform ttcourseCounter_10 = new TranslateTransform();
        TranslateTransform ttcourseCounter_1 = new TranslateTransform();

        RotateTransform rtHeading = new RotateTransform();
        RotateTransform rtbearingNo1 = new RotateTransform();
        RotateTransform rtbearingNo2 = new RotateTransform();
        RotateTransform rtheadingMarker = new RotateTransform();

        double heading = 0.0;
        double bearingNo1 = 0.0;
        double bearingNo2 = 0.0;
        double headingMarker = 0.0;
        double courseArrow = 0.0;
        double deviation = 0.0;
        double rangeCounter_100 = 0.0;
        double rangeCounter_10 = 0.0;
        double rangeCounter_1 = 0.0;
        double course_100 = 0.0;
        double course_10 = 0.0;
        double course_1 = 0.0;
        double to_from_1 = 0.0;
        double bearingFlag = 0.0;
        double poweroffFlag = 0.0;
        double rangeFlag = 0.0;

        double lheading = 0.0;
        double lbearingNo1 = 0.0;
        double lbearingNo2 = 0.0;
        double lheadingMarker = 0.0;
        double lcourseArrow = 0.0;
        double ldeviation = 0.0;
        double lrangeCounter_100 = 0.0;
        double lrangeCounter_10 = 0.0;
        double lrangeCounter_1 = 0.0;
        double lcourse_100 = 0.0;
        double lcourse_10 = 0.0;
        double lcourse_1 = 0.0;
        double lto_from_1 = 0.0;
        double lbearingFlag = 0.0;
        double lpoweroffFlag = 0.0;
        double lrangeFlag = 0.0;

        public F5E_HSI()
        {
            InitializeComponent();

            Flagg_TO.Visibility = System.Windows.Visibility.Hidden;
            Flagg_FROM.Visibility = System.Windows.Visibility.Hidden;
            Flagg_OFF.Visibility = System.Windows.Visibility.Visible;
            Flagg_NAV.Visibility = System.Windows.Visibility.Visible;
            Flagg_Range_OFF.Visibility = System.Windows.Visibility.Visible;
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
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { heading = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { bearingNo1 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { bearingNo2 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { headingMarker = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { courseArrow = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { deviation = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { rangeCounter_100 = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { rangeCounter_10 = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { rangeCounter_1 = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }
                               if (vals.Length > 9) { course_100 = Convert.ToDouble(vals[9], CultureInfo.InvariantCulture); }
                               if (vals.Length > 10) { course_10 = Convert.ToDouble(vals[10], CultureInfo.InvariantCulture); }
                               if (vals.Length > 11) { course_1 = Convert.ToDouble(vals[11], CultureInfo.InvariantCulture); }
                               if (vals.Length > 12) { poweroffFlag = Convert.ToDouble(vals[12], CultureInfo.InvariantCulture); }
                               if (vals.Length > 13) { rangeFlag = Convert.ToDouble(vals[13], CultureInfo.InvariantCulture); }
                               if (vals.Length > 14) { to_from_1 = Convert.ToDouble(vals[14], CultureInfo.InvariantCulture); }
                               if (vals.Length > 15) { bearingFlag = Convert.ToDouble(vals[15], CultureInfo.InvariantCulture); }

                               if (heading != lheading)
                               {
                                   rtHeading.Angle = heading * 360;
                                   Heading.RenderTransform = rtHeading;
                               }

                               if (bearingNo1 != lbearingNo1)
                               {
                                   rtbearingNo1.Angle = (bearingNo1 * 360);// + (heading * 360);
                                   Bearing1.RenderTransform = rtbearingNo1;
                               }

                               if (bearingNo2 != lbearingNo2)
                               {
                                   rtbearingNo2.Angle = (bearingNo2 * 360); // + (heading * 360);
                                   Bearing2.RenderTransform = rtbearingNo2;
                               }

                               if (headingMarker != lheadingMarker)
                               {
                                   rtheadingMarker.Angle = (headingMarker * 360); // + (heading * 360);
                                   Heading_Marker.RenderTransform = rtheadingMarker;
                               }

                               if (course_100 != lcourse_100)
                               {
                                   ttcourseCounter_100.Y = course_100 * -220;
                                   Course_100.RenderTransform = ttcourseCounter_100;
                               }

                               if (course_10 != lcourse_10)
                               {
                                   ttcourseCounter_10.Y = course_10 * -220;
                                   Course_10.RenderTransform = ttcourseCounter_10;
                               }
                               if (course_1 != lcourse_1)
                               {
                                   ttcourseCounter_1.Y = course_1 * -220;
                                   Course_1.RenderTransform = ttcourseCounter_1;
                               }

                               if (rangeCounter_100 != lrangeCounter_100)
                               {
                                   ttrangeCounter_100.Y = rangeCounter_100 * -220;
                                   Range_100.RenderTransform = ttrangeCounter_100;
                               }

                               if (rangeCounter_10 != lrangeCounter_10)
                               {
                                   ttrangeCounter_10.Y = rangeCounter_10 * -220;
                                   Range_10.RenderTransform = ttrangeCounter_10;
                               }

                               if (rangeCounter_1 != lrangeCounter_1)
                               {
                                   ttrangeCounter_1.Y = rangeCounter_1 * -220;
                                   Range_1.RenderTransform = ttrangeCounter_1;
                               }

                               if (courseArrow != lcourseArrow || deviation != ldeviation)
                               {
                                   grp = new TransformGroup();
                                   rtCourse = new RotateTransform();
                                   ttdeviation = new TranslateTransform();

                                   rtCourse.Angle = (courseArrow * 360); // + (heading * -360);
                                   ttdeviation.X = deviation * 55;
                                   grp.Children.Add(ttdeviation);
                                   grp.Children.Add(rtCourse);
                                   Course.RenderTransform = rtCourse;
                                   CDI.RenderTransform = grp;
                               }
                               Flagg_OFF.Visibility = (poweroffFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Flagg_Range_OFF.Visibility = (rangeFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Flagg_NAV.Visibility = (bearingFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               Flagg_TO.Visibility = (to_from_1 > 0.1) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Flagg_FROM.Visibility = (to_from_1 < -0.1) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lheading = heading;
                               lbearingNo1 = bearingNo1;
                               lbearingNo2 = bearingNo2;
                               lheadingMarker = headingMarker;
                               lcourseArrow = courseArrow;
                               ldeviation = deviation;
                               lrangeCounter_100 = rangeCounter_100;
                               lrangeCounter_10 = rangeCounter_10;
                               lrangeCounter_1 = rangeCounter_1;
                               lto_from_1 = to_from_1;
                               lbearingFlag = bearingFlag;
                               lpoweroffFlag = poweroffFlag;
                               lrangeFlag = rangeFlag;
                               lcourse_100 = course_100;
                               lcourse_10 = course_10;
                               lcourse_1 = course_1;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Globalization;

namespace HawgTouch.GaugePack.Gauges
{
    /// <summary>
    /// Interaction logic for USHSI.xaml
    /// </summary>
    public partial class USHSI : UserControl, IHawgTouchGauge
    {
        private string _DataImportID;
        public string DataImportID
        {
            get { return _DataImportID; }
            set { _DataImportID = value; }
        }

        public USHSI()
        {
            InitializeComponent();

            this.Flagg_to.Visibility = System.Windows.Visibility.Hidden;
            this.Flagg_from.Visibility = System.Windows.Visibility.Hidden;
            this.Power_Off_Flagg.Visibility = System.Windows.Visibility.Visible;
            this.Bearing_Flagg.Visibility = System.Windows.Visibility.Visible;
        }

        public System.Windows.Size Size
        {
            get { return new System.Windows.Size(345, 289); }
        }

        public void UpdateGauge(string strData)
        {
            string[] vals = strData.Split(';');

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           double heading = 0.0;
                           double bearingNo1 = 0.0;
                           double bearingNo2 = 0.0;
                           double headingMarker = 0.0;
                           double courseArrow = 0.0;
                           double deviation = 0.0;
                           double rangeCounter_100 = 0.0;
                           double rangeCounter_10 = 0.0;
                           double rangeCounter_1 = 0.0;
                           double to_from_1 = 0.0;
                           double to_from_2 = 0.0;
                           double bearingFlag = 0.0;
                           double poweroffFlag = 0.0;
                           double rangeFlag = 0.0;
                           String sCourse = "";

                           try
                           {
                               if (vals.Length > 0) { heading = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { bearingNo1 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { bearingNo2 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { headingMarker = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { courseArrow = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { deviation = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { rangeCounter_100 = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { rangeCounter_10 = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { rangeCounter_1 = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }
                               if (vals.Length > 9) { to_from_1 = Convert.ToDouble(vals[9], CultureInfo.InvariantCulture); }
                               if (vals.Length > 10) { to_from_2 = Convert.ToDouble(vals[10], CultureInfo.InvariantCulture); }
                               if (vals.Length > 11) { bearingFlag = Convert.ToDouble(vals[11], CultureInfo.InvariantCulture); }
                               if (vals.Length > 12) { poweroffFlag = Convert.ToDouble(vals[12], CultureInfo.InvariantCulture); }
                               if (vals.Length > 13) { rangeFlag = Convert.ToDouble(vals[13], CultureInfo.InvariantCulture); }
                           }
                           catch { return; }

                           this.Power_Off_Flagg.Visibility = (poweroffFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                           this.Bearing_Flagg.Visibility = (bearingFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                           RotateTransform rtHeading = new RotateTransform();
                           rtHeading.Angle = heading * 360;
                           this.Heading.RenderTransform = rtHeading;

                           RotateTransform rtbearingNo1 = new RotateTransform();
                           rtbearingNo1.Angle = (bearingNo1 * 360);// + (heading * 360);
                           this.Bearing1.RenderTransform = rtbearingNo1;

                           RotateTransform rtbearingNo2 = new RotateTransform();
                           rtbearingNo2.Angle = (bearingNo2 * 360);// + (heading * 360);
                           this.Bearing2.RenderTransform = rtbearingNo2;

                           RotateTransform rtheadingMarker = new RotateTransform();
                           rtheadingMarker.Angle = (headingMarker * 360); // + (heading * 360);
                           this.Heading_Marker.RenderTransform = rtheadingMarker;

                           double courseCounter_100 = 0.0;
                           double courseCounter_10 = 0.0;
                           double courseCounter_1 = 0.0;
                           double courceCounter = (courseArrow * 360) + (heading * -360);

                           if (courceCounter > 360){courceCounter -= 360;}
                           if (courceCounter < 0){courceCounter += 360;}

                           try
                           {
                               sCourse = Convert.ToInt16(Convert.ToDouble(courceCounter, CultureInfo.InvariantCulture)).ToString();

                               if (sCourse.Length == 0) { sCourse = "000"; }
                               else if (sCourse.Length == 1) { sCourse = "00" + sCourse; }
                               else if (sCourse.Length == 2) { sCourse = "0" + sCourse; }

                               courseCounter_100 = Convert.ToDouble(sCourse[0].ToString(), CultureInfo.InvariantCulture);
                               courseCounter_10 = Convert.ToDouble(sCourse[1].ToString(), CultureInfo.InvariantCulture);
                               courseCounter_1 = Convert.ToDouble(sCourse[2].ToString(), CultureInfo.InvariantCulture);
                           }
                           catch { }

                           TranslateTransform ttcourseCounter_100 = new TranslateTransform();
                           ttcourseCounter_100.Y = courseCounter_100 * -22;
                           this.Course_100.RenderTransform = ttcourseCounter_100;

                           TranslateTransform ttcourseCounter_10 = new TranslateTransform();
                           ttcourseCounter_10.Y = courseCounter_10 * -22;
                           this.Course_10.RenderTransform = ttcourseCounter_10;

                           TranslateTransform ttcourseCounter_1 = new TranslateTransform();
                           ttcourseCounter_1.Y = courseCounter_1 * -22;
                           this.Course_1.RenderTransform = ttcourseCounter_1;

                           TranslateTransform ttrangeCounter_100 = new TranslateTransform();
                           ttrangeCounter_100.Y = rangeCounter_100 * -220;
                           this.DistanceToWaypoint_100.RenderTransform = ttrangeCounter_100;

                           TranslateTransform ttrangeCounter_10 = new TranslateTransform();
                           ttrangeCounter_10.Y = rangeCounter_10 * -220;
                           this.DistanceToWaypoint_10.RenderTransform = ttrangeCounter_10;

                           TranslateTransform ttrangeCounter_1 = new TranslateTransform();
                           ttrangeCounter_1.Y = rangeCounter_1 * -220;
                           this.DistanceToWaypoint_1.RenderTransform = ttrangeCounter_1;

                           TransformGroup grp = new TransformGroup();
                           RotateTransform rtCourse = new RotateTransform();
                           TranslateTransform ttdeviation = new TranslateTransform();

                           rtCourse.Angle = (courseArrow * 360); // + (heading * -360);
                           ttdeviation.X = deviation * 55;
                           grp.Children.Add(ttdeviation);
                           grp.Children.Add(rtCourse);
                           this.Course.RenderTransform = rtCourse;
                           this.Course_Deviation_Indicator.RenderTransform = grp;

                           this.Flagg_to.Visibility = (to_from_1 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                           this.Flagg_from.Visibility = (to_from_2 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                       }));
        }
    }
}

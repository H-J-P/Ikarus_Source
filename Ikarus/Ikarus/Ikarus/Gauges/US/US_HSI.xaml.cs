using System;
using System.Windows.Controls;
using System.Data;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for USHSI.xaml
    /// </summary>
    public partial class US_HSI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
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
        double to_from_1 = 0.0;
        double to_from_2 = 0.0;
        double bearingFlag = 0.0;
        double poweroffFlag = 0.0;
        double rangeFlag = 0.0;
        double courseCounter_100 = 0.0;
        double courseCounter_10 = 0.0;
        double courseCounter_1 = 0.0;
        double courceCounter = 0.0;

        double lheading = 0.0;
        double lbearingNo1 = 0.0;
        double lbearingNo2 = 0.0;
        double lheadingMarker = 0.0;
        double lcourseArrow = 0.0;
        double ldeviation = 0.0;
        double lrangeCounter_100 = 0.0;
        double lrangeCounter_10 = 0.0;
        double lrangeCounter_1 = 0.0;
        double lto_from_1 = 0.0;
        double lto_from_2 = 0.0;
        double lbearingFlag = 0.0;
        double lpoweroffFlag = 0.0;
        double lrangeFlag = 0.0;
        double lcourseCounter_100 = 0.0;
        double lcourseCounter_10 = 0.0;
        double lcourseCounter_1 = 0.0;
        //double lcourceCounter = 0.0;

        public US_HSI()
        {
            InitializeComponent();
            if (MainWindow.editmode) MakeDraggable(this, this);

            Flagg_to.Visibility = System.Windows.Visibility.Hidden;
            Flagg_from.Visibility = System.Windows.Visibility.Hidden;
            Power_Off_Flagg.Visibility = System.Windows.Visibility.Visible;
            Bearing_Flagg.Visibility = System.Windows.Visibility.Visible;
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
        }

        public string GetID() { return dataImportID; }

        private void LoadBmaps()
        {
            DataRow[] dataRows = MainWindow.dtInstruments.Select("IDInst=" + dataImportID);

            if (dataRows.Length > 0)
            {
                string frame = dataRows[0]["ImageFrame"].ToString();
                string light = dataRows[0]["ImageLight"].ToString();

                try
                {
                    if (frame.Length > 4)
                    {
                        if (File.Exists(Environment.CurrentDirectory + "\\Images\\Frames\\" + frame))
                            Frame.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + frame));
                    }
                    if (light.Length > 4)
                    {
                        if (File.Exists(Environment.CurrentDirectory + "\\Images\\Frames\\" + light))
                            Light.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + light));
                    }
                    SwitchLight(false);
                }
                catch { }
            }
        }

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
            return 345; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               string sCourse = "";

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

                               Power_Off_Flagg.Visibility = (poweroffFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Bearing_Flagg.Visibility = (bearingFlag > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

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

                               courceCounter = (courseArrow * 360) + (heading * -360);

                               if (courceCounter > 360) { courceCounter -= 360; }
                               if (courceCounter < 0) { courceCounter += 360; }

                               sCourse = Convert.ToInt16(Convert.ToDouble(courceCounter, CultureInfo.InvariantCulture)).ToString();

                               if (sCourse.Length == 0) { sCourse = "000"; }
                               else if (sCourse.Length == 1) { sCourse = "00" + sCourse; }
                               else if (sCourse.Length == 2) { sCourse = "0" + sCourse; }

                               courseCounter_100 = Convert.ToDouble(sCourse[0].ToString(), CultureInfo.InvariantCulture);
                               courseCounter_10 = Convert.ToDouble(sCourse[1].ToString(), CultureInfo.InvariantCulture);
                               courseCounter_1 = Convert.ToDouble(sCourse[2].ToString(), CultureInfo.InvariantCulture);

                               if (courseCounter_100 != lcourseCounter_100)
                               {
                                   ttcourseCounter_100.Y = courseCounter_100 * -22;
                                   Course_100.RenderTransform = ttcourseCounter_100;
                               }

                               if (courseCounter_10 != lcourseCounter_10)
                               {
                                   ttcourseCounter_10.Y = courseCounter_10 * -22;
                                   Course_10.RenderTransform = ttcourseCounter_10;
                               }
                               if (courseCounter_1 != lcourseCounter_1)
                               {
                                   ttcourseCounter_1.Y = courseCounter_1 * -22;
                                   Course_1.RenderTransform = ttcourseCounter_1;
                               }

                               if (rangeCounter_100 != lrangeCounter_100)
                               {
                                   ttrangeCounter_100.Y = rangeCounter_100 * -220;
                                   DistanceToWaypoint_100.RenderTransform = ttrangeCounter_100;
                               }

                               if (rangeCounter_10 != lrangeCounter_10)
                               {
                                   ttrangeCounter_10.Y = rangeCounter_10 * -220;
                                   DistanceToWaypoint_10.RenderTransform = ttrangeCounter_10;
                               }

                               if (rangeCounter_1 != lrangeCounter_1)
                               {
                                   ttrangeCounter_1.Y = rangeCounter_1 * -220;
                                   DistanceToWaypoint_1.RenderTransform = ttrangeCounter_1;
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
                                   Course_Deviation_Indicator.RenderTransform = grp;
                               }
                               Flagg_to.Visibility = (to_from_1 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Flagg_from.Visibility = (to_from_2 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

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
                               lto_from_2 = to_from_2;
                               lbearingFlag = bearingFlag;
                               lpoweroffFlag = poweroffFlag;
                               lrangeFlag = rangeFlag;
                               lcourseCounter_100 = courseCounter_100;
                               lcourseCounter_10 = courseCounter_10;
                               lcourseCounter_1 = courseCounter_1;
                               //lcourceCounter = courceCounter;
                           }
                           catch { return; }
                       }));
        }

        private void MakeDraggable(System.Windows.UIElement moveThisElement, System.Windows.UIElement movedByElement)
        {
            System.Windows.Point originalPoint = new System.Windows.Point(0, 0), currentPoint;
            TranslateTransform trUsercontrol = new TranslateTransform(0, 0);
            bool isMousePressed = false;

            movedByElement.MouseLeftButtonDown += (a, b) =>
            {
                isMousePressed = true;
                originalPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
            };

            movedByElement.MouseLeftButtonUp += (a, b) =>
            {
                isMousePressed = false;
                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID);
            };
            movedByElement.MouseLeave += (a, b) => isMousePressed = false;

            movedByElement.MouseMove += (a, b) =>
            {
                if (!isMousePressed || !MainWindow.editmode) return;

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                trUsercontrol.X += currentPoint.X - originalPoint.X;
                trUsercontrol.Y += currentPoint.Y - originalPoint.Y;
                moveThisElement.RenderTransform = trUsercontrol;


            };
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}

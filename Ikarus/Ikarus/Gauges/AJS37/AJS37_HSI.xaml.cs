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
    /// Interaktionslogik für AJS37_HSI.xaml
    /// </summary>
    public partial class AJS37_HSI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        private double heading = 0.0;
        private double commandedCourse = 0.0;
        private double fixedCourseIndexer = 0.0;

        private double rwr33 = 0.0;
        private double rwr27 = 0.0;
        private double rwr21 = 0.0;
        private double rwr15 = 0.0;
        private double rwr9 = 0.0;
        private double rwr3 = 0.0;
        private double warningFlag = 0.0;

        private double lheading = 0.0;
        private double lcommandedCourse = 0.0;
        private double lfixedCourseIndexer = 0.0;

        private double lrwr33 = 0.0;
        private double lrwr27 = 0.0;
        private double lrwr21 = 0.0;
        private double lrwr15 = 0.0;
        private double lrwr9 = 0.0;
        private double lrwr3 = 0.0;
        private double lwarningFlag = 0.0;

        RotateTransform rtHeading = new RotateTransform();
        RotateTransform rtCommandedCourse = new RotateTransform();
        RotateTransform rtFixedCourseIndexer = new RotateTransform();

        public AJS37_HSI()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            RWR_3.Visibility = System.Windows.Visibility.Hidden;
            RWR_9.Visibility =System.Windows.Visibility.Hidden;
            RWR_15.Visibility =  System.Windows.Visibility.Hidden;
            RWR_21.Visibility = System.Windows.Visibility.Hidden;
            RWR_27.Visibility = System.Windows.Visibility.Hidden;
            RWR_33.Visibility = System.Windows.Visibility.Hidden;
            Warning_flag.Visibility =  System.Windows.Visibility.Hidden;
            ALTITUDE_WARNING.Visibility = System.Windows.Visibility.Hidden;
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
            return 167; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { heading = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { commandedCourse = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { fixedCourseIndexer = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { rwr3 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { rwr9 = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { rwr15 = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { rwr21 = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { rwr27 = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { rwr33 = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }
                               if (vals.Length > 9) { warningFlag = Convert.ToDouble(vals[9], CultureInfo.InvariantCulture); }

                               if (lheading != heading)
                               {
                                   rtHeading.Angle = heading * 360;
                                   Heading.RenderTransform = rtHeading;
                               }
                               if (lcommandedCourse != commandedCourse)
                               {
                                   rtCommandedCourse.Angle = commandedCourse * 360;
                                   Commanded_Course.RenderTransform = rtCommandedCourse;
                               }
                               if (lfixedCourseIndexer != fixedCourseIndexer)
                               {
                                   rtFixedCourseIndexer.Angle = fixedCourseIndexer * 360;
                                   Fixed_course_indexer.RenderTransform = rtFixedCourseIndexer;
                               }

                               if (lrwr3 != rwr3)
                                   RWR_3.Visibility = (rwr3 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lrwr9 != rwr9)
                                   RWR_9.Visibility = (rwr9 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lrwr15 != rwr15)
                                   RWR_15.Visibility = (rwr15 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lrwr21 != rwr21)
                                   RWR_21.Visibility = (rwr21 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lrwr27 != rwr27)
                                   RWR_27.Visibility = (rwr27 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lrwr33 != rwr33)
                                   RWR_33.Visibility = (rwr33 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               if (lwarningFlag != warningFlag)
                                   Warning_flag.Visibility = (warningFlag > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lheading = heading;
                               lcommandedCourse = commandedCourse;
                               lfixedCourseIndexer = fixedCourseIndexer;

                               lrwr3 = rwr3;
                               lrwr9 = rwr9;
                               lrwr15 = rwr15;
                               lrwr21 = rwr21;
                               lrwr27 = rwr27;
                               lrwr33 = rwr33;
                               lwarningFlag = warningFlag;
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

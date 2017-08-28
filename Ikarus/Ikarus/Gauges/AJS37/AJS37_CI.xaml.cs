using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für AJS37_HSI.xaml
    /// </summary>
    public partial class AJS37_CI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        private double heading = 0.0;
        private double commandedCourse = 0.0;
        private double rwr33 = 0.0;
        private double rwr27 = 0.0;
        private double rwr21 = 0.0;
        private double rwr15 = 0.0;
        private double rwr9 = 0.0;
        private double rwr3 = 0.0;
        private double warningFlag = 0.0;
        private double radarRange = 0.0;

        private double lheading = 0.0;
        private double lcommandedCourse = 0.0;
        private double lrwr33 = 0.0;
        private double lrwr27 = 0.0;
        private double lrwr21 = 0.0;
        private double lrwr15 = 0.0;
        private double lrwr9 = 0.0;
        private double lrwr3 = 0.0;
        private double lwarningFlag = 0.0;
        private double lradarRange = 0.0;

        RotateTransform rtHeading = new RotateTransform();
        RotateTransform rtCommandedCourse = new RotateTransform();

        public AJS37_CI()
        {
            InitializeComponent();

            RWR_3.Visibility = System.Windows.Visibility.Hidden;
            RWR_9.Visibility =System.Windows.Visibility.Hidden;
            RWR_15.Visibility =  System.Windows.Visibility.Hidden;
            RWR_21.Visibility = System.Windows.Visibility.Hidden;
            RWR_27.Visibility = System.Windows.Visibility.Hidden;
            RWR_33.Visibility = System.Windows.Visibility.Hidden;
            Warning_flag.Visibility =  System.Windows.Visibility.Visible;

            Range15.Visibility = System.Windows.Visibility.Hidden;
            Range30.Visibility = System.Windows.Visibility.Hidden;
            Range60.Visibility = System.Windows.Visibility.Hidden;
            Range120.Visibility = System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { heading = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { commandedCourse = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { rwr3 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { rwr9 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { rwr15 = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { rwr21 = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { rwr27 = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { rwr33 = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { warningFlag = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }
                               if (vals.Length > 9) { radarRange = Convert.ToDouble(vals[9], CultureInfo.InvariantCulture); }

                               if (lheading != heading)
                               {
                                   rtHeading.Angle = heading * -180;
                                   Heading.RenderTransform = rtHeading;
                               }
                               if (lcommandedCourse != commandedCourse)
                               {
                                   rtCommandedCourse.Angle = commandedCourse * 360;
                                   Commanded_Course.RenderTransform = rtCommandedCourse;
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
                                   Warning_flag.Visibility = (warningFlag > 0.9) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;

                               if (lradarRange != radarRange)
                               {
                                   if (radarRange >= 0.0 && radarRange < 0.1)
                                   {
                                       Range15.Visibility = System.Windows.Visibility.Hidden;
                                       Range30.Visibility = System.Windows.Visibility.Hidden;
                                       Range60.Visibility = System.Windows.Visibility.Hidden;
                                       Range120.Visibility = System.Windows.Visibility.Hidden;
                                   }
                                   if (radarRange >= 0.1 && radarRange < 0.2)
                                   {
                                       Range15.Visibility = System.Windows.Visibility.Visible;
                                       Range30.Visibility = System.Windows.Visibility.Hidden;
                                       Range60.Visibility = System.Windows.Visibility.Hidden;
                                       Range120.Visibility = System.Windows.Visibility.Hidden;
                                   }
                                   if (radarRange >= 0.2 && radarRange < 0.3)
                                   {
                                       Range15.Visibility = System.Windows.Visibility.Hidden;
                                       Range30.Visibility = System.Windows.Visibility.Visible;
                                       Range60.Visibility = System.Windows.Visibility.Hidden;
                                       Range120.Visibility = System.Windows.Visibility.Hidden;
                                   }
                                   if (radarRange >= 0.3 && radarRange < 0.4)
                                   {
                                       Range15.Visibility = System.Windows.Visibility.Hidden;
                                       Range30.Visibility = System.Windows.Visibility.Hidden;
                                       Range60.Visibility = System.Windows.Visibility.Visible;
                                       Range120.Visibility = System.Windows.Visibility.Hidden;
                                   }
                                   if (radarRange >= 0.4 && radarRange < 0.5)
                                   {
                                       Range15.Visibility = System.Windows.Visibility.Hidden;
                                       Range30.Visibility = System.Windows.Visibility.Hidden;
                                       Range60.Visibility = System.Windows.Visibility.Hidden;
                                       Range120.Visibility = System.Windows.Visibility.Visible;
                                   }
                               }

                               lheading = heading;
                               lcommandedCourse = commandedCourse;

                               lrwr3 = rwr3;
                               lrwr9 = rwr9;
                               lrwr15 = rwr15;
                               lrwr21 = rwr21;
                               lrwr27 = rwr27;
                               lrwr33 = rwr33;
                               lwarningFlag = warningFlag;
                               lradarRange = radarRange;
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

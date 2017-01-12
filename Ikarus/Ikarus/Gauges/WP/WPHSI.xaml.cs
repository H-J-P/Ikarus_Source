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
    /// Interaction logic for WP_HSI.xaml
    /// </summary>
    public partial class WPHSI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double heading = 0.0;
        double commandedCourseNeedle = 0.0;
        double commandedHeadingNeedle = 0.0;
        double bearingNeedle = 0.0;
        double headingWarningFlag = 0.0;
        double courseWarningFlag = 0.0;
        double glideslopeWarningFlag = 0.0;
        double rangeCounter100 = 0.0;
        double rangeCounter10 = 0.0;
        double rangeCounter1 = 0.0;
        double longitudinalDeviation = 0.0;
        double lateralDeviation = 0.0;
        double rangeUnavailableFlag = 0.0;
        double courseUnavailableFlag = 0.0;
        double heading100 = 0.0;
        double heading10 = 0.0;
        double heading1 = 0.0;

        double lheading = 0.0;
        double lcommandedCourseNeedle = 0.0;
        double lcommandedHeadingNeedle = 0.0;
        double lbearingNeedle = 0.0;
        double lheadingWarningFlag = 0.0;
        double lcourseWarningFlag = 0.0;
        double lglideslopeWarningFlag = 0.0;
        double lrangeCounter100 = 0.0;
        double lrangeCounter10 = 0.0;
        double lrangeCounter1 = 0.0;
        double llongitudinalDeviation = 0.0;
        double llateralDeviation = 0.0;
        double lrangeUnavailableFlag = 0.0;
        double lcourseUnavailableFlag = 0.0;
        double lheading100 = 0.0;
        double lheading10 = 0.0;
        double lheading1 = 0.0;

        String sHeading = "";

        RotateTransform rtHeading = new RotateTransform();
        RotateTransform rtCommandedCourseNeedle = new RotateTransform();
        RotateTransform rtCommandedHeadingNeedle = new RotateTransform();
        RotateTransform rtBearingNeedle = new RotateTransform();
        TranslateTransform ttRangeCounter100 = new TranslateTransform();
        TranslateTransform ttRangeCounter10 = new TranslateTransform();
        TranslateTransform ttRangeCounter1 = new TranslateTransform();
        TranslateTransform ttLongitudinalDeviation = new TranslateTransform();
        TranslateTransform ttLateralDeviation = new TranslateTransform();
        TranslateTransform ttHeading100 = new TranslateTransform();
        TranslateTransform ttHeading1 = new TranslateTransform();
        TranslateTransform ttHeading10 = new TranslateTransform();

        public WPHSI()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);
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
            return 206.0; // Width
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
                               if (vals.Length > 1) { commandedCourseNeedle = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { commandedHeadingNeedle = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { bearingNeedle = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { headingWarningFlag = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { courseWarningFlag = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { glideslopeWarningFlag = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { rangeCounter100 = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { rangeCounter10 = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }
                               if (vals.Length > 9) { rangeCounter1 = Convert.ToDouble(vals[9], CultureInfo.InvariantCulture); }
                               if (vals.Length > 10) { longitudinalDeviation = Convert.ToDouble(vals[10], CultureInfo.InvariantCulture); }
                               if (vals.Length > 11) { lateralDeviation = Convert.ToDouble(vals[11], CultureInfo.InvariantCulture); }
                               if (vals.Length > 12) { rangeUnavailableFlag = Convert.ToDouble(vals[12], CultureInfo.InvariantCulture); }
                               if (vals.Length > 13) { courseUnavailableFlag = Convert.ToDouble(vals[13], CultureInfo.InvariantCulture); }

                               if (lheading != heading)
                               {
                                   rtHeading.Angle = heading * -360;
                                   KA50_needleCOMP_HSI.RenderTransform = rtHeading;
                               }
                               if (lcommandedCourseNeedle != commandedCourseNeedle)
                               {
                                   rtCommandedCourseNeedle.Angle = commandedCourseNeedle * 360 + (heading * -360);
                                   KA50_needleDTA_HSI.RenderTransform = rtCommandedCourseNeedle;
                               }
                               if (lcommandedHeadingNeedle != commandedHeadingNeedle)
                               {
                                   rtCommandedHeadingNeedle.Angle = commandedHeadingNeedle * 360 + (heading * -360);
                                   DesiredHeadingIndex.RenderTransform = rtCommandedHeadingNeedle;
                               }
                               if (lbearingNeedle != bearingNeedle)
                               {
                                   rtBearingNeedle.Angle = bearingNeedle * 360;
                                   KA50_needleHDG_HSI.RenderTransform = rtBearingNeedle;
                               }

                               KA50_needleKC_HSI.Visibility = headingWarningFlag > 0.8 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               KA50_needleK_HSI.Visibility = courseWarningFlag > 0.8 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               KA50_needleG_HSI.Visibility = glideslopeWarningFlag > 0.8 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               if (lrangeCounter100 != rangeCounter100)
                               {
                                   ttRangeCounter100.Y = rangeCounter100 * -152;
                                   KM50_KMC_HSI.RenderTransform = ttRangeCounter100;
                               }
                               if (lrangeCounter10 != rangeCounter10)
                               {
                                   ttRangeCounter10.Y = rangeCounter10 * -152;
                                   KM50_KMX_HSI.RenderTransform = ttRangeCounter10;
                               }
                               if (lrangeCounter1 != rangeCounter1)
                               {
                                   ttRangeCounter1.Y = rangeCounter1 * -152;
                                   KM50_KMI_HSI.RenderTransform = ttRangeCounter1;
                               }
                               if (llongitudinalDeviation != longitudinalDeviation)
                               {
                                   ttLongitudinalDeviation.Y = longitudinalDeviation * -40;
                                   KA50_needleHOR_HSI.RenderTransform = ttLongitudinalDeviation;
                               }
                               if (llateralDeviation != lateralDeviation)
                               {
                                   ttLateralDeviation.X = lateralDeviation * 40;
                                   KA50_needleVERT_HSI.RenderTransform = ttLateralDeviation;
                               }
                               sHeading = Convert.ToInt16(360 * Convert.ToDouble(commandedCourseNeedle, CultureInfo.InvariantCulture)).ToString();

                               if (sHeading.Length == 0)
                                   sHeading = "000";
                               else if (sHeading.Length == 1)
                                   sHeading = "00" + sHeading;
                               else if (sHeading.Length == 2)
                                   sHeading = "0" + sHeading;

                               heading100 = Convert.ToDouble(sHeading[0].ToString(), CultureInfo.InvariantCulture);
                               heading10 = Convert.ToDouble(sHeading[1].ToString(), CultureInfo.InvariantCulture);
                               heading1 = Convert.ToDouble(sHeading[2].ToString(), CultureInfo.InvariantCulture);

                               if (lheading100 != heading100)
                               {
                                   ttHeading100.Y = heading100 * -16.5;
                                   KM50_DTAC_HSI.RenderTransform = ttHeading100;
                               }
                               if (lheading10 != heading10)
                               {
                                   ttHeading10.Y = heading10 * -16.5;
                                   KM50_DTAX_HSI.RenderTransform = ttHeading10;
                               }
                               if (lheading1 != heading1)
                               {
                                   ttHeading1.Y = heading1 * -16.5;
                                   KM50_DTAI_HSI.RenderTransform = ttHeading1;
                               }
                               lheading = heading;
                               lcommandedCourseNeedle = commandedCourseNeedle;
                               lcommandedHeadingNeedle = commandedHeadingNeedle;
                               lbearingNeedle = bearingNeedle;
                               lheadingWarningFlag = headingWarningFlag;
                               lcourseWarningFlag = courseWarningFlag;
                               lglideslopeWarningFlag = glideslopeWarningFlag;
                               lrangeCounter100 = rangeCounter100;
                               lrangeCounter10 = rangeCounter10;
                               lrangeCounter1 = rangeCounter1;
                               llongitudinalDeviation = longitudinalDeviation;
                               llateralDeviation = lateralDeviation;
                               lrangeUnavailableFlag = rangeUnavailableFlag;
                               lcourseUnavailableFlag = courseUnavailableFlag;
                               lheading100 = heading100;
                               lheading10 = heading10;
                               lheading1 = heading1;
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

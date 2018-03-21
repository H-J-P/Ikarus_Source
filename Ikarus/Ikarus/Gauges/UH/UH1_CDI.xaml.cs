using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for UH1_CDI.xaml
    /// </summary>
    public partial class UH1_CDI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double verticalBar = 0.0;
        double horizontalBar = 0.0;
        double toMarker = 0.0;
        double fromMarker = 0.0;
        double rotCourseCard = 0.0;
        double verticalOff = 0.0;
        double horizontalOff = 0.0;

        double lverticalBar = 0.0;
        double lhorizontalBar = 0.0;
        double ltoMarker = 0.0;
        double lfromMarker = 0.0;
        double lrotCourseCard = 0.0;
        double lverticalOff = 0.0;
        double lhorizontalOff = 0.0;

        RotateTransform rtVerticalBar = new RotateTransform();
        RotateTransform rtHorizontalBar = new RotateTransform();
        RotateTransform rtRotCourseCard = new RotateTransform();

        public UH1_CDI()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            VerticalOFF.Visibility = System.Windows.Visibility.Visible;
            HorisontalOFF.Visibility = System.Windows.Visibility.Visible;
            ToMarker.Visibility = System.Windows.Visibility.Visible;
            FromMarker.Visibility = System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { verticalBar = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { horizontalBar = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { toMarker = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { fromMarker = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { rotCourseCard = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { verticalOff = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { horizontalOff = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }

                               if (lverticalBar != verticalBar)
                               {
                                   rtVerticalBar.Angle = verticalBar * -35;
                                   VerticalBar.RenderTransform = rtVerticalBar;
                               }
                               if (lhorizontalBar != horizontalBar)
                               {
                                   rtHorizontalBar.Angle = horizontalBar * -38;
                                   HorisontalBar.RenderTransform = rtHorizontalBar;
                               }
                               ToMarker.Visibility = toMarker > 0.8 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               FromMarker.Visibility = fromMarker > 0.8 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               if (lrotCourseCard != rotCourseCard)
                               {
                                   rtRotCourseCard.Angle = rotCourseCard * 360;
                                   RotCourseCard.RenderTransform = rtRotCourseCard;
                               }
                               VerticalOFF.Visibility = verticalOff > 0.8 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               HorisontalOFF.Visibility = horizontalOff > 0.8 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lverticalBar = verticalBar;
                               lhorizontalBar = horizontalBar;
                               ltoMarker = toMarker;
                               lfromMarker = fromMarker;
                               lrotCourseCard = rotCourseCard;
                               lverticalOff = verticalOff;
                               lhorizontalOff = horizontalOff;
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

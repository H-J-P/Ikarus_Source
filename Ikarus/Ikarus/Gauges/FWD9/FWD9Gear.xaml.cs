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
    /// Interaction logic for FWD9Gear.xaml
    /// </summary>
    public partial class FWD9Gear : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double leftGearUp = 0.0;
        double leftGearDown = 0.0;
        double rightGearUp = 0.0;
        double rightGearDown = 0.0;

        double flapsUp = 0.0;
        double flapsStart = 0.0;
        double flapsDown = 0.0;

        public FWD9Gear()
        {
            InitializeComponent();

            L_GEAR_UP.Visibility = System.Windows.Visibility.Hidden;
            L_GEAR_DOWN.Visibility = System.Windows.Visibility.Hidden;
            R_GEAR_UP.Visibility = System.Windows.Visibility.Hidden;
            R_GEAR_DOWN.Visibility = System.Windows.Visibility.Hidden;

            FLAPS_UP.Visibility = System.Windows.Visibility.Hidden;
            FLAPS_START.Visibility = System.Windows.Visibility.Hidden;
            FLAPS_DOWN.Visibility = System.Windows.Visibility.Hidden;

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
            string[] vals = _output.Split(',');

        }

        public double GetSize()
        {
            return 130.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { leftGearUp = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { leftGearDown = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { rightGearUp = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { rightGearDown = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { flapsUp = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { flapsStart = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { flapsDown = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }

                               L_GEAR_UP.Visibility = (leftGearUp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               L_GEAR_DOWN.Visibility = (leftGearDown > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               R_GEAR_UP.Visibility = (rightGearUp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               R_GEAR_DOWN.Visibility = (rightGearDown > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               FLAPS_UP.Visibility = (flapsUp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               FLAPS_START.Visibility = (flapsStart > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               FLAPS_DOWN.Visibility = (flapsDown > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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

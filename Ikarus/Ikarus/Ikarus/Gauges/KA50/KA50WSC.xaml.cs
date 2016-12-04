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
    /// Interaction logic for KA50WSC.xaml
    /// </summary>
    public partial class KA50WSC : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double lampSta1WeapIsPresent = 0.0;
        double lampSta2WeapIsPresent = 0.0;
        double lampSta3WeapIsPresent = 0.0;
        double lampSta4WeapIsPresent = 0.0;
        double lampSta1WeapIsReady = 0.0;
        double lampSta2WeapIsReady = 0.0;
        double lampSta3WeapIsReady = 0.0;
        double lampSta4WeapIsReady = 0.0;

        public KA50WSC()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            KA50_1Pre_WSC.Visibility = System.Windows.Visibility.Visible;
            KA50_2Pre_WSC.Visibility = System.Windows.Visibility.Visible;
            KA50_3Pre_WSC.Visibility = System.Windows.Visibility.Visible;
            KA50_4Pre_WSC.Visibility = System.Windows.Visibility.Visible;
            KA50_1Rdy_WSC.Visibility = System.Windows.Visibility.Visible;
            KA50_2Rdy_WSC.Visibility = System.Windows.Visibility.Visible;
            KA50_3Rdy_WSC.Visibility = System.Windows.Visibility.Visible;
            KA50_4Rdy_WSC.Visibility = System.Windows.Visibility.Visible;

            StationType.Visibility = System.Windows.Visibility.Hidden;
            StationType.Text = "00";

            StationCount.Visibility = System.Windows.Visibility.Hidden;
            CannonAmmoCount.Visibility = System.Windows.Visibility.Hidden;

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
            return 212.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               StationType.Text = "";
                               StationCount.Text = "";
                               CannonAmmoCount.Text = "";

                               if (vals.Length > 0) { lampSta1WeapIsPresent = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { lampSta2WeapIsPresent = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { lampSta3WeapIsPresent = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { lampSta4WeapIsPresent = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { lampSta1WeapIsReady = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { lampSta2WeapIsReady = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { lampSta3WeapIsReady = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { lampSta4WeapIsReady = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { StationType.Text = vals[8]; }
                               if (vals.Length > 9) { StationCount.Text = vals[9]; }
                               if (vals.Length > 10) { CannonAmmoCount.Text = vals[10]; }

                               StationType.Visibility = System.Windows.Visibility.Visible;
                               StationCount.Visibility = System.Windows.Visibility.Visible;
                               CannonAmmoCount.Visibility = System.Windows.Visibility.Visible;

                               KA50_1Pre_WSC.Visibility = (lampSta1WeapIsPresent > 0.9) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
                               KA50_2Pre_WSC.Visibility = (lampSta2WeapIsPresent > 0.9) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
                               KA50_3Pre_WSC.Visibility = (lampSta3WeapIsPresent > 0.9) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
                               KA50_4Pre_WSC.Visibility = (lampSta4WeapIsPresent > 0.9) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
                               KA50_1Rdy_WSC.Visibility = (lampSta1WeapIsReady > 0.9) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
                               KA50_2Rdy_WSC.Visibility = (lampSta2WeapIsReady > 0.9) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
                               KA50_3Rdy_WSC.Visibility = (lampSta3WeapIsReady > 0.9) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
                               KA50_4Rdy_WSC.Visibility = (lampSta4WeapIsReady > 0.9) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
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

            movedByElement.MouseLeftButtonUp += (a, b) => isMousePressed = false;
            movedByElement.MouseLeave += (a, b) => isMousePressed = false;

            movedByElement.MouseMove += (a, b) =>
            {
                if (!isMousePressed || !MainWindow.editmode) return;

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                trUsercontrol.X += currentPoint.X - originalPoint.X;
                trUsercontrol.Y += currentPoint.Y - originalPoint.Y;
                moveThisElement.RenderTransform = trUsercontrol;

                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID);
            };
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}

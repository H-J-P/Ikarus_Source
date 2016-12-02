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
    /// Interaction logic for A10_ALT.xaml
    /// </summary>
    public partial class A10_ALT : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        RotateTransform rtalt100FP = new RotateTransform();
        TranslateTransform ttalt10000 = new TranslateTransform();
        TranslateTransform ttalt1000 = new TranslateTransform();
        TranslateTransform ttalt100 = new TranslateTransform();
        TranslateTransform ttpressure_0 = new TranslateTransform();
        TranslateTransform ttpressure_1 = new TranslateTransform();
        TranslateTransform ttpressure_2 = new TranslateTransform();
        TranslateTransform ttpressure_3 = new TranslateTransform();

        double alt100FP = 0.0;
        double alt10000 = 0.0;
        double alt1000 = 0.0;
        double alt100 = 0.0;
        double pressure_0 = 0.0;
        double pressure_1 = 0.0;
        double pressure_2 = 0.0;
        double pressure_3 = 0.0;
        double flag_pneu = 0.0;

        double lalt100FP = 0.0;
        double lalt10000 = 0.0;
        double lalt1000 = 0.0;
        double lalt100 = 0.0;
        double lpressure_0 = 0.0;
        double lpressure_1 = 0.0;
        double lpressure_2 = 0.0;
        double lpressure_3 = 0.0;

        public A10_ALT()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            Flagg_Elect.Visibility = System.Windows.Visibility.Visible;
            Flagg_Pneu.Visibility = System.Windows.Visibility.Hidden;
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
            return 255.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           vals = strData.Split(';');

                           try
                           {
                               if (vals.Length > 0) { alt100FP = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { alt10000 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { alt1000 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { alt100 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { pressure_0 = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { pressure_1 = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { pressure_2 = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { pressure_3 = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { flag_pneu = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }
                           }
                           catch { return; }

                           if (alt100FP != lalt100FP)
                           {
                               rtalt100FP.Angle = alt100FP * 360;
                               AltBar.RenderTransform = rtalt100FP;
                           }

                           if (alt10000 != lalt10000)
                           {
                               ttalt10000.Y = alt10000 * -390;
                               AltBar_10_000.RenderTransform = ttalt10000;
                           }

                           if (alt1000 != lalt1000)
                           {
                               ttalt1000.Y = alt1000 * -390;
                               AltBar_1_000.RenderTransform = ttalt1000;
                           }

                           if (alt100 != lalt100)
                           {
                               ttalt100.Y = alt100 * -390;
                               AltBar_100.RenderTransform = ttalt100;
                           }
                           if (pressure_0 != lpressure_0)
                           {
                               ttpressure_0.Y = pressure_0 * -180;
                               BasicAtmospherePressure_1.RenderTransform = ttpressure_0;
                           }

                           if (pressure_1 != lpressure_1)
                           {
                               ttpressure_1.Y = pressure_1 * -180;
                               BasicAtmospherePressure_10.RenderTransform = ttpressure_1;
                           }
                           if (pressure_2 != lpressure_2)
                           {
                               ttpressure_2.Y = pressure_2 * -180;
                               BasicAtmospherePressure_100.RenderTransform = ttpressure_2;
                           }
                           if (pressure_2 != lpressure_3)
                           {
                               ttpressure_3.Y = pressure_3 * -180;
                               BasicAtmospherePressure_1000.RenderTransform = ttpressure_3;
                           }

                           Flagg_Elect.Visibility = System.Windows.Visibility.Hidden;

                           lalt100FP = alt100FP;
                           lalt10000 = alt10000;
                           lalt1000 = alt1000;
                           lalt100 = alt100;
                           lpressure_0 = pressure_0;
                           lpressure_1 = pressure_1;
                           lpressure_2 = pressure_2;
                           lpressure_3 = pressure_3;
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

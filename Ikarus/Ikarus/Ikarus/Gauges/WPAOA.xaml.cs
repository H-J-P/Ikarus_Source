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
    /// Interaction logic for WPAOA.xaml
    /// </summary>
    public partial class WPAOA : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double aoa = 0.0;
        double gLoad = 0.0;
        double gLoadMax = 0.0;
        double gLoadMin = 0.0;
        double marker = 0.0;

        double laoa = 0.0;
        double lgLoad = 0.0;
        double lgLoadMax = 0.0;
        double lgLoadMin = 0.0;
        double lmarker = 0.0;

        RotateTransform rtgLoad = new RotateTransform();
        RotateTransform rtgLoadMax = new RotateTransform();
        RotateTransform rtgLoadMin = new RotateTransform();
        RotateTransform rtAoa = new RotateTransform();

        public WPAOA()
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
            string[] vals = _output.Split(',');

        }

        public double GetSize()
        {
            return 167.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { aoa = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { gLoad = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { gLoadMax = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { gLoadMin = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { marker = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               if (laoa != aoa)
                               {
                                   rtAoa.Angle = aoa * 130;
                                   AOA.RenderTransform = rtAoa;
                               }
                               if (lgLoad != gLoad)
                               {
                                   rtgLoad.Angle = gLoad * -120;
                                   GLoad.RenderTransform = rtgLoad;
                               }
                               if (lgLoadMax != gLoadMax)
                               {
                                   rtgLoadMax.Angle = gLoadMax * -120;
                                   GLmax.RenderTransform = rtgLoadMax;
                               }
                               if (lgLoadMin != gLoadMin)
                               {
                                   rtgLoadMin.Angle = gLoadMin * -120;
                                   GLmin.RenderTransform = rtgLoadMin;
                               }
                               Marks_1.Visibility = marker > 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Marks_2.Visibility = marker > 0.9 ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;

                               laoa = aoa;
                               lgLoad = gLoad;
                               lgLoadMax = gLoadMax;
                               lgLoadMin = gLoadMin;
                               lmarker = marker;
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

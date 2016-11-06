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
    /// Interaction logic for MI8_FuelQ.xaml
    /// </summary>
    public partial class MI8_FuelQ : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        const int valueScaleIndexUpper = 8;
        const int valueScaleIndexLower = 6;

        double fuelScaleUpper = 0.0;
        double fuelScaleLower = 0.0;

        double lfuelScaleUpper = 0.0;
        double lfuelScaleLower = 0.0;

        RotateTransform rtFuelScaleUpper = new RotateTransform();
        RotateTransform rtFuelScaleLower = new RotateTransform();

        public MI8_FuelQ()
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
            return 170.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { fuelScaleUpper = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { fuelScaleLower = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               double[] valueScale = new double[valueScaleIndexUpper] { 0.0, 0.131, 0.244, 0.414, 0.647, 0.705, 0.9, 1.0 };
                               double[] degreeDial = new double[valueScaleIndexUpper] { 0, 19, 41, 73, 120, 144, 166, 180 };

                               if (lfuelScaleUpper != fuelScaleUpper)
                               {
                                   for (int n = 0; n < (valueScaleIndexUpper - 1); n++)
                                   {
                                       if (fuelScaleUpper >= valueScale[n] && fuelScaleUpper <= valueScale[n + 1])
                                       {
                                           rtFuelScaleUpper.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (fuelScaleUpper - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   FuelScaleUpper.RenderTransform = rtFuelScaleUpper;
                               }
                               if (lfuelScaleLower != fuelScaleLower)
                               {
                                   valueScale = new double[valueScaleIndexLower] { 0.0, 0.165, 0.283, 0.393, 0.618, 1.0 };
                                   degreeDial = new double[valueScaleIndexLower] { 0, 25, 49, 70, 116, 180 };

                                   for (int n = 0; n < (valueScaleIndexLower - 1); n++)
                                   {
                                       if (fuelScaleLower >= valueScale[n] && fuelScaleLower <= valueScale[n + 1])
                                       {
                                           rtFuelScaleLower.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (fuelScaleLower - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   FuelScaleLower.RenderTransform = rtFuelScaleLower;
                               }
                               lfuelScaleUpper = fuelScaleUpper;
                               lfuelScaleLower = fuelScaleLower;
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

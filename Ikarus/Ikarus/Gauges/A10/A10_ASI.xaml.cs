using System;
using System.Data;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for A10_ASI.xaml
    /// </summary>
    public partial class A10_ASI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        RotateTransform rtairspeedNeedle = new RotateTransform();
        RotateTransform rtAirspeedMax = new RotateTransform();
        TranslateTransform ttAirspeedDial = new TranslateTransform();

        double airspeedNeedle = 0.0;
        double airspeedDial = 0.0;
        double alt10000 = 0.0;
        double alt1000 = 0.0;
        double alt100 = 0.0;

        double lairspeedNeedle = 0.0;
        double lairspeedDial = 0.0;
        double lalt10000 = 0.0;
        double lalt1000 = 0.0;
        double lalt100 = 0.0;

        public A10_ASI()
        {
            InitializeComponent();
            if (MainWindow.editmode) MakeDraggable(this, this);
            IASMAX.Visibility = System.Windows.Visibility.Hidden;
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
            string[] vals = _input.Split(',');

            if (vals.Length < 3) return;

            valueScale = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                valueScale[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
            valueScaleIndex = vals.Length;
        }

        public void SetOutput(string _output)
        {
            string[] vals = _output.Split(',');

            if (vals.Length < 3) return;

            degreeDial = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                degreeDial[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
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

                           double altitude = 0.0;
                           double airSpeedMax = 0.0;

                           try
                           {
                               if (vals.Length > 0) { airspeedNeedle = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { airspeedDial = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { alt10000 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { alt1000 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { alt100 = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               altitude = (Math.Round(alt10000, 1) * 10000 + Math.Round(alt1000, 1) * 1000 + Math.Round(alt100, 1) * 100) * 10;

                               if (airspeedNeedle < 0.0) { airspeedNeedle = 0.0; }

                               if (lairspeedNeedle != airspeedNeedle)
                               {
                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (airspeedNeedle > valueScale[n] && airspeedNeedle <= valueScale[n + 1])
                                       {
                                           rtairspeedNeedle.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (airspeedNeedle - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   IAS.RenderTransform = rtairspeedNeedle;
                               }
                           }
                           catch { return; }

                           // http://www.arndt-bruenner.de/mathe/9/geradedurchzweipunkte.htm
                           // AirspeedNeedle    altitude    airspeedMax
                           //------------------------------------------
                           // 500             = 0               0.906   f(x) = -19/1250000·x + 453/500
                           // 450             = 5000            0.811   f(x) = -399/27500000·x + 9719/11000
                           // 400             = 11000           0.719   f(x) = -307/21500000·x + 37671/43000
                           // 350             = 17500           0.620   f(x) = -191/12500000·x + 4437/5000
                           // 300             = 25000           0.517
                           // 250             = 32500           0.412

                           try
                           {
                               airSpeedMax = Convert.ToDouble(((-191.0 * altitude / 12500000.0) + (4437.0 / 5000.0)).ToString().Replace(",", "."), CultureInfo.InvariantCulture);

                               for (int n = 0; n < valueScaleIndex - 1; n++)
                               {
                                   if (airSpeedMax > valueScale[n] && airSpeedMax <= valueScale[n + 1])
                                   {
                                       rtAirspeedMax.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (airSpeedMax - valueScale[n]) + degreeDial[n];
                                       break;
                                   }
                               }
                               IAS_max.RenderTransform = rtAirspeedMax;

                               if (lairspeedDial != airspeedDial)
                               {
                                   ttAirspeedDial.X = (airspeedDial * -315) - 15;
                                   IAS_10.RenderTransform = ttAirspeedDial;
                               }
                           }
                           catch { return; }

                           lairspeedNeedle = airspeedNeedle;
                           lairspeedDial = airspeedDial;
                           lalt10000 = alt10000;
                           lalt1000 = alt1000;
                           lalt100 = alt100;
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

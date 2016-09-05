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
    /// Interaction logic for MI8_AOA.xaml
    /// </summary>
    public partial class MI8_RotorPitch : UserControl, I_Ikarus
    {
        private string dataImportID = "";

        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        public MI8_RotorPitch()
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
            string[] vals = _input.Split(',');

            if (vals.Length < 3) { return; }

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

            if (vals.Length < 3) { return; }

            degreeDial = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                degreeDial[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
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
                               double rotorPitch = 0.0;

                               if (vals.Length > 0) { rotorPitch = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               RotateTransform rtrotorPitch = new RotateTransform();

                               for (int n = 0; n < (valueScaleIndex - 1); n++)
                               {
                                   if (rotorPitch >= valueScale[n] && rotorPitch <= valueScale[n + 1])
                                   {
                                       rtrotorPitch.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (rotorPitch - valueScale[n]) + degreeDial[n];
                                       break;
                                   }
                               }
                               RotorPitch.RenderTransform = rtrotorPitch;
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

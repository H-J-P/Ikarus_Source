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
    /// Interaction logic for MIG21_Mach.xaml
    /// </summary>
    public partial class MIG21_Mach : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double tas = 0.0;
        double mach = 0.0;
        double ltas = 0.0;
        double lmach = 0.0;

        RotateTransform rttas = new RotateTransform();
        RotateTransform rtmach = new RotateTransform();

        public MIG21_Mach()
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
                           const int valueScaleIndex = 6;

                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { tas = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { mach = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               // 101   TAS_indicator.input =                    { 0.0,  167,   278,  417,  555,  833 }
                               double[] valueScale = new double[valueScaleIndex] { 0.0, 0.20, 0.309, 0.49, 0.67, 1.00 };
                               double[] degreeDial = new double[valueScaleIndex] { 0, 13, 69, 180, 208, 348 };

                               if (ltas != tas)
                               {
                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (tas > valueScale[n] && tas <= valueScale[n + 1])
                                       {
                                           rttas.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (tas - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   TAS_indicator.RenderTransform = rttas;
                               }
                               if (lmach != mach)
                               {
                                   // 102   M_indicator.input             = { 0.0,   0.6,   1.0,  1.8,  2.0, 3.0 }
                                   valueScale = new double[valueScaleIndex] { 0.0, 0.202, 0.312, 0.6, 0.66, 1.00 };
                                   degreeDial = new double[valueScaleIndex] { 0, 13, 69, 180, 208, 348 };

                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (mach > valueScale[n] && mach <= valueScale[n + 1])
                                       {
                                           rtmach.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (mach - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   M_indicator.RenderTransform = rtmach;
                               }
                               ltas = tas;
                               lmach = mach;
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

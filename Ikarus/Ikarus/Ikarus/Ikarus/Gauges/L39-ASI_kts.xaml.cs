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
    /// Interaction logic for L39_ASI_kts.xaml
    /// </summary>
    public partial class L39_ASI_kts : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        RotateTransform rtIAS = new RotateTransform();
        RotateTransform rtTAS = new RotateTransform();
        RotateTransform rtMach = new RotateTransform();

        double ias = 0.0;
        double tas = 0.0;
        double mach = 0.0;

        double lias = 0.0;
        double ltas = 0.0;
        double lmach = 0.0;

        public L39_ASI_kts()
        {
            InitializeComponent();
            if (MainWindow.editmode) MakeDraggable(this, this);

            rtIAS.Angle = 10;
            IAS.RenderTransform = rtIAS;

            rtTAS.Angle = 10;
            TAS.RenderTransform = rtTAS;

            rtMach.Angle = 90; // +rtIAS.Angle;
            MACH.RenderTransform = rtMach;
        }
        public string GetID() { return dataImportID; }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
        }

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
            return 170.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           vals = strData.Split(';');

                           // IAS / TAS {0.0, 1.0}                           { 0.0, 100.0, 150.0, 200.0, 300.0, 400.0, 500.0, 600.0, 700.0, 800.0, 1200.0 }
                           //Double[] valueScale = new double[valueScaleIndex] { 0.0,  0.08, 0.186, 0.296, 0.436,  0.55, 0.635, 0.705, 0.765, 0.824, 1.0 };
                           //Double[] degreeDial = new double[valueScaleIndex] {  10,  29.5,    69, 108.5,   157,   197,   225,   251, 272.5,   294, 360 };

                           try
                           {
                               if (vals.Length > 0) { ias = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { tas = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { mach = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (ias < 0.0) ias = 0.0;
                               if (tas < 0.0) tas = 0.0;
                               if (mach < 0.0) mach = 0.0;

                               if (lias != ias)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (ias >= valueScale[n] && ias <= valueScale[n + 1])
                                       {
                                           rtIAS.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (ias - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   IAS.RenderTransform = rtIAS;
                               }

                               if (ltas != tas)
                               {
                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (tas >= valueScale[n] && tas <= valueScale[n + 1])
                                       {
                                           rtTAS.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (tas - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   TAS.RenderTransform = rtTAS;
                               }

                               if (mach == 0.0)
                               {
                                   rtMach.Angle = 90;
                                   MACH.RenderTransform = rtMach;
                               }

                               if (lmach != mach && mach > 0.4)
                               {
                                   mach = mach - 0.4;
                                   rtMach.Angle = (mach * - 220) + 90;
                                   MACH.RenderTransform = rtMach;
                               }
                               lias = ias;
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

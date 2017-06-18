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
    /// Interaction logic for L39_ACC.xaml
    /// </summary>
    public partial class L39_ACC : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        private static double range = (225 + 113);
        private double acc = 0.0;
        private double minG = 0.0;
        private double maxG = 0.0;

        private double lacc = 0.0;
        private double lminG = 0.0;
        private double lmaxG = 0.0;

        private RotateTransform rtAcc = new RotateTransform();
        private RotateTransform rtMinG = new RotateTransform();
        private RotateTransform rtMaxG = new RotateTransform();

        public L39_ACC()
        {
            InitializeComponent();
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;
            helper = new GaugesHelper(dataImportID, windowID, "Instruments");
            if (MainWindow.editmode) { helper.MakeDraggable(this, this); }
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
                               // AccelerationMin.input = { -5.0,	1.0}
                               // AccelerationMin.output = { 0.31,	0.695}

                               vals = strData.Split(';');

                               if (vals.Length > 0) { acc = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { minG = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { maxG = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (acc < 0.0) acc = 0.0;
                               if (maxG < 0.0) maxG = 0.0;
                               if (minG < 0.31) minG = 0.31;
                               if (minG > 0.695) minG = 0.695;

                               minG = (minG - 0.31) / (0.695 - 0.31);

                               if (lacc != acc)
                               {
                                   rtAcc.Angle = acc * range - 113;
                                   ACCELEROMETER.RenderTransform = rtAcc;
                               }
                               if (lminG != minG)
                               {
                                   rtMinG.Angle = ((minG * (112 + 23)) - 112);
                                   MIN_G_needle.RenderTransform = rtMinG;
                               }
                               if (lmaxG != maxG)
                               {
                                   rtMaxG.Angle = maxG * range - 113;
                                   MAX_G_needle.RenderTransform = rtMaxG;
                               }
                               lacc = acc;
                               lminG = minG;
                               lmaxG = maxG;
                           }
                           catch { return; }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}

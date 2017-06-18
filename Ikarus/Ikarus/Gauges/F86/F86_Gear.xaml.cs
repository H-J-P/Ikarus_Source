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
    /// Interaction logic for F86_Gear.xaml
    /// </summary>
    public partial class F86_Gear : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        public F86_Gear()
        {
            InitializeComponent();

            Left_Locked.Visibility = System.Windows.Visibility.Hidden;
            Right_Locked.Visibility = System.Windows.Visibility.Hidden;
            Nose_Locked.Visibility = System.Windows.Visibility.Hidden;

            Left_UP.Visibility = System.Windows.Visibility.Hidden;
            Right_UP.Visibility = System.Windows.Visibility.Hidden;
            Nose_UP.Visibility = System.Windows.Visibility.Hidden;

            BackgroundLeft.Visibility = System.Windows.Visibility.Hidden;
            BackgroundRight.Visibility = System.Windows.Visibility.Hidden;
            BackgroundNose.Visibility = System.Windows.Visibility.Hidden;
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

                               double leftGear = 0.0;
                               double rightGear = 0.0;
                               double noseGear = 0.0;

                               if (vals.Length > 0) { leftGear = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { rightGear = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { noseGear = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               Left_Locked.Visibility = (leftGear < 0.2) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Right_Locked.Visibility = (rightGear < 0.2) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Nose_Locked.Visibility = (noseGear < 0.2) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               Left_UP.Visibility = (leftGear > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Right_UP.Visibility = (rightGear > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Nose_UP.Visibility = (noseGear > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               BackgroundLeft.Visibility = (leftGear < 0.2 || leftGear > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               BackgroundRight.Visibility = (rightGear < 0.2 || rightGear > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               BackgroundNose.Visibility = (noseGear < 0.2 || noseGear > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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

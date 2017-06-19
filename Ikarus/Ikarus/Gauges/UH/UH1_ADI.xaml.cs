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
    /// Interaction logic for USADI.xaml
    /// </summary>
    public partial class UH1_ADI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double bank = 0.0;
        double pitch = 0.0;
        double flagOff = 0.0;

        double lbank = 0.0;
        double lpitch = 0.0;
        double lFlagOff = 0.0;

        TransformGroup grp = new TransformGroup();
        RotateTransform rt = new RotateTransform();
        TranslateTransform tt = new TranslateTransform();

        public UH1_ADI()
        {
            InitializeComponent();

            Flagg_off.Visibility = System.Windows.Visibility.Visible;
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
            return 314; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { bank = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { pitch = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { flagOff = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (lbank != bank || lpitch != pitch)
                               {
                                   grp = new TransformGroup();
                                   rt = new RotateTransform();
                                   tt = new TranslateTransform();

                                   tt.Y = pitch * -270;
                                   rt.Angle = bank * 180;
                                   grp.Children.Add(tt);
                                   grp.Children.Add(rt);

                                   Pitch.RenderTransform = grp;
                                   Bank.RenderTransform = rt;
                               }
                               Flagg_off.Visibility = (flagOff > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               lbank = bank;
                               lpitch = pitch;
                               lFlagOff = flagOff;
                           }
                           catch { return; };
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}

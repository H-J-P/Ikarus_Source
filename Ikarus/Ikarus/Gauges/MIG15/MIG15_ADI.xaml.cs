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
    /// Interaction logic for MIG15_ADI.xaml
    /// </summary>
    public partial class MIG15_ADI : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double roll = 0.0;
        double pitch = 0.0;
        double failureFlag = 0.0;
        double sideslip = 0.0;
        double turn = 0.0;
        double horizon = 0.0;

        double lroll = 0.0;
        double lpitch = 0.0;
        double lfailureFlag = 0.0;
        double lsideslip = 0.0;
        double lturn = 0.0;
        double lhorizon = 0.0;

        TransformGroup grp = new TransformGroup();
        RotateTransform rt = new RotateTransform();
        TranslateTransform tt = new TranslateTransform();

        TranslateTransform ttPitch = new TranslateTransform();
        TranslateTransform ttPitch2 = new TranslateTransform();
        RotateTransform rtSideslip = new RotateTransform();
        RotateTransform rtTurn = new RotateTransform();
        TranslateTransform ttHorizont = new TranslateTransform();

        public MIG15_ADI()
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
                               vals = strData.Split(';');

                               if (vals.Length > 0) { roll = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { pitch = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { sideslip = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { failureFlag = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { turn = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { horizon = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }

                               if (lroll != roll || lpitch != pitch)
                               {
                                   grp = new TransformGroup();
                                   rt = new RotateTransform();
                                   tt = new TranslateTransform();

                                   tt.Y = pitch * 270;
                                   if (tt.Y > 60) { tt.Y = 60; }
                                   if (tt.Y < -60) { tt.Y = -60; }
                                   rt.Angle = roll * 180;
                                   grp.Children.Add(rt);
                                   grp.Children.Add(tt);

                                   AGK_47B_roll.RenderTransform = grp;
                               }

                               if (lpitch != pitch)
                               {
                                   ttPitch.Y = pitch * 270;
                                   AGK_47B_pitch.RenderTransform = ttPitch;

                                   ttPitch2.Y = pitch * 270;
                                   if (ttPitch2.Y > 60) { ttPitch2.Y = 60; }
                                   if (ttPitch2.Y < -60) { ttPitch2.Y = -60; }
                                   AGK_47B_pitch_2.RenderTransform = ttPitch2;
                               }
                               if (lsideslip != sideslip)
                               {
                                   rtSideslip.Angle = sideslip * 12;
                                   AGK_47B_sideslip.RenderTransform = rtSideslip;
                               }

                               AGK_47B_failure_flag.Visibility = (failureFlag > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               if (lturn != turn)
                               {
                                   rtTurn.Angle = turn * -10;
                                   AGK_47B_turn.RenderTransform = rtTurn;
                               }
                               if (lhorizon != horizon)
                               {
                                   ttHorizont.Y = horizon * -20;
                                   AGK_47B_horizon.RenderTransform = ttHorizont;
                               }
                               lroll = roll;
                               lpitch = pitch;
                               lfailureFlag = failureFlag;
                               lsideslip = sideslip;
                               lturn = turn;
                               lhorizon = horizon;
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

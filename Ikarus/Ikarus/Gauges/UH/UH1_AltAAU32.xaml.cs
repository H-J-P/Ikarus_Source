using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for UH1_AltAAU32.xaml
    /// </summary>
    public partial class UH1_AltAAU32 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double altPointer = 0.0;
        double alt10000 = 0.0;
        double alt1000 = 0.0;
        double alt100 = 0.0;
        double pressure1000 = 0.0;
        double pressure_1000 = 0.0;
        double pressure0100 = 0.0;
        double pressure0010 = 0.0;
        double pressure0001 = 0.0;
        double offFlag = 0.0;

        double laltPointer = 0.0;
        double lalt10000 = 0.0;
        double lalt1000 = 0.0;
        double lalt100 = 0.0;
        double lpressure1000 = 0.3;
        double lpressure0010 = 0.0;
        double lpressure0001 = 0.0;
        double loffFlag = 0.0;

        RotateTransform rtAltPointer = new RotateTransform();
        TranslateTransform ttalt10000 = new TranslateTransform();
        TranslateTransform ttalt1000 = new TranslateTransform();
        TranslateTransform ttalt100 = new TranslateTransform();
        TranslateTransform ttPressure1000 = new TranslateTransform();
        TranslateTransform ttPressure0100 = new TranslateTransform();
        TranslateTransform ttPressure0010 = new TranslateTransform();
        TranslateTransform ttPressure0001 = new TranslateTransform();

        string pressure_1000_100 = "";

        public UH1_AltAAU32()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

            CodeOff_flag.Visibility = System.Windows.Visibility.Visible;
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;

            helper = new GaugesHelper(dataImportID, windowID, "Instruments");
            helper.LoadBmaps(Frame, Light);

            SwitchLight(false);

            if (MainWindow.editmode) { helper.MakeDraggable(this, this); }
        }

        public string GetID() { return dataImportID; }

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
            return Frame.Width;
        }

        public double GetSizeY()
        {
            return Frame.Height;
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { altPointer = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { alt10000 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { alt1000 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { alt100 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { pressure1000 = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { pressure0010 = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { pressure0001 = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { offFlag = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }

                               if (alt10000 < 0.0) { alt10000 = 0.0; }
                               if (alt1000 < 0.0) { alt1000 = 0.0; }
                               if (alt100 < 0.0) { alt100 = 0.0; }

                               if (pressure1000 < 0.0) { pressure1000 = 0.0; }
                               if (pressure1000 > 0.3) { pressure1000 = 0.3; }
                               if (pressure0010 < 0.0) { pressure0010 = 0.0; }
                               if (pressure0001 < 0.0) { pressure0001 = 0.0; }

                               if (laltPointer != altPointer)
                               {
                                   rtAltPointer.Angle = altPointer * 360;
                                   Pointer.RenderTransform = rtAltPointer;
                               }
                               if (lalt10000 != alt10000)
                               {
                                   ttalt10000.Y = alt10000 * -388;
                                   Alt1AAU_10000_footCount.RenderTransform = ttalt10000;
                               }
                               if (lalt1000 != alt1000)
                               {
                                   ttalt1000.Y = alt1000 * -388;
                                   Alt1AAU_1000_footCount.RenderTransform = ttalt1000;
                               }
                               if (lalt100 != alt100)
                               {
                                   ttalt100.Y = alt100 * -388;
                                   Alt1AAU_100_footCount.RenderTransform = ttalt100;
                               }
                               if (lpressure1000 != pressure1000)
                               {
                                   // 0.0 = 28, 0.1 = 29, 0.2 = 30, 0.3 = 31
                                   if (pressure1000 == 0.0) { pressure_1000_100 = "28"; }
                                   else if (pressure1000 == 0.1) { pressure_1000_100 = "29"; }
                                   else if (pressure1000 == 0.2) { pressure_1000_100 = "30"; }
                                   else if (pressure1000 == 0.3) { pressure_1000_100 = "31"; }
                                   else pressure_1000_100 = "29";

                                   pressure_1000 = Convert.ToDouble(pressure_1000_100[0].ToString(), CultureInfo.InvariantCulture);
                                   pressure0100 = Convert.ToDouble(pressure_1000_100[1].ToString(), CultureInfo.InvariantCulture);

                                   ttPressure1000.Y = pressure_1000 * -18.0;
                                   AAU_32_Drum_Counter_1000.RenderTransform = ttPressure1000;

                                   ttPressure0100.Y = pressure0100 * -18.0;
                                   AAU_32_Drum_Counter_0100.RenderTransform = ttPressure0100;
                               }
                               if (lpressure0010 != pressure0010)
                               {
                                   ttPressure0010.Y = pressure0010 * -180;
                                   AAU_32_Drum_Counter_0010.RenderTransform = ttPressure0010;
                               }
                               if (lpressure0001 != pressure0001)
                               {
                                   ttPressure0001.Y = pressure0001 * -180;
                                   AAU_32_Drum_Counter_0001.RenderTransform = ttPressure0001;
                               }
                               CodeOff_flag.Visibility = offFlag > 0.8 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               laltPointer = altPointer;
                               lalt10000 = alt10000;
                               lalt1000 = alt1000;
                               lalt100 = alt100;
                               lpressure1000 = pressure1000;
                               lpressure0010 = pressure0010;
                               lpressure0001 = pressure0001;
                               loffFlag = offFlag;
                           }
                           catch (Exception e) { ImportExport.LogMessage(GetType().Name + " got data and failed with exception: " + e.ToString()); }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}

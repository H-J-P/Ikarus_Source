using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für MIG_29_FuelQ_3.xaml
    /// </summary>
    public partial class MIG29FuelQ : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double totalFuel_0_5 = 0.0;
        double totalFuel_5_8 = 0.0;
        double light1 = 0.0;
        double light3 = 0.0;
        double light_KP = 0.0;
        double light_NK = 0.0;
        double light_NO = 0.0;

        double range = 0.0;
        double range_x = 0.0;
        double range_c = 0.0;
        double range_m = 0.0;
        string sRange = "";

        double ltotalFuel_0_5 = 0.0;
        double ltotalFuel_5_8 = 0.0;
        double lrange = 0.0;

        TranslateTransform ttTotalFuel_0_5 = new TranslateTransform();
        TranslateTransform ttTotalFuel_5_8 = new TranslateTransform();
        TranslateTransform ttRange_X = new TranslateTransform();
        TranslateTransform ttRange_C = new TranslateTransform();
        TranslateTransform ttRange_M = new TranslateTransform();

        public MIG29FuelQ()
        {
            InitializeComponent();

            Light_1.Visibility = System.Windows.Visibility.Hidden;
            Light_3.Visibility = System.Windows.Visibility.Hidden;
            Light_KP.Visibility = System.Windows.Visibility.Hidden;
            Light_NK.Visibility = System.Windows.Visibility.Hidden;
            Light_NO.Visibility = System.Windows.Visibility.Hidden;

            ttTotalFuel_0_5.Y = 181 - (totalFuel_0_5 * 181);
            TotalFuel_0_5.RenderTransform = ttTotalFuel_0_5;

            ttTotalFuel_5_8.Y = 96 - (totalFuel_5_8 * 96);
            TotalFuel_5_8.RenderTransform = ttTotalFuel_5_8;
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

                               if (vals.Length > 0) { totalFuel_0_5 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { totalFuel_5_8 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { light1 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { light3 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { light_KP = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { light_NK = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { light_NO = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { range = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }

                               Light_1.Visibility = (light1 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Light_3.Visibility = (light3 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Light_KP.Visibility = (light_KP > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Light_NK.Visibility = (light_NK > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Light_NO.Visibility = (light_NO > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               if (totalFuel_0_5 < 0) totalFuel_0_5 = 0;
                               if (totalFuel_5_8 < 0) totalFuel_5_8 = 0;

                               if (ltotalFuel_0_5 != totalFuel_0_5)
                               {
                                   ttTotalFuel_0_5.Y = 181 - (totalFuel_0_5 * 181);
                                   TotalFuel_0_5.RenderTransform = ttTotalFuel_0_5;
                               }
                               if (ltotalFuel_5_8 != totalFuel_5_8)
                               {
                                   ttTotalFuel_5_8.Y = 96 - (totalFuel_5_8 * 96);
                                   TotalFuel_5_8.RenderTransform = ttTotalFuel_5_8;
                               }

                               if (lrange != range)
                               {
                                   sRange = Convert.ToInt16(range).ToString();

                                   if (sRange.Length == 0) { sRange = "000"; }
                                   else if (sRange.Length == 1) { sRange = "00" + sRange; }
                                   else if (sRange.Length == 2) { sRange = "0" + sRange; }

                                   range_m = Convert.ToDouble(sRange[0].ToString(), CultureInfo.InvariantCulture);
                                   range_c = Convert.ToDouble(sRange[1].ToString(), CultureInfo.InvariantCulture);
                                   range_x = Convert.ToDouble(sRange[2].ToString(), CultureInfo.InvariantCulture);

                                   ttRange_X.Y = range_x * -17.3;
                                   Range_X.RenderTransform = ttRange_X;

                                   ttRange_C.Y = range_c * -17.3;
                                   Range_C.RenderTransform = ttRange_C;

                                   ttRange_M.Y = range_m * -17.3;
                                   Range_M.RenderTransform = ttRange_M;
                               }
                               ltotalFuel_0_5 = totalFuel_0_5;
                               ltotalFuel_5_8 = totalFuel_5_8;
                               lrange = range;
                           }
                           catch (Exception e) { ImportExport.LogMessage(GetType().Name + " got data '" + strData + "' and failed with exception: " + e.ToString()); }
                       }));
        }
        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}

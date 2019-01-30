using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für MIG_29_FuelQ_3.xaml
    /// </summary>
    public partial class MIG29FuelQV3 : UserControl
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


        public MIG29FuelQV3()
        {
            InitializeComponent();

            Light_1.Visibility = System.Windows.Visibility.Hidden;
            Light_3.Visibility = System.Windows.Visibility.Hidden;
            Light_KP.Visibility = System.Windows.Visibility.Hidden;
            Light_NK.Visibility = System.Windows.Visibility.Hidden;
            Light_NO.Visibility = System.Windows.Visibility.Hidden;
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

                               Light_1.Visibility = (light1 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Light_3.Visibility = (light3 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Light_KP.Visibility = (light_KP > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Light_NK.Visibility = (light_NK > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Light_NO.Visibility = (light_NO > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               if (totalFuel_0_5 < 0) totalFuel_0_5 = 0;
                               if (totalFuel_5_8 < 0) totalFuel_5_8 = 0;

                               TotalFuel_0_5.Height = totalFuel_0_5 * 181;
                               TotalFuel_5_8.Height = totalFuel_5_8 * 92;

                               //string sTotalFuel = Convert.ToInt16(totalFuel).ToString();

                               //if (sTotalFuel.Length == 0) { sTotalFuel = "000"; }
                               //else if (sTotalFuel.Length == 1) { sTotalFuel = "00" + sTotalFuel; }
                               //else if (sTotalFuel.Length == 2) { sTotalFuel = "0" + sTotalFuel; }

                               //double totalFuel_M = Convert.ToDouble(sTotalFuel[0].ToString(), CultureInfo.InvariantCulture);
                               //double totalFuel_C = Convert.ToDouble(sTotalFuel[1].ToString(), CultureInfo.InvariantCulture);
                               //double totalFuel_X = Convert.ToDouble(sTotalFuel[2].ToString(), CultureInfo.InvariantCulture);

                               //TranslateTransform ttTotalFuel_X = new TranslateTransform();
                               //TranslateTransform ttTotalFuel_C = new TranslateTransform();
                               //TranslateTransform ttTotalFuel_M = new TranslateTransform();

                               //ttTotalFuel_X.Y = totalFuel_X * -152;
                               //TotalFuel_X.RenderTransform = ttTotalFuel_X;

                               //ttTotalFuel_C.Y = totalFuel_C * -152;
                               //TotalFuel_C.RenderTransform = ttTotalFuel_C;

                               //ttTotalFuel_M.Y = totalFuel_M * -152;
                               //TotalFuel_M.RenderTransform = ttTotalFuel_M;
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

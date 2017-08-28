using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for WPALTlate.xaml
    /// </summary>
    public partial class WPALTlate : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double altBar_M = 0.0;
        double altBar_C = 0.0;
        double baroPressure = 0.0;
        double altKm = 0.0;

        double laltBar_M = 0.0;
        double laltBar_C = 0.0;
        double lbaroPressure = 0.0;
        double laltKm = 0.0;

        RotateTransform rtAltBar_M = new RotateTransform();
        RotateTransform rtAltBar_C = new RotateTransform();
        TranslateTransform ttBaroPressure_I = new TranslateTransform();
        TranslateTransform ttBaroPressure_X = new TranslateTransform();
        TranslateTransform ttBaroPressure_C = new TranslateTransform();
        TranslateTransform ttBaroPressure_M = new TranslateTransform();
        TranslateTransform ttAltKM01 = new TranslateTransform();
        TranslateTransform ttAltKM10 = new TranslateTransform();

        public WPALTlate()
        {
            InitializeComponent();

            Flagg_off_red.Visibility = System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { altBar_M = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { altBar_C = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { baroPressure = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { altKm = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (laltBar_M != altBar_M)
                               {
                                   rtAltBar_M.Angle = altBar_M * 360;
                                   AltBar_M.RenderTransform = rtAltBar_M;
                               }
                               if (laltBar_C != altBar_C)
                               {
                                   rtAltBar_C.Angle = altBar_C * 360;
                                   AltBar_C.RenderTransform = rtAltBar_C;
                               }

                               if (lbaroPressure != baroPressure)
                               {
                                   String sBaroPressure = Convert.ToInt16(baroPressure).ToString();

                                   if (sBaroPressure.Length == 0) { sBaroPressure = "0000"; }
                                   if (sBaroPressure.Length == 1) { sBaroPressure = "000"; }
                                   else if (sBaroPressure.Length == 2) { sBaroPressure = "00" + sBaroPressure; }
                                   else if (sBaroPressure.Length == 3) { sBaroPressure = "0" + sBaroPressure; }

                                   double baroPressure_M = Convert.ToDouble(sBaroPressure[0].ToString(), CultureInfo.InvariantCulture);
                                   double baroPressure_C = Convert.ToDouble(sBaroPressure[1].ToString(), CultureInfo.InvariantCulture);
                                   double baroPressure_X = Convert.ToDouble(sBaroPressure[2].ToString(), CultureInfo.InvariantCulture);
                                   double baroPressure_I = Convert.ToDouble(sBaroPressure[3].ToString(), CultureInfo.InvariantCulture);

                                   ttBaroPressure_I.Y = baroPressure_I * -13;
                                   BasicAtmospherePressure_I.RenderTransform = ttBaroPressure_I;

                                   ttBaroPressure_X.Y = baroPressure_X * -13;
                                   BasicAtmospherePressure_X.RenderTransform = ttBaroPressure_X;

                                   ttBaroPressure_C.Y = baroPressure_C * -13;
                                   BasicAtmospherePressure_C.RenderTransform = ttBaroPressure_C;

                                   ttBaroPressure_M.Y = baroPressure_M * -13;
                                   BasicAtmospherePressure_M.RenderTransform = ttBaroPressure_M;
                               }
                               if (laltKm != altKm)
                               {
                                   String sAltKM = Convert.ToInt16(altKm).ToString();

                                   if (sAltKM.Length == 0) { sAltKM = "00" + sAltKM; }
                                   else if (sAltKM.Length == 1) { sAltKM = "0" + sAltKM; }

                                   double altKM10 = Convert.ToDouble(sAltKM[0].ToString(), CultureInfo.InvariantCulture);
                                   double altKM01 = Convert.ToDouble(sAltKM[1].ToString(), CultureInfo.InvariantCulture);

                                   //altKM10 += altKM01 / 10;
                                   altKM01 = altKm % 10;

                                   ttAltKM01.Y = altKM01 * -23.33;
                                   AltBar_C_barrel.RenderTransform = ttAltKM01;

                                   ttAltKM10.Y = altKM10 * -23.33;
                                   AltBar_M_barrel.RenderTransform = ttAltKM10;
                               }

                               laltBar_M = altBar_M;
                               laltBar_C = altBar_C;
                               lbaroPressure = baroPressure;
                               laltKm = altKm;
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

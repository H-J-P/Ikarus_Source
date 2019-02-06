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
        double altkm = 0.0;

        double altkm01 = 0.0;
        double altkm10 = 0.0;
        string sAltkm = "";

        double laltBar_M = 0.0;
        double laltBar_C = 0.0;
        double lbaroPressure = 0.0;
        double laltkm = 0.0;

        string sBaroPressure = "";

        double baroPressure_M = 0.0;
        double baroPressure_C = 0.0;
        double baroPressure_X = 0.0;
        double baroPressure_I = 0.0;

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
                               if (vals.Length > 3) { altkm = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (altBar_M < 0.0) altBar_M = 0;
                               if (altBar_C < 0.0) altBar_C = 0;
                               if (baroPressure < 0.0) baroPressure = 0;
                               if (altkm < 0.0) altkm = 0;


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
                               if (laltkm != altkm)
                               {
                                   if (altkm < 10)
                                   {
                                       sAltkm = altkm.ToString();
                                       altkm01 = Convert.ToDouble(sAltkm[0].ToString(), CultureInfo.InvariantCulture);
                                       altkm10 = 0;
                                   }
                                   else
                                   {
                                       sAltkm = altkm.ToString();
                                       altkm01 = Convert.ToDouble(sAltkm[1].ToString(), CultureInfo.InvariantCulture);
                                       altkm10 = Convert.ToDouble(sAltkm[0].ToString(), CultureInfo.InvariantCulture);
                                   }
                                   ttAltKM10.Y = altkm10 * -23.3;
                                   AltBar_M_barrel.RenderTransform = ttAltKM10;

                                   ttAltKM01.Y = altkm01 * -23.3;
                                   AltBar_C_barrel.RenderTransform = ttAltKM01;
                               }
                               if (lbaroPressure != baroPressure)
                               {
                                   sBaroPressure = Convert.ToInt16(baroPressure).ToString();

                                   if (sBaroPressure.Length == 0) { sBaroPressure = "0000"; }
                                   if (sBaroPressure.Length == 1) { sBaroPressure = "000" + sBaroPressure; }
                                   else if (sBaroPressure.Length == 2) { sBaroPressure = "00" + sBaroPressure; }
                                   else if (sBaroPressure.Length == 3) { sBaroPressure = "0" + sBaroPressure; }

                                   baroPressure_M = Convert.ToDouble(sBaroPressure[0].ToString(), CultureInfo.InvariantCulture);
                                   baroPressure_C = Convert.ToDouble(sBaroPressure[1].ToString(), CultureInfo.InvariantCulture);
                                   baroPressure_X = Convert.ToDouble(sBaroPressure[2].ToString(), CultureInfo.InvariantCulture);
                                   baroPressure_I = Convert.ToDouble(sBaroPressure[3].ToString(), CultureInfo.InvariantCulture);

                                   ttBaroPressure_I.Y = baroPressure_I * -13;
                                   BasicAtmospherePressure_I.RenderTransform = ttBaroPressure_I;

                                   ttBaroPressure_X.Y = baroPressure_X * -13;
                                   BasicAtmospherePressure_X.RenderTransform = ttBaroPressure_X;

                                   ttBaroPressure_C.Y = baroPressure_C * -13;
                                   BasicAtmospherePressure_C.RenderTransform = ttBaroPressure_C;

                                   ttBaroPressure_M.Y = baroPressure_M * -13;
                                   BasicAtmospherePressure_M.RenderTransform = ttBaroPressure_M;
                               }
                               laltBar_M = altBar_M;
                               laltBar_C = altBar_C;
                               lbaroPressure = baroPressure;
                               laltkm = altkm;
                           }
                           catch (Exception e) { ImportExport.LogMessage(GetType().Name  + " got data: '" + strData + "' and failed with exception: " + e.ToString()); }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}

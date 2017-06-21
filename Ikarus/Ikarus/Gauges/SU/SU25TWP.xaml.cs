using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for SU25TWP.xaml
    /// </summary>
    public partial class SU25TWP : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double mainConnon = 0.0;
        double[] weaponPresend = new double[10] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
        double[] weaponActive = new double[10] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
        double weaponType = 0.0;
        double outerCannon = 0.0;
        double innerCannon = 0.0;
        double reserveWeapon = 0.0;

        public SU25TWP()
        {
            InitializeComponent();

            Loaded1.Visibility = System.Windows.Visibility.Hidden;
            Loaded2.Visibility = System.Windows.Visibility.Hidden;
            Loaded3.Visibility = System.Windows.Visibility.Hidden;
            Loaded4.Visibility = System.Windows.Visibility.Hidden;
            Loaded5.Visibility = System.Windows.Visibility.Hidden;
            Loaded6.Visibility = System.Windows.Visibility.Hidden;
            Loaded7.Visibility = System.Windows.Visibility.Hidden;
            Loaded8.Visibility = System.Windows.Visibility.Hidden;
            Loaded9.Visibility = System.Windows.Visibility.Hidden;
            Loaded10.Visibility = System.Windows.Visibility.Hidden;

            Ready1.Visibility = System.Windows.Visibility.Hidden;
            Ready2.Visibility = System.Windows.Visibility.Hidden;
            Ready3.Visibility = System.Windows.Visibility.Hidden;
            Ready4.Visibility = System.Windows.Visibility.Hidden;
            Ready5.Visibility = System.Windows.Visibility.Hidden;
            Ready6.Visibility = System.Windows.Visibility.Hidden;
            Ready7.Visibility = System.Windows.Visibility.Hidden;
            Ready8.Visibility = System.Windows.Visibility.Hidden;
            Ready9.Visibility = System.Windows.Visibility.Hidden;
            Ready10.Visibility = System.Windows.Visibility.Hidden;

            Active_MSL.Visibility = System.Windows.Visibility.Hidden;
            Active_GUN.Visibility = System.Windows.Visibility.Hidden;
            Active_BOM.Visibility = System.Windows.Visibility.Hidden;
            Active_ROK.Visibility = System.Windows.Visibility.Hidden;
            Active_Off.Visibility = System.Windows.Visibility.Visible;

            Gun_Full.Visibility = System.Windows.Visibility.Hidden;
            Gun_Half.Visibility = System.Windows.Visibility.Hidden;
            Gun_Quarter.Visibility = System.Windows.Visibility.Hidden;
            Gun_E.Visibility = System.Windows.Visibility.Hidden;
            Gun_Off.Visibility = System.Windows.Visibility.Visible;

            GunpodIn_Full.Visibility = System.Windows.Visibility.Hidden;
            GunpodIn_Half.Visibility = System.Windows.Visibility.Hidden;
            GunpodIn_Quarter.Visibility = System.Windows.Visibility.Hidden;
            GunpodIn_E.Visibility = System.Windows.Visibility.Hidden;
            Gunpod_Inner_Off.Visibility = System.Windows.Visibility.Visible;

            Gun_Outer_Full.Visibility = System.Windows.Visibility.Hidden;
            GunpodOut_Half.Visibility = System.Windows.Visibility.Hidden;
            GunpodOut_Quarter.Visibility = System.Windows.Visibility.Hidden;
            GunpodOut_E.Visibility = System.Windows.Visibility.Hidden;
            Gunpod_Outer_Off.Visibility = System.Windows.Visibility.Visible;

            Reserve_GUN.Visibility = System.Windows.Visibility.Hidden;
            Reserve_off.Visibility = System.Windows.Visibility.Visible;
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
            return 544.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { mainConnon = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               for (int n = 1; n < 11; n++)
                               {
                                   if (vals.Length > n) { weaponPresend[n - 1] = Convert.ToDouble(vals[n], CultureInfo.InvariantCulture); }
                               }

                               for (int n = 11; n < 21; n++)
                               {
                                   if (vals.Length > n) { weaponActive[n - 11] = Convert.ToDouble(vals[n], CultureInfo.InvariantCulture); }
                               }

                               if (vals.Length > 21) { weaponType = Convert.ToDouble(vals[21], CultureInfo.InvariantCulture); }
                               if (vals.Length > 22) { outerCannon = Convert.ToDouble(vals[22], CultureInfo.InvariantCulture); }
                               if (vals.Length > 23) { innerCannon = Convert.ToDouble(vals[23], CultureInfo.InvariantCulture); }
                               if (vals.Length > 24) { reserveWeapon = Convert.ToDouble(vals[24], CultureInfo.InvariantCulture); }

                               Loaded1.Visibility = (weaponPresend[0] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded2.Visibility = (weaponPresend[1] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded3.Visibility = (weaponPresend[2] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded4.Visibility = (weaponPresend[3] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded5.Visibility = (weaponPresend[4] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded6.Visibility = (weaponPresend[5] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded7.Visibility = (weaponPresend[6] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded8.Visibility = (weaponPresend[7] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded9.Visibility = (weaponPresend[8] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Loaded10.Visibility = (weaponPresend[9] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               Ready1.Visibility = (weaponActive[0] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready2.Visibility = (weaponActive[1] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready3.Visibility = (weaponActive[2] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready4.Visibility = (weaponActive[3] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready5.Visibility = (weaponActive[4] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready6.Visibility = (weaponActive[5] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready7.Visibility = (weaponActive[6] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready8.Visibility = (weaponActive[7] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready9.Visibility = (weaponActive[8] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Ready10.Visibility = (weaponActive[9] > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               if (weaponType == 0.0)
                               {
                                   Active_MSL.Visibility = System.Windows.Visibility.Hidden;
                                   Active_GUN.Visibility = System.Windows.Visibility.Hidden;
                                   Active_BOM.Visibility = System.Windows.Visibility.Hidden;
                                   Active_ROK.Visibility = System.Windows.Visibility.Hidden;
                                   Active_Off.Visibility = System.Windows.Visibility.Visible;
                               }
                               else if (weaponType == 0.1)
                               {
                                   Active_MSL.Visibility = System.Windows.Visibility.Visible;
                                   Active_GUN.Visibility = System.Windows.Visibility.Hidden;
                                   Active_BOM.Visibility = System.Windows.Visibility.Hidden;
                                   Active_ROK.Visibility = System.Windows.Visibility.Hidden;
                                   Active_Off.Visibility = System.Windows.Visibility.Hidden;
                               }
                               else if (weaponType == 0.2)
                               {
                                   Active_MSL.Visibility = System.Windows.Visibility.Hidden;
                                   Active_GUN.Visibility = System.Windows.Visibility.Hidden;
                                   Active_BOM.Visibility = System.Windows.Visibility.Hidden;
                                   Active_ROK.Visibility = System.Windows.Visibility.Visible;
                                   Active_Off.Visibility = System.Windows.Visibility.Hidden;
                               }
                               else if (weaponType == 0.3)
                               {
                                   Active_MSL.Visibility = System.Windows.Visibility.Hidden;
                                   Active_GUN.Visibility = System.Windows.Visibility.Hidden;
                                   Active_BOM.Visibility = System.Windows.Visibility.Visible;
                                   Active_ROK.Visibility = System.Windows.Visibility.Hidden;
                                   Active_Off.Visibility = System.Windows.Visibility.Hidden;
                               }

                               if (mainConnon == 1.0)
                               {
                                   Gun_Full.Visibility = System.Windows.Visibility.Visible;
                                   Gun_Half.Visibility = System.Windows.Visibility.Hidden;
                                   Gun_Quarter.Visibility = System.Windows.Visibility.Hidden;
                                   Gun_E.Visibility = System.Windows.Visibility.Hidden;
                                   Gun_Off.Visibility = System.Windows.Visibility.Hidden;
                               }
                               else if (mainConnon == 0.3)
                               {
                                   Gun_Full.Visibility = System.Windows.Visibility.Hidden;
                                   Gun_Half.Visibility = System.Windows.Visibility.Visible;
                                   Gun_Quarter.Visibility = System.Windows.Visibility.Hidden;
                                   Gun_E.Visibility = System.Windows.Visibility.Hidden;
                                   Gun_Off.Visibility = System.Windows.Visibility.Hidden;
                               }
                               else if (mainConnon == 0.6)
                               {
                                   Gun_Full.Visibility = System.Windows.Visibility.Hidden;
                                   Gun_Half.Visibility = System.Windows.Visibility.Hidden;
                                   Gun_Quarter.Visibility = System.Windows.Visibility.Visible;
                                   Gun_E.Visibility = System.Windows.Visibility.Hidden;
                                   Gun_Off.Visibility = System.Windows.Visibility.Hidden;
                               }
                               else if (mainConnon == 0.1)
                               {
                                   Gun_Full.Visibility = System.Windows.Visibility.Hidden;
                                   Gun_Half.Visibility = System.Windows.Visibility.Hidden;
                                   Gun_Quarter.Visibility = System.Windows.Visibility.Hidden;
                                   Gun_E.Visibility = System.Windows.Visibility.Visible;
                                   Gun_Off.Visibility = System.Windows.Visibility.Hidden;
                               }
                               else if (mainConnon == 0.0)
                               {
                                   Gun_Full.Visibility = System.Windows.Visibility.Hidden;
                                   Gun_Half.Visibility = System.Windows.Visibility.Hidden;
                                   Gun_Quarter.Visibility = System.Windows.Visibility.Hidden;
                                   Gun_E.Visibility = System.Windows.Visibility.Hidden;
                                   Gun_Off.Visibility = System.Windows.Visibility.Visible;
                               }

                               if (innerCannon == 1.0)
                               {
                                   GunpodIn_Full.Visibility = System.Windows.Visibility.Visible;
                                   GunpodIn_Half.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodIn_Quarter.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodIn_E.Visibility = System.Windows.Visibility.Hidden;
                                   Gunpod_Inner_Off.Visibility = System.Windows.Visibility.Hidden;
                               }
                               else if (innerCannon == 0.3)
                               {
                                   GunpodIn_Full.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodIn_Half.Visibility = System.Windows.Visibility.Visible;
                                   GunpodIn_Quarter.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodIn_E.Visibility = System.Windows.Visibility.Hidden;
                                   Gunpod_Inner_Off.Visibility = System.Windows.Visibility.Hidden;
                               }
                               else if (innerCannon == 0.6)
                               {
                                   GunpodIn_Full.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodIn_Half.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodIn_Quarter.Visibility = System.Windows.Visibility.Visible;
                                   GunpodIn_E.Visibility = System.Windows.Visibility.Hidden;
                                   Gunpod_Inner_Off.Visibility = System.Windows.Visibility.Hidden;
                               }
                               else if (innerCannon == 0.1)
                               {
                                   GunpodIn_Full.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodIn_Half.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodIn_Quarter.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodIn_E.Visibility = System.Windows.Visibility.Visible;
                                   Gunpod_Inner_Off.Visibility = System.Windows.Visibility.Hidden;
                               }
                               else if (innerCannon == 0.0)
                               {
                                   GunpodIn_Full.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodIn_Half.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodIn_Quarter.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodIn_E.Visibility = System.Windows.Visibility.Hidden;
                                   Gunpod_Inner_Off.Visibility = System.Windows.Visibility.Visible;
                               }

                               if (outerCannon == 1.0)
                               {
                                   Gun_Outer_Full.Visibility = System.Windows.Visibility.Visible;
                                   GunpodOut_Half.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodOut_Quarter.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodOut_E.Visibility = System.Windows.Visibility.Hidden;
                                   Gunpod_Outer_Off.Visibility = System.Windows.Visibility.Hidden;
                               }
                               else if (outerCannon == 0.3)
                               {
                                   Gun_Outer_Full.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodOut_Half.Visibility = System.Windows.Visibility.Visible;
                                   GunpodOut_Quarter.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodOut_E.Visibility = System.Windows.Visibility.Hidden;
                                   Gunpod_Outer_Off.Visibility = System.Windows.Visibility.Hidden;
                               }
                               else if (outerCannon == 0.6)
                               {
                                   Gun_Outer_Full.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodOut_Half.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodOut_Quarter.Visibility = System.Windows.Visibility.Visible;
                                   GunpodOut_E.Visibility = System.Windows.Visibility.Hidden;
                                   Gunpod_Outer_Off.Visibility = System.Windows.Visibility.Hidden;
                               }
                               else if (outerCannon == 0.1)
                               {
                                   Gun_Outer_Full.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodOut_Half.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodOut_Quarter.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodOut_E.Visibility = System.Windows.Visibility.Visible;
                                   Gunpod_Outer_Off.Visibility = System.Windows.Visibility.Hidden;
                               }
                               else if (outerCannon == 0.0)
                               {
                                   Gun_Outer_Full.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodOut_Half.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodOut_Quarter.Visibility = System.Windows.Visibility.Hidden;
                                   GunpodOut_E.Visibility = System.Windows.Visibility.Hidden;
                                   Gunpod_Outer_Off.Visibility = System.Windows.Visibility.Visible;
                               }

                               if (reserveWeapon == 1.0)
                               {
                                   Reserve_GUN.Visibility = System.Windows.Visibility.Visible;
                                   Reserve_off.Visibility = System.Windows.Visibility.Hidden;
                               }
                               else
                               {
                                   Reserve_GUN.Visibility = System.Windows.Visibility.Hidden;
                                   Reserve_off.Visibility = System.Windows.Visibility.Visible;
                               }
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

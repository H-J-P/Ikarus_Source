using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für WPSPO15.xaml
    /// </summary>
    public partial class WPSPO15 : UserControl, I_Ikarus
    {
        private string dataImportID = "";

        private int timerOn = 0;
        private int timerOff = 0;
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double power = 0.0, emissionUp = 0.0, emissionBottom = 0.0;
        double locked = 0.0, secondaryLeftBack = 0.0, secondaryLeft90 = 0.0, secondaryLeft50 = 0.0, secondaryLeft30 = 0.0;
        double secondaryLeft10 = 0.0, secondaryRight10 = 0.0, secondaryRight30 = 0.0, secondaryRight50 = 0.0, secondaryRight90 = 0.0;
        double secondaryRightBack = 0.0, secondaryType_AIR = 0.0, secondaryType_LRR = 0.0, secondaryType_MRR = 0.0, secondaryType_SRR = 0.0;
        double secondaryType_EWR = 0.0, secondaryType_AWACS = 0.0;
        double primaryThreatLeftBack = 0.0, primaryThreatLeft90 = 0.0, primaryThreatLeft50 = 0.0, primaryThreatLeft30 = 0.0, primaryThreatLeft10 = 0.0;
        double primaryThreatRight10 = 0.0, primaryThreatRight30 = 0.0, primaryThreatRight50 = 0.0, primaryThreatRight90 = 0.0, primaryThreatRightBack = 0.0;
        double primaryAir = 0.0, primaryLongrange = 0.0, primaryMediumrange = 0.0, primaryShortrange = 0.0, primaryEarlywarning = 0.0, primaryAWACS = 0.0;
        double emmissionME = 0.0, emissionLevel1 = 0.0, emissionLevel2 = 0.0, emissionLevel3 = 0.0, emissionLevel4 = 0.0, emissionLevel5 = 0.0, emissionLevel6 = 0.0;
        double emissionLevel7 = 0.0, emissionLevel8 = 0.0, emissionLevel9 = 0.0, emissionLevel10 = 0.0, emissionLevel11 = 0.0, emissionLevel12 = 0.0, emissionLevel13 = 0.0;
        double emissionLevel14 = 0.0, emissionLevel15 = 0.0;

        public WPSPO15()
        {
            InitializeComponent();

            Power.Visibility = System.Windows.Visibility.Hidden;

            EmissionUp.Visibility = System.Windows.Visibility.Hidden;
            EmissionBottom.Visibility = System.Windows.Visibility.Hidden;
            PrimaryThreatLeftBack.Visibility = System.Windows.Visibility.Hidden;
            PrimaryThreatLeft90.Visibility = System.Windows.Visibility.Hidden;
            PrimaryThreatLeft50.Visibility = System.Windows.Visibility.Hidden;
            PrimaryThreatLeft30.Visibility = System.Windows.Visibility.Hidden;
            PrimaryThreatLeft10.Visibility = System.Windows.Visibility.Hidden;
            PrimaryThreatRight10.Visibility = System.Windows.Visibility.Hidden;
            PrimaryThreatRight30.Visibility = System.Windows.Visibility.Hidden;
            PrimaryThreatRight50.Visibility = System.Windows.Visibility.Hidden;
            PrimaryThreatRight90.Visibility = System.Windows.Visibility.Hidden;
            PrimaryThreatRightBack.Visibility = System.Windows.Visibility.Hidden;

            PrimaryAir.Visibility = System.Windows.Visibility.Hidden;
            PrimaryLongrange.Visibility = System.Windows.Visibility.Hidden;
            PrimaryMediumrange.Visibility = System.Windows.Visibility.Hidden;
            PrimaryShortrange.Visibility = System.Windows.Visibility.Hidden;
            PrimaryEarlywarning.Visibility = System.Windows.Visibility.Hidden;
            PrimaryAWACS.Visibility = System.Windows.Visibility.Hidden;

            EmissionUp.Visibility = System.Windows.Visibility.Hidden;
            EmissionBottom.Visibility = System.Windows.Visibility.Hidden;
            Locked.Visibility = System.Windows.Visibility.Hidden;

            SecondaryThreatLeftBack.Visibility = System.Windows.Visibility.Hidden;
            SecondaryThreatLeft90.Visibility = System.Windows.Visibility.Hidden;
            SecondaryThreatLeft50.Visibility = System.Windows.Visibility.Hidden;
            SecondaryThreatLeft30.Visibility = System.Windows.Visibility.Hidden;
            SecondaryThreatLeft10.Visibility = System.Windows.Visibility.Hidden;
            SecondaryThreatRight10.Visibility = System.Windows.Visibility.Hidden;
            SecondaryThreatRight30.Visibility = System.Windows.Visibility.Hidden;
            SecondaryThreatRight50.Visibility = System.Windows.Visibility.Hidden;
            SecondaryThreatRight90.Visibility = System.Windows.Visibility.Hidden;
            SecondaryThreatRightBack.Visibility = System.Windows.Visibility.Hidden;

            SecondaryAir.Visibility = System.Windows.Visibility.Hidden;
            SecondaryLongrange.Visibility = System.Windows.Visibility.Hidden;
            SecondaryMediumrange.Visibility = System.Windows.Visibility.Hidden;
            SecondarySortrange.Visibility = System.Windows.Visibility.Hidden;
            SecondaryEarlywarning.Visibility = System.Windows.Visibility.Hidden;
            SecondaryAWACS.Visibility = System.Windows.Visibility.Hidden;

            EmmissionME.Visibility = System.Windows.Visibility.Hidden;
            EmissionLevel1.Visibility = System.Windows.Visibility.Hidden;
            EmissionLevel2.Visibility = System.Windows.Visibility.Hidden;
            EmissionLevel3.Visibility = System.Windows.Visibility.Hidden;
            EmissionLevel4.Visibility = System.Windows.Visibility.Hidden;
            EmissionLevel5.Visibility = System.Windows.Visibility.Hidden;
            EmissionLevel6.Visibility = System.Windows.Visibility.Hidden;
            EmissionLevel7.Visibility = System.Windows.Visibility.Hidden;
            EmissionLevel8.Visibility = System.Windows.Visibility.Hidden;
            EmmissionLevel9.Visibility = System.Windows.Visibility.Hidden;
            EmmissionLevel10.Visibility = System.Windows.Visibility.Hidden;
            EmmissionLevel11.Visibility = System.Windows.Visibility.Hidden;
            EmmissionLevel12.Visibility = System.Windows.Visibility.Hidden;
            EmmissionLevel13.Visibility = System.Windows.Visibility.Hidden;
            EmmissionLevel14.Visibility = System.Windows.Visibility.Hidden;
            EmmissionLevel15.Visibility = System.Windows.Visibility.Hidden;

            Audio_OFF.Visibility = System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { power = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { emissionUp = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { emissionBottom = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { locked = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { secondaryLeftBack = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               if (vals.Length > 5) { secondaryLeft90 = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { secondaryLeft50 = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { secondaryLeft30 = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { secondaryLeft10 = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }
                               if (vals.Length > 9) { secondaryRight10 = Convert.ToDouble(vals[9], CultureInfo.InvariantCulture); }

                               if (vals.Length > 10) { secondaryRight30 = Convert.ToDouble(vals[10], CultureInfo.InvariantCulture); }
                               if (vals.Length > 11) { secondaryRight50 = Convert.ToDouble(vals[11], CultureInfo.InvariantCulture); }
                               if (vals.Length > 12) { secondaryRight90 = Convert.ToDouble(vals[12], CultureInfo.InvariantCulture); }
                               if (vals.Length > 13) { secondaryRightBack = Convert.ToDouble(vals[13], CultureInfo.InvariantCulture); }
                               if (vals.Length > 14) { secondaryType_AIR = Convert.ToDouble(vals[14], CultureInfo.InvariantCulture); }

                               if (vals.Length > 15) { secondaryType_LRR = Convert.ToDouble(vals[15], CultureInfo.InvariantCulture); }
                               if (vals.Length > 16) { secondaryType_MRR = Convert.ToDouble(vals[16], CultureInfo.InvariantCulture); }
                               if (vals.Length > 17) { secondaryType_SRR = Convert.ToDouble(vals[17], CultureInfo.InvariantCulture); }
                               if (vals.Length > 18) { secondaryType_EWR = Convert.ToDouble(vals[18], CultureInfo.InvariantCulture); }
                               if (vals.Length > 19) { secondaryType_AWACS = Convert.ToDouble(vals[19], CultureInfo.InvariantCulture); }

                               if (vals.Length > 20) { primaryThreatLeftBack = Convert.ToDouble(vals[20], CultureInfo.InvariantCulture); }
                               if (vals.Length > 21) { primaryThreatLeft90 = Convert.ToDouble(vals[21], CultureInfo.InvariantCulture); }
                               if (vals.Length > 22) { primaryThreatLeft50 = Convert.ToDouble(vals[22], CultureInfo.InvariantCulture); }
                               if (vals.Length > 23) { primaryThreatLeft30 = Convert.ToDouble(vals[23], CultureInfo.InvariantCulture); }
                               if (vals.Length > 24) { primaryThreatLeft10 = Convert.ToDouble(vals[24], CultureInfo.InvariantCulture); }

                               if (vals.Length > 25) { primaryThreatRight10 = Convert.ToDouble(vals[25], CultureInfo.InvariantCulture); }
                               if (vals.Length > 26) { primaryThreatRight30 = Convert.ToDouble(vals[26], CultureInfo.InvariantCulture); }
                               if (vals.Length > 27) { primaryThreatRight50 = Convert.ToDouble(vals[27], CultureInfo.InvariantCulture); }
                               if (vals.Length > 28) { primaryThreatRight90 = Convert.ToDouble(vals[28], CultureInfo.InvariantCulture); }
                               if (vals.Length > 29) { primaryThreatRightBack = Convert.ToDouble(vals[29], CultureInfo.InvariantCulture); }

                               if (vals.Length > 30) { primaryAir = Convert.ToDouble(vals[30], CultureInfo.InvariantCulture); }
                               if (vals.Length > 31) { primaryLongrange = Convert.ToDouble(vals[31], CultureInfo.InvariantCulture); }
                               if (vals.Length > 32) { primaryMediumrange = Convert.ToDouble(vals[32], CultureInfo.InvariantCulture); }
                               if (vals.Length > 33) { primaryShortrange = Convert.ToDouble(vals[33], CultureInfo.InvariantCulture); }
                               if (vals.Length > 34) { primaryEarlywarning = Convert.ToDouble(vals[34], CultureInfo.InvariantCulture); }

                               if (vals.Length > 35) { primaryAWACS = Convert.ToDouble(vals[35], CultureInfo.InvariantCulture); }
                               if (vals.Length > 36) { emmissionME = Convert.ToDouble(vals[36], CultureInfo.InvariantCulture); }
                               if (vals.Length > 37) { emissionLevel1 = Convert.ToDouble(vals[37], CultureInfo.InvariantCulture); }
                               if (vals.Length > 38) { emissionLevel2 = Convert.ToDouble(vals[38], CultureInfo.InvariantCulture); }
                               if (vals.Length > 39) { emissionLevel3 = Convert.ToDouble(vals[39], CultureInfo.InvariantCulture); }

                               if (vals.Length > 40) { emissionLevel4 = Convert.ToDouble(vals[40], CultureInfo.InvariantCulture); }
                               if (vals.Length > 41) { emissionLevel5 = Convert.ToDouble(vals[41], CultureInfo.InvariantCulture); }
                               if (vals.Length > 42) { emissionLevel6 = Convert.ToDouble(vals[42], CultureInfo.InvariantCulture); }
                               if (vals.Length > 43) { emissionLevel7 = Convert.ToDouble(vals[43], CultureInfo.InvariantCulture); }
                               if (vals.Length > 44) { emissionLevel8 = Convert.ToDouble(vals[44], CultureInfo.InvariantCulture); }

                               if (vals.Length > 45) { emissionLevel9 = Convert.ToDouble(vals[45], CultureInfo.InvariantCulture); }
                               if (vals.Length > 46) { emissionLevel10 = Convert.ToDouble(vals[46], CultureInfo.InvariantCulture); }
                               if (vals.Length > 47) { emissionLevel11 = Convert.ToDouble(vals[47], CultureInfo.InvariantCulture); }
                               if (vals.Length > 48) { emissionLevel12 = Convert.ToDouble(vals[48], CultureInfo.InvariantCulture); }
                               if (vals.Length > 49) { emissionLevel13 = Convert.ToDouble(vals[49], CultureInfo.InvariantCulture); }

                               if (vals.Length > 50) { emissionLevel14 = Convert.ToDouble(vals[50], CultureInfo.InvariantCulture); }
                               if (vals.Length > 51) { emissionLevel15 = Convert.ToDouble(vals[51], CultureInfo.InvariantCulture); }


                               EmissionUp.Visibility = (emissionUp > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               EmissionBottom.Visibility = (emissionBottom > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               if (locked == 0.5)
                               {
                                   Locked.Visibility = (timerOn < 110) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                                   if (timerOn < 110)
                                       timerOn += 10;
                                   else
                                   {
                                       if (timerOff < 60) timerOff += 10;
                                       else { timerOn = 0; timerOff = 0; }
                                   }
                               }
                               else
                               {
                                   Locked.Visibility = (locked == 1.0) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                                   timerOn = 0; timerOff = 0;
                               }

                               SecondaryThreatLeftBack.Visibility = (secondaryLeftBack > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SecondaryThreatLeft90.Visibility = (secondaryLeft90 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SecondaryThreatLeft50.Visibility = (secondaryLeft50 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SecondaryThreatLeft30.Visibility = (secondaryLeft30 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SecondaryThreatLeft10.Visibility = (secondaryLeft10 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SecondaryThreatRight10.Visibility = (secondaryRight10 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SecondaryThreatRight30.Visibility = (secondaryRight30 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SecondaryThreatRight50.Visibility = (secondaryRight50 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SecondaryThreatRight90.Visibility = (secondaryRight90 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SecondaryThreatRightBack.Visibility = (secondaryRightBack > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               SecondaryAir.Visibility = (secondaryType_AIR > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SecondaryLongrange.Visibility = (secondaryType_LRR > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SecondaryMediumrange.Visibility = (secondaryType_MRR > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SecondarySortrange.Visibility = (secondaryType_SRR > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SecondaryEarlywarning.Visibility = (secondaryType_EWR > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               SecondaryAWACS.Visibility = (secondaryType_AWACS > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               Power.Visibility = (power > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               PrimaryThreatLeftBack.Visibility = primaryThreatLeftBack > 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               PrimaryThreatLeft90.Visibility = primaryThreatLeft90 > 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               PrimaryThreatLeft50.Visibility = primaryThreatLeft50 > 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               PrimaryThreatLeft30.Visibility = primaryThreatLeft30 > 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               PrimaryThreatLeft10.Visibility = primaryThreatLeft10 > 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               PrimaryThreatRight10.Visibility = primaryThreatRight10 > 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               PrimaryThreatRight30.Visibility = primaryThreatRight30 > 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               PrimaryThreatRight50.Visibility = primaryThreatRight50 > 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               PrimaryThreatRight90.Visibility = primaryThreatRight90 > 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               PrimaryThreatRightBack.Visibility = primaryThreatRightBack > 0.9 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               PrimaryAir.Visibility = (primaryAir > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               PrimaryLongrange.Visibility = (primaryLongrange > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               PrimaryMediumrange.Visibility = (primaryMediumrange > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               PrimaryShortrange.Visibility = (primaryShortrange > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               PrimaryEarlywarning.Visibility = (primaryEarlywarning > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               PrimaryAWACS.Visibility = (primaryAWACS > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               EmmissionME.Visibility = (emmissionME > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               EmissionLevel1.Visibility = (emissionLevel1 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               EmissionLevel2.Visibility = (emissionLevel2 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               EmissionLevel3.Visibility = (emissionLevel3 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               EmissionLevel4.Visibility = (emissionLevel4 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               EmissionLevel5.Visibility = (emissionLevel5 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               EmissionLevel6.Visibility = (emissionLevel6 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               EmissionLevel7.Visibility = (emissionLevel7 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               EmissionLevel8.Visibility = (emissionLevel8 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               EmmissionLevel9.Visibility = (emissionLevel9 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               EmmissionLevel10.Visibility = (emissionLevel10 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               EmmissionLevel11.Visibility = (emissionLevel11 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               EmmissionLevel12.Visibility = (emissionLevel12 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               EmmissionLevel13.Visibility = (emissionLevel13 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               EmmissionLevel14.Visibility = (emissionLevel14 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               EmmissionLevel15.Visibility = (emissionLevel15 > 0.9) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                           }
                           catch { return; }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}

﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for SU27Fuel.xaml
    /// </summary>
    public partial class SU27Fuel : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double totalFuel_0_5 = 0.0;
        double totalFuel_3_9 = 0.0;
        double light1 = 0.0;
        double light2 = 0.0;
        double light3 = 0.0;
        double light4 = 0.0;
        double bingoLight = 0.0;

        public SU27Fuel()
        {
            InitializeComponent();

            Light1.Visibility = System.Windows.Visibility.Hidden;
            Light2.Visibility = System.Windows.Visibility.Hidden;
            Light3.Visibility = System.Windows.Visibility.Hidden;
            Light4.Visibility = System.Windows.Visibility.Hidden;
            BingoLight.Visibility = System.Windows.Visibility.Hidden;

            TotalFuel_0_5_A.Height = 5.0;
            TotalFuel_3_9_A.Height = 8.0;
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
                               if (vals.Length > 1) { totalFuel_3_9 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { light1 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { light2 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { light3 = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { light4 = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { bingoLight = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }

                               if (totalFuel_0_5 > 1.0) { totalFuel_0_5 = 1.0; }
                               if (totalFuel_0_5 < 0.0) { totalFuel_0_5 = 0.0; }
                               if (totalFuel_3_9 > 1.0) { totalFuel_3_9 = 1.0; }
                               if (totalFuel_3_9 < 0.0) { totalFuel_3_9 = 0.0; }

                               TotalFuel_0_5_A.Height = (totalFuel_0_5 * 170) + 5;
                               TotalFuel_3_9_A.Height = (totalFuel_3_9 * 207) + 8;

                               Light1.Visibility = (light1 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Light2.Visibility = (light2 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Light3.Visibility = (light3 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Light4.Visibility = (light4 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               BingoLight.Visibility = (bingoLight > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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

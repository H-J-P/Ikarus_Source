﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für ME4KDblP.xaml
    /// </summary>
    public partial class BfK4DblP : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        RotateTransform rtOil = new RotateTransform();
        RotateTransform rtFuel = new RotateTransform();

        double oilPressure = 0.0;
        double fuelPressure = 0.0;

        double loilPressure = 0.0;
        double lfuelPressure = 0.0;

        public BfK4DblP()
        {
            InitializeComponent();

            shadow.Visibility = MainWindow.shadowChecked ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { oilPressure = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { fuelPressure = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }

                               if (oilPressure < 0.0) oilPressure = 0.0;
                               if (fuelPressure < 0.0) fuelPressure = 0.0;

                               if (loilPressure != oilPressure)
                               {
                                   rtOil.Angle = oilPressure * -138;
                                   Lw_DblP_Needle_Oil.RenderTransform = rtOil;
                               }
                               if (lfuelPressure != fuelPressure)
                               {
                                   rtFuel.Angle = fuelPressure * 143;
                                   Lw_DblP_Needle_Fuel.RenderTransform = rtFuel;
                               }
                               loilPressure = oilPressure;
                               lfuelPressure = fuelPressure;
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

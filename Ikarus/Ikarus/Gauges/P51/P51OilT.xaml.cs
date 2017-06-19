﻿using System;
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
    /// Interaction logic for P51OilT.xaml
    /// </summary>
    public partial class P51OilT : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double oilTemp = 0.0;
        double oilPressure = 0.0;
        double fuelPressure = 0.0;

        double loilTemp = 0.0;
        double loilPressure = 0.0;
        double lfuelPressure = 0.0;

        RotateTransform rtoilTemp = new RotateTransform();
        RotateTransform rtoilPressure = new RotateTransform();
        RotateTransform rtfuelPressure = new RotateTransform();

        public P51OilT()
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
            return 255.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { oilTemp = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { oilPressure = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { fuelPressure = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (oilTemp < 0.0) oilTemp = 0.0;
                               if (oilPressure < 0.0) oilPressure = 0.0;
                               if (fuelPressure < 0.0) fuelPressure = 0.0;

                               if (loilTemp != oilTemp)
                               {
                                   rtoilTemp.Angle = oilTemp * 180;
                                   Oil_Temperature.RenderTransform = rtoilTemp;
                               }
                               if (loilPressure != oilPressure)
                               {
                                   rtoilPressure.Angle = oilPressure * 180;
                                   Oil_Pressure.RenderTransform = rtoilPressure;
                               }
                               if (lfuelPressure != fuelPressure)
                               {
                                   rtfuelPressure.Angle = fuelPressure * -180;
                                   Fuel_Pressure.RenderTransform = rtfuelPressure;
                               }
                               loilTemp = oilTemp;
                               loilPressure = oilPressure;
                               lfuelPressure = fuelPressure;
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

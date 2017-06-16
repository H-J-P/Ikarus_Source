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
    /// Interaction logic for F15Fuel.xaml
    /// </summary>
    public partial class F15Fuel : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        string fuelCounter1 = "";
        string fuelCounter2 = "";
        string fuelCounter3 = "";
        double fuelNeedle = 0.0;

        double lfuelNeedle = 0.0;

        RotateTransform rtfuelQuantity = new RotateTransform();

        public F15Fuel()
        {
            InitializeComponent();

            FuelLeft.Text = "";
            FuelRight.Text = "";
            FuelTotal.Text = "";
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

                               if (vals.Length > 0) { fuelCounter1 = vals[0]; }
                               if (vals.Length > 1) { fuelCounter2 = vals[1]; }
                               if (vals.Length > 2) { fuelCounter3 = vals[2]; }
                               if (vals.Length > 3) { fuelNeedle = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (fuelNeedle < 0.0) fuelNeedle = 0.0;

                               if (lfuelNeedle != fuelNeedle)
                               {
                                   rtfuelQuantity.Angle = fuelNeedle * 220;
                                   TotalFuel.RenderTransform = rtfuelQuantity;
                               }
                               FuelLeft.Text = fuelCounter1;
                               FuelRight.Text = fuelCounter2;
                               FuelTotal.Text = fuelCounter3;

                               lfuelNeedle = fuelNeedle;
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

﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;


namespace Ikarus
{
    /// <summary>
    /// Interaction logic for P51FuelQWing.xaml
    /// </summary>
    public partial class P51FuelQWing : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        private double[] valueScale = new double[] { };
        private double[] degreeDial = new double[] { };
        int valueScaleIndex = 0;
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double tank = 0.0;
        double ltank = 0.0;

        RotateTransform rtTank = new RotateTransform();

        public P51FuelQWing()
        {
            InitializeComponent();
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
            helper.SetInput(ref _input, ref valueScale, ref valueScaleIndex, 3);
        }

        public void SetOutput(string _output)
        {
            helper.SetOutput(ref _output, ref degreeDial, 3);
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

                               if (ltank != tank)
                               {
                                   // Fuel_Tank_Fuselage.input		                = {0.0, 5.0, 15.0, 30.0, 45.0, 60.0, 75.0, 92.0} -- US GAL
                                   //double[] valueScale = new double[valueScaleIndex] { 0.0, 0.2, 0.36, 0.52, 0.65, 0.77, 0.92, 1.0 };
                                   //double[] degreeDial = new double[valueScaleIndex] { 0.0, 34.0, 66.0, 94.0, 119, 141, 167, 180 };

                                   if (vals.Length > 0) { tank = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                                   for (int n = 0; n < valueScaleIndex - 1; n++)
                                   {
                                       if (tank >= valueScale[n] && tank <= valueScale[n + 1])
                                       {
                                           rtTank.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (tank - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   if (MainWindow.editmode)
                                   {
                                       Cockpit.UpdateInOut(dataImportID, "1", tank.ToString(), Convert.ToInt32(rtTank.Angle).ToString());
                                   }
                                   Fuel_Wing.RenderTransform = rtTank;
                               }
                               ltank = tank;
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

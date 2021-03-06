﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für LwAFN2.xaml   Author: HJP
    /// </summary>
    public partial class LwAFN2 : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double valueHorizon = 0.0;
        double valueVertical = 0.0;
        double valueLamp = 0.0;

        double lvalueHorizon = 0.0;
        double lvalueVertical = 0.0;
        double lvalueLamp = 0.0;

        RotateTransform rtDistance = new RotateTransform();
        RotateTransform rtVertical = new RotateTransform();
        RotateTransform rtLamp = new RotateTransform();

        public LwAFN2()
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

                               if (vals.Length > 0) { valueHorizon = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { valueVertical = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { valueLamp = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (lvalueHorizon != valueHorizon)
                               {
                                   rtDistance.Angle = (valueHorizon * 26) - 13; // -27;
                                   Lw_AFN2_Needle_distance.RenderTransform = rtDistance;
                               }
                               if (lvalueVertical != valueVertical)
                               {
                                   rtVertical.Angle = (valueVertical * -13);
                                   Lw_AFN2_Needle_bearing.RenderTransform = rtVertical;
                               }
                               if (lvalueLamp != valueLamp)
                               {
                                   rtLamp.Angle = valueLamp * -45;
                                   Lw_AFN2_Light.RenderTransform = rtLamp;
                               }
                               lvalueHorizon = valueHorizon;
                               lvalueVertical = valueVertical;
                               lvalueLamp = valueLamp;
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

﻿using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaktionslogik für P51Blinker.xaml
    /// </summary>
    public partial class P51Blinker : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        double blinker = 0.0;
        double lblinker = 0.0;

        TranslateTransform ttBlinkerUp = new TranslateTransform();
        TranslateTransform ttBlinkerDown = new TranslateTransform();

        public P51Blinker()
        {
            InitializeComponent();

            ttBlinkerUp.Y = -20;
            Oxygen_Flow_Blinker_UP.RenderTransform = ttBlinkerUp;

            ttBlinkerDown.Y = 20;
            Oxygen_Flow_Blinker_DOWN.RenderTransform = ttBlinkerDown;
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

                               if (vals.Length > 0) { blinker = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (blinker < 0.0) blinker = 0.0;

                               if (lblinker != blinker)
                               {
                                   ttBlinkerUp.Y = blinker * -20;
                                   Oxygen_Flow_Blinker_UP.RenderTransform = ttBlinkerUp;

                                   ttBlinkerDown.Y = blinker * 20;
                                   Oxygen_Flow_Blinker_DOWN.RenderTransform = ttBlinkerDown;
                               }
                               lblinker = blinker;
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

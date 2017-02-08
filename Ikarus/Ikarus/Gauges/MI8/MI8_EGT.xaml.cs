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
    /// Interaction logic for MI8_EGT.xaml
    /// </summary>
    public partial class MI8_EGT : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double leftEngineTemperatureHund = 0.0;
        double leftEngineTemperatureTenth = 0.0;
        double rightEngineTemperatureHund = 0.0;
        double rightEngineTemperatureTenth = 0.0;

        double lleftEngineTemperatureHund = 0.0;
        double lleftEngineTemperatureTenth = 0.0;
        double lrightEngineTemperatureHund = 0.0;
        double lrightEngineTemperatureTenth = 0.0;

        RotateTransform rtLeftEngineTemperatureHund = new RotateTransform();
        RotateTransform rtLeftEngineTemperatureTenth = new RotateTransform();
        RotateTransform rtRightEngineTemperatureHund = new RotateTransform();
        RotateTransform rtRightEngineTemperatureTenth = new RotateTransform();

        public MI8_EGT()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
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
            return 170.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { leftEngineTemperatureHund = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { leftEngineTemperatureTenth = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { rightEngineTemperatureHund = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { rightEngineTemperatureTenth = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }

                               if (lleftEngineTemperatureHund != leftEngineTemperatureHund)
                               {
                                   rtLeftEngineTemperatureHund.Angle = leftEngineTemperatureHund * 267;
                                   LeftEngineTemperatureHund.RenderTransform = rtLeftEngineTemperatureHund;
                               }
                               if (lleftEngineTemperatureTenth != leftEngineTemperatureTenth)
                               {
                                   rtLeftEngineTemperatureTenth.Angle = leftEngineTemperatureTenth * 360;
                                   LeftEngineTemperatureTenth.RenderTransform = rtLeftEngineTemperatureTenth;
                               }
                               if (lrightEngineTemperatureHund != rightEngineTemperatureHund)
                               {
                                   rtRightEngineTemperatureHund.Angle = rightEngineTemperatureHund * 267;
                                   RightEngineTemperatureHund.RenderTransform = rtRightEngineTemperatureHund;
                               }
                               if (lrightEngineTemperatureTenth != rightEngineTemperatureTenth)
                               {
                                   rtRightEngineTemperatureTenth.Angle = rightEngineTemperatureTenth * 360;
                                   RightEngineTemperatureTenth.RenderTransform = rtRightEngineTemperatureTenth;
                               }
                               lleftEngineTemperatureHund = leftEngineTemperatureHund;
                               lleftEngineTemperatureTenth = leftEngineTemperatureTenth;
                               lrightEngineTemperatureHund = rightEngineTemperatureHund;
                               lrightEngineTemperatureTenth = rightEngineTemperatureTenth;
                           }
                           catch { return; }
                       }));
        }

        private void MakeDraggable(System.Windows.UIElement moveThisElement, System.Windows.UIElement movedByElement)
        {
            System.Windows.Point originalPoint = new System.Windows.Point(0, 0), currentPoint;
            TranslateTransform trUsercontrol = new TranslateTransform(0, 0);
            bool isMousePressed = false;

            movedByElement.MouseLeftButtonDown += (a, b) =>
            {
                isMousePressed = true;
                originalPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
            };

            movedByElement.MouseLeftButtonUp += (a, b) =>
            {
                isMousePressed = false;
                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID);
            };
            movedByElement.MouseLeave += (a, b) => isMousePressed = false;

            movedByElement.MouseMove += (a, b) =>
            {
                if (!isMousePressed || !MainWindow.editmode) return;

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                trUsercontrol.X += currentPoint.X - originalPoint.X;
                trUsercontrol.Y += currentPoint.Y - originalPoint.Y;
                moveThisElement.RenderTransform = trUsercontrol;


            };
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}
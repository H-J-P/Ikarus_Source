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
    /// Interaktionslogik für AJS37_ACC.xaml
    /// </summary>
    public partial class AJS37_ACC : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double acceleration = 0.0;
        double accelerationMax = 0.0;
        double accelerationMin = 0.0;

        double lacceleration = 0.0;
        double laccelerationMax = 0.0;
        double laccelerationMin = 0.0;

        RotateTransform rtAcceleration = new RotateTransform();
        RotateTransform rtAccelerationMax = new RotateTransform();
        RotateTransform rtAccelerationMin = new RotateTransform();

        public AJS37_ACC()
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

                               const int valueScaleIndex = 5;

                               if (vals.Length > 0) { acceleration = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { accelerationMax = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { accelerationMin = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }

                               if (lacceleration != acceleration)
                               {
                                   // 110   ACCELEROMETER.input =                    {  -5.0,     1,   5,    8, 10.0 } 
                                   double[] valueScale = new double[valueScaleIndex] { -0.41, 0.096, 0.5, 0.81, 1 };
                                   double[] degreeDial = new double[valueScaleIndex] { -110, 23, 114, 180, 225 };

                                   for (int n = 0; n < (valueScaleIndex - 1); n++)
                                   {
                                       if (acceleration >= valueScale[n] && acceleration <= valueScale[n + 1])
                                       {
                                           rtAcceleration.Angle = (degreeDial[n] - degreeDial[n + 1]) / (valueScale[n] - valueScale[n + 1]) * (acceleration - valueScale[n]) + degreeDial[n];
                                           break;
                                       }
                                   }
                                   ACCELEROMETER.RenderTransform = rtAcceleration;
                               }
                               if (laccelerationMax != accelerationMax)
                               {
                                   rtAccelerationMax.Angle = (accelerationMax * 270);
                                   MAX_G_needle.RenderTransform = rtAccelerationMax;
                               }
                               if (laccelerationMin != accelerationMin)
                               {
                                   rtAccelerationMin.Angle = (accelerationMin * -60);
                                   MIN_G_needle.RenderTransform = rtAccelerationMin;
                               }
                               lacceleration = acceleration;
                               laccelerationMax = accelerationMax;
                               laccelerationMin = accelerationMin;
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
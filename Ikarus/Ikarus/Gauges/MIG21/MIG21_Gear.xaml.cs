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
    /// Interaktionslogik für MIG21_Gear.xaml
    /// </summary>
    public partial class MIG21_Gear : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double gearNoseUp = 0.0;
        double gearNoseDown = 0.0;
        double gearLeftUp = 0.0;
        double gearLeftDown = 0.0;
        double gearRightUp = 0.0;
        double gearRightDown = 0.0;
        double airBrake = 0.0;
        double flaps = 0.0;
        double checkGear = 0.0;

        public MIG21_Gear()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            GEAR_NOSE_UP_LIGHT.Visibility = System.Windows.Visibility.Hidden;
            GEAR_NOSE_DOWN_LIGHT.Visibility = System.Windows.Visibility.Hidden;
            GEAR_LEFT_UP_LIGHT.Visibility = System.Windows.Visibility.Hidden;
            GEAR_LEFT_DOWN_LIGHT.Visibility = System.Windows.Visibility.Hidden;
            GEAR_RIGHT_UP_LIGHT.Visibility = System.Windows.Visibility.Hidden;
            GEAR_RIGHT_DOWN_LIGHT.Visibility = System.Windows.Visibility.Hidden;
            AIRBRAKE_LIGHT.Visibility = System.Windows.Visibility.Hidden;
            FLAPS_LIGHT.Visibility = System.Windows.Visibility.Hidden;
            CHECK_GEAR_LIGHT.Visibility = System.Windows.Visibility.Hidden;
        }

        public System.Windows.Size Size
        {
            get { return new System.Windows.Size(176, 88); }
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
            return 176.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { gearNoseUp = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { gearNoseDown = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { gearLeftUp = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { gearLeftDown = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { gearRightUp = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { gearRightDown = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { airBrake = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }
                               if (vals.Length > 7) { flaps = Convert.ToDouble(vals[7], CultureInfo.InvariantCulture); }
                               if (vals.Length > 8) { checkGear = Convert.ToDouble(vals[8], CultureInfo.InvariantCulture); }

                               GEAR_NOSE_UP_LIGHT.Visibility = (gearNoseUp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               GEAR_NOSE_DOWN_LIGHT.Visibility = (gearNoseDown > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               GEAR_LEFT_UP_LIGHT.Visibility = (gearLeftUp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               GEAR_LEFT_DOWN_LIGHT.Visibility = (gearLeftDown > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               GEAR_RIGHT_UP_LIGHT.Visibility = (gearRightUp > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               GEAR_RIGHT_DOWN_LIGHT.Visibility = (gearRightDown > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               AIRBRAKE_LIGHT.Visibility = (airBrake > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               FLAPS_LIGHT.Visibility = (flaps > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               CHECK_GEAR_LIGHT.Visibility = (checkGear > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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
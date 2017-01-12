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
    /// Interaction logic for UH1_RadioCompassIndicator.xaml
    /// </summary>
    public partial class UH1_RadioCompassIndicator : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double coursePointer1 = 0.0;
        double coursePointer2 = 0.0;
        double heading = 0.0;
        double annunciator = 0.0;
        double powerFail = 0.0;

        double lcoursePointer1 = 0.0;
        double lcoursePointer2 = 0.0;
        double lheading = 0.0;
        double lannunciator = 0.0;
        double lpowerFail = 0.0;

        RotateTransform rtHeading = new RotateTransform();
        RotateTransform rtCoursePointer1 = new RotateTransform();
        RotateTransform rtCoursePointer2 = new RotateTransform();
        RotateTransform rtPowerFail = new RotateTransform();
        RotateTransform rtAnnunciator = new RotateTransform();

        public UH1_RadioCompassIndicator()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            rtPowerFail.Angle = 5;
            RMI_Off_Flagg.RenderTransform = rtPowerFail;
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

                               if (vals.Length > 0) { coursePointer1 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { coursePointer2 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { heading = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { annunciator = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { powerFail = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }

                               if (lheading != heading)
                               {
                                   rtHeading.Angle = heading * -360;
                                   RMI_Heading.RenderTransform = rtHeading;
                               }

                               if (lcoursePointer1 != coursePointer1)
                               {
                                   //rtCoursePointer1.Angle = coursePointer1 == 0.0 ? 0 : (coursePointer1 * 360) + rtHeading.Angle - 59.94; // Offset 0.1665
                                   rtCoursePointer1.Angle = coursePointer1 * 360;
                                   RMI_CoursePointer1.RenderTransform = rtCoursePointer1;
                               }

                               if (lcoursePointer2 != coursePointer2)
                               {
                                   //rtCoursePointer2.Angle = coursePointer2 == 0.0 ? 0 : (coursePointer2 * 360) + rtHeading.Angle - 59.94; // Offset 0.1665
                                   rtCoursePointer2.Angle = coursePointer2 * 360;
                                   RMI_CoursePointer2.RenderTransform = rtCoursePointer2;
                               }

                               if (lpowerFail != powerFail)
                               {
                                   rtPowerFail.Angle = (powerFail * -20) + 5;
                                   RMI_Off_Flagg.RenderTransform = rtPowerFail;
                               }

                               if (lannunciator != annunciator)
                               {
                                   rtAnnunciator.Angle = (annunciator * -12) + 6;
                                   Annunciator1.RenderTransform = rtAnnunciator;
                               }

                               lcoursePointer1 = coursePointer1;
                               lcoursePointer2 = coursePointer2;
                               lheading = heading;
                               lannunciator = annunciator;
                               lpowerFail = powerFail;
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

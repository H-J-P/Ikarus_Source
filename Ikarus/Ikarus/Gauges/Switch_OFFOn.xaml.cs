﻿using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Globalization;
using System.Data;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Switch_OFFOn : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private string pictureOn = "";
        private string pictureOff = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        public Switch_OFFOn()
        {
            InitializeComponent();
            Focusable = false;

            DesignFrame.Visibility = System.Windows.Visibility.Hidden;

            if (MainWindow.editmode)
            {
                MakeDraggable(this, this);
                DesignFrame.StrokeThickness = 2.0;
                DesignFrame.Visibility = System.Windows.Visibility.Visible;

                Color color = Color.FromArgb(90, 255, 0, 0);
                UpperRec.StrokeThickness = 2.0;
                UpperRec.Stroke = new SolidColorBrush(color);
            }

            SwitchUp.Visibility = System.Windows.Visibility.Hidden;
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
        }

        public string GetID() { return dataImportID; }

        public void SetInput(string _input)
        {

        }

        public void SwitchLight(bool _on)
        {
            //Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }
        public void SetOutput(string _output)
        {
        }

        private void LoadBmaps()
        {
            if (dataImportID != "")
            {
                dataRows = MainWindow.dtSwitches.Select("ID=" + dataImportID);

                if (dataRows.Length > 0)
                {
                    pictureOn = dataRows[0]["FilePictureOn"].ToString();
                    pictureOff = dataRows[0]["FilePictureOff"].ToString();
                }

                try
                {
                    SwitchUp.Source = null;
                    SwitchDown.Source = null;

                    if (File.Exists(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureOn))
                        SwitchUp.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureOn));

                    if (File.Exists(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureOn))
                        SwitchDown.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureOff));
                }
                catch { }
            }
        }

        public double GetSize()
        {
            return 70; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               double switchState = 0.0;

                               if (vals.Length > 0) { switchState = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               if (switchState > 0.8) { SetValue(1.0, false); }
                               else { SetValue(0.0, false); }
                           }
                           catch { return; };
                       }));
        }

        private void SetValue(double _value, bool _event, bool dontReset = false)
        {
            try
            {
                Switches switches = MainWindow.switches.Find(x => x.ID == Convert.ToInt32(dataImportID));

                if (switches != null)
                {
                    switches.value = _value;
                    switches.events = _event;
                    switches.dontReset = dontReset;
                }

                if (_value > 0.0)
                {
                    SwitchUp.Visibility = System.Windows.Visibility.Visible;
                    SwitchDown.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    if (_value < 1.0)
                    {
                        if (switches != null)
                        {
                            switches.value = 0.0;
                            switches.oldValue = 1.0;
                        }
                    }
                    SwitchDown.Visibility = System.Windows.Visibility.Visible;
                    SwitchUp.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            catch { return; }
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

            movedByElement.MouseLeftButtonUp += (a, b) => isMousePressed = false;
            movedByElement.MouseLeave += (a, b) => isMousePressed = false;

            movedByElement.MouseMove += (a, b) =>
            {
                if (!isMousePressed || !MainWindow.editmode) return;

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                trUsercontrol.X += currentPoint.X - originalPoint.X;
                trUsercontrol.Y += currentPoint.Y - originalPoint.Y;
                moveThisElement.RenderTransform = trUsercontrol;

                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "ID", MainWindow.dtSwitches, dataImportID);
            };
        }

        //----- OFF
        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "ID", MainWindow.dtSwitches, dataImportID, e.Delta);
        }

        private void UpperRec_TouchDown(object sender, TouchEventArgs e)
        {
            SetValue(1.0, true, true);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void UpperRec_TouchUp(object sender, TouchEventArgs e)
        {
            SetValue(0.0, true, false);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void UpperRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetValue(1.0, true, true);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void UpperRec_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SetValue(0.0, true, false);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }
    }
}

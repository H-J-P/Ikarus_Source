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
    public partial class ButtonPanelOffOn : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private string pictureOn = "";
        private string pictureOff = "";
        private int windowID = 0;
        private int panelID = 0;
        private int _on = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        public ButtonPanelOffOn()
        {
            InitializeComponent();
            Focusable = false;


            DesignFrame.Visibility = System.Windows.Visibility.Hidden;

            if (MainWindow.editmode)
            {
                MakeDraggable(this, this);
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
        }

        public void SetOutput(string _output)
        {
        }

        private void LoadBmaps()
        {
            if (dataImportID != "")
            {
                dataRows = MainWindow.dtSwitches.Select("ID=" + dataImportID);

                try
                {
                    panelID = Convert.ToInt16(dataRows[0]["Input"]);
                    _on = Convert.ToInt16(dataRows[0]["Output"]);
                }
                catch 
                { 
                    panelID = 0;
                    _on = 0;
                }

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

                if (_on == 1)
                {
                    if (MainWindow.cockpitWindows.Count > panelID) MainWindow.cockpitWindows[panelID].Visibility = System.Windows.Visibility.Visible;
                    SwitchUp.Visibility = System.Windows.Visibility.Visible;
                    SwitchDown.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    if (MainWindow.cockpitWindows.Count > panelID) MainWindow.cockpitWindows[panelID].Visibility = System.Windows.Visibility.Hidden;
                    SwitchUp.Visibility = System.Windows.Visibility.Hidden;
                    SwitchDown.Visibility = System.Windows.Visibility.Visible;
                }
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
                           vals = strData.Split(';');

                           double switchState = 0.0;

                           try
                           {
                               if (vals.Length > 0) { switchState = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                           }
                           catch { return; };

                           if (switchState > 0.8) { SetValue(1.0, false); }
                           else { SetValue(0.0, false); }
                       }));
        }

        private void SetValue(double _value, bool _event)
        {
            try
            {
                dataRows = MainWindow.dtSwitches.Select("ID=" + dataImportID);

                if (dataRows.Length > 0)
                {
                    try
                    {
                        panelID = Convert.ToInt16(dataRows[0]["Input"]) - 1;
                    }
                    catch { panelID = 99999; }

                    if (panelID < 0) panelID = 99999;
                }

                if (_value > 0.0)
                {
                    SwitchUp.Visibility = System.Windows.Visibility.Visible;
                    SwitchDown.Visibility = System.Windows.Visibility.Hidden;
                    if (MainWindow.cockpitWindows.Count > panelID) MainWindow.cockpitWindows[panelID].Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    SwitchUp.Visibility = System.Windows.Visibility.Hidden;
                    SwitchDown.Visibility = System.Windows.Visibility.Visible;
                    if (MainWindow.cockpitWindows.Count > panelID) MainWindow.cockpitWindows[panelID].Visibility = System.Windows.Visibility.Hidden;
                }
            }
            catch { }
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

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "ID", MainWindow.dtSwitches, dataImportID, e.Delta);
        }

        private void UpperRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SwitchUp.Visibility == System.Windows.Visibility.Visible)
            {
                SetValue(0.0, true);
            }
            else
            {
                SetValue(1.0, true);
            }
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void UpperRec_TouchDown(object sender, TouchEventArgs e)
        {
            if (SwitchUp.Visibility == System.Windows.Visibility.Visible)
            {
                SetValue(0.0, true);
            }
            else
            {
                SetValue(1.0, true);            
            }
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }
    }
}

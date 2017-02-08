﻿using System;
using System.Data;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Globalization;

namespace Ikarus

{
    /// <summary>
    /// Interaction logic for MIG29_FuelQV2.xaml
    /// </summary>
    public partial class MIG29FuelQ : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double totalFuel_0_4 = 0.0;
        double totalFuel_4_5_5 = 0.0;
        double totalFuel = 0.0;
        double light1 = 0.0;
        double light2 = 0.0;
        double light3 = 0.0;
        double bingoLight = 0.0;

        public MIG29FuelQ()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            TotalFuelLong.Height = 3;
            TotalFuelShort.Height = 3;
            Light1.Visibility = System.Windows.Visibility.Hidden;
            Light2.Visibility = System.Windows.Visibility.Hidden;
            Light3.Visibility = System.Windows.Visibility.Hidden;
            BingoLight.Visibility = System.Windows.Visibility.Hidden;
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
            return 136.0; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { totalFuel_0_4 = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { totalFuel_4_5_5 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { light1 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { light2 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { light3 = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { bingoLight = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }
                               if (vals.Length > 6) { totalFuel = Convert.ToDouble(vals[6], CultureInfo.InvariantCulture); }

                               Light1.Visibility = (light1 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Light2.Visibility = (light2 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Light3.Visibility = (light3 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               BingoLight.Visibility = (bingoLight > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               if (totalFuel_0_4 < 0) totalFuel_0_4 = 0;
                               if (totalFuel_4_5_5 < 0) totalFuel_4_5_5 = 0;

                               TotalFuelLong.Height = totalFuel_0_4 * 179;
                               TotalFuelShort.Height = totalFuel_4_5_5 * 69;

                               //string sTotalFuel = Convert.ToInt16(totalFuel).ToString();

                               //if (sTotalFuel.Length == 0) { sTotalFuel = "000"; }
                               //else if (sTotalFuel.Length == 1) { sTotalFuel = "00" + sTotalFuel; }
                               //else if (sTotalFuel.Length == 2) { sTotalFuel = "0" + sTotalFuel; }

                               //double totalFuel_M = Convert.ToDouble(sTotalFuel[0].ToString(), CultureInfo.InvariantCulture);
                               //double totalFuel_C = Convert.ToDouble(sTotalFuel[1].ToString(), CultureInfo.InvariantCulture);
                               //double totalFuel_X = Convert.ToDouble(sTotalFuel[2].ToString(), CultureInfo.InvariantCulture);

                               //TranslateTransform ttTotalFuel_X = new TranslateTransform();
                               //TranslateTransform ttTotalFuel_C = new TranslateTransform();
                               //TranslateTransform ttTotalFuel_M = new TranslateTransform();

                               //ttTotalFuel_X.Y = totalFuel_X * -152;
                               //TotalFuel_X.RenderTransform = ttTotalFuel_X;

                               //ttTotalFuel_C.Y = totalFuel_C * -152;
                               //TotalFuel_C.RenderTransform = ttTotalFuel_C;

                               //ttTotalFuel_M.Y = totalFuel_M * -152;
                               //TotalFuel_M.RenderTransform = ttTotalFuel_M;
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

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
    /// Interaction logic for SU33FuelQ.xaml
    /// </summary>
    public partial class SU33FuelQ : UserControl, I_Ikarus
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        double totalFuel = 0.0;
        double light1 = 0.0;
        double light2 = 0.0;
        double light3 = 0.0;
        double light4 = 0.0;
        double bingoLight = 0.0;

        double totalFuel_M = 0.0;
        double totalFuel_C = 0.0;
        double totalFuel_X = 0.0;

        double ltotalFuel_M = 0.0;
        double ltotalFuel_C = 0.0;
        double ltotalFuel_X = 0.0;

        TranslateTransform ttTotalFuel_X = new TranslateTransform();
        TranslateTransform ttTotalFuel_C = new TranslateTransform();
        TranslateTransform ttTotalFuel_M = new TranslateTransform();

        public SU33FuelQ()
        {
            InitializeComponent();

            if (MainWindow.editmode) MakeDraggable(this, this);

            TotalFuel1.Height = 3;

            Light1.Visibility = System.Windows.Visibility.Hidden;
            Light2.Visibility = System.Windows.Visibility.Hidden;
            Light3.Visibility = System.Windows.Visibility.Hidden;
            Light4.Visibility = System.Windows.Visibility.Hidden;
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

                               if (vals.Length > 0) { totalFuel = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                               if (vals.Length > 1) { light1 = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture); }
                               if (vals.Length > 2) { light2 = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture); }
                               if (vals.Length > 3) { light3 = Convert.ToDouble(vals[3], CultureInfo.InvariantCulture); }
                               if (vals.Length > 4) { light4 = Convert.ToDouble(vals[4], CultureInfo.InvariantCulture); }
                               if (vals.Length > 5) { bingoLight = Convert.ToDouble(vals[5], CultureInfo.InvariantCulture); }

                               Light1.Visibility = (light1 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Light2.Visibility = (light2 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Light3.Visibility = (light3 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               Light4.Visibility = (light4 > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                               BingoLight.Visibility = (bingoLight > 0.8) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                               if (totalFuel < 0) { totalFuel = 0; }

                               TotalFuel1.Height = (totalFuel * 200) + 3;

                               string sTotalFuel = Convert.ToInt16(totalFuel * 12000).ToString();

                               if (sTotalFuel.Length == 0) { sTotalFuel = "000"; }
                               else if (sTotalFuel.Length == 1) { sTotalFuel = "00" + sTotalFuel; }
                               else if (sTotalFuel.Length == 2) { sTotalFuel = "0" + sTotalFuel; }

                               totalFuel_M = Convert.ToDouble(sTotalFuel[0].ToString(), CultureInfo.InvariantCulture);
                               totalFuel_C = Convert.ToDouble(sTotalFuel[1].ToString(), CultureInfo.InvariantCulture);
                               totalFuel_X = Convert.ToDouble(sTotalFuel[2].ToString(), CultureInfo.InvariantCulture);

                               if (ltotalFuel_X != totalFuel_X)
                               {
                                   ttTotalFuel_X.Y = totalFuel_X * -16.8;
                                   TotalFuel_X.RenderTransform = ttTotalFuel_X;
                               }
                               if (ltotalFuel_C != totalFuel_C)
                               {
                                   ttTotalFuel_C.Y = totalFuel_C * -16.8;
                                   TotalFuel_C.RenderTransform = ttTotalFuel_C;
                               }
                               if (ltotalFuel_M != totalFuel_M)
                               {
                                   ttTotalFuel_M.Y = totalFuel_M * -16.8;
                                   TotalFuel_M.RenderTransform = ttTotalFuel_M;
                               }
                               ltotalFuel_M = totalFuel_M;
                               ltotalFuel_C = totalFuel_C;
                               ltotalFuel_X = totalFuel_X;
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
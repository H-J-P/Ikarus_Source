using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ikarus
{
    public class GaugesHelper
    {
        private string dataImportID = "";
        private int windowID = 0;
        private string tableName;

        public GaugesHelper(string _dataImportID, int _windowID, string _tableName)
        {
            dataImportID = _dataImportID;
            windowID = _windowID;
            tableName = _tableName;
        }

        public void MakeDraggable(System.Windows.UIElement moveThisElement, System.Windows.UIElement movedByElement)
        {
            System.Windows.Point originalPoint = new System.Windows.Point(0, 0), currentPoint;
            TranslateTransform trUsercontrol = new TranslateTransform(0, 0);
            bool isMousePressed = false;
            bool isRotated = false;

            movedByElement.MouseLeftButtonDown += (a, b) =>
            {
                isMousePressed = true;
                originalPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                isRotated = MainWindow.cockpitWindows[windowID].UpdatePosition(moveThisElement.PointToScreen(new System.Windows.Point(0, 0)), tableName, dataImportID);
            };

            movedByElement.MouseLeftButtonUp += (a, b) =>
            {
                isMousePressed = false;
                isRotated = MainWindow.cockpitWindows[windowID].UpdatePosition(moveThisElement.PointToScreen(new System.Windows.Point(0, 0)), tableName, dataImportID);
            };
            movedByElement.MouseLeave += (a, b) => isMousePressed = false;

            movedByElement.MouseMove += (a, b) =>
            {
                if (!isMousePressed) return;

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                trUsercontrol.X += currentPoint.X - originalPoint.X;
                trUsercontrol.Y += currentPoint.Y - originalPoint.Y;
                moveThisElement.RenderTransform = trUsercontrol;
            };
        }

        public void MakeDraggable_New(System.Windows.UIElement moveThisElement, System.Windows.UIElement movedByElement)
        {
            System.Windows.Point originalPoint = new System.Windows.Point(0, 0), currentPoint;
            TranslateTransform trUsercontrol = new TranslateTransform(0, 0);
            System.Windows.Point pointToScreen = new System.Windows.Point(0, 0);
            bool isMouseLeftButtonDown = false;
            bool isRotated = false;

            movedByElement.MouseLeftButtonDown += (a, b) =>
            {
                b.Handled = true;

                isMouseLeftButtonDown = true;
                originalPoint = b.GetPosition(moveThisElement);
            };

            movedByElement.MouseLeftButtonUp += (a, b) =>
            {
                b.Handled = true;


                pointToScreen = moveThisElement.PointToScreen(new System.Windows.Point(0, 0));
                currentPoint = b.GetPosition(moveThisElement);

                //pointToScreen.X += currentPoint.X - originalPoint.X;
                //pointToScreen.Y += currentPoint.Y - originalPoint.Y;

                //isRotated = MainWindow.cockpitWindows[windowID].UpdatePosition(moveThisElement.PointToScreen(new System.Windows.Point(0, 0)), tableName, dataImportID); // Select it in tab
                isRotated = MainWindow.cockpitWindows[windowID].UpdatePosition(pointToScreen, tableName, dataImportID); // Select it in tab

                isMouseLeftButtonDown = false;
            };

            movedByElement.MouseLeave += (a, b) =>
            {
                b.Handled = true;

                isMouseLeftButtonDown = false;
            };

            movedByElement.MouseMove += (a, b) =>
            {
                b.Handled = true;

                if (isMouseLeftButtonDown)
                {
                    currentPoint = b.GetPosition(moveThisElement);
                    //trUsercontrol.X += currentPoint.X * 0.75 - originalPoint.X;
                    //trUsercontrol.Y += currentPoint.Y * 0.75 - originalPoint.Y;
                    trUsercontrol.X += currentPoint.X - originalPoint.X;
                    trUsercontrol.Y += currentPoint.Y - originalPoint.Y;
                    moveThisElement.RenderTransform = trUsercontrol;
                }
            };
        }

        public void LoadBmaps(Image frame, Image light)
        {
            DataRow[] dataRows = MainWindow.dtInstruments.Select("IDInst=" + dataImportID);

            if (dataRows.Length > 0)
            {
                string frameName = dataRows[0]["ImageFrame"].ToString();
                string lightName = dataRows[0]["ImageLight"].ToString();

                try
                {
                    if (frameName.Length > 4)
                    {
                        if (!File.Exists(Environment.CurrentDirectory + "\\Images\\Frames\\" + frameName))
                            frameName = "-";
                    }
                    else
                    {
                        frameName = "-";
                    }
                    if (lightName.Length > 4)
                    {
                        if (!File.Exists(Environment.CurrentDirectory + "\\Images\\Frames\\" + lightName))
                            lightName = "-";
                    }
                    else
                    {
                        lightName = "-";
                    }
                    if (frameName.Length > 4)
                        frame.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + frameName));

                    if (lightName.Length > 4)
                        light.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Frames\\" + lightName));
                }
                catch { }
            }
        }

        public void SetInput(ref string _input, ref double[] valueScale, ref int valueScaleIndex, int limit)
        {
            string[] vals = _input.Split(',');

            if (vals.Length < limit) return;

            valueScale = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                valueScale[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
            valueScaleIndex = vals.Length;
        }

        public void SetOutput(ref string _output, ref double[] degreeDial, int limit)
        {
            string[] vals = _output.Split(',');

            if (vals.Length < limit) return;

            degreeDial = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                degreeDial[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
        }
    }
}

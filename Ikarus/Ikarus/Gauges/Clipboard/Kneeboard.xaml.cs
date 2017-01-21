using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Globalization;
using System.Data;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for Kneeboard.xaml
    /// </summary>
    public partial class Kneeboard : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private string[] files = new string[] { };
        private string bmapName = "";
        private List<string> filenames = new List<string>();
        private int windowID = 0;
        private int fileNumber = 0;
        private bool touchDown = false;
        private string pathToPicture = "";
        BitmapImage bitmapImage = new BitmapImage();

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }
        public Kneeboard()
        {
            InitializeComponent();
            Focusable = false;
            pathToPicture = Environment.CurrentDirectory + "\\Kneeboards\\" + MainWindow.readFile + "\\";

            DirectoryInfo directoryInfo = new DirectoryInfo(pathToPicture);

            foreach (var file in directoryInfo.GetFiles("*.png"))
            {
                filenames.Add(file.Name);
            }
            foreach (var file in directoryInfo.GetFiles("*.jpg"))
            {
                filenames.Add(file.Name);
            }

            files = filenames.ToArray();
            Array.Sort(files);
            LoadPicture(fileNumber);

            DesignFrame.Visibility = System.Windows.Visibility.Hidden;

            if (MainWindow.editmode)
            {
                MakeDraggable(this, this);
                DesignFrame.StrokeThickness = 1.0;
                DesignFrame.Visibility = System.Windows.Visibility.Visible;
                Color color = Color.FromArgb(90, 255, 0, 0);
                RightRec.StrokeThickness = 1.0;
                RightRec.Stroke = new SolidColorBrush(color);
                LeftRec.StrokeThickness = 1.0;
                LeftRec.Stroke = new SolidColorBrush(color);
            }
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
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
        }

        public double GetSize()
        {
            return DesignFrame.Width;
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {

                       }));
        }

        private void LoadPicture(int pictureIndex)
        {
            try
            {
                bmapName = files.GetValue(pictureIndex).ToString();

                if (File.Exists(pathToPicture + bmapName))
                {
                    Picture.Source = null;

                    bitmapImage = new BitmapImage(new Uri(pathToPicture + bmapName));
                    bitmapImage.DecodePixelHeight = Convert.ToInt32(DesignFrame.Height);
                    bitmapImage.DecodePixelWidth = Convert.ToInt32(DesignFrame.Width);

                    Picture.Source = bitmapImage;
                    //Picture.Stretch = Stretch.None;
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
                b.Handled = true;
                if (!touchDown)
                {
                    isMousePressed = true;
                    originalPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                }
            };

            movedByElement.MouseLeftButtonUp += (a, b) =>
            {
                b.Handled = true;
                isMousePressed = false;
                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID);
            };
            movedByElement.MouseLeave += (a, b) =>
            {
                b.Handled = true;
                isMousePressed = false;
            };
            movedByElement.MouseMove += (a, b) =>
            {
                b.Handled = true;

                if (!isMousePressed || !MainWindow.editmode) return;

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                trUsercontrol.X += currentPoint.X - originalPoint.X;
                trUsercontrol.Y += currentPoint.Y - originalPoint.Y;
                moveThisElement.RenderTransform = trUsercontrol;
            };
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            e.Handled = true;
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }

        private void RightRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (!touchDown)
            {
                fileNumber++;
                if (fileNumber > files.Length - 1) fileNumber = files.Length - 1;
                LoadPicture(fileNumber);
            }
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void RightRec_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = true;

            fileNumber++;
            if (fileNumber > files.Length - 1) fileNumber = files.Length - 1;
            LoadPicture(fileNumber);

            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void RightRec_TouchUp(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = false;
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LeftRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!touchDown)
            {
                fileNumber--;
                if (fileNumber < 0) fileNumber = 0;
                LoadPicture(fileNumber);
            }
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LeftRec_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = true;

            fileNumber--;
            if (fileNumber < 0) fileNumber = 0;
            LoadPicture(fileNumber);

            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LeftRec_TouchUp(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = false;
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }
    }
}

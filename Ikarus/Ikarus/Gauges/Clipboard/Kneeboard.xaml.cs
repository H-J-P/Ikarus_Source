using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for Kneeboard.xaml
    /// </summary>
    public partial class Kneeboard : UserControl, I_Ikarus
    {
        private bool touchDown = false;
        private bool leftMousePressed = false;
        private double deltaRotate = 90;
        private double deltaScale = 0.025;
        private double diffX = 0.0;
        private double diffY = 0.0;
        private double rotatePicture = 0.0;
        private double scalePicture = 1.0;

        private int windowID = 0;
        private int fileNumber = 0;

        private string dataImportID = "";
        private string[] files = new string[] { };
        private string bmapName = "";
        private string pathToPicture = "";

        private DataRow[] dataRows = new DataRow[] { };
        private List<string> filenames = new List<string>();
        private Point lastTouchDownPoint;
        private TransformGroup transformGroup = new TransformGroup();
        private TranslateTransform translateTransform = new TranslateTransform();
        private ScaleTransform transformScale = new ScaleTransform();
        private RotateTransform rotateTransform = new RotateTransform();
        private BitmapImage bitmapImage = new BitmapImage();
        private DirectoryInfo directoryInfo = null;

        public void SetWindowID(int _windowID) { windowID = _windowID; }

        public int GetWindowID() { return windowID; }

        public Kneeboard()
        {
            InitializeComponent();
            Focusable = false;

            // Load checklists
            pathToPicture = Environment.CurrentDirectory + "\\Kneeboards\\Mods\\" + MainWindow.readFile + "\\";
            LoadDirectoryInfo(pathToPicture);
            filenames.Sort();

            // Load airport charts
            if (MainWindow.map == "")
            {
                pathToPicture = Environment.CurrentDirectory + "\\Kneeboards\\AirportCharts\\CaucasusBase\\";
            }
            else
            {
                pathToPicture = Environment.CurrentDirectory + "\\Kneeboards\\AirportCharts\\" + MainWindow.map + "\\";
            }
            LoadDirectoryInfo(pathToPicture);

            files = filenames.ToArray();
            LoadPicture(fileNumber);

            DesignFrame.Visibility = Visibility.Hidden;

            if (MainWindow.editmode)
            {
                MakeDraggable(this, this);
                DesignFrame.StrokeThickness = 1.0;
                DesignFrame.Visibility = Visibility.Visible;
                Color color = Color.FromArgb(90, 255, 0, 0);

                RightRec.StrokeThickness = 1.0;
                RightRec.Stroke = new SolidColorBrush(color);
                LeftRec.StrokeThickness = 1.0;
                LeftRec.Stroke = new SolidColorBrush(color);

                ZoomPlus.StrokeThickness = 1.0;
                ZoomPlus.Stroke = new SolidColorBrush(color);
                ZoomMinus.StrokeThickness = 1.0;
                ZoomMinus.Stroke = new SolidColorBrush(color);

                RotatePlus.StrokeThickness = 1.0;
                RotatePlus.Stroke = new SolidColorBrush(color);
                RotateMinus.StrokeThickness = 1.0;
                RotateMinus.Stroke = new SolidColorBrush(color);
            }
        }

        private void LoadDirectoryInfo(string pathToPicture)
        {
            if (Directory.Exists(pathToPicture))
            {
                directoryInfo = new DirectoryInfo(pathToPicture);
                try
                {
                    foreach (var file in directoryInfo.GetFiles("*.png"))
                    {
                        if (file.Name.IndexOf("._") == -1)
                            filenames.Add(pathToPicture + file.Name);
                    }
                }
                catch { }
                try
                {
                    foreach (var file in directoryInfo.GetFiles("*.jpg"))
                    {
                        if (file.Name.IndexOf("._") == -1)
                            filenames.Add(pathToPicture + file.Name);
                    }
                }
                catch { }
            }
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
            Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void SetOutput(string _output)
        {
        }

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

        public double GetSize()
        {
            return DesignFrame.Width;
        }

        public double GetSizeY()
        {
            return DesignFrame.Height;
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

                if (File.Exists(bmapName))
                {
                    Picture.Source = null;

                    bitmapImage = new BitmapImage(new Uri(bmapName));
                    bitmapImage.DecodePixelHeight = Convert.ToInt32(DesignFrame.Height);
                    bitmapImage.DecodePixelWidth = Convert.ToInt32(DesignFrame.Width);

                    Picture.Source = bitmapImage;
                }
            }
            catch { }
        }

        private void MakeDraggable(UIElement moveThisElement, UIElement movedByElement)
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

        private void MakeTransforms()
        {
            transformGroup = new TransformGroup();

            rotateTransform.CenterX = Picture.Width / 2;
            rotateTransform.CenterY = Picture.Height / 2;
            rotateTransform = new RotateTransform(rotatePicture);

            if (rotatePicture < 0) { rotatePicture  = 270; }
            if (rotatePicture > 270) { rotatePicture = 0; }

            if (rotatePicture  == 0)
                translateTransform = new TranslateTransform(diffX, diffY);
            if (rotatePicture == 90)
                translateTransform = new TranslateTransform(diffY, -diffX);
            if (rotatePicture == 180)
                translateTransform = new TranslateTransform(-diffX, -diffY);
            if (rotatePicture == 270)
                translateTransform = new TranslateTransform(-diffY, diffX);

            transformScale = new ScaleTransform(scalePicture, scalePicture);

            transformGroup.Children.Add(translateTransform);
            transformGroup.Children.Add(transformScale);
            transformGroup.Children.Add(rotateTransform);

            Picture.RenderTransform = transformGroup;

            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        #region Events
        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            e.Handled = true;
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }

        #region Change pages
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
        #endregion

        #region Move
        private void Picture_TouchMove(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            Point currentPoint = e.GetTouchPoint(this).Position;
            diffX = currentPoint.X - lastTouchDownPoint.X;
            diffY = currentPoint.Y - lastTouchDownPoint.Y;

            if (Math.Abs(diffX) > 0 || Math.Abs(diffY) > 0)
            {
                MakeTransforms();
            }
        }

        private void Picture_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = true;
            lastTouchDownPoint = e.GetTouchPoint(Picture).Position;
        }

        private void Picture_TouchUp(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = false;
        }

        private void Picture_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!touchDown)
            {
                leftMousePressed = true;
                lastTouchDownPoint = e.GetPosition(this);
            }
        }

        private void Picture_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            leftMousePressed = false;
        }

        private void Picture_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            if (!touchDown)
            {
                if (leftMousePressed)
                {
                    System.Windows.Point currentPoint = e.GetPosition(this);

                    diffX = currentPoint.X - lastTouchDownPoint.X;
                    diffY = currentPoint.Y - lastTouchDownPoint.Y;

                    if (Math.Abs(diffX) > 0 || Math.Abs(diffY) > 0)
                    {
                        MakeTransforms();
                    }
                }
            }
        }
        #endregion

        #region Rotate
        private void RotatePlus_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = true;
            rotatePicture += deltaRotate;
            MakeTransforms();
        }

        private void RotatePlus_TouchUp(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = false;
        }

        private void RotatePlus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!touchDown)
            {
                rotatePicture += deltaRotate;
                MakeTransforms();
            }
        }

        private void RotateMinus_TouchUp(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = false;
        }

        private void RotateMinus_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = true;
            rotatePicture -= deltaRotate;
            MakeTransforms();
        }

        private void RotateMinus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!touchDown)
            {
                rotatePicture -= deltaRotate;
                MakeTransforms();
            }
        }
        #endregion

        #region Zoom
        private void Picture_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            if (!touchDown)
            {
                if (!MainWindow.editmode)
                {
                    if (e.Delta < 0)
                        scalePicture -= deltaScale;
                    else
                        scalePicture += deltaScale;

                    MakeTransforms();
                }
            }
        }

        private void ZoomPlus_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = true;
            scalePicture += 0.025;
            MakeTransforms();
        }

        private void ZoomPlus_TouchUp(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = false;
        }

        private void ZoomPlus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!touchDown)
            {
                scalePicture += deltaScale;
                MakeTransforms();
            }
        }

        private void ZoomMinus_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = true;
            scalePicture -= deltaScale;
            MakeTransforms();
        }

        private void ZoomMinus_TouchUp(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = false;
        }

        private void ZoomMinus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!touchDown)
            {
                scalePicture -= deltaScale;
                MakeTransforms();
            }
        }
        #endregion

        #endregion
    }
}

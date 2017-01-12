using System;
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
    public partial class ButtonWithRepeat : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private string pictureOn = "";
        private string pictureOff = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        private double[] input = new double[] { };
        private double[] output = new double[] { };
        private double switchValue = 0.0;
        private int state = 0;
        private Switches switches = null;
        private bool touchDown = false;
        BitmapImage bitmapImage = new BitmapImage();

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        public ButtonWithRepeat()
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

            switches = MainWindow.switches.Find(x => x.ID == Convert.ToInt32(dataImportID));
        }

        public string GetID() { return dataImportID; }

        public void SetInput(string _input)
        {
            string[] vals = _input.Split(',');
            input = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                input[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
        }

        public void SetOutput(string _output)
        {
            string[] vals = _output.Split(',');
            output = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                output[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
        }

        public void SwitchLight(bool _on)
        {
            //Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }
        private void SetContour()
        {
            int bitmapHeight = bitmapImage.PixelHeight / 2; // Jumping Jack
            int bitmapWidth = bitmapImage.PixelWidth / 2;

            DesignFrame.Height = bitmapHeight;
            DesignFrame.Width = bitmapWidth;

            SwitchUp.Height = bitmapHeight;
            SwitchUp.Width = bitmapWidth;

            SwitchDown.Height = bitmapHeight;
            SwitchDown.Width = bitmapWidth;

            UpperRec.Height = bitmapHeight;
            UpperRec.Width = bitmapWidth;
        }

        private void LoadBmaps()
        {
            if (dataImportID != "")
            {
                dataRows = MainWindow.dtSwitches.Select("ID=" + dataImportID);
                bool findPictureOn = false;

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
                    {
                        bitmapImage = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureOn));
                        SetContour();
                        SwitchUp.Source = bitmapImage;

                        findPictureOn = true;
                    }

                    if (File.Exists(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureOff))
                    {
                        if (findPictureOn)
                        {
                            SwitchDown.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureOff));
                        }
                        else
                        {
                            bitmapImage = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureOff));
                            SetContour();
                            SwitchDown.Source = bitmapImage;
                        }
                    }
                }
                catch { }
            }
        }

        public double GetSize()
        {
            return SwitchUp.Width; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               if (switches.ignoreNextPackage)
                               {
                                   switches.ignoreNextPackage = false;
                                   MainWindow.getAllDscData = true;
                                   return;
                               }

                               vals = strData.Split(';');

                               if (vals.Length > 0) { switchValue = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               for (int i = 0; i < input.Length; i++)
                               {
                                   if (input[i] == switchValue)
                                   {
                                       state = i;

                                       if (state > 0)
                                       {
                                           SwitchUp.Visibility = System.Windows.Visibility.Visible;
                                           SwitchDown.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       else
                                       {
                                           SwitchDown.Visibility = System.Windows.Visibility.Visible;
                                           SwitchUp.Visibility = System.Windows.Visibility.Hidden;
                                       }
                                       break;
                                   }
                               }
                           }
                           catch { return; };
                       }));
        }

        private void SetValue(int _state)
        {
            try
            {
                //Switches switches = MainWindow.switches.Find(x => x.ID == Convert.ToInt32(dataImportID));

                if (switches == null) return;

                switches.value = output[_state];
                switches.oldValue = _state == 1 ? output[0] : output[1]; // Send 0.0 one time
                switches.events = true;
                switches.dontReset = _state == 1; // only repeat with value 1.0. Not with value 0.0

                if (_state > 0)
                {
                    SwitchUp.Visibility = System.Windows.Visibility.Visible;
                    SwitchDown.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
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

            movedByElement.MouseLeftButtonUp += (a, b) =>
            {
                isMousePressed = false;
                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "ID", MainWindow.dtSwitches, dataImportID);
            };
            movedByElement.MouseLeave += (a, b) => isMousePressed = false;

            movedByElement.MouseMove += (a, b) =>
            {
                if (!isMousePressed || !MainWindow.editmode) return;

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                trUsercontrol.X += currentPoint.X - originalPoint.X;
                trUsercontrol.Y += currentPoint.Y - originalPoint.Y;
                moveThisElement.RenderTransform = trUsercontrol;

                //MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "ID", MainWindow.dtSwitches, dataImportID);
            };
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "ID", MainWindow.dtSwitches, dataImportID, e.Delta);
        }

        private void UpperRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!touchDown)
            {
                SetValue(1);
            }
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void UpperRec_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = true;
            SetValue(1);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void UpperRec_TouchUp(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = false;
            SetValue(0);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void UpperRec_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            touchDown = false;
            SetValue(0);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }
    }
}

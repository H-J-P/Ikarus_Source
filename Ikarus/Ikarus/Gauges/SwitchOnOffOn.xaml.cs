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
    public partial class SwitchOnOffOn : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private string pictureUp = "";
        private string pictureMiddle = "";
        private string pictureDown = "";
        private double state = 0.0;
        private double switchState = 0.0;
        private int windowID = 0;
        private string[] vals = new string[] { };

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        public SwitchOnOffOn()
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
                LowerRec.StrokeThickness = 2.0;
                LowerRec.Stroke = new SolidColorBrush(color);
            }

            SwitchUp.Visibility = System.Windows.Visibility.Hidden;
            SwitchDown.Visibility = System.Windows.Visibility.Hidden;
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
        }

        public string GetID() { return dataImportID; }

        public void SwitchLight(bool _on)
        {
            //Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }
        public void SetInput(string _input)
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

                if (dataRows.Length > 0)
                {
                    pictureUp = dataRows[0]["FilePictureOn"].ToString();
                    pictureMiddle = dataRows[0]["FilePictureOff"].ToString();
                    pictureDown = dataRows[0]["FilePicture2On"].ToString();
                }

                try
                {
                    SwitchUp.Source = null;
                    SwitchMiddle.Source = null;
                    SwitchDown.Source = null;

                    if (File.Exists(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureUp))
                        SwitchUp.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureUp));

                    if (File.Exists(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureMiddle))
                        SwitchMiddle.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureMiddle));

                    if (File.Exists(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureDown))
                        SwitchDown.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureDown));
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
                           vals = strData.Split(';');

                           switchState = 0.0;

                           try
                           {
                               if (vals.Length > 0) { switchState = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
                           }
                           catch { return; };

                           SetValue(2.0, false);
                       }));
        }

        private void SetValue(double _value, bool _event, bool dontReset = false)
        {
            try
            {
                if (_value == 1.0)
                {
                    state += 1.0;
                    if (state > 1.0) state = 1.0;
                }
                if (_value == 0.0)
                {
                    state -= 1.0;
                    if (state < -1.0) state = -1.0;
                }

                if (_value == 2.0)
                {
                    state = switchState;
                }

                Switches switches = MainWindow.switches.Find(x => x.ID == Convert.ToInt32(dataImportID));

                if (switches != null)
                {
                    switches.value = _value;
                    switches.events = _event;
                    switches.dontReset = dontReset;
                }

                if (state == 1.0)
                {
                    SwitchUp.Visibility = System.Windows.Visibility.Visible;
                    SwitchMiddle.Visibility = System.Windows.Visibility.Hidden;
                    SwitchDown.Visibility = System.Windows.Visibility.Hidden;
                }

                if (state == 0.0)
                {
                    SwitchUp.Visibility = System.Windows.Visibility.Hidden;
                    SwitchMiddle.Visibility = System.Windows.Visibility.Visible;
                    SwitchDown.Visibility = System.Windows.Visibility.Hidden;
                }

                if (state == -1.0)
                {
                    SwitchUp.Visibility = System.Windows.Visibility.Hidden;
                    SwitchMiddle.Visibility = System.Windows.Visibility.Hidden;
                    SwitchDown.Visibility = System.Windows.Visibility.Visible;
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

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "ID", MainWindow.dtSwitches, dataImportID, e.Delta);
        }

        private void UpperRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetValue(1.0, true);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LowerRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetValue(0.0, true);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void UpperRec_TouchDown(object sender, TouchEventArgs e)
        {
            SetValue(1.0, true);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LowerRec_TouchDown(object sender, TouchEventArgs e)
        {
            SetValue(0.0, true);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }
    }
}

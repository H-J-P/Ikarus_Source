using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Rotary : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private string pictureKnob = "";
        private string pictureBase = "";

        private double minValue = 0.0;
        private double maxValue = 1.0;
        private double step = 0.05;
        private double switchState = 0.0;
        private double oldState = 0.0;
        private double animation = 0.0;
        private string[] vals = new string[] { };
        private bool touchDown = false;

        private int windowID = 0;
        private Switches switches = null;
        private RotateTransform rtKnob = new RotateTransform();

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        public Rotary()
        {
            InitializeComponent();
            Focusable = false;
            DesignFrame.Visibility = System.Windows.Visibility.Hidden;

            if (MainWindow.editmode)
            {
                MakeDraggable(this, this);
                DesignFrame.Visibility = System.Windows.Visibility.Visible;
                Color color = Color.FromArgb(90, 255, 0, 0);
                LeftRec.StrokeThickness = 2.0;
                LeftRec.Stroke = new SolidColorBrush(color);
                RightRec.StrokeThickness = 2.0;
                RightRec.Stroke = new SolidColorBrush(color);
            }
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;

            switches = MainWindow.switches.Find(x => x.ID == Convert.ToInt32(dataImportID));
        }

        public string GetID() { return dataImportID; }

        public void SwitchLight(bool _on)
        {
            //Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void SetInput(string _input)
        {
            string[] vals = _input.Split(',');

            if (vals.Length > 0) minValue = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture);
            if (vals.Length > 1) maxValue = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture);
            if (vals.Length > 2) step = Convert.ToDouble(vals[2], CultureInfo.InvariantCulture);

            switchState = minValue;
            oldState = minValue;
        }

        public void SetOutput(string _output)
        {
            dataRows = MainWindow.dtSwitches.Select("ID=" + dataImportID);

            if (dataRows.Length > 0)
            {
                pictureKnob = dataRows[0]["FilePictureOn"].ToString();
                pictureBase = dataRows[0]["FilePictureOff"].ToString();
            }

            try
            {
                SwitchKnob.Source = null;
                SwitchBase.Source = null;

                if (File.Exists(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureKnob))
                    SwitchKnob.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureKnob));

                if (File.Exists(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureBase))
                    SwitchBase.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Switches\\" + pictureBase));
            }
            catch { }
        }

        public double GetSize()
        {
            return 100; // Width
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               if (switches == null) return;

                               if (switches.ignoreNextPackage)
                               {
                                   switches.ignoreNextPackage = false;
                                   MainWindow.getAllDscData = true;
                                   return;
                               }

                               vals = strData.Split(';');

                               if (vals.Length > 0) { switchState = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               if (switchState > maxValue) switchState = maxValue;
                               if (switchState < minValue) switchState = minValue;

                               if (oldState != switchState) animation += oldState < switchState ? step : -step;

                               oldState = switchState;

                               rtKnob = new RotateTransform();
                               rtKnob.Angle = animation * 720;
                               SwitchKnob.RenderTransform = rtKnob;

                               switches.value = switchState;
                               switches.events = false;
                           }
                           catch { return; };
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

        private void SetValue(double _value)
        {
            try
            {
                if (switches == null) return;

                //MainWindow.refeshPopup = true;
                switchState = oldState;
                switches.events = true;

                if (_value == 1.0) animation += step;
                if (_value == -1.0) animation -= step;

                if (_value == 1.0) switchState += step;
                if (_value == -1.0) switchState -= step;

                switchState = Convert.ToDouble(string.Format("{0:0.00000}", switchState), CultureInfo.InvariantCulture); // w.g. E-format

                if (switchState > maxValue) switchState = maxValue;
                if (switchState < minValue) switchState = minValue;

                switches.value = switchState;
                oldState = switchState;

                rtKnob = new RotateTransform();
                rtKnob.Angle = animation * 720;
                SwitchKnob.RenderTransform = rtKnob;
            }
            catch { return; }
        }

        private void RightRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!touchDown)
            {
                SetValue(1.0);
            }
            touchDown = false;
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LeftRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!touchDown)
            {
                SetValue(-1.0);
            }
            touchDown = false;
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void RightRec_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = true;
            SetValue(1.0);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LeftRec_TouchDown(object sender, TouchEventArgs e)
        {
            e.Handled = true;
            touchDown = true;
            SetValue(-1.0);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }
    }
}

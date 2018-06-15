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
    public partial class Drum : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private string pictureKnob = "";
        private string pictureBase = "";

        private double minValue = 0.0;
        private double maxValue = 1.0;
        private double step = 0.05;

        private double startAngle = 0.0;
        private double finalAngle = 360;

        private int relative = 0;

        private double switchState = 0.0;
        private double oldState = 0.0;
        private double animation = 0.0;
        private string[] vals = new string[] { };
        private bool touchDown = false;

        private int windowID = 0;
        private Switches switches = null;
        //private RotateTransform rtKnob = new RotateTransform();
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        public Drum()
        {
            InitializeComponent();
            Focusable = false;
            DesignFrame.Visibility = System.Windows.Visibility.Hidden;
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;

            switches = MainWindow.switches.Find(x => x.ID == Convert.ToInt32(dataImportID));
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;
            helper = new GaugesHelper(dataImportID, windowID, "Switches");

            if (MainWindow.editmode)
            {
                helper.MakeDraggable(this, this);

                DesignFrame.Visibility = System.Windows.Visibility.Visible;
                Color color = Color.FromArgb(90, 255, 0, 0);
                DownRec.StrokeThickness = 2.0;
                DownRec.Stroke = new SolidColorBrush(color);
                UpRec.StrokeThickness = 2.0;
                UpRec.Stroke = new SolidColorBrush(color);
            }
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
            if (vals.Length > 3) relative = Convert.ToInt32(vals[3], CultureInfo.InvariantCulture);

            switchState = minValue;
            oldState = minValue;
        }

        public void SetOutput(string _output)
        {
            string[] vals = _output.Split(',');

            if (vals.Length == 2)
            {
                startAngle = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture);

                if (vals[1] != "1.0")
                    finalAngle = Convert.ToDouble(vals[1], CultureInfo.InvariantCulture);
            }

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

        public double GetSizeY()
        {
            return SwitchBase.Height;
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

                               //rtKnob = new RotateTransform();
                               //rtKnob.Angle = (animation * finalAngle) + startAngle;
                               //SwitchKnob.RenderTransform = rtKnob;

                               switches.value = switchState;
                               switches.events = false;
                           }
                           catch { return; };
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Switches", dataImportID, e.Delta);
        }

        private void SetValue(double _value)
        {
            try
            {
                if (switches == null) return;

                switchState = oldState;
                switches.events = true;

                if (_value == 1.0) animation += step;
                if (_value == -1.0) animation += step * -1;

                if (_value == 1.0) switchState += step;
                if (_value == -1.0) switchState += step * -1;

                switchState = Convert.ToDouble(string.Format("{0:0.00000}", switchState), CultureInfo.InvariantCulture); // E-format

                if (switchState > maxValue) switchState = maxValue;
                if (switchState < minValue) switchState = minValue;

                if (relative == 0)
                {
                    switches.value = switchState;
                }
                else
                {
                    if (_value == 1.0) switches.value = step;
                    if (_value == -1.0) switches.value = step * -1;
                    switches.oldValue = switches.oldValue * -1;
                }

                oldState = switchState;

                //rtKnob = new RotateTransform();
                //rtKnob.Angle = (animation * finalAngle) + startAngle;
                //SwitchKnob.RenderTransform = rtKnob;
            }
            catch { return; }
        }

        private void    RightRec_MouseDown(object sender, MouseButtonEventArgs e)
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

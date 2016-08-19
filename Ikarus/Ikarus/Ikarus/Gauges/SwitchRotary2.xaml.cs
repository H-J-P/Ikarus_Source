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
    public partial class MultiSwitch2 : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private string pictureKnob = "";
        private string pictureBase = "";
        private string[] vals = new string[] { };

        private double[] input = new double[] { };
        private double[] output = new double[] { };

        private double state = 0.0;
        private double switchState = 0.0;
        private int windowID = 0;
        private bool found = false;

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        public MultiSwitch2()
        {
            InitializeComponent();
            Focusable = false;

            DesignFrame.Visibility = System.Windows.Visibility.Hidden;

            ValueSwitch.Text = switchState.ToString();

            if (MainWindow.editmode)
            {
                MakeDraggable(this, this);
                DesignFrame.StrokeThickness = 1.0;
                DesignFrame.Visibility = System.Windows.Visibility.Visible;
                Color color = Color.FromArgb(90, 255, 0, 0);
                LeftRec.StrokeThickness = 1.0;
                LeftRec.Stroke = new SolidColorBrush(color);
                RightRec.StrokeThickness = 1.0;
                RightRec.Stroke = new SolidColorBrush(color);
            }
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
        }

        public string GetID() { return dataImportID; }

        public void SwitchLight(bool _on)
        {
            //Light.Visibility = _on ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }
        public void SetInput(string _input)
        {
            string[] vals = _input.Split(',');
            input = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                input[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }

            Switches switches = MainWindow.switches.Find(x => x.ID == Convert.ToInt32(dataImportID));

            if (switches != null)
            {
                switches.value = input[0]; // First State
                switches.sendRelease = false;
                state = input[0];
            }
        }

        public void SetOutput(string _output)
        {
            string[] vals = _output.Split(',');

            if (vals.Length > 1)
            {
                output = new double[vals.Length];

                for (int i = 0; i < vals.Length; i++)
                {
                    output[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
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
        }

        public double GetSize()
        {
            return 100; // Width
        }

        // -- Magnetos (Off, M1, M2, M1+M2) {0.0, 0.1, 0.2, 0.3}  knob / base
        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');
                               switchState = 0.0;

                               if (vals.Length > 0) { switchState = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               //if (switchState == 1.0) switchState = 0.0;

                               for (int i = 0; i < input.Length; i++)
                               {
                                   try
                                   {
                                       if (input[i] == switchState)
                                       {
                                           RotateTransform rtKnob = new RotateTransform();
                                           rtKnob.Angle = output[i];
                                           SwitchKnob.RenderTransform = rtKnob;
                                       }
                                   }
                                   catch { }
                               }

                               ValueSwitch.Text = switchState.ToString();

                               Switches switches = MainWindow.switches.Find(x => x.ID == Convert.ToInt32(dataImportID));

                               if (switches != null)
                               {
                                   switches.value = state;
                                   switches.events = false;
                                   switches.dontReset = false;
                               }
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

        private void SetValue(double _value, bool _event, bool dontReset = false)
        {
            try
            {
                found = false;

                if (switchState == 1.0) switchState = 0.0;

                Switches switches = MainWindow.switches.Find(x => x.ID == Convert.ToInt32(dataImportID));

                if (switches == null)
                {
                    //switchState = state; // Old State
                    //switches.oldValue = -1.0;
                    return;
                }

                if (_value == 1.0)
                {
                    if (switchState == 1.0) switchState = 0.0;

                    for (int i = 0; i < input.Length - 1; i++)
                    {
                        if (input[i] == switchState)
                        {
                            RotateTransform rtKnob = new RotateTransform();
                            rtKnob.Angle = output[i + 1];
                            SwitchKnob.RenderTransform = rtKnob;

                            switches.value = input[i + 1];
                            switchState = input[i + 1];

                            found = true;
                            break;
                        }
                    }
                }

                if (_value == -1.0)
                {
                    if (switchState == 0.0) switchState = 1.0;

                    for (int i = 1; i < input.Length; i++)
                    {
                        if (input[i] == switchState)
                        {
                            RotateTransform rtKnob = new RotateTransform();
                            rtKnob.Angle = output[i - 1];
                            SwitchKnob.RenderTransform = rtKnob;

                            switches.value = input[i - 1];
                            switchState = input[i - 1];
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    RotateTransform rtKnob = new RotateTransform();
                    rtKnob.Angle = switchState == input[0] ? output[input.Length - 1] : output[0];
                    SwitchKnob.RenderTransform = rtKnob;

                    switches.value = switchState == input[0] ? input[input.Length - 1] : input[0];
                    switchState = switchState == input[0] ? input[input.Length - 1] : input[0];
                }
                switches.events = _event;
                switches.dontReset = false;
                switches.oldValue = -1.0;

                //if (switchState == 1.0) switchState = 0.0;

                ValueSwitch.Text = switchState.ToString();
            }
            catch { }
        }

        private void RightRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetValue(1.0, true);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LeftRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetValue(-1.0, true);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void RightRec_TouchDown(object sender, TouchEventArgs e)
        {
            SetValue(1.0, true);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LeftRec_TouchDown(object sender, TouchEventArgs e)
        {
            SetValue(-1.0, true);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }
    }
}

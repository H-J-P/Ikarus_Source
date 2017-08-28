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
    public partial class RotaryWithPosition : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private string pictureKnob = "";
        private string pictureBase = "";
        private string[] vals = new string[] { };

        private double[] input = new double[] { };
        private double[] output = new double[] { };
        private double switchState = 0.0;
        private double oldState = 0.0;

        private int windowID = 0;
        private Switches switches = null;
        private RotateTransform rtKnob = new RotateTransform();
        GaugesHelper helper = null;
        private bool found = false;

        public int GetWindowID() { return windowID; }

        public RotaryWithPosition()
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
                LeftRec.StrokeThickness = 2.0;
                LeftRec.Stroke = new SolidColorBrush(color);
                RightRec.StrokeThickness = 2.0;
                RightRec.Stroke = new SolidColorBrush(color);
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
            input = new double[vals.Length];

            for (int i = 0; i < vals.Length; i++)
            {
                input[i] = Convert.ToDouble(vals[i], CultureInfo.InvariantCulture);
            }
            switchState = input[0];
            oldState = input[0];
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

        public double GetSizeY()
        {
            return SwitchBase.Height; // Width
        }

        // -- Magnetos (Off, M1, M2, M1+M2) {0.0, 0.1, 0.2, 0.3}  knob / base
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

                               if (vals.Length > 0) { switchState = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }

                               oldState = switchState;
                               found = false;

                               for (int i = 0; i < input.Length; i++)
                               {
                                   try
                                   {
                                       if (input[i] == switchState)
                                       {
                                           rtKnob = new RotateTransform();
                                           rtKnob.Angle = output[i];
                                           SwitchKnob.RenderTransform = rtKnob;
                                           found = true;
                                           break;
                                       }
                                   }
                                   catch { return; }
                               }

                               if (!found)
                               {
                                   oldState = input[0];
                                   rtKnob = new RotateTransform();
                                   rtKnob.Angle = output[0];
                                   SwitchKnob.RenderTransform = rtKnob;
                               }

                               if (switches != null)
                               {
                                   switches.value = switchState;
                                   switches.events = false;
                                   switches.dontReset = false;
                                   switches.sendRelease = false;
                               }
                           }
                           catch { return; };
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "ID", MainWindow.dtSwitches, dataImportID, e.Delta);
        }

        private void SetValue(double _value)
        {
            try
            {
                //Switches switches = MainWindow.switches.Find(x => x.ID == Convert.ToInt32(dataImportID));

                if (switches == null) return;

                switchState = oldState;
                switches.events = true;
                switches.dontReset = false;

                if (_value == 1.0)
                {
                    for (int i = 0; i < input.Length - 1; i++)
                    {
                        if (input[i] == switchState)
                        {
                            rtKnob = new RotateTransform();
                            rtKnob.Angle = output[i + 1];
                            SwitchKnob.RenderTransform = rtKnob;

                            switches.value = input[i + 1];
                            break;
                        }
                    }
                }

                if (_value == -1.0)
                {
                    for (int i = 1; i < input.Length; i++)
                    {
                        if (input[i] == switchState)
                        {
                            rtKnob = new RotateTransform();
                            rtKnob.Angle = output[i - 1];
                            SwitchKnob.RenderTransform = rtKnob;

                            switches.value = input[i - 1];
                            break;
                        }
                    }
                }
                oldState = switches.value;
            }
            catch { return; }
        }

        private void RightRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetValue(1.0);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LeftRec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetValue(-1.0);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void RightRec_TouchDown(object sender, TouchEventArgs e)
        {
            SetValue(1.0);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        private void LeftRec_TouchDown(object sender, TouchEventArgs e)
        {
            SetValue(-1.0);
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }
    }
}

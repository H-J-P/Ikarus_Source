using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for DisplayTest.xaml
    /// </summary>
    public partial class Display : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        private const string defaultFontColor = "FF9430"; // Hexdezimal Color Value (Red)
        private const string defaultErrorText = "ERROR";
        private string errorText = "";
        private string fontColor = defaultFontColor;
        private string numberChars = "";
        private int numberOfSegments = 0;
        private string textForDisplay = "";
        private double value = 0.0;
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        public Display()
        {
            InitializeComponent();
            Segments.Text = "";
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
            InitialDisplay();
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;
            helper = new GaugesHelper(dataImportID, windowID, "Instruments");

            if (MainWindow.editmode)
            {
                helper.MakeDraggable(this, this);
                Segments.Text = "8";
            }
        }

        public string GetID() { return dataImportID; }

        private void InitialDisplay()
        {
            dataRows = MainWindow.dtInstrumentFunctions.Select("IDInst=" + dataImportID); // instrument functions

            if (dataRows.Length > 0)
            {
                fontColor = dataRows[0]["Input"].ToString().ToUpper();
                numberChars = dataRows[0]["Output"].ToString();

                if (fontColor.Length != 6)
                {
                    fontColor = defaultFontColor;
                }
                else
                {
                    if (!IsHexString(fontColor))
                    {
                        fontColor = defaultFontColor;
                        errorText = defaultErrorText;
                    }
                }

                if (!int.TryParse(numberChars, out numberOfSegments)) { numberOfSegments = 5; }
           }
            else
            {
                fontColor = defaultFontColor;
                numberOfSegments = 5;
                errorText = defaultErrorText;
            }

            Segments.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF" + fontColor);

            if (numberOfSegments > 1)
            {
                PathBackground.Width = PathBackground.Width + (Segments.Width * numberOfSegments);// - Segments.Width;
                PathBorder.Width = PathBorder.Width + (Segments.Width * numberOfSegments);// - Segments.Width;
                this.Width = PathBackground.Width + 12; // Segments.Width / 2;
                Segments.Width = PathBorder.Width;
            }
            Segments.Text = errorText;
        }

        public bool IsHexString(string value)
        {
            string hx = "0123456789ABCDEF";
            foreach (char c in value.ToUpper())
            {
                if (!hx.Contains(char.ToString(c)))
                    return false;
            }
            return true;
        }

        private void LoadBmaps()
        {
            dataRows = MainWindow.dtInstruments.Select("IDInst=" + dataImportID);

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
            return PathBackground.Width;
        }

        public double GetSizeY()
        {
            return PathBackground.Height;
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               vals = strData.Split(';');

                               if (vals.Length > 0) { textForDisplay = vals[0]; }

                               if (numberOfSegments == 1)
                               {
                                   value = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture);

                                   if (value < 1)
                                   {
                                       value *= 10;
                                       textForDisplay = value.ToString().Substring(0, numberOfSegments);
                                   }
                               }

                               if (textForDisplay.Length > numberOfSegments)
                                   textForDisplay = textForDisplay.Substring(0, numberOfSegments);

                               Segments.Text = textForDisplay;
                           }
                           catch { return; }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}



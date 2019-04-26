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
    public partial class DisplayDotMatrixAJS : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        private const string font = "LED Board-7";

        private string fontColor = "ABF9AB"; // Hexdezimal Color Value (Light Green)
        private string backColor = "FF000000";
        private string borderColor = "FF808080";
        private string[] colors = new string[] { };

        private string numberChars = "";
        private int numberOfSegments = 1;
        private string textForDisplay = "";
        private double value = 0.0;
        private bool isAscii = true;

        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        public DisplayDotMatrixAJS()
        {
            InitializeComponent();
            Segments.Text = "0";
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
            }
        }

        public string GetID() { return dataImportID; }

        private void InitialDisplay()
        {
            dataRows = MainWindow.dtInstrumentFunctions.Select("IDInst=" + dataImportID); // instrument functions

            if (dataRows.Length > 0)
            {
                colors = dataRows[0]["Input"].ToString().ToUpper().Split(',');

                if (colors.Length > 0) { fontColor = colors[0].Trim(); }
                if (colors.Length > 1) { backColor = colors[1].Trim(); }
                if (colors.Length > 2) { borderColor = colors[2].Trim(); }

                numberChars = dataRows[0]["Output"].ToString();
                isAscii = Convert.ToBoolean(dataRows[0]["Ascii"]);

                if (!isAscii)
                    Segments.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                else
                    Segments.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                if (!int.TryParse(numberChars, out numberOfSegments)) { numberOfSegments = 5; }

                if (MainWindow.editmode)
                {
                    Segments.Text = new String('8', numberOfSegments);
                }
            }

            FontFamily fontFamily = new FontFamily(font);
            string[] checkFontFamily = new string[1] { "" };
            fontFamily.FamilyNames.Values.CopyTo(checkFontFamily, 0);

            if (checkFontFamily[0] != font) // verify loaded font
            {
                Segments.FontFamily = new FontFamily("Courier New"); // switch to a default font
                Segments.FontSize = 40;
                Segments.Width = 24;
            }

            try
            {
                if (fontColor.Length == 6 && IsHexString(fontColor))
                    Segments.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF" + fontColor);
                if (fontColor.Length == 8 && IsHexString(fontColor))
                    Segments.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#" + fontColor);

                if (backColor.Length == 6 && IsHexString(backColor))
                    PathBackground.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF" + backColor);
                if (backColor.Length == 8 && IsHexString(backColor))
                    PathBackground.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#" + backColor);

                if (borderColor.Length == 6 && IsHexString(borderColor))
                    PathBorder.Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF" + borderColor);
                if (borderColor.Length == 8 && IsHexString(borderColor))
                    PathBorder.Stroke = (SolidColorBrush)new BrushConverter().ConvertFromString("#" + borderColor);
            }
            catch { }

            if (numberOfSegments > 0)
            {
                PathBackground.Width = PathBackground.Width + (Segments.Width * numberOfSegments);// - Segments.Width;
                PathBorder.Width = PathBorder.Width + (Segments.Width * numberOfSegments);// - Segments.Width;
                this.Width = PathBackground.Width + 12; //Segments.Width / 2;
                Segments.Width = PathBorder.Width;
            }
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
                               if (MainWindow.editmode)
                               {
                                   Segments.Text = new String('8', numberOfSegments);
                                   return;
                               }

                               vals = strData.Split(';');

                               if (vals.Length > 0) { textForDisplay = vals[0]; }

                               if (!isAscii)
                               {
                                   value = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture);

                                   if (value < 1)
                                   {
                                       value *= 10 + 1;
                                       textForDisplay = value.ToString().Substring(0, numberOfSegments);
                                   }
                               }

                               if (textForDisplay.Length > numberOfSegments)
                                   textForDisplay = textForDisplay.Substring(0, numberOfSegments);

                               Segments.Text = textForDisplay;
                           }
                           catch
                           {
                               return;
                           }
                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}



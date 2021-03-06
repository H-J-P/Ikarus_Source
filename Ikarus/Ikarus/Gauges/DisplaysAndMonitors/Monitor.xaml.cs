using System;
using System.Data;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MonitorDotMatrix.xaml
    /// </summary>
    public partial class Monitor : UserControl, I_Ikarus
    {
        private static string newline = Environment.NewLine;
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        private const string font = "Courier New";
        private double fontSize = 22;
        private double lineWidth = 13.2;
        private double lineHeight = 23.8;
        private const string defaultFontColor = "ABF9AB"; // Hexdezimal Color Value (Light Green)
        private bool errorText = false;

        private string[] colors = new string[] { };
        private string fontColor = "ABF9AB";
        private string backColor = "00000000";

        private Thickness thickness = new Thickness(0, 0, 0, 0);

        private string numberCharsNumberLines = "";
        private int numberOfSegments = 1;
        private int numberOfLines = 1;
        private string[] textForDisplay = new string[] {"Init", "ERROR"};
        private System.Collections.Generic.List<TextBlock> textBlockLines = new System.Collections.Generic.List<TextBlock>();
        private int[] codeChar = new int[19];
        private string[] replaceString = new string[19];
        private string[] character = new string[19];
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        public Monitor()
        {
            InitializeComponent();
            DesignFrame.Visibility = Visibility.Hidden;

            Line1.Text = "";

            codeChar[0] = 0x00BB;
            codeChar[1] = 0x00AB;
            codeChar[2] = 0x00AE;
            codeChar[3] = 0x00A1;
            codeChar[4] = 0x00A9;
            codeChar[5] = 0x00B0;
            codeChar[6] = 0x00B6;
            codeChar[7] = 0x00B1;
            codeChar[8] = 0x003A;
            codeChar[9] = 0x2192;
            codeChar[10] = 0x0030;
            codeChar[11] = 0x0031;
            codeChar[12] = 0x2299;
            codeChar[13] = 0x2190;
            codeChar[14] = 0x2337;
            codeChar[15] = 0x2195;
            codeChar[16] = 0x2588;
            codeChar[17] = 0x003A;
            codeChar[18] = 0x002A;


            replaceString[0] = "0x00BB";
            replaceString[1] = "0x00AB";
            replaceString[2] = "0x00AE";
            replaceString[3] = "0x00A1";
            replaceString[4] = "0x00A9";
            replaceString[5] = "0x00B0";
            replaceString[6] = "0x00B6";
            replaceString[7] = "0x00B1";
            replaceString[8] = "0x003A";
            replaceString[9] = "0x2192";
            replaceString[10] = "0x0030";
            replaceString[11] = "0x0031";
            replaceString[12] = "0x2299";
            replaceString[13] = "0x2190";
            replaceString[14] = "0x2337";
            replaceString[15] = "0x2195";
            replaceString[16] = "0x2588";
            replaceString[17] = "0x003A";
            replaceString[18] = "0x002A";

            for (int n = 0; n < codeChar.Length; n++)
            {
                character[n] = char.ConvertFromUtf32(codeChar[n]);
            }
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
                DesignFrame.Visibility = Visibility.Visible;
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

                numberCharsNumberLines = dataRows[0]["Output"].ToString();

                if (fontColor.Length != 6)
                {
                    fontColor = defaultFontColor;
                }
                else
                {
                    if (!IsHexString(fontColor))
                    {
                        fontColor = defaultFontColor;
                        errorText = true;
                    }
                }

                string[] TmpSplit = numberCharsNumberLines.Split(new char[] { ',' });
                if (TmpSplit.Length == 2)
                {
                    if (!int.TryParse(TmpSplit[0], out numberOfSegments))
                    {
                        numberOfSegments = 5;
                        errorText = true;
                    }

                    if (!int.TryParse(TmpSplit[1], out numberOfLines))

                    {
                        numberOfLines = 2;;
                        errorText = true;
                    }

                    if(numberOfSegments < 1 || numberOfLines < 1)
                    {
                        numberOfSegments = 5;
                        numberOfLines = 2;
                        errorText = true;
                    }
                }
                else
                {
                    numberOfSegments = 5;
                    numberOfLines = 2;
                    errorText = true;
                }
            }
            else
            {
                fontColor = defaultFontColor;
                numberOfSegments = 5;
                numberOfLines = 2;
                errorText = true;
            }

            FontFamily fontFamily = new FontFamily(font);
            string[] checkFontFamily = new string[1] { "" };
            fontFamily.FamilyNames.Values.CopyTo(checkFontFamily, 0);

            if (checkFontFamily[0] != font) // verify loaded font
            {
                fontFamily = new FontFamily("Courier New"); // switch to a default font
                fontSize = 22;
                lineWidth = 13;
                lineHeight = 19;
            }

            lineWidth = lineWidth * numberOfSegments;
            Light.Width = lineWidth;
            Light.Height = lineHeight * numberOfLines;
            this.Width = Light.Width;
            this.Height = Light.Height;
            DesignFrame.Width = Light.Width;
            DesignFrame.Height = Light.Height;

            if (numberOfLines > 0)
            {
                for (int i = 0; i < numberOfLines; i++)
                {
                    textBlockLines.Add(new TextBlock());
                    textBlockLines[i].Name = "Line_" + i.ToString();

                    textBlockLines[i].Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF" + defaultFontColor);
                    textBlockLines[i].Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00000000");

                    try
                    {
                        if (fontColor.Length == 6 && IsHexString(fontColor))
                            textBlockLines[i].Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF" + fontColor);
                        if (fontColor.Length == 8 && IsHexString(fontColor))
                            textBlockLines[i].Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#" + fontColor);

                        if (backColor.Length == 6 && IsHexString(backColor))
                            textBlockLines[i].Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF" + backColor);
                        if (backColor.Length == 8 && IsHexString(backColor))
                            textBlockLines[i].Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#" + backColor);
                    }
                    catch { }

                    textBlockLines[i].FontFamily = fontFamily;
                    textBlockLines[i].FontSize = fontSize;
                    textBlockLines[i].Width = lineWidth;
                    textBlockLines[i].Height = lineHeight;
                    textBlockLines[i].VerticalAlignment = VerticalAlignment.Top;
                    textBlockLines[i].Margin = thickness;

                    MonitorGrid.Children.Add(textBlockLines[i]);
                    thickness.Top = thickness.Top + lineHeight;
                }
            }

            if (errorText)
            {
                textBlockLines[0].Text = textForDisplay[0];
                textBlockLines[1].Text = textForDisplay[1];
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
            DataRow[] dataRows = MainWindow.dtInstruments.Select("IDInst=" + dataImportID);

            if (dataRows.Length > 0)
            {
                string light = dataRows[0]["ImageLight"].ToString();

                try
                {
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
            return Light.Width;
        }

        public double GetSizeY()
        {
            return Light.Height;
        }

        public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           string[] vals = strData.Split(';');

                           string[] Lines = new string[textBlockLines.Count];
                           
                           try
                           {
                               for(int i=0; i < textBlockLines.Count; i++)
                               {
                                   Lines[i] = "";
                                   if (vals.Length > i)
                                   {
                                       Lines[i] = ReplaceStrings(vals[i]);
                                       if (Lines[i].Length > numberOfSegments) { Lines[i] = Lines[i].Substring(0, numberOfSegments); }
                                   }
                                   textBlockLines[i].Text = Lines[i];
                               }
                           }
                           catch (Exception e) { ImportExport.LogMessage(GetType().Name + " got data and failed with exception: " + e.ToString()); }
                       }));
        }

        private string ReplaceStrings(string line)
        {
            for (int n = 0; n < replaceString.Length; n++)
            {
                try
                {
                    line = line.Replace(replaceString[n], character[n]);
                }
                catch { }
            }
            return line;
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "Instruments", dataImportID, e.Delta);
        }
    }
}

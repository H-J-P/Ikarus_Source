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
    /// Interaction logic for Monitor7Segment.xaml
    /// </summary>
    public partial class Monitor7Segment : UserControl, I_Ikarus
    {
        private static string newline = Environment.NewLine;
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };

        private const string font = "Digital-7 Mono";
        private double fontSize = 22;
        private double lineWidth = 10.1;
        private double lineHeight = 18;
        private const string defaultFontColor = "FF9430"; // Hexdezimal Color Value (Red)
        private bool errorText = false;
        private string fontColor = defaultFontColor;
        private Thickness thickness = new Thickness(0, 0, 0, 0);

        private string numberCharsNumberLines = "";
        private int numberOfSegments = 1;
        private int numberOfLines = 1;
        private string[] textForDisplay = new string[] {"Init", "ERROR"};
        private System.Collections.Generic.List<TextBlock> textBlockLines = new System.Collections.Generic.List<TextBlock>();
        private int[] codeChar = new int[19];
        private string[] replaceString = new string[19];
        private string[] character = new string[19];

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        public Monitor7Segment()
        {
            InitializeComponent();
            DesignFrame.Visibility = Visibility.Hidden;

            if (MainWindow.editmode)
            {
                MakeDraggable(this, this);
                DesignFrame.Visibility = Visibility.Visible;
            }
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

        public string GetID() { return dataImportID; }

        private void InitialDisplay()
        {
            dataRows = MainWindow.dtInstrumentFunctions.Select("IDInst=" + dataImportID); // instrument functions

            if (dataRows.Length > 0)
            {
                fontColor = dataRows[0]["Input"].ToString().ToUpper();
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
                lineWidth = 13.2;
                lineHeight = 23.7;
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
                    textBlockLines[i].Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#00000000");
                    textBlockLines[i].Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF" + fontColor);
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
            return Light.Width; // Width
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
                           catch { return; };
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
                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID);
            };

            movedByElement.MouseLeave += (a, b) => isMousePressed = false;

            movedByElement.MouseMove += (a, b) =>
            {
                if (!isMousePressed || !MainWindow.editmode) return;

                currentPoint = ((System.Windows.Input.MouseEventArgs)b).GetPosition(moveThisElement);
                trUsercontrol.X += currentPoint.X - originalPoint.X;
                trUsercontrol.Y += currentPoint.Y - originalPoint.Y;
                moveThisElement.RenderTransform = trUsercontrol;
            };
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "IDInst", MainWindow.dtInstruments, dataImportID, e.Delta);
        }
    }
}

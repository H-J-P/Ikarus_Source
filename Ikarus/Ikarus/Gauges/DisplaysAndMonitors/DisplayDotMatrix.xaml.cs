using System;
using System.Windows.Controls;
using System.Data;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for DisplayTest.xaml
    /// </summary>
    public partial class DisplayDotMatrix : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private int windowID = 0;
        private string[] vals = new string[] { };
        private const string font = "LED Board-7";
        private const string defaultFontColor = "ABF9AB"; // Hexdezimal Color Value (Light Green)
        private const string defaultErrorText = "ERROR";
        private string errorText = "";
        private string fontColor = defaultFontColor;
        private string numberChars = "";
        private int numberOfSegments = 0;
        private string textForDisplay = "";

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        public DisplayDotMatrix()
        {
            InitializeComponent();
            if (MainWindow.editmode) MakeDraggable(this, this);
            Segments.Text = "";
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
                //errorText = "Init1";
            }
            else
            {
                fontColor = defaultFontColor;
                numberOfSegments = 5;
                errorText = defaultErrorText;
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
            Segments.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF" + fontColor);

            PathBackground.Width = PathBackground.Width + (Segments.Width * numberOfSegments);// - Segments.Width;
            PathBorder.Width = PathBorder.Width + (Segments.Width * numberOfSegments);// - Segments.Width;
            this.Width = PathBackground.Width + 12; //Segments.Width / 2;
            Segments.Width = PathBorder.Width;

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
            return PathBackground.Width; // Width
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

                               if (textForDisplay.Length > numberOfSegments)
                                   textForDisplay = textForDisplay.Substring(0, numberOfSegments);

                               Segments.Text = textForDisplay;
                           }
                           catch { return; }
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


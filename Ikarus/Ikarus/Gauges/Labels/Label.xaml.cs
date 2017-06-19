using System;
using System.Data;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Label : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private string imageFrame = "";
        private int windowID = 0;
        BitmapImage bitmapImage = new BitmapImage();
        GaugesHelper helper = null;

        public int GetWindowID() { return windowID; }

        public Label()
        {
            InitializeComponent();
            Focusable = false;

            DesignFrame.Visibility = System.Windows.Visibility.Hidden;
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
        }

        public void SetWindowID(int _windowID)
        {
            windowID = _windowID;
            helper = new GaugesHelper(dataImportID, windowID, "Accessories");

            if (MainWindow.editmode)
            {
                helper.MakeDraggable(this, this);
                DesignFrame.Visibility = System.Windows.Visibility.Visible;
            }
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
            dataRows = MainWindow.dtAccessories.Select("ID=" + dataImportID);

            if (dataRows.Length > 0)
            {
                imageFrame = dataRows[0]["FilePictureOn"].ToString();
            }

            try
            {
                ImageFrame.Source = null;

                if (File.Exists(Environment.CurrentDirectory + "\\Images\\Accessories\\" + imageFrame))
                {
                    bitmapImage = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Accessories\\" + imageFrame));

                    int bitmapHeight = bitmapImage.PixelHeight / 2; // Jumping Jack
                    int bitmapWidth = bitmapImage.PixelWidth / 2;

                    DesignFrame.Height = bitmapHeight;
                    DesignFrame.Width  = bitmapWidth;

                    ImageFrame.Height  = bitmapHeight;
                    ImageFrame.Width   = bitmapWidth;

                    ImageFrame.Source  = bitmapImage;
                }
            }
            catch { }
        }

        public double GetSize()
        {
            return ImageFrame.Width; // Width
        }

    public void UpdateGauge(string strData)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           //string[] vals = strData.Split(';');


                       }));
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "ID", MainWindow.dtAccessories, dataImportID, e.Delta);
        }
    }
}


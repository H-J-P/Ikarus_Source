using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Globalization;
using System.Data;
using System.IO;
using System.Windows.Media.Imaging;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Label200x200 : UserControl, I_Ikarus
    {
        private DataRow[] dataRows = new DataRow[] { };
        private string dataImportID = "";
        private string imageFrame = "";
        private int windowID = 0;

        public void SetWindowID(int _windowID) { windowID = _windowID; }
        public int GetWindowID() { return windowID; }

        public Label200x200()
        {
            InitializeComponent();
            Focusable = false;

            DesignFrame.Visibility = System.Windows.Visibility.Hidden;

            if (MainWindow.editmode)
            {
                MakeDraggable(this, this);
                DesignFrame.Visibility = System.Windows.Visibility.Visible;
            }
        }

        public void SetID(string _dataImportID)
        {
            dataImportID = _dataImportID;
            LoadBmaps();
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
                    ImageFrame.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Images\\Accessories\\" + imageFrame));

            }
            catch { }
        }

        public double GetSize()
        {
            return 200; // Width
        }

        public void UpdateGauge(string strData)
        {
            //string[] vals = strData.Split(';');

            //Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //           (Action)(() =>
            //           {

            //               double lampState = 0.0;

            //               try
            //               {
            //                   if (vals.Length > 0) { lampState = Convert.ToDouble(vals[0], CultureInfo.InvariantCulture); }
            //               }
            //               catch { return; };

            //               if (lampState > 0.08) { SetValue(1.0,false); }
            //               else { SetValue(0.0,false); }
            //           }));
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

                MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "ID", MainWindow.dtAccessories, dataImportID);
            };
        }

        private void Light_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (MainWindow.editmode) MainWindow.cockpitWindows[windowID].UpdatePosition(PointToScreen(new System.Windows.Point(0, 0)), "ID", MainWindow.dtAccessories, dataImportID, e.Delta);
        }
    }
}

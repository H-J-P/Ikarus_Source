using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBoxInstruments : Window
    {
        private static int index = 0;
        private static string classname = "";
        private static string name = "";
        private static string picture_ = "";
        private static string posX = "";
        private static string posY = "";
        private static string size = "";
        private static string rotate = "";
        private static string imageForFrame = "";
        private static string imageForGlass = "";
        private static string windowID = "";

        public MessageBoxInstruments(int _index, string _class)
        {
            try
            {
                index = _index;
                classname = _class;

                DataRow[] dataRows = MainWindow.dtInstruments.Select("IDInst=" + _index);

                if (dataRows.Length > 0)
                {
                    name = dataRows[0]["Name"].ToString();
                    posX = dataRows[0]["PosX"].ToString();
                    posY = dataRows[0]["PosY"].ToString();
                    size = dataRows[0]["Size"].ToString();
                    rotate = dataRows[0]["Rotate"].ToString();
                    imageForFrame = dataRows[0]["ImageFrame"].ToString();
                    imageForGlass = dataRows[0]["ImageLight"].ToString();
                    windowID = dataRows[0]["WindowID"].ToString();
                }
            }
            catch (Exception ex) { ImportExport.LogMessage("Dialog Gauge: " + ex.ToString()); }

            Start();
        }

        public MessageBoxInstruments()
        {
            Start();
        }

        private void Start()
        {
            InitializeComponent();

            Classname.ItemsSource = null;
            Classname.ItemsSource = MainWindow.dtClassnames.DefaultView;
            Classname.Text = classname;

            WindowID.ItemsSource = null;
            WindowID.ItemsSource = MainWindow.dtWindows.DefaultView;
            WindowID.SelectedIndex = Convert.ToInt16(windowID) - 1;

            Desc.Text = name;
            ImageForFrame.Text = imageForFrame;
            ImageForGlass.Text = imageForGlass;
            PosX.Text = posX;
            PosY.Text = posY;
            Size.Text = size;
            Rotate.Text = rotate;

            Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateData();
            Close();
        }

        private string FileSelectDialog(string defaultExt, string filter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = MainWindow.currentDirectory + "\\Images\\Frames";

            dlg.DefaultExt = defaultExt;
            dlg.Filter = filter;

            var result = dlg.ShowDialog();
            if (result == false) { return ""; }

            return dlg.FileName.Substring(dlg.FileName.LastIndexOf("\\") + 1);
        }

        private void UpdateData()
        {
            try
            {
                DataRow[] dataRows = MainWindow.dtInstruments.Select("IDInst=" + index);

                if (dataRows.Length > 0)
                {
                    dataRows[0]["Name"] = Desc.Text;
                    dataRows[0]["Class"] = Classname.Text;
                    dataRows[0]["PosX"] = PosX.Text;
                    dataRows[0]["PosY"] = PosY.Text;
                    dataRows[0]["Size"] = Size.Text;
                    dataRows[0]["Rotate"] = Rotate.Text;
                    dataRows[0]["ImageFrame"] = ImageForFrame.Text;
                    dataRows[0]["ImageLight"] = ImageForGlass.Text;
                    dataRows[0]["WindowID"] = WindowID.SelectedIndex + 1;
                }
            }
            catch (Exception ex) { ImportExport.LogMessage("Transmit Gauge: " + ex.ToString()); }
        }

        private void Classname_DropDownClosed(object sender, EventArgs e)
        {
            //ChangeSourceDatagridSwitches();
        }

        private void DataGridSwitches_MouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void ImageForGlassSelect_Click(object sender, RoutedEventArgs e)
        {
            picture_ = FileSelectDialog(".png", "PNG files (*.png)|*.png|All files (*.*)|*.*");

            if (picture_ != "") { ImageForGlass.Text = picture_; }
        }

        private void ImageForFrameSelect_Click(object sender, RoutedEventArgs e)
        {
            picture_ = FileSelectDialog(".png", "PNG files (*.png)|*.png|All files (*.*)|*.*");

            if (picture_ != "") { ImageForFrame.Text = picture_; }
        }
    }
}

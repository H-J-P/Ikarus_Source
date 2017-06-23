using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBoxAccessories : Window
    {
        private static int index = 0;
        private static string classname = "";
        private static string name = "";
        private static string pictureOn = "";
        private static string posX = "";
        private static string posY = "";
        private static string size = "";
        private static string rotate = "";
        private static string windowID = "";

        public MessageBoxAccessories(int _index)
        {
            try
            {
                DataRow[] dataRows = MainWindow.dtAccessories.Select("ID=" + _index);
                index = _index;

                if (dataRows.Length > 0)
                {
                    //index = Convert.ToInt32(dataRows[0]["ID"]);
                    classname = dataRows[0]["Class"].ToString();
                    name = dataRows[0]["Name"].ToString();
                    pictureOn = dataRows[0]["FilePictureOn"].ToString();
                    posX = dataRows[0]["PosX"].ToString();
                    posY = dataRows[0]["PosY"].ToString();
                    size = dataRows[0]["Size"].ToString();
                    rotate = dataRows[0]["Rotate"].ToString();
                    windowID = dataRows[0]["WindowID"].ToString();
                }
            }
            catch (Exception ex) { ImportExport.LogMessage("Dialog Accessory: " + ex.ToString()); }

            Start();
        }

        public MessageBoxAccessories()
        {
            Start();
        }

        private void Start()
        {
            InitializeComponent();

            WindowID.ItemsSource = null;
            WindowID.ItemsSource = MainWindow.dtWindows.DefaultView;
            WindowID.SelectedIndex = Convert.ToInt16(windowID) - 1;

            Classname.Text = classname;
            ImageOn.Text = pictureOn;
            Desc.Text = name;
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

        private void ChangeSourceDatagridSwitches()
        {
        }

        private string FileSelectDialog(string defaultExt, string filter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = MainWindow.currentDirectory + "\\Images\\Accessories";

            string path = Environment.CurrentDirectory;

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
                DataRow[] dataRows = MainWindow.dtAccessories.Select("ID=" + index);

                if (dataRows.Length > 0)
                {
                    dataRows[0]["ID"] = index;
                    dataRows[0]["Class"] = Classname.Text;
                    dataRows[0]["Name"] = Desc.Text;
                    dataRows[0]["FilePictureOn"] = ImageOn.Text;
                    dataRows[0]["PosX"] = PosX.Text;
                    dataRows[0]["PosY"] = PosY.Text;
                    dataRows[0]["Size"] = Size.Text;
                    dataRows[0]["Rotate"] = Rotate.Text;
                    dataRows[0]["WindowID"] = WindowID.SelectedIndex + 1;
                }
            }
            catch (Exception ex) { ImportExport.LogMessage("Transmit Accessory: " + ex.ToString()); }
        }

        private void ImageOnSelect_Click(object sender, RoutedEventArgs e)
        {
            pictureOn = FileSelectDialog(".png", "PNG files (*.png)|*.png|All files (*.*)|*.*");

            if (pictureOn != "") { ImageOn.Text = pictureOn; }
        }

        private void Classname_DropDownClosed(object sender, EventArgs e)
        {
            //ChangeSourceDatagridSwitches();
        }

        private void DataGridSwitches_MouseWheel(object sender, MouseWheelEventArgs e)
        {

        }
    }
}

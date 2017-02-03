using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBoxLamps : Window
    {
        private static string classname = "";
        private static int id = 0;
        private static int arg_number = 0;
        private static string name = "";
        private static string pictureOn = "";
        private static string pictureOff = "";
        private static string posX = "";
        private static string posY = "";
        private static string size = "";
        private static string rotate = "";
        private static DataTable lamps = new DataTable();
        private static string windowID = "";

        public MessageBoxLamps(int _id)
        {
            try
            {
                DataRow[] dataRows = MainWindow.dtLamps.Select("ID=" + _id);

                if (dataRows.Length > 0)
                {
                    id = _id;
                    classname = dataRows[0]["Class"].ToString();
                    arg_number = Convert.ToInt32(dataRows[0]["Arg_number"]);
                    name = dataRows[0]["Name"].ToString();
                    pictureOn = dataRows[0]["FilePictureOn"].ToString();
                    pictureOff = dataRows[0]["FilePictureOff"].ToString();
                    posX = dataRows[0]["PosX"].ToString();
                    posY = dataRows[0]["PosY"].ToString();
                    size = dataRows[0]["Size"].ToString();
                    rotate = dataRows[0]["Rotate"].ToString();
                    windowID = dataRows[0]["WindowID"].ToString();
                }
            }
            catch (Exception ex) { ImportExport.LogMessage("Transmit Lamp: " + ex.ToString()); }

            Start();
        }

        public MessageBoxLamps()
        {
            Start();
        }

        private void Start()
        {
            InitializeComponent();

            DataRow[] dr = null;

            if (MainWindow.dtMasterLamps != null)
            {
                dr = MainWindow.dtMasterLamps.Select("Type='Lamp'", "Description");
                lamps = dr.CopyToDataTable<DataRow>();
                DataGridLamps.ItemsSource = lamps.DefaultView;
                DataGridLamps.CanUserAddRows = false;
                DataGridLamps.CanUserDeleteRows = true;
                DataGridLamps.IsReadOnly = false;

                DataGridLamps.ScrollIntoView(DataGridLamps.Items[DataGridLamps.Items.Count - 1]);
                try
                {
                    if (arg_number != 0)
                    {
                        for (int n = 0; n < dr.Length; n++)
                        {
                            if (Convert.ToInt32(dr[n][0]) == arg_number)
                            {
                                DataGridLamps.SelectedIndex = n;
                                DataGridLamps.ScrollIntoView(DataGridLamps.Items[n]);
                                break;
                            }
                        }
                    }
                    else
                    {
                        //DataGridLamps.SelectedIndex = 0;
                        DataGridLamps.ScrollIntoView(DataGridLamps.Items[0]);
                    }
                }
                catch { }
            }

            WindowID.ItemsSource = null;
            WindowID.ItemsSource = MainWindow.dtWindows.DefaultView;
            WindowID.SelectedIndex = Convert.ToInt16(windowID) - 1;

            Classname.Text = classname;
            ImageOn.Text = pictureOn;
            ImageOff.Text = pictureOff;
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

        private void DataGridSwitches_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //UpdateData();
            //Close();
        }

        private string FileSelectDialog(string defaultExt, string filter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = MainWindow.currentDirectory + "\\Images\\Lamps";

            dlg.DefaultExt = defaultExt;
            dlg.Filter = filter;

            var result = dlg.ShowDialog();
            if (result == false) { return ""; }

            return dlg.FileName.Substring(dlg.FileName.LastIndexOf("\\") + 1);
        }

        private void UpdateData()
        {
            if (DataGridLamps.SelectedItem == null) return;

            try
            {
                DataRowView rowView = (DataRowView)DataGridLamps.SelectedItem;

                DataRow[] dataRows = MainWindow.dtLamps.Select("ID=" + id);

                if (dataRows.Length > 0)
                {
                    dataRows[0]["Class"] = Classname.Text;
                    dataRows[0]["Arg_number"] = Convert.ToInt32(rowView.Row.ItemArray[0]);
                    dataRows[0]["Name"] = rowView.Row.ItemArray[2];
                    dataRows[0]["FilePictureOn"] = ImageOn.Text;
                    dataRows[0]["FilePictureOff"] = ImageOff.Text;
                    dataRows[0]["PosX"] = PosX.Text;
                    dataRows[0]["PosY"] = PosY.Text;
                    dataRows[0]["Size"] = Size.Text;
                    dataRows[0]["Rotate"] = Rotate.Text;
                    dataRows[0]["WindowID"] = WindowID.SelectedIndex + 1;
                }
            }
            catch (Exception ex) { ImportExport.LogMessage("Transmit Lamp: " + ex.ToString()); }
        }

        private void ImageOnSelect_Click(object sender, RoutedEventArgs e)
        {
            pictureOn = FileSelectDialog(".png", "PNG files (*.png)|*.png|All files (*.*)|*.*");

            if (pictureOn != "") { ImageOn.Text = pictureOn; }
        }

        private void ImageOffSelect_Click(object sender, RoutedEventArgs e)
        {
            pictureOff = FileSelectDialog(".png", "PNG files (*.png)|*.png|All files (*.*)|*.*");

            if (pictureOff != "") { ImageOff.Text = pictureOff; }
        }
        private void Classname_DropDownClosed(object sender, EventArgs e)
        {
            //ChangeSourceDatagridSwitches();
        }
    }
}

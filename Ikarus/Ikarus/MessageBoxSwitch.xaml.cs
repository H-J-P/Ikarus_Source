using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBoxSwitch : Window
    {
        private bool setSwitch = true;
        private static int index = 0;
        private static int clickable = 0;
        private static string name = "";
        private static string classname = "";
        private static string pictureOn = "";
        private static string pictureOff = "";
        private static string picture2On = "";
        private static string rotate = "";
        private static string input = "";
        private static string output = "";
        private static string posX = "";
        private static string posY = "";
        private static string size = "";
        private static string windowID = "";
        private static DataTable switches = new DataTable();
        private static DataRow[] dataRows = null;
        private static bool classNameChanged = false;

        public MessageBoxSwitch(int _index)
        {
            DataRow[] dataRows = MainWindow.dtSwitches.Select("ID=" + _index);
            index = _index;

            try
            {
                if (dataRows.Length > 0)
                {
                    classname = dataRows[0]["Class"].ToString();
                    clickable = Convert.ToInt32(dataRows[0]["ClickabledataID"]);
                    name = dataRows[0]["Name"].ToString();
                    pictureOn = dataRows[0]["FilePictureOn"].ToString();
                    pictureOff = dataRows[0]["FilePictureOff"].ToString();
                    picture2On = dataRows[0]["FilePicture2On"].ToString();
                    input = dataRows[0]["Input"].ToString();
                    output = dataRows[0]["Output"].ToString();
                    posX = dataRows[0]["PosX"].ToString();
                    posY = dataRows[0]["PosY"].ToString();
                    size = dataRows[0]["Size"].ToString();
                    rotate = dataRows[0]["Rotate"].ToString();
                    windowID = dataRows[0]["WindowID"].ToString();
                }
            }
            catch (Exception ex) { ImportExport.LogMessage("Dialog Switch: " + ex.ToString()); }

            Start();
        }

        public MessageBoxSwitch()
        {
            Start();
        }

        private void Start()
        {
            InitializeComponent();

            DataGridSwitches.CanUserAddRows = false;
            DataGridSwitches.CanUserDeleteRows = true;
            DataGridSwitches.IsReadOnly = false;

            WindowID.ItemsSource = null;
            WindowID.ItemsSource = MainWindow.dtWindows.DefaultView;
            WindowID.SelectedIndex = Convert.ToInt16(windowID) - 1;

            Classname.Text = classname;
            ImageOn.Text = pictureOn;
            ImageOff.Text = pictureOff;
            Image2on.Text = picture2On;
            Input.Text = input;
            Output.Text = output;
            PosX.Text = posX;
            PosY.Text = posY;
            Size.Text = size;
            Rotate.Text = rotate;

            if (MainWindow.dtMasterSwitches != null)
            {
                ChangeSourceDatagridSwitches();

                DataGridSwitches.ScrollIntoView(DataGridSwitches.Items[DataGridSwitches.Items.Count - 1]);

                try
                {
                    if (clickable != 0)
                    {
                        for (int n = 0; n < dataRows.Length; n++)
                        {
                            if (Convert.ToInt32(dataRows[n][0]) == clickable)
                            {
                                DataGridSwitches.SelectedIndex = n;
                                DataGridSwitches.ScrollIntoView(DataGridSwitches.Items[n]);
                                break;
                            }
                        }
                    }
                    else
                    {
                        //DataGridSwitches.SelectedIndex = 0;
                        DataGridSwitches.ScrollIntoView(DataGridSwitches.Items[0]);
                    }
                }
                catch { }
            }
            Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateData();
            Close();
        }

        private void ChangeSourceDatagridSwitches()
        {
            if (MainWindow.dtMasterSwitches != null)
            {
                setSwitch = true;

                if (Classname.Text == "Potentiometer") setSwitch = false;
                if (Classname.Text == "Rotary") setSwitch = false;
                if (Classname.Text == "RotaryWithPosition") setSwitch = false;

                if (Classname.Text == "MultiSwitch" || Classname.Text == "Rotary")
                {
                    dataRows = MainWindow.dtMasterSwitches.Select("Type='Switch' OR Type='Rotary'", "Discription");
                }
                else
                {
                    if (setSwitch)
                    {
                        dataRows = MainWindow.dtMasterSwitches.Select("Type='Switch'", "Discription");
                    }
                    else
                    {
                        dataRows = MainWindow.dtMasterSwitches.Select("Type='Rotary'", "Discription");
                    }
                }
                switches = dataRows.CopyToDataTable<DataRow>();
                DataGridSwitches.ItemsSource = null;
                DataGridSwitches.ItemsSource = switches.DefaultView;

                if (classNameChanged)
                {
                    if (Classname.Text == "Potentiometer" || Classname.Text == "Rotary")
                    {
                        Input.Text = "0.0,1.0,0.01";
                        Output.Text = " 0, 360";
                    }

                    if (Classname.Text == "MultiSwitch")
                    {
                        Input.Text = "0.0,0.1,0.2,0.3,0.4";
                        Output.Text = " 0, 45, 90, 135, 180";
                    }

                    if (Classname.Text == "SwitchOffOn" || Classname.Text == "ButtonOffOn" || Classname.Text == "ButtonWithRelease" || 
                        Classname.Text == "Button45x45" || Classname.Text == "Button70x70" || Classname.Text == "ButtonWithRepeat" || 
                        Classname.Text == "Switch_OFFOn")
                    {
                        Input.Text = "0.0,1.0";
                        Output.Text = "0.0,1.0";
                    }

                    if (Classname.Text == "SwitchOnOffOn" || Classname.Text == "Switch_OnOFFON" || Classname.Text == "Switch_OnOFFOn")
                    {
                        Input.Text = "-1.0,0.0,1.0";
                        Output.Text = "-1.0,0.0,1.0";
                    }
                    classNameChanged = false;
                }
            }
        }

        private string FileSelectDialog(string defaultExt, string filter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = MainWindow.currentDirectory + "\\Images\\Switches";

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
                DataRowView rowView = (DataRowView)DataGridSwitches.SelectedItem;

                DataRow[] dataRows = MainWindow.dtSwitches.Select("ID=" + index);

                if (dataRows.Length > 0)
                {
                    dataRows[0]["Class"] = Classname.Text;

                    if (DataGridSwitches.SelectedItem != null)
                    {
                        dataRows[0]["ClickabledataID"] = Convert.ToInt32(rowView.Row.ItemArray[0]);
                        dataRows[0]["Name"] = rowView.Row.ItemArray[3].ToString();

                        if (rowView.Row.ItemArray[5].ToString() == "Switch")
                            dataRows[0]["DcsID"] = rowView.Row.ItemArray[4].ToString();
                        else
                            dataRows[0]["DcsID"] = rowView.Row.ItemArray[5].ToString();
                    }
                    else
                    {
                        dataRows[0]["ClickabledataID"] = 0;
                        dataRows[0]["Name"] = "-";
                    }
                    dataRows[0]["FilePictureOn"] = ImageOn.Text;
                    dataRows[0]["FilePictureOff"] = ImageOff.Text;
                    dataRows[0]["FilePicture2On"] = Image2on.Text;
                    dataRows[0]["Input"] = Input.Text;
                    dataRows[0]["Output"] = Output.Text;
                    dataRows[0]["PosX"] = PosX.Text;
                    dataRows[0]["PosY"] = PosY.Text;
                    dataRows[0]["Size"] = Size.Text;
                    dataRows[0]["Rotate"] = Rotate.Text;
                    dataRows[0]["WindowID"] = WindowID.SelectedIndex + 1;
                }
            }
            catch (Exception ex) { ImportExport.LogMessage("Transmit Switch: " + ex.ToString()); }
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

        private void Image2OnSelect_Click(object sender, RoutedEventArgs e)
        {
            pictureOn = FileSelectDialog(".png", "PNG files (*.png)|*.png|All files (*.*)|*.*");

            if (pictureOn != "") { Image2on.Text = pictureOn; }
        }

        private void Classname_DropDownClosed(object sender, EventArgs e)
        {
            classNameChanged = true;
            ChangeSourceDatagridSwitches();
        }

        private void DataGridSwitches_MouseWheel(object sender, MouseWheelEventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ikarus
{
    /// <summary>
    /// Interaction logic for Instruments.xaml
    /// </summary>
    public partial class Cockpit : Window
    {
        #region members

        string backgroundFile = "";
        public static Window mainwindow = null;

        private static double posX = 0.0;
        private static double posY = 0.0;
        private static double sizeUsercontrol = 0.0;
        private static double size = 0.0;
        private static double scaling = 0.0;
        private static double rotate = 0.0;
        private static string package = "";
        private static string objectID = "";
        private static string dcsID = "";
        private static int instancePos = 0;

        private I_Ikarus interfaceUserControl;

        private UserControl userControl = new UserControl();
        private List<object> accessories = new List<object>();
        private List<object> gaugeObjects = new List<object>();
        private List<object> lampObjects = new List<object>();
        private List<object> switchObjects = new List<object>();
        private static DataRow[] instanceRows = new DataRow[] { };
        private static DataRow[] instancePosRows = new DataRow[] { };
        private static DataRow[] instanceFctRows = new DataRow[] { };
        private static DataRow[] dataRows = new DataRow[] { };

        private static double leftPos = 0.0;
        private static double topPos = 0.0;
        private static double centerX = 0.0;
        private static double centerY = 0.0;

        private static TransformGroup transformGroup = new TransformGroup();
        private static TranslateTransform transformTrans = new TranslateTransform();
        private static ScaleTransform transformScale = new ScaleTransform();
        private static RotateTransform transformRotate = new RotateTransform();
        private static Point resultPoint = new Point();
        private static double scrollWeel = 0.0;
        private int windowID = 0;

        private Instrument instrument = null;
        private Switches switches = null;
        private Lamps lamp = null;

        #endregion

        #region Functions

        public Cockpit(int _windowID, string _backgroundFile)
        {
            try
            {
                InitializeComponent();

                RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;

                windowID = _windowID - 1;
                Focusable = false;
                Topmost = true;
                BackImage.Source = null;
                MousePosition.Text = "";

                ImportExport.LogMessage("Start loading Cockpit ... " + _backgroundFile);

                backgroundFile = Environment.CurrentDirectory + "\\Images\\Backgounds\\" + _backgroundFile;

                if (File.Exists(backgroundFile)) BackImage.Source = new BitmapImage(new Uri(backgroundFile));

                #region generate accessories dynamically -----

                accessories.Clear();

                instanceRows = MainWindow.dtAccessories.Select("WindowID=" + _windowID.ToString());

                for (int i = 0; i < instanceRows.Length; i++)
                {
                    GenerateAccessory(Convert.ToInt16(instanceRows[i][0]), instanceRows[i][1].ToString(), instanceRows[i][2].ToString());
                }
                #endregion

                #region generate gauges dynamically -----

                gaugeObjects.Clear();

                instanceRows = MainWindow.dtInstruments.Select("WindowID=" + _windowID.ToString());

                for (int i = 0; i < instanceRows.Length; i++)
                {
                    GenerateGauge(Convert.ToInt16(instanceRows[i][0]), instanceRows[i][2].ToString(), instanceRows[i][1].ToString());
                }
                #endregion

                #region generate lamps dynamically -----

                lampObjects.Clear();

                instanceRows = MainWindow.dtLamps.Select("WindowID=" + _windowID.ToString());

                for (int i = 0; i < instanceRows.Length; i++)
                {
                    GenerateLamp(Convert.ToInt16(instanceRows[i][0]), instanceRows[i][1].ToString(), instanceRows[i][3].ToString());
                }
                #endregion

                #region generate switches dynamically -----

                switchObjects.Clear();

                instanceRows = MainWindow.dtSwitches.Select("WindowID=" + _windowID.ToString());

                for (int i = 0; i < instanceRows.Length; i++)
                {
                    GenerateSwitch(Convert.ToInt16(instanceRows[i][0]), instanceRows[i][1].ToString(), instanceRows[i][3].ToString());
                }
                #endregion

                ImportExport.LogMessage("End loading Cockpit ... " + _backgroundFile);
            }
            catch (Exception e) { ImportExport.LogMessage("Generate Cockpit .... " + e.ToString()); }
        }

        private void GenerateAccessory(int instanceID, string classname, string name)
        {
            try
            {
                accessories.Add(Activator.CreateInstance(Type.GetType("Ikarus." + classname)));
                int i = accessories.Count - 1;
                userControl = (UserControl)accessories[i];
                userControl.Focusable = false;

                interfaceUserControl = (I_Ikarus)userControl;
                interfaceUserControl.SetID(instanceID.ToString()); // importent
                interfaceUserControl.SetWindowID(windowID);

                SetObjectsPara(ref MainWindow.dtAccessories, ref instanceID);

                MainGrid.Children.Add(userControl);
            }
            catch (Exception e) { ImportExport.LogMessage("Generate accessories: " + e.ToString()); }
        }

        private void GenerateGauge(int instanceID, string classname, string name)
        {
            try
            {
                if (classname == "-" || classname == "") return;

                try
                {
                    gaugeObjects.Add(Activator.CreateInstance(Type.GetType("Ikarus." + classname)));
                }
                catch
                {
                    ImportExport.LogMessage("Gauge not found: " + classname.ToString());
                    gaugeObjects.Add(Activator.CreateInstance(Type.GetType("Ikarus.Experimental")));
                }

                try
                {
                    int i = gaugeObjects.Count - 1;
                    userControl = (UserControl)gaugeObjects[i];
                    userControl.Focusable = false;
                    userControl.ToolTip = instanceID + " - " + name;

                    interfaceUserControl = (I_Ikarus)userControl;
                    interfaceUserControl.SetID(instanceID.ToString()); // importent
                    interfaceUserControl.SetWindowID(windowID);

                    userControl.VerticalAlignment = VerticalAlignment.Top;
                    userControl.HorizontalAlignment = HorizontalAlignment.Left;

                    instancePosRows = MainWindow.dtInstruments.Select("IDInst=" + instanceID.ToString());

                    leftPos = double.Parse(instancePosRows[0]["PosX"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                    topPos = double.Parse(instancePosRows[0]["PosY"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);

                    userControl.Margin = new Thickness(leftPos, topPos, 0, 0);
                    userControl.ClipToBounds = true;

                    if (MainWindow.editmode)
                    {
                        userControl.BorderBrush = new SolidColorBrush(Colors.Orange);
                        userControl.BorderThickness = new Thickness(1.0);
                    }

                    sizeUsercontrol = interfaceUserControl.GetSize();
                    size = double.Parse(instancePosRows[0]["Size"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                    scaling = size / sizeUsercontrol;
                    rotate = double.Parse(instancePosRows[0]["Rotate"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);

                    userControl.RenderTransformOrigin = new Point(0.5, 0.5);

                    transformGroup = new TransformGroup();
                    transformScale = new ScaleTransform();
                    transformRotate = new RotateTransform();

                    transformScale.ScaleX = scaling;
                    transformScale.ScaleY = scaling;
                    transformRotate.Angle = rotate;

                    transformGroup.Children.Add(transformScale);
                    transformGroup.Children.Add(transformRotate);
                    userControl.LayoutTransform = transformGroup;
                }
                catch (Exception e) { ImportExport.LogMessage("Generate gauges - get usercontrol:  .. " + e.ToString()); }

                instanceFctRows = MainWindow.dtInstrumentFunctions.Select("IDInst=" + instanceID); // instrument functions

                for (int j = 0; j < instanceFctRows.Length; j++)
                {
                    try
                    {
                        interfaceUserControl.SetInput(instanceFctRows[j][7].ToString());
                        interfaceUserControl.SetOutput(instanceFctRows[j][8].ToString());
                    }
                    catch (Exception e)
                    {
                        ImportExport.LogMessage("Generate gauges - Set Input/output: " + userControl.ToString() + " .... " + e.ToString());
                    }
                }
                MainGrid.Children.Add(userControl);
            }
            catch (Exception e) { ImportExport.LogMessage("Generate gauges: " + userControl.ToString() + " .... " + e.ToString()); }
        }

        private void GenerateLamp(int instanceID, string classname, string name)
        {
            try
            {
                lampObjects.Add(Activator.CreateInstance(Type.GetType("Ikarus." + classname)));
                int i = lampObjects.Count - 1;
                userControl = (UserControl)lampObjects[i];
                userControl.Focusable = false;
                userControl.ToolTip = name;

                interfaceUserControl = (I_Ikarus)userControl;

                interfaceUserControl.SetID(instanceID.ToString()); // importent Arg_number
                interfaceUserControl.SetWindowID(windowID);

                instanceFctRows = MainWindow.dtLamps.Select("ID=" + instanceID.ToString());

                SetObjectsPara(ref MainWindow.dtLamps, ref instanceID);

                MainGrid.Children.Add(userControl);
            }
            catch (Exception e) { ImportExport.LogMessage("Generate lamps: " + e.ToString()); }
        }

        private void GenerateSwitch(int instanceID, string classname, string name)
        {
            try
            {
                switchObjects.Add(Activator.CreateInstance(Type.GetType("Ikarus." + classname)));
                int i = switchObjects.Count - 1;
                userControl = (UserControl)switchObjects[i];
                userControl.Focusable = false;

                interfaceUserControl = (I_Ikarus)userControl;

                interfaceUserControl.SetID(instanceID.ToString()); // importent
                interfaceUserControl.SetWindowID(windowID);

                instanceFctRows = MainWindow.dtSwitches.Select("ID=" + instanceID.ToString());
                dcsID = "";

                try
                {
                    interfaceUserControl.SetInput(instanceFctRows[0]["Input"].ToString());
                    interfaceUserControl.SetOutput(instanceFctRows[0]["Output"].ToString());
                    dcsID = instanceFctRows[0]["DcsID"].ToString();
                }
                catch { }

                userControl.ToolTip = dcsID + " - " + name;
                SetObjectsPara(ref MainWindow.dtSwitches, ref instanceID);

                MainGrid.Children.Add(userControl);
            }
            catch (Exception e) { ImportExport.LogMessage("Generate switches: " + classname + " .... " + e.ToString()); }
        }

        /// <summary>
        /// Gets the current mouse position on screen
        /// </summary>
        private Point GetMousePosition()
        {
            // Position of the mouse relative to the window
            var position = Mouse.GetPosition(this);

            // Add the window position
            return new Point(position.X + this.Left, position.Y + this.Top);
        }
        private void SetObjectsPara(ref DataTable table, ref int instanceID)
        {
            try
            {
                instancePosRows = table.Select("ID=" + instanceID.ToString());

                userControl.VerticalAlignment = VerticalAlignment.Top;
                userControl.HorizontalAlignment = HorizontalAlignment.Left;

                leftPos = double.Parse(instancePosRows[0]["PosX"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                topPos = double.Parse(instancePosRows[0]["PosY"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);

                userControl.Margin = new Thickness(leftPos, topPos, 0, 0);
                userControl.ClipToBounds = true;

                sizeUsercontrol = interfaceUserControl.GetSize();
                size = double.Parse(instancePosRows[0]["Size"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                scaling = size / sizeUsercontrol;
                rotate = double.Parse(instancePosRows[0]["Rotate"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);

                userControl.RenderTransformOrigin = new Point(0.5, 0.5);

                centerX = sizeUsercontrol / 2;
                centerY = interfaceUserControl.GetSizeY() / 2;

                if (table.TableName == "Switches" || table.TableName == "Lamps")
                    rotate = 0.0;

                transformGroup = new TransformGroup();
                transformScale = new ScaleTransform(scaling, scaling, centerX, centerY);
                transformRotate = new RotateTransform(rotate, centerX, centerY);

                transformGroup.Children.Add(transformScale);
                transformGroup.Children.Add(transformRotate);

                userControl.LayoutTransform = transformGroup;
            }
            catch (Exception e) { ImportExport.LogMessage("SetObjectsPara " + e.ToString()); }
        }

        public void Close_Cockpit()
        {
            try
            {
                for (int n = 0; n < accessories.Count; n++)
                {
                    accessories[n] = null;
                }

                for (int n = 0; n < gaugeObjects.Count; n++)
                {
                    gaugeObjects[n] = null;
                }

                for (int n = 0; n < lampObjects.Count; n++)
                {
                    lampObjects[n] = null;
                }

                for (int n = 0; n < switchObjects.Count; n++)
                {
                    switchObjects[n] = null;
                }

                MainWindow.cockpitWindows[windowID] = null;
                Visibility = Visibility.Hidden;

                accessories.Clear();
                gaugeObjects.Clear();
                lampObjects.Clear();
                switchObjects.Clear();

                GC.Collect(0, GCCollectionMode.Forced);

                Close();
            }
            catch (Exception ex) { ImportExport.LogMessage("Close_Cockpit: " + ex.ToString()); }
        }

        public int GetObjectID(ref List<object> objects, ref int instanceID)
        {
            try
            {
                for (int i = 0; i < objects.Count; i++)
                {
                    userControl = (UserControl)objects[i];
                    interfaceUserControl = (I_Ikarus)userControl;
                    objectID = interfaceUserControl.GetID();

                    if (Convert.ToInt32(objectID) == instanceID)
                    {
                        instancePos = i;
                        break;
                    }
                }
            }
            catch (Exception ex) { ImportExport.LogMessage("GetObjectID: " + ex.ToString()); }

            return instancePos;
        }

        public void UpdateInstruments(int instanceID, bool editmode)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Send,
            (Action)(() =>
            {
                try
                {
                    userControl = (UserControl)gaugeObjects[GetObjectID(ref gaugeObjects, ref instanceID)];
                    interfaceUserControl = (I_Ikarus)userControl;

                    #region sinmulation

                    if (!editmode)
                    {
                        instrument = MainWindow.instruments.Find(x => x.instID == instanceID);
                        package = "";

                        if (instrument != null)
                        {
                            for (int k = 0; k < instrument.instrumentFunction.Count; k++)
                            {
                                if (k < instrument.instrumentFunction.Count - 1)
                                {
                                    if (instrument.instrumentFunction[k].ascii)
                                        package += instrument.instrumentFunction[k].asciiValue + ";";
                                    else
                                        package += instrument.instrumentFunction[k].value.ToString(CultureInfo.InvariantCulture) + ";";
                                }
                                else
                                {
                                    if (instrument.instrumentFunction[k].ascii)
                                        package += (instrument.instrumentFunction[k].asciiValue);
                                    else
                                        package += (instrument.instrumentFunction[k].value.ToString(CultureInfo.InvariantCulture));
                                }
                            }
                        }
                    }
                    #endregion

                    #region slider test
                    else
                    {
                        instanceRows = MainWindow.dtInstrumentFunctions.Select("IDInst=" + instanceID, "IDFct ASC");
                        package = "";

                        for (int k = 0; k < instanceRows.Length; k++)
                        {
                            if (k < instanceRows.Length - 1)
                            {
                                if (Convert.ToBoolean(instanceRows[k]["Ascii"]))
                                    package += instanceRows[k]["AsciiValue"].ToString() + ";";
                                else
                                    package += instanceRows[k]["Value"].ToString().Replace(",", ".") + ";";
                            }
                            else
                            {
                                if (Convert.ToBoolean(instanceRows[k]["Ascii"]))
                                    package += instanceRows[k]["AsciiValue"].ToString().Replace(",", ".");
                                else
                                    package += instanceRows[k]["Value"].ToString().Replace(",", ".");
                            }
                        }
                    }
                    #endregion

                    interfaceUserControl.UpdateGauge(package);

                    if (MainWindow.detailLog) { ImportExport.LogMessage("Update gauge " + userControl.ToString() + " with package: " + package); }
                }
                catch (Exception e) { ImportExport.LogMessage("Update gauge: " + userControl.ToString() + e.ToString()); }
            }));
        }

        public void UpdateInstrumentLights(bool isChecked, string _backgroundFile)
        {
            for (int i = 0; i < gaugeObjects.Count; i++)
            {
                try
                {
                    userControl = (UserControl)gaugeObjects[i];
                    interfaceUserControl = (I_Ikarus)userControl;
                    interfaceUserControl.SwitchLight(isChecked);
                }
                catch (Exception e)
                {
                    ImportExport.LogMessage("Update instruments lights: " + e.ToString());
                }
            }
            backgroundFile = Environment.CurrentDirectory + "\\Images\\Backgounds\\" + _backgroundFile;

            if (File.Exists(backgroundFile)) BackImage.Source = new BitmapImage(new Uri(backgroundFile));
        }

        public void UpdateSwitches(int instanceID)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               switches = MainWindow.switches.Find(x => x.ID == instanceID);
                               userControl = (UserControl)switchObjects[GetObjectID(ref switchObjects, ref instanceID)];

                               if (switches != null)
                               {
                                   if (switches.ignoreNextPackage)
                                   {
                                       switches.ignoreNextPackage = false;

                                       if (MainWindow.detailLog) { ImportExport.LogMessage("Ignore value for ID: " + switches.dcsID.ToString() + " class: " + userControl.ToString() + " value: " + switches.value.ToString()); }
                                   }
                                   else
                                   {
                                       interfaceUserControl = (I_Ikarus)userControl;
                                       package = switches.value.ToString(CultureInfo.InvariantCulture);
                                       interfaceUserControl.UpdateGauge(package);

                                       if (MainWindow.detailLog || MainWindow.switchLog) { ImportExport.LogMessage("Update ID: " + switches.dcsID.ToString() + " class: " + userControl.ToString() + " with value: " + package); }
                                   }
                               }
                           }
                           catch (Exception e) { ImportExport.LogMessage("Update switch ID: " + switches.dcsID.ToString() + e.ToString()); }
                       }));
        }

        public void UpdateLamps(int instanceID)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           try
                           {
                               lamp = MainWindow.lamps.Find(x => x.ID == instanceID);

                               if (lamp != null)
                               {
                                   userControl = (UserControl)lampObjects[GetObjectID(ref lampObjects, ref instanceID)];
                                   interfaceUserControl = (I_Ikarus)userControl;

                                   package = lamp.value.ToString(CultureInfo.InvariantCulture);
                                   interfaceUserControl.UpdateGauge(package);

                                   if (MainWindow.detailLog) { ImportExport.LogMessage("Update ID: " + lamp.argNumber.ToString() + " Class: " + userControl.ToString() + " with value: " + package); }
                               }
                           }
                           catch (Exception e) { ImportExport.LogMessage("Update lamp ID: " + lamp.argNumber.ToString() + e.ToString()); }
                       }));
        }

        public static void UpdateInOut(string idInst, string idFct, string inValue, string outValue)
        {
            if (!MainWindow.editmode) return;

            try
            {
                dataRows = MainWindow.dtInstrumentFunctions.Select("IDInst=" + idInst + " AND IDFct=" + idFct);

                if (dataRows.Length > 0)
                {
                    dataRows[0]["In"] = inValue;
                    dataRows[0]["Out"] = outValue;
                }
            }
            catch (Exception e) { ImportExport.LogMessage("UpdateInstrumentsFunctionInOut .. " + e.ToString()); }
        }

        public bool UpdatePosition(Point coordinates, string tableName, string ID, int deltaSize = 0)
        {
            try
            {
                if (tableName == "Accessories")
                    dataRows = MainWindow.dtAccessories.Select("ID=" + ID);
                if (tableName == "Instruments")
                    dataRows = MainWindow.dtInstruments.Select("IDInst=" + ID);
                if (tableName == "Switches")
                    dataRows = MainWindow.dtSwitches.Select("ID=" + ID);
                if (tableName == "Lamps")
                    dataRows = MainWindow.dtLamps.Select("ID=" + ID);

                if (dataRows.Length > 0)
                {
                    if (Left < 0.0)
                        resultPoint.X = (Left * -1.0) + coordinates.X;
                    else
                        resultPoint.X = (Left > 0.0) ? coordinates.X - Left : coordinates.X;

                    if (Top < 0.0)
                        resultPoint.Y = (Top * -1.0) + coordinates.Y;
                    else
                        resultPoint.Y = (Top > 0.0) ? coordinates.Y - Top : coordinates.Y;

                    scrollWeel += deltaSize;

                    if (scrollWeel > 1.0) scrollWeel = 1.0;
                    if (scrollWeel < -1.0) scrollWeel = -1.0;

                    if (tableName == "Switches" || tableName == "Lamps")
                        rotate = 0.0;
                    else
                        rotate = double.Parse(dataRows[0]["Rotate"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);

                    if (rotate == 0.0)
                    {
                        dataRows[0]["PosX"] = Convert.ToInt32(resultPoint.X);
                        dataRows[0]["PosY"] = Convert.ToInt32(resultPoint.Y);
                    }

                    if (scrollWeel == 1.0) { dataRows[0]["Size"] = Convert.ToInt32(dataRows[0]["Size"]) + 5; }

                    if (scrollWeel == -1.0) { dataRows[0]["Size"] = Convert.ToInt32(dataRows[0]["Size"]) - 5; }

                    //if (tableName == "Lamps") ID = dataRows[0]["ID"].ToString();

                    scrollWeel = 0.0;

                    posX = double.Parse(dataRows[0]["PosX"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                    posY = double.Parse(dataRows[0]["PosY"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                    size = double.Parse(dataRows[0]["Size"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);

                    UpdateTransform(tableName, Convert.ToInt32(dataRows[0][0]), posX, posY, size, rotate);
                }
                ((MainWindow)Application.Current.MainWindow).SelectDataGridItem(tableName, Convert.ToInt32(ID));

                MainWindow.refreshCockpit = true;
            }
            catch (Exception e) { ImportExport.LogMessage("UpdatePosition .. " + e.ToString()); }

            return (rotate != 0.0);
        }

        public void UpdateTransform(string tableName, int importID, double posX, double posY, double size, double rotate)
        {
            try
            {
                if (tableName == "Accessories")
                {
                    userControl = (UserControl)accessories[GetObjectID(ref accessories, ref importID)];
                    interfaceUserControl = (I_Ikarus)userControl;
                }

                if (tableName == "Instruments")
                {
                    userControl = (UserControl)gaugeObjects[GetObjectID(ref gaugeObjects, ref importID)];
                    interfaceUserControl = (I_Ikarus)userControl;
                }

                if (tableName == "Switches")
                {
                    userControl = (UserControl)switchObjects[GetObjectID(ref switchObjects, ref importID)];
                    interfaceUserControl = (I_Ikarus)userControl;
                    rotate = 0.0;
                }

                if (tableName == "Lamps")
                {
                    userControl = (UserControl)lampObjects[GetObjectID(ref lampObjects, ref importID)];
                    interfaceUserControl = (I_Ikarus)userControl;
                }

                sizeUsercontrol = interfaceUserControl.GetSize();
                scaling = size / sizeUsercontrol;
                centerX = sizeUsercontrol / 2;
                centerY = interfaceUserControl.GetSizeY() / 2;

                transformGroup = new TransformGroup();
                transformTrans = new TranslateTransform(posX, posY);
                transformScale = new ScaleTransform(scaling, scaling, centerX, centerY);
                transformRotate = new RotateTransform(rotate, centerX, centerY);

                transformGroup.Children.Add(transformTrans);
                transformGroup.Children.Add(transformScale);
                transformGroup.Children.Add(transformRotate);
                userControl.LayoutTransform = transformGroup;
            }
            catch (Exception e) { ImportExport.LogMessage("Update position, size and rotation: " + e.ToString()); }

            UpdateLayout();
            InvalidateVisual();
        }

        private void SetFocusTo()
        {
            Focusable = false;
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        }

        #endregion

        #region event

        private void BackImage_GotFocus(object sender, RoutedEventArgs e)
        {
            if (windowID == 0) MainWindow.refeshPopup = true;
            SetFocusTo();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
            if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
            MemoryManagement.Reduce();
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            if (windowID == 0) MainWindow.refeshPopup = true;
            SetFocusTo();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (windowID == 0) MainWindow.refeshPopup = true;
            SetFocusTo();
        }

        #endregion

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainWindow.editmode)
                MousePosition.Text = GetMousePosition().ToString();
        }
    }
}

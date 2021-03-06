﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Drawing.Imaging;
using System.Reflection;
using Newtonsoft.Json;

namespace Ikarus
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/ee658248.aspx
        /// http://www.philosophicalgeek.com/2012/07/16/how-to-debug-gc-issues-using-perfview/
        /// PerfMon.exe
        /// Perfview.exe
        /// </summary>

        #region Member

        public enum State
        {
            startup,
            init,
            run,
            reset,
            stop
        }
        private State timerstate = State.startup;

        private bool cleanupMemory = true;
        public static bool cockpitWindowActiv = false;
        public static bool dataLog = false;
        public static bool detailLog = false;
        public static bool editmode = false;
        public static bool shadowChecked = true;

        public static bool activateR = true; // Data package R 

        private bool functionTabIsVisible = true;
        public static bool getAllDscData = false;
        private bool initInstruments = true;
        private bool lightsChecked = false;
        private bool lStateEnabled = true;
        private bool lUdpEnabled = true;
        private bool idFound = false;

        public static bool refreshCockpit = false;
        private bool refreshInstruments = false;
        public static bool refeshPopup = false;
        public static bool switchLog = false;
        public static bool mainGotFocus = false;

        public static CultureInfo cult = new CultureInfo("en-GB");
        //---------------------- D A T A B A S E ----------------------
        public static DataSet1 dsInstruments = new DataSet1();
        public static DataSet2 dsConfig = new DataSet2();
        public static DataSet dsJSON = new DataSet();

        public static DataTable dtConfig;
        public static DataTable dtInstruments;
        public static DataTable dtInstrumentFunctions;
        public static DataTable dtParameter;
        public static DataTable dtLamps;
        public static DataTable dtSwitches;
        public static DataTable dtAccessories;
        public static DataTable dtClassnames;
        public static DataTable dtWindows;
        //---------------------- Tables for JSON -----------------------------
        public static DataSet dsJson;
        public static DataTable dtjsonLamps;
        public static DataTable dtjsonSwitches;
        public static DataTable dtjsonInstFunctions;
        //---------------------- D A C datas --------------------------------- 
        public static Masterdata dsMaster = new Masterdata();
        public static DataTable dtMasterLamps;
        public static DataTable dtMasterSwitches;

        private static DataRowView rowView = null;
        private static DataRow[] dataRows = new DataRow[] { };
        public static DataRow dataRow = null;
        private static DataRow[] dataRowsInstrumentsFunction = new DataRow[] { };
        private static DataRow[] dataRowsMasterSwitches = new DataRow[] { };
        private static DataRow[] dataRowsSwitches = new DataRow[] { };
        private static Assembly assembly = Assembly.GetExecutingAssembly();

        public static List<Cockpit> cockpitWindows = new List<Cockpit> { };
        public static List<Instrument> instruments = new List<Instrument> { };
        public static List<Lamps> lamps = new List<Lamps> { };
        public static List<Switches> switches = new List<Switches> { };
        private static List<string> updateLamp = new List<string>();
        public static List<MainWindow> mainWindow = new List<MainWindow>();

        private static Thread udpThread = null;

        private int cockpitRefreshLoopCounterMax = 1;
        private int cockpitRefreshLoopCounter = 0;
        private int dscDataLoopCounterMax = 3000; // 60 sec.
        private int getAllDscDataLoopCounter = 0;
        private int panel = 0;
        private int logCount = 0;
        private int loopCounter = 0;
        private int loopMax = 150;
        //private int renderTier = 0;
        private int selectedAccessories = -1;
        private int selectedIndexLamps = 0;
        private int selectedIndexSwitches = 0;
        private int selectedInstrument = -1;
        private int selectedFunction = -1;
        private int selectedLamp = -1;
        private int selectedSwitch = -1;
        private int selectedTab = -1;
        private int selectedWindows = -1;
        private int windowID = 0;

        private string identifier = "";
        private string background = "";
        private string dbFilename = "";
        private static string newline = Environment.NewLine;
        public static string currentDirectory = Environment.CurrentDirectory;
        private string portListener = "";
        private string receivedData = "";
        private string[] receivedItems = new string[] { };
        public static string readFile = "";
        private string lastFile = "-";
        private string searchStringForFile = "File";
        private string lastSelectedInstrumentsClass = "";
        public static string map = "";
        private string package = "";
        public static string processNameDCS = "DCS";
        public static string lightOnColor = "95E295"; // green
        private string newGrabValue = "";
        public static string json = "";
        public static bool jsonChecked = false;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;

            Thread.CurrentThread.CurrentCulture = cult;
            Thread.CurrentThread.CurrentUICulture = cult;

            Version.Content = ((AssemblyCopyrightAttribute)assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute))).Copyright +
                "  Version " + Assembly.GetEntryAssembly().GetName().Version.ToString();

            this.Title = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;

            mainWindow.Add(this);

            try
            {
                lbLogging.Visibility = Visibility.Visible;

                File.Text = "";
                IPAddess.Text = "127.0.0.1";
                PortListener.Text = "1625";
                portListener = PortListener.Text;
                PortSender.Text = "26027";
                textBoxLightColor.Text = "95E295";
                getAllDscDataLoopCounter = dscDataLoopCounterMax;
                cockpitRefreshLoopCounter = cockpitRefreshLoopCounterMax;

                ImportExport.LogMessage(Version.Content + "   Application started .. ");

                try
                {
                    ImportExport.XmlToDataSet("Config.xml", dsConfig);

                    if (dsConfig.Tables.Count > 1)
                    {
                        dtConfig = dsConfig.Tables[0];
                        dtClassnames = dsConfig.Tables[1];

                        if (dtConfig.Rows.Count > 0)
                        {
                            dbFilename = dtConfig.Rows[0][4].ToString();
                            IPAddess.Text = dtConfig.Rows[0][5].ToString();
                            PortListener.Text = dtConfig.Rows[0][6].ToString();

                            portListener = dtConfig.Rows[0][6].ToString();
                            PortSender.Text = dtConfig.Rows[0][7].ToString();
                            checkBoxShadow.IsChecked = Convert.ToBoolean(dtConfig.Rows[0][10]); //

                            if (DBNull.Value.Equals(dtConfig.Rows[0]["JSON"]))
                            {
                                dtConfig.Rows[0]["JSON"] = false;
                            }
                            jsonChecked = Convert.ToBoolean(dtConfig.Rows[0]["JSON"]);
                            checkJSON.IsChecked = jsonChecked;

                            if (jsonChecked) { SendJSON.Visibility = Visibility.Visible; }
                            else { SendJSON.Visibility = Visibility.Hidden; }


                            if (dbFilename.Length > 0)
                            {
                                Main.Title += " - ( Configured for " + dbFilename.Substring(0, dbFilename.LastIndexOf(".")) + " )";
                            }
                        }
                    }
                    else
                    {
                        ImportExport.LogMessage("++++++ Config.xml problem: None definitions for configuration loaded.");
                    }
                }
                catch (Exception e) { ImportExport.LogMessage("Config.xml problem .. " + e.ToString()); }

                try
                {
                    if (dtConfig.Rows.Count > 0)
                    {
                        if (dbFilename.Length > 0)
                        {
                            ImportExport.XmlToDataSet(dbFilename, dsInstruments);
                            ImportExport.XmlToDataSet(dbFilename.Substring(0, dbFilename.LastIndexOf(".")) + ".xml", dsMaster);
                            readFile = dbFilename.Substring(0, dbFilename.LastIndexOf("."));
                        }
                        else
                        {
                            ImportExport.LogMessage("++++++ None configuration loaded. File=''");
                        }

                        if (dsMaster.Tables.Count > 0)
                        {
                            dtMasterLamps = dsMaster.Tables[0];
                            if (dtMasterLamps.Rows.Count == 0) ImportExport.LogMessage("++++++ None definitions for lamps found.");
                        }
                        else
                        {
                            ImportExport.LogMessage("++++++ None definitions for lamps found.");
                        }
                        if (dsMaster.Tables.Count > 1)
                        {
                            dtMasterSwitches = dsMaster.Tables[1];
                            if (dtMasterSwitches.Rows.Count == 0) ImportExport.LogMessage("++++++ None definitions for switches found.");
                        }
                        else
                        {
                            ImportExport.LogMessage("++++++ None definitions for switches found.");
                        }
                    }
                }
                catch (Exception e) { ImportExport.LogMessage("XmlToDataSet problem .. " + e.ToString()); }

                DataGridWindows.Visibility = Visibility.Visible;

                dtInstruments = dsInstruments.Tables[0];
                dtInstrumentFunctions = dsInstruments.Tables[1];
                dtLamps = dsInstruments.Tables[2];
                dtSwitches = dsInstruments.Tables[3];
                dtAccessories = dsInstruments.Tables[4];
                dtParameter = dsInstruments.Tables[5];
                dtWindows = dsInstruments.Tables[6];

                SetWindowID(dtInstruments);
                SetWindowID(dtLamps);
                SetWindowID(dtSwitches);
                SetWindowID(dtAccessories);

                DatagridFunction.ItemsSource = dtInstrumentFunctions.DefaultView;
                RefreshDatagrids();
                DatabaseResetValue();
                FillClasses();

                if (dtWindows.Rows.Count > 0)
                    textBoxLightColor.Text = dtWindows.Rows[0][9].ToString();

                lightOnColor = textBoxLightColor.Text;

                for (int n = 0; n < dtSwitches.Rows.Count; n++) // update switches
                {
                    dtSwitches.Rows[n]["DontReset"] = false;
                    dtSwitches.Rows[n]["Event"] = false;
                }

                if (dtParameter.Rows.Count > 0)
                {
                    background = dtParameter.Rows[0][0].ToString();
                }

                if (dtWindows.Rows.Count == 0)
                {
                    if (dtConfig.Rows.Count > 0)
                        dtWindows.Rows.Add(1, "Front Panel", dtConfig.Rows[0][0].ToString(), dtConfig.Rows[0][1].ToString(), dtConfig.Rows[0][2].ToString(), dtConfig.Rows[0][3].ToString(), background);
                }

                tabControl1.SelectedIndex = 5;
                selectedTab = 0;

                lbEditMode.Visibility = Visibility.Hidden;
                Refresh.Visibility = Visibility.Hidden;

                ListBox1.ItemsSource = ImportExport.logItems;

                functionTabIsVisible = true;
                if (!functionTabIsVisible) tabControl1.Items.Remove(Function);

                detailLog = false;
                checkBoxLog.IsChecked = detailLog;
                lbLogging.Visibility = detailLog ? Visibility.Visible : Visibility.Hidden;

                switchLog = false;
                checkBoxLogSwitches.SetCurrentValue(CheckBox.IsCheckedProperty, switchLog);

                lbLogging.Visibility = switchLog ? Visibility.Visible : Visibility.Hidden;

                cleanupMemory = false;

                DataGridInstruments.CanUserDeleteRows = true;
                DatagridFunction.CanUserDeleteRows = true;
                DataGridSwitches.CanUserDeleteRows = true;
                DataGridLamps.CanUserDeleteRows = true;
                DataGridWindows.CanUserDeleteRows = true;

                //dsJSON.Tables.Add(DataTable "Swichtes")
            }
            catch (Exception e) { ImportExport.LogMessage("Startup problem .. " + e.ToString()); }

            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           udpThread = new Thread(new ThreadStart(StartListener))
                           {
                               IsBackground = true
                           };
                           udpThread.Start();
                       }));

            timerstate = State.run;
            StartTimer();
            StartUDPTimer();


            //GetTier();
            //GC.Collect(0, GCCollectionMode.Forced);
        }

        #region Timer / Mainloop

        private void StartTimer()
        {
            DispatcherTimer timerMain = new DispatcherTimer(DispatcherPriority.Normal);
            timerMain.Tick += TimerMain_Tick;
            timerMain.Interval = TimeSpan.FromMilliseconds(10.0); // 100
            timerMain.Start();
        }

        private void StartUDPTimer()
        {
            DispatcherTimer updTimer = new DispatcherTimer(DispatcherPriority.Normal);
            updTimer.Tick += UdpTimerTick;
            updTimer.Interval = TimeSpan.FromMilliseconds(1.0);
            updTimer.Start();
        }

        private void UdpTimerTick(object sender, EventArgs e)
        {
            try
            {
                if (UDP.receivedDataStack.Count > 0)
                {
                    if (lUdpEnabled)
                    {
                        lUdpEnabled = false;

                        receivedData = UDP.receivedDataStack[0];
                        UDP.receivedDataStack.RemoveAt(0);

                        GrabValues();

                        lUdpEnabled = true;
                    }
                    receivedData = "";
                }
                SendDataFromButtonEvent();
            }
            catch (Exception f)
            {
                ImportExport.LogMessage("UdpTimerTick ... " + f.ToString(), true);
            }
        }

        private void TimerMain_Tick(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           #region stop - Editor Mode

                           if (timerstate == State.stop)
                           {
                               if (lStateEnabled)
                               {
                                   lStateEnabled = false;

                                   SliderTest(); // Get the values from the slider

                                   lStateEnabled = true;
                               }
                           }
                           #endregion

                           #region run - New Data from DCS

                           if (timerstate == State.run)
                           {
                               if (lStateEnabled)
                               {
                                   lStateEnabled = false;

                                   lStateEnabled = true;
                               }
                           }
                           #endregion

                           #region Switches Update

                           if (lStateEnabled)
                           {
                               lStateEnabled = false;


                               lStateEnabled = true;
                           }

                           #endregion

                           #region tools

                           if (lStateEnabled)
                           {
                               lStateEnabled = false;

                               if (cleanupMemory)
                               {
                                   loopCounter++;

                                   if (loopCounter >= loopMax)
                                   {
                                       MemoryManagement.Reduce();
                                       loopCounter = 0;
                                   }
                               }
                               lStateEnabled = true;
                           }
                           #endregion

                           #region refresh switches

                           if (lStateEnabled)
                           {
                               lStateEnabled = false;

                               try
                               {
                                   getAllDscData = true;

                                   if (getAllDscData)
                                   {
                                       if (--getAllDscDataLoopCounter < 1)
                                       {
                                           RefreshAllSwitches(); // "R"
                                           getAllDscDataLoopCounter = dscDataLoopCounterMax;

                                           if (switchLog) UpdateLog();
                                       }
                                   }
                               }
                               catch (Exception ex)
                               {
                                   ImportExport.LogMessage("Refresh all switches: " + ex.ToString());
                               }
                               lStateEnabled = true;
                           }
                           #endregion

                           #region refresh popup windows;

                           if (lStateEnabled)
                           {
                               lStateEnabled = false;

                               try
                               {
                                   if (cockpitWindowActiv && refeshPopup)
                                   {
                                       if (--cockpitRefreshLoopCounter < 1)
                                       {
                                           bool refresh = false;

                                           for (int i = 1; i < dtWindows.Rows.Count; i++)
                                           {
                                               try
                                               {
                                                   refresh = Convert.ToBoolean(dtWindows.Rows[i][8]);
                                               }
                                               catch
                                               {
                                                   refresh = true;
                                                   dtWindows.Rows[i][8] = true;
                                               }

                                               if (cockpitWindows[i].IsVisible && refresh)
                                               {
                                                   cockpitWindows[i].Activate();
                                               }
                                           }
                                           try
                                           {
                                               if (!editmode) ProzessHelper.SetFocusToExternalApp(processNameDCS);
                                           }
                                           catch { }
                                           cockpitRefreshLoopCounter = cockpitRefreshLoopCounterMax;
                                           refeshPopup = false;
                                       }
                                   }
                               }
                               catch (Exception ex)
                               {
                                   ImportExport.LogMessage("Refresh Panels: " + ex.ToString());
                               }

                               lStateEnabled = true;
                           }

                           #endregion
                       }));
        }

        #endregion

        #region member functions

        private void GenerateJSONDataset()
        {
            try
            {
                dtjsonLamps = new DataTable("Lamp");
                dtjsonLamps.Columns.Add("Name");
                dtjsonLamps.Columns.Add("Type");
                dtjsonLamps.Columns.Add("ExportID");

                for (int i = 0; i < dtLamps.Rows.Count; i++)
                {
                    dataRow = dtjsonLamps.NewRow();
                    dataRow["Name"] = dtLamps.Rows[i]["Name"].ToString();
                    dataRow["Type"] = "ID";
                    dataRow["ExportID"] = dtLamps.Rows[i]["Arg_number"].ToString();
                    dtjsonLamps.Rows.Add(dataRow);
                }
                dtjsonLamps.AcceptChanges();

                dtjsonSwitches = new DataTable("Switch");
                dtjsonSwitches.Columns.Add("Name");
                dtjsonSwitches.Columns.Add("Type");
                dtjsonSwitches.Columns.Add("ExportID");

                for (int i = 0; i < dtSwitches.Rows.Count; i++)
                {
                    dataRow = dtjsonSwitches.NewRow();
                    dataRow["Name"] = dtSwitches.Rows[i]["Name"].ToString();
                    dataRow["Type"] = "ID";
                    dataRow["ExportID"] = dtSwitches.Rows[i]["DcsID"].ToString();
                    dtjsonSwitches.Rows.Add(dataRow);
                }
                dtjsonSwitches.AcceptChanges();

                dtjsonInstFunctions = new DataTable("Gauge");
                dtjsonInstFunctions.Columns.Add("Name");
                dtjsonInstFunctions.Columns.Add("Type");
                dtjsonInstFunctions.Columns.Add("DeviceID");
                dtjsonInstFunctions.Columns.Add("Format");
                dtjsonInstFunctions.Columns.Add("ExportID");
                dtjsonInstFunctions.Columns.Add("negateValue");

                dataRows = dtInstrumentFunctions.Select("Name Like '*'", "IDInst ASC");

                for (int i = 0; i < dataRows.Length; i++)
                {
                    DataRow[] gauges = dtInstruments.Select("IDInst ='" + dataRows[i]["IDInst"].ToString() + "'");
                    string name = gauges[0]["Name"].ToString();

                    dataRow = dtjsonInstFunctions.NewRow();
                    dataRow["Name"] = name + " - " + dataRows[i]["Name"].ToString();
                    dataRow["Type"] = dataRows[i]["Type"].ToString();
                    dataRow["DeviceID"] = dataRows[i]["DeviceID"].ToString();
                    dataRow["Format"] = dataRows[i]["Format"].ToString();
                    dataRow["ExportID"] = dataRows[i]["Arg_number"].ToString();
                    dataRow["negateValue"] = dataRows[i]["negateValue"].ToString();
                    dtjsonInstFunctions.Rows.Add(dataRow);
                }
                dtjsonInstFunctions.AcceptChanges();

                dsJSON = new DataSet();
                dsJSON.Tables.Add(dtjsonLamps);
                dsJSON.Tables.Add(dtjsonSwitches);
                dsJSON.Tables.Add(dtjsonInstFunctions);

                GenerateJSONfromDatatable(ref dsJSON);

            }
            catch (Exception ex)
            {
                ImportExport.LogMessage("GenerateJSONDataset: " + ex.ToString());
            }
        }

        private string GenerateJSONfromDatatable(ref DataSet dsJSON)
        {
            json = JsonConvert.SerializeObject(dsJSON, Formatting.Indented);

            return json;
        }

        private void CockpitClose()
        {
            try
            {
                cockpitWindowActiv = false;
                buttonShow.Content = "Show Cockpit";
                lastFile = "";

                for (int n = 0; n < dtWindows.Rows.Count; n++)
                {
                    if (cockpitWindows[n] != null) cockpitWindows[n].Close_Cockpit();
                }
                ImportExport.LogMessage("Cockpit closed .. ");
                cockpitWindows.Clear();
                UpdateLog();
                MemoryManagement.Reduce();
            }
            catch (Exception ex) { ImportExport.LogMessage("Cockpit closed: " + ex.ToString()); }
        }

        private void CockpitShow()
        {
            try
            {
                DatabaseResetValue();
                Mouse.OverrideCursor = Cursors.Wait;

                try
                {
                    for (int i = 0; i < cockpitWindows.Count; i++)
                    {
                        if (cockpitWindows[i] != null) cockpitWindows[i].Close_Cockpit();
                    }
                }
                catch (Exception ex) { ImportExport.LogMessage("Close all old panels and load a new configuration ... " + ex.ToString()); }

                cockpitWindows.Clear();

                for (int i = 0; i < dtWindows.Rows.Count; i++)
                {
                    try
                    {
                        cockpitWindows.Add(new Cockpit(Convert.ToInt16(dtWindows.Rows[i]["WindowID"]), dtWindows.Rows[i]["Background"].ToString()));

                        cockpitWindows[i].Left = double.Parse(dtWindows.Rows[i]["PosX"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        cockpitWindows[i].Top = double.Parse(dtWindows.Rows[i]["PosY"].ToString().Replace(", ", "."), CultureInfo.InvariantCulture);
                        cockpitWindows[i].Height = double.Parse(dtWindows.Rows[i]["Height"].ToString().Replace(", ", "."), CultureInfo.InvariantCulture);
                        cockpitWindows[i].Width = double.Parse(dtWindows.Rows[i]["Width"].ToString().Replace(", ", "."), CultureInfo.InvariantCulture);

                        if (i == 0)
                            cockpitWindows[i].Show();

                        if (lightsChecked) cockpitWindows[i].UpdateInstrumentLights(lightsChecked, dtWindows.Rows[i]["BackgroundNight"].ToString());
                    }
                    catch (Exception ex) { ImportExport.LogMessage("Construct Panels ... " + ex.ToString()); }
                }

                HidePanels();

                DatagridFunction.ItemsSource = dtInstrumentFunctions.DefaultView;

                cockpitWindowActiv = true;
                buttonShow.Content = "Close Cockpit";
                ImportExport.LogMessage("Cockpit opened .. ");

                Mouse.OverrideCursor = null;
                UDP.receivedDataStack.Clear();
                MemoryManagement.Reduce();
            }
            catch (Exception ex) { ImportExport.LogMessage("Cockpit opened: " + ex.ToString()); }
        }

        private void CockpitLoad(ref string receivedData)
        {
            try
            {
                if (GrabFile(searchStringForFile, ref receivedData))
                {
                    ImportExport.LogMessage("DCS start command for modul: " + readFile);

                    if (readFile.Length > 0 && readFile != lastFile)
                    {
                        if (System.IO.File.Exists(currentDirectory + "\\" + readFile + ".ikarus"))
                        {
                            dbFilename = readFile + ".ikarus";
                            LoadConfiguration(readFile);
                        }
                        else
                        {
                            ImportExport.LogMessage("File not found: " + readFile + ".ikarus ... ", true);
                        }
                    }
                }
            }
            catch (Exception ex) { ImportExport.LogMessage("Load Cockpit: " + ex.ToString()); }
        }

        private void CopyListBoxToClipboard()
        {
            StringBuilder buffer = new StringBuilder();

            try
            {
                for (int i = 0; i < ImportExport.logItems.Count; i++)
                {
                    try
                    {
                        buffer.Append((ImportExport.logItems[i] + newline).ToString());
                    }
                    catch
                    {
                        Clipboard.SetText(buffer.ToString());
                    }
                }
                Clipboard.SetText(buffer.ToString());
            }
            catch (Exception ex) { ImportExport.LogMessage("Copy to clipboard: " + ex.ToString()); }
        }

        private static void DatabaseResetValue()
        {
            ImportExport.LogMessage("Reset Cockpit .. ");

            for (int i = 0; i < dtInstrumentFunctions.Rows.Count; i++)
            {
                try
                {
                    dtInstrumentFunctions.Rows[i]["AsciiValue"] = "";
                    dtInstrumentFunctions.Rows[i]["OldAsciiValue"] = "-";
                    dtInstrumentFunctions.Rows[i]["Value"] = 0.0;
                    dtInstrumentFunctions.Rows[i]["OldValue"] = 1.0;
                    dtInstrumentFunctions.Rows[i]["In"] = "";
                    dtInstrumentFunctions.Rows[i]["Out"] = "";

                    if (DBNull.Value.Equals(dtInstrumentFunctions.Rows[i]["Type"]))
                    {
                        dtInstrumentFunctions.Rows[i]["Type"] = "ID";
                        dtInstrumentFunctions.Rows[i]["DeviceID"] = "-";
                        dtInstrumentFunctions.Rows[i]["Format"] = "-";
                        dtInstrumentFunctions.Rows[i]["negateValue"] = "0";
                    }
                }
                catch (Exception e) { ImportExport.LogMessage("Reset Database Values dtInstrumentFunctions .. " + e.ToString()); }
            }

            for (int i = 0; i < dtLamps.Rows.Count; i++)
            {
                try
                {
                    dtLamps.Rows[i]["Value"] = 0.0;
                    dtLamps.Rows[i]["OldValue"] = 1.0;
                }
                catch (Exception e) { ImportExport.LogMessage("Reset Database Values dtLamps .. " + e.ToString()); }
            }

            for (int i = 0; i < dtSwitches.Rows.Count; i++)
            {
                try
                {
                    dtSwitches.Rows[i]["Value"] = 0.0;
                    dtSwitches.Rows[i]["OldValue"] = 1.0;
                }
                catch (Exception e) { ImportExport.LogMessage("Reset Database Values dtSwitches .. " + e.ToString()); }
            }
        }

        private void FillClasses()
        {
            try
            {
                instruments.Clear();
                lamps.Clear();
                switches.Clear();
                //maxWindowsID = dtWindows.Rows.Count;

                for (int i = 0; i < dtInstruments.Rows.Count; i++)
                {
                    instruments.Add(new Instrument(Convert.ToInt32(dtInstruments.Rows[i]["IDInst"]), dtInstruments.Rows[i]["Class"].ToString(), Convert.ToInt32(dtInstruments.Rows[i]["WindowID"])));

                    dataRowsInstrumentsFunction = dtInstrumentFunctions.Select("IDInst=" + dtInstruments.Rows[i]["IDInst"].ToString());

                    for (int n = 0; n < dataRowsInstrumentsFunction.Length; n++)
                    {
                        instruments[i].instrumentFunction.Add(new InstrumentFunction(Convert.ToInt32(dataRowsInstrumentsFunction[n]["Arg_number"]), Convert.ToBoolean(dataRowsInstrumentsFunction[n]["Ascii"])));
                    }
                }

                for (int i = 0; i < dtLamps.Rows.Count; i++)
                {
                    lamps.Add(new Lamps(Convert.ToInt32(dtLamps.Rows[i]["ID"]), Convert.ToInt32(dtLamps.Rows[i]["Arg_number"]), Convert.ToInt32(dtLamps.Rows[i]["WindowID"])));
                }

                for (int i = 0; i < dtSwitches.Rows.Count; i++)
                {
                    try
                    {
                        switches.Add(new Switches(Convert.ToInt32(dtSwitches.Rows[i]["ID"]), Convert.ToInt32(dtSwitches.Rows[i]["WindowID"]), Convert.ToInt32(dtSwitches.Rows[i]["ClickabledataID"]), dtSwitches.Rows[i]["Class"].ToString()));

                        if (dtMasterSwitches != null)
                        {
                            dataRowsMasterSwitches = dtMasterSwitches.Select("ID='" + switches[i].clickabledataID.ToString() + "'");

                            if (dataRowsMasterSwitches.Length > 0)
                            {
                                if (dataRowsMasterSwitches[0]["DcsID"].ToString() != "")
                                    switches[i].dcsID = Convert.ToInt32(dataRowsMasterSwitches[0]["DcsID"]);

                                if (dataRowsMasterSwitches[0]["DeviceID"].ToString() != "")
                                    switches[i].deviceID = Convert.ToInt32(dataRowsMasterSwitches[0]["DeviceID"]);

                                if (dataRowsMasterSwitches[0]["ButtonID"].ToString() != "")
                                    switches[i].buttonID = Convert.ToInt32(dataRowsMasterSwitches[0]["ButtonID"]);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        ImportExport.LogMessage("Switch " + i + " .. FillClasses problem .. " + e.ToString());
                    }
                }
            }
            catch (Exception e) { ImportExport.LogMessage("FillClasses problem .. " + e.ToString()); }

            //GenerateJSONfromDatatable(dtSwitches);
            GenerateJSONDataset();
        }

        private void GrabMap(ref string gotData)
        {
            string[] receivedItems = gotData.Split(':');

            try
            {
                for (int n = 0; n < receivedItems.Length; n++)
                {
                    if (receivedItems[n].IndexOf("Map=") != -1)
                    {
                        map = receivedItems[n].Substring(receivedItems[n].IndexOf("=", 0) + 1);
                        ImportExport.LogMessage("Got data for map used: " + map);
                        break;
                    }
                }
            }
            catch (Exception e) { ImportExport.LogMessage("GrabMap problem .. " + e.ToString()); }
        }

        private bool GrabFile(string ID, ref string gotData)
        {
            string[] receivedItems = gotData.Split(':');

            try
            {
                for (int n = 0; n < receivedItems.Length; n++)
                {
                    if (receivedItems[n].IndexOf(ID, 0) == 0)
                    {
                        readFile = receivedItems[n].Substring(receivedItems[n].IndexOf("=", 0) + 1);
                        return true;
                    }
                }
            }
            catch (Exception e) { ImportExport.LogMessage("GrabFile problem .. " + e.ToString()); }

            return false;
        }

        private string GrabValue()
        {
            try
            {
                idFound = false;

                if (receivedData.IndexOf(identifier, 0) > -1) // Dirty quickcheck before loop
                {
                    idFound = true;

                    for (int n = 0; n < receivedItems.Length; n++)
                    {
                        if (receivedItems[n].IndexOf(identifier, 0) == 0)
                        {
                            return receivedItems[n].Substring(receivedItems[n].IndexOf("=", 0) + 1);
                        }
                    }
                }
            }
            catch (Exception e) { ImportExport.LogMessage("GrabValue problem .. " + e.ToString()); }

            return "";
        }

        public void GrabValues()
        {
            try
            {
                #region Exportscript

                if (receivedData == null) return;
                if (receivedData == "") return;

                if (receivedData.IndexOf("Ikarus=stop") != -1)
                {
                    dbFilename = readFile + ".ikarus";
                    LoadConfiguration(readFile);
                    MemoryManagement.Reduce();
                }

                if (receivedData.IndexOf("Map=") != -1)
                {
                    GrabMap(ref receivedData);
                }

                if (receivedData.IndexOf(searchStringForFile) != -1)
                {
                    CockpitLoad(ref receivedData);
                }

                if (receivedData.IndexOf("2222=1.0") != -1)
                {
                    Lights_IsChecked(true);
                }

                if (receivedData.IndexOf("2222=0.0") != -1)
                {
                    Lights_IsChecked(false);
                }

                #endregion

                if (!cockpitWindowActiv) { return; }

                if (receivedData.Length < 3) { return; }

                newGrabValue = "";
                receivedItems = receivedData.Split(':');

                #region Gauges

                for (int i = 0; i < instruments.Count; i++)
                {
                    try
                    {
                        refreshInstruments = false;

                        for (int n = 0; n < instruments[i].instrumentFunction.Count; n++)
                        {
                            try
                            {
                                if (instruments[i].instrumentFunction[n].argNumber > 0)
                                {
                                    identifier = instruments[i].instrumentFunction[n].argNumber.ToString() + "=";
                                    newGrabValue = GrabValue();
                                }
                                else
                                {
                                    idFound = false;
                                    newGrabValue = "";
                                }

                                if (instruments[i].instrumentFunction[n].ascii)
                                {
                                    if (idFound)
                                    {
                                        if (newGrabValue != instruments[i].instrumentFunction[n].oldAsciiValue)
                                        {
                                            instruments[i].instrumentFunction[n].asciiValue = newGrabValue;
                                            instruments[i].instrumentFunction[n].oldAsciiValue = newGrabValue;

                                            refreshInstruments = true;
                                        }
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        if (newGrabValue != "")
                                        {
                                            try
                                            {
                                                instruments[i].instrumentFunction[n].value = double.Parse(newGrabValue, CultureInfo.InvariantCulture);

                                                if (instruments[i].instrumentFunction[n].value != instruments[i].instrumentFunction[n].oldValue || initInstruments)
                                                {
                                                    refreshInstruments = true;
                                                    instruments[i].instrumentFunction[n].oldValue = instruments[i].instrumentFunction[n].value;
                                                }
                                            }
                                            catch
                                            {
                                                refreshInstruments = true;
                                                instruments[i].instrumentFunction[n].ascii = true;
                                                instruments[i].instrumentFunction[n].asciiValue = newGrabValue;
                                                instruments[i].instrumentFunction[n].oldAsciiValue = newGrabValue;
                                            }
                                        }
                                    }
                                    catch { }
                                }
                            }
                            catch { }
                        }

                        try
                        {
                            panel = instruments[i].windowID - 1;

                            if (refreshInstruments && cockpitWindows[panel] != null)
                            {
                                cockpitWindows[panel].UpdateInstruments(instruments[i].instID, false);
                            }
                        }
                        catch { }
                    }
                    catch (Exception e)
                    {
                        ImportExport.LogMessage("GrabValues: Update instruments ID " + instruments[i].classname + ": " + e.ToString());
                    }
                }
                initInstruments = false;

                #endregion

                #region Lamps

                for (int n = 0; n < lamps.Count; n++)
                {
                    try
                    {
                        if (lamps[n].argNumber > 0)
                        {
                            identifier = lamps[n].argNumber.ToString() + "=";
                            newGrabValue = GrabValue();
                        }

                        if (newGrabValue != "")
                        {
                            lamps[n].value = double.Parse(newGrabValue, CultureInfo.InvariantCulture);

                            if (lamps[n].value != lamps[n].oldValue)
                            {
                                lamps[n].oldValue = lamps[n].value;

                                if (detailLog || switchLog) { ImportExport.LogMessage("Got data for DCS export ID: " + lamps[n].argNumber.ToString() + " value " + newGrabValue); }

                                try
                                {
                                    panel = lamps[n].windowID - 1;

                                    if (cockpitWindowActiv && cockpitWindows[panel] != null)
                                        cockpitWindows[panel].UpdateLamps(lamps[n].ID);
                                }
                                catch { }
                            }
                        }
                    }
                    catch (Exception e) { ImportExport.LogMessage("GrabValues: Lamp " + (n + 1).ToString() + " problem .. " + e.ToString()); }
                }
                #endregion

                #region Switches

                for (int n = 0; n < switches.Count; n++)
                {
                    try
                    {
                        if (switches[n].dcsID > 0)
                        {
                            identifier = switches[n].dcsID.ToString() + "=";
                            newGrabValue = GrabValue();
                        }

                        if (newGrabValue != "")
                        {
                            if (switches[n].ignoreNextPackage || switches[n].ignoreAllPackage)
                            {
                                if (detailLog) { ImportExport.LogMessage("Ignore data for switch ID: " + switches[n].dcsID.ToString() + " value: " + newGrabValue); }

                                switches[n].ignoreNextPackage = false;
                            }
                            else
                            {
                                switches[n].value = double.Parse(newGrabValue, CultureInfo.InvariantCulture);

                                if (detailLog) { ImportExport.LogMessage("Got data for switch ID: " + switches[n].dcsID.ToString() + " value " + newGrabValue); }

                                if (switches[n].value != switches[n].oldValue)
                                {
                                    try
                                    {
                                        panel = switches[n].windowID - 1;

                                        if (cockpitWindowActiv && cockpitWindows[panel] != null)
                                            cockpitWindows[panel].UpdateSwitches(switches[n].ID);
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch (Exception f)
                    {
                        ImportExport.LogMessage("Error with switch ID: " + switches[n].dcsID.ToString() + " value " + newGrabValue + "..." + f.ToString());
                    }
                }
                #endregion
            }
            catch (Exception e) { ImportExport.LogMessage("receivedData: " + receivedData + " GrabValues problem .. " + e.ToString()); }
        }

        private void HidePanels()
        {
            int visible = 0;
            int panelID = 1;

            dataRows = dtSwitches.Select("Class='SwitchPanelOffOn' OR Class='ButtonPanelOffOn'");

            for (int i = 0; i < dataRows.Length; i++)
            {
                try
                {
                    panelID = Convert.ToInt32(dataRows[i]["Input"]);
                }
                catch (Exception ex) { ImportExport.LogMessage("Hide Panel: " + panelID + " ... " + ex.ToString()); return; }

                try
                {
                    visible = Convert.ToInt32(dataRows[i]["Output"]);
                }
                catch { visible = 0; }

                try
                {
                    cockpitWindows[panelID - 1].Visibility = (visible == 0) ? Visibility.Hidden : Visibility.Visible;
                }
                catch { return; }
            }
        }

        private void LoadConfiguration(string filename)
        {
            try
            {
                lastFile = filename;
                readFile = filename;

                try
                {
                    if (cockpitWindowActiv)
                    {
                        try
                        {
                            for (int i = 0; i < cockpitWindows.Count; i++)
                            {
                                if (cockpitWindows[i] != null) cockpitWindows[i].Close_Cockpit();
                            }
                            cockpitWindows.Clear();
                        }
                        catch (Exception ex) { ImportExport.LogMessage("Close all old panels and load a new configuration ... " + ex.ToString()); }
                    }

                    ResetTables();
                    ImportExport.XmlToDataSet(filename + ".ikarus", dsInstruments);

                    dsMaster = new Masterdata();
                    ImportExport.XmlToDataSet(filename + ".xml", dsMaster); // Load db for Switches and Lamps

                    if (dsMaster.Tables.Count > 0)
                    {
                        dtMasterLamps = dsMaster.Tables[0];
                        if (dtMasterLamps.Rows.Count == 0) ImportExport.LogMessage("++++++ None definitions for lamps found.");
                    }
                    else
                    {
                        ImportExport.LogMessage("++++++ None definitions for lamps found.");
                    }
                    if (dsMaster.Tables.Count > 1)
                    {
                        dtMasterSwitches = dsMaster.Tables[1];
                        if (dtMasterSwitches.Rows.Count == 0) ImportExport.LogMessage("++++++ None definitions for switches found.");
                    }
                    else
                    {
                        ImportExport.LogMessage("++++++ None definitions for switches found.");
                    }

                    for (int n = 0; n < dtSwitches.Rows.Count; n++) // update switches
                    {
                        dtSwitches.Rows[n]["DontReset"] = false;
                        dtSwitches.Rows[n]["Event"] = false;
                    }

                    SetWindowID(dtInstruments);
                    SetWindowID(dtLamps);
                    SetWindowID(dtSwitches);
                    SetWindowID(dtAccessories);

                    textBoxLightColor.Text = dtWindows.Rows[0][9].ToString();
                    lightOnColor = textBoxLightColor.Text;

                    FillClasses();
                }
                catch (Exception e) { ImportExport.LogMessage("XmlToDataSet problem .. " + e.ToString()); }

                try
                {
                    background = dtParameter.Rows[0][0].ToString();
                }
                catch { }

                Main.Title = "Ikarus - ( Configured for " + filename + " )";

                if (dtWindows.Rows.Count == 0)
                {
                    dtWindows.Rows.Add(1, "Front Panel", dtConfig.Rows[0][0].ToString(), dtConfig.Rows[0][1].ToString(), dtConfig.Rows[0][2].ToString(), dtConfig.Rows[0][3].ToString(), background);
                }

                RefreshDatagrids();
                CockpitShow();
                UpdateLog();

                if (activateR)
                {
                    package = "R";
                    UDP.UDPSender(IPAddess.Text.Trim(), Convert.ToInt32(PortSender.Text), package); // send a package to DCS
                }
                timerstate = State.run;
            }
            catch (Exception e) { ImportExport.LogMessage("Load configuration problem .. " + e.ToString()); }
        }

        private void RefreshDatagrids()
        {
            for (int i = 0; i < dtInstruments.Rows.Count; i++)
            {
                dtInstruments.Rows[i]["Reactivate"] = false;
            }

            for (int i = 0; i < dtSwitches.Rows.Count; i++)
            {
                dtSwitches.Rows[i]["Event"] = false;
            }
            try
            {
                DataGridInstruments.ItemsSource = dtInstruments.DefaultView; // DefaultView is important
                DataGridInstruments.CanUserAddRows = false;
            }
            catch { }
            try
            {
                DatagridFunction.ItemsSource = dtInstrumentFunctions.DefaultView;
                DatagridFunction.CanUserAddRows = false;
            }
            catch { }
            try
            {
                DataGridSwitches.ItemsSource = dtSwitches.DefaultView;
                DataGridSwitches.CanUserAddRows = false;
            }
            catch { }
            try
            {
                DataGridLamps.ItemsSource = dtLamps.DefaultView;
                DataGridLamps.CanUserAddRows = false;
            }
            catch { }
            try
            {
                DataGridAccessories.ItemsSource = dtAccessories.DefaultView;
                DataGridAccessories.CanUserAddRows = false;
            }
            catch { }
            try
            {
                DataGridWindows.ItemsSource = dtWindows.DefaultView;
                DataGridWindows.CanUserAddRows = false;
            }
            catch { }
        }

        private void RefreshAllSwitches()
        {
            try
            {
                if (activateR)
                {
                    for (int i = 0; i < switches.Count; i++)
                    {
                        try
                        {
                            //switches[i].oldValue = 0.0;
                            switches[i].ignoreNextPackage = false;
                            switches[i].doit = false;
                        }
                        catch { }
                    }
                    if (cockpitWindowActiv)
                    {
                        package = "R";
                        UDP.UDPSender(IPAddess.Text.Trim(), Convert.ToInt32(PortSender.Text), package);
                    }
                    //if (detailLog || switchLog) { ImportExport.LogMessage("Send package to " + IPAddess.Text.Trim() + ":" + PortSender.Text + " - Package: " + package); }
                }
                getAllDscData = false;
            }
            catch (Exception ex) { ImportExport.LogMessage("RefreshAllSwitches: " + ex.ToString()); }
        }

        private void ResetTables()
        {
            dsInstruments.Clear();
            dtInstruments.Columns["IDInst"].AutoIncrementStep = -1;
            dtInstruments.Columns["IDInst"].AutoIncrementSeed = -1;
            dtInstruments.Columns["IDInst"].AutoIncrementStep = 1;
            dtInstruments.Columns["IDInst"].AutoIncrementSeed = 1;

            //dtInstrumentFunctions.Columns["ID"].AutoIncrementStep = -1;
            //dtInstrumentFunctions.Columns["ID"].AutoIncrementSeed = -1;
            //dtInstrumentFunctions.Columns["ID"].AutoIncrementStep = 1;
            //dtInstrumentFunctions.Columns["ID"].AutoIncrementSeed = 1;

            dtLamps.Clear();
            dtLamps.Columns["ID"].AutoIncrementStep = -1;
            dtLamps.Columns["ID"].AutoIncrementSeed = -1;
            dtLamps.Columns["ID"].AutoIncrementStep = 1;
            dtLamps.Columns["ID"].AutoIncrementSeed = 1;

            dtSwitches.Clear();
            dtSwitches.Columns["ID"].AutoIncrementStep = -1;
            dtSwitches.Columns["ID"].AutoIncrementSeed = -1;
            dtSwitches.Columns["ID"].AutoIncrementStep = 1;
            dtSwitches.Columns["ID"].AutoIncrementSeed = 1;

            dtAccessories.Clear();
            dtAccessories.Columns["ID"].AutoIncrementStep = -1;
            dtAccessories.Columns["ID"].AutoIncrementSeed = -1;
            dtAccessories.Columns["ID"].AutoIncrementStep = 1;
            dtAccessories.Columns["ID"].AutoIncrementSeed = 1;

            dtWindows.Clear();
            dtWindows.Columns["WindowID"].AutoIncrementStep = -1;
            dtWindows.Columns["WindowID"].AutoIncrementSeed = -1;
            dtWindows.Columns["WindowID"].AutoIncrementStep = 1;
            dtWindows.Columns["WindowID"].AutoIncrementSeed = 1;
        }

        private void ScreenCapture(int locationX, int locationY, int width, int height)
        {
            try
            {
                using (Bitmap bitmap = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(new System.Drawing.Point(locationX, locationY), new System.Drawing.Point(0, 0), bitmap.Size);
                    }

                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog()
                    {
                        FileName = "Log",
                        DefaultExt = ".png",
                        Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*"
                    };
                    var result = dlg.ShowDialog();
                    if (result == false) { return; }

                    bitmap.Save(dlg.FileName, ImageFormat.Png);
                }
            }
            catch (Exception ex) { ImportExport.LogMessage("ScreenCapture: " + ex.ToString()); }
        }

        private void StartListener()
        {
            try
            {
                UDP.StartListener(Convert.ToInt16(portListener.Trim()));
            }
            catch (Exception e) { ImportExport.LogMessage("StartListener problem .. " + e.ToString()); }
        }

        public void SelectDataGridItem(string _tablename, int ID)
        {
            if (_tablename == "Accessories")
            {
                tabControl1.SelectedIndex = functionTabIsVisible ? 4 : 3;
                selectedLamp = -1;
                selectedSwitch = -1;
                selectedInstrument = -1;

                for (int i = 0; i < dtAccessories.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dtAccessories.Rows[i]["ID"]) == ID)
                    {
                        selectedAccessories = i;
                        DataGridAccessories.SelectedItem = DataGridAccessories.Items[i];
                        DataGridAccessories.ScrollIntoView(DataGridAccessories.Items[i]);
                        break;
                    }
                }
                return;
            }
            if (_tablename == "Instruments")
            {
                tabControl1.SelectedIndex = 0;
                selectedLamp = -1;
                selectedSwitch = -1;
                selectedAccessories = -1;

                for (int i = 0; i < dtInstruments.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dtInstruments.Rows[i]["IDInst"]) == ID)
                    {
                        DataGridInstruments.SelectedItem = DataGridInstruments.Items[i];
                        DataGridInstruments.ScrollIntoView(DataGridInstruments.Items[i]);
                        break;
                    }
                }
                return;
            }
            if (_tablename == "Lamps")
            {
                tabControl1.SelectedIndex = functionTabIsVisible ? 3 : 2;
                selectedInstrument = -1;
                selectedSwitch = -1;
                selectedAccessories = -1;

                for (int i = 0; i < dtLamps.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dtLamps.Rows[i]["ID"]) == ID)
                    {
                        DataGridLamps.SelectedItem = DataGridLamps.Items[i];
                        DataGridLamps.ScrollIntoView(DataGridLamps.Items[i]);
                        break;
                    }
                }
                return;
            }

            if (_tablename == "Switches")
            {
                tabControl1.SelectedIndex = functionTabIsVisible ? 2 : 1;
                selectedLamp = -1;
                selectedInstrument = -1;
                selectedAccessories = -1;

                for (int i = 0; i < dtSwitches.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dtSwitches.Rows[i]["ID"]) == ID)
                    {
                        DataGridSwitches.SelectedItem = DataGridSwitches.Items[i];
                        DataGridSwitches.ScrollIntoView(DataGridSwitches.Items[i]);
                        break;
                    }
                }
            }
            if (_tablename == "Log")
            {
                ListBox1.SelectedIndex = logCount - 1;
                ListBox1.ScrollIntoView(logCount - 1);
            }
        }

        private void SendDataFromButtonEvent()
        {
            for (int n = 0; n < switches.Count; n++)
            {
                try
                {
                    if (switches[n].value != switches[n].oldValue || switches[n].doit)
                    {
                        if (!switches[n].dontReset)
                            switches[n].oldValue = switches[n].value; //<---

                        if (switches[n].events)
                        {
                            if (windowID == 0) refeshPopup = true;

                            if (!switches[n].dontReset) switches[n].events = false; //<---

                            switches[n].ignoreNextPackage = true; // from DCS
                            getAllDscData = true;
                            getAllDscDataLoopCounter = dscDataLoopCounterMax; // next refresh for switches in 3 sec.

                            package = "C" + switches[n].deviceID.ToString() + "," + (3000 + switches[n].buttonID).ToString() + "," + switches[n].value.ToString();

                            UDP.UDPSender(IPAddess.Text.Trim(), Convert.ToInt32(PortSender.Text), package, switches[n].dcsID.ToString()); //<--- send a package to DCS

                            if (switches[n].sendRelease) // && switches[n].value > 0.0)
                            {
                                package = "C" + switches[n].deviceID.ToString() + "," + (3000 + switches[n].buttonID).ToString() + "," + (0.0).ToString();
                                UDP.UDPSender(IPAddess.Text.Trim(), Convert.ToInt32(PortSender.Text), package, switches[n].dcsID.ToString());
                            }
                        }
                    }
                }
                catch (Exception f)
                {
                    ImportExport.LogMessage("Error with switch ID: " + switches[n].dcsID.ToString() + " value " + newGrabValue + "..." + f.ToString());
                }
            }
        }

        private void SetWindowID(DataTable table)
        {
            try
            {
                if (table.Rows.Count > 0)
                {
                    if (table.Rows[0]["WindowID"] == null || table.Rows[0]["WindowID"] == DBNull.Value)
                    {
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            table.Rows[i]["WindowID"] = 1;
                        }
                        ImportExport.LogMessage(table.TableName + ": Set WindowID = 1");
                    }
                }
            }
            catch (Exception e) { ImportExport.LogMessage("Set WindowID .. " + e.ToString()); }
        }

        private void ShowDetailInstruments()
        {
            if (DataGridInstruments.SelectedItem == null) return;

            if (DataGridInstruments.SelectedIndex == -1) return;

            try
            {
                rowView = (DataRowView)DataGridInstruments.SelectedItem;
                selectedInstrument = Convert.ToInt32(rowView.Row.ItemArray[0].ToString());

                if (rowView.Row.ItemArray[2].ToString() != "-" && rowView.Row.ItemArray[2].ToString() != "")
                {
                    lastSelectedInstrumentsClass = rowView.Row.ItemArray[2].ToString();
                }
                MessageBoxInstruments mbi = new MessageBoxInstruments(selectedInstrument, lastSelectedInstrumentsClass);
            }
            catch (Exception e) { ImportExport.LogMessage("Detail instrument: " + e.ToString()); }
        }

        private void ShowDetailLamps()
        {
            if (DataGridLamps.SelectedItem == null) return;

            if (DataGridLamps.SelectedIndex == -1) return;

            try
            {
                rowView = (DataRowView)DataGridLamps.SelectedItem;
                selectedIndexLamps = Convert.ToInt32(rowView.Row.ItemArray[0].ToString());

                MessageBoxLamps mbl = new MessageBoxLamps(selectedIndexLamps);
            }
            catch (Exception e) { ImportExport.LogMessage("Detail lamp: " + e.ToString()); }
        }

        private void ShowDetailSwitch()
        {
            if (DataGridSwitches.SelectedItem == null) return;

            if (DataGridSwitches.SelectedIndex == -1) return;

            try
            {
                rowView = (DataRowView)DataGridSwitches.SelectedItem;
                selectedIndexSwitches = Convert.ToInt32(rowView.Row.ItemArray[0].ToString());

                MessageBoxSwitch mbs = new MessageBoxSwitch(selectedIndexSwitches);
            }
            catch (Exception ex) { ImportExport.LogMessage("Detail switch: " + ex.ToString()); }
        }

        private void SliderTest()
        {
            double valueNew = 0.0;
            double valueOld = 0.0;
            string asciiValue = "";
            string oldAsciiValue = "";

            for (int i = 0; i < dtInstruments.Rows.Count; i++)
            {
                windowID = Convert.ToInt16(dtInstruments.Rows[i]["WindowID"]) - 1;

                dataRowsInstrumentsFunction = dtInstrumentFunctions.Select("IDInst=" + dtInstruments.Rows[i]["IDInst"].ToString());
                bool refreshInstruments = false;

                for (int n = 0; n < dataRowsInstrumentsFunction.Length; n++)
                {
                    try
                    {
                        valueNew = double.Parse(dataRowsInstrumentsFunction[n]["Value"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        valueOld = double.Parse(dataRowsInstrumentsFunction[n]["OldValue"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                        asciiValue = dataRowsInstrumentsFunction[n]["AsciiValue"].ToString();
                        oldAsciiValue = dataRowsInstrumentsFunction[n]["OldAsciiValue"].ToString();
                    }
                    catch
                    {
                        valueNew = 0.0;
                    }

                    if (valueNew != valueOld || asciiValue != oldAsciiValue)
                    {
                        dataRowsInstrumentsFunction[n]["OldValue"] = valueNew;
                        dataRowsInstrumentsFunction[n]["OldAsciiValue"] = asciiValue;
                        refreshInstruments = true;
                    }
                }
                try
                {
                    if (refreshInstruments && cockpitWindows[windowID] != null)
                        cockpitWindows[windowID].UpdateInstruments(Convert.ToInt32(dtInstruments.Rows[i]["IDInst"].ToString()), true);
                }
                catch { }
            }
        }

        private void UpdateLog()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background,
                       (Action)(() =>
                       {
                           ListBox1.ItemsSource = null;
                           ListBox1.ItemsSource = ImportExport.logItems;
                           logCount = ImportExport.logItems.Count;
                       }));
        }

        #endregion

        #region events

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            if ((tabControl1.SelectedIndex == 5 && functionTabIsVisible) || (tabControl1.SelectedIndex == 4 && !functionTabIsVisible))
            {
                try
                {
                    dataRow = dtWindows.NewRow();
                    dtWindows.Rows.Add(dataRow);
                    dtWindows.AcceptChanges();
                    DataGridWindows.ItemsSource = dtWindows.DefaultView;
                    DataGridWindows.ScrollIntoView(DataGridWindows.Items[DataGridWindows.Items.Count - 1]);
                    DataGridWindows.SelectedIndex = DataGridWindows.Items.Count - 1;
                }
                catch { }
                return;
            }

            if ((tabControl1.SelectedIndex == 4 && functionTabIsVisible) || (tabControl1.SelectedIndex == 3 && !functionTabIsVisible))
            {
                try
                {
                    dataRow = dtAccessories.NewRow();
                    dtAccessories.Rows.Add(dataRow);
                    dtAccessories.AcceptChanges();
                    DataGridAccessories.ItemsSource = dtAccessories.DefaultView;
                    DataGridAccessories.ScrollIntoView(DataGridAccessories.Items[DataGridAccessories.Items.Count - 1]);
                    DataGridAccessories.SelectedIndex = DataGridAccessories.Items.Count - 1;
                }
                catch { }
                return;
            }

            if ((tabControl1.SelectedIndex == 3 && functionTabIsVisible) || (tabControl1.SelectedIndex == 2 && !functionTabIsVisible))
            {
                try
                {
                    dataRow = dtLamps.NewRow();
                    dtLamps.Rows.Add(dataRow);
                    dtLamps.AcceptChanges();
                    DataGridLamps.ItemsSource = dtLamps.DefaultView;
                    DataGridLamps.ScrollIntoView(DataGridLamps.Items[DataGridLamps.Items.Count - 1]);
                    DataGridLamps.SelectedIndex = DataGridLamps.Items.Count - 1;
                }
                catch { }
                return;
            }

            if ((tabControl1.SelectedIndex == 2 && functionTabIsVisible) || (tabControl1.SelectedIndex == 1 && !functionTabIsVisible))
            {
                try
                {
                    dataRow = dtSwitches.NewRow();
                    dtSwitches.Rows.Add(dataRow);
                    dtSwitches.AcceptChanges();
                    DataGridSwitches.ItemsSource = dtSwitches.DefaultView;
                    DataGridSwitches.ScrollIntoView(DataGridSwitches.Items[DataGridSwitches.Items.Count - 1]);
                    DataGridSwitches.SelectedIndex = DataGridSwitches.Items.Count - 1;
                }
                catch { }
                return;
            }

            if (tabControl1.SelectedIndex == 0)
            {
                try
                {
                    dataRow = dtInstruments.NewRow();
                    dtInstruments.Rows.Add(dataRow);
                    dtInstruments.AcceptChanges();
                    DataGridInstruments.ItemsSource = dtInstruments.DefaultView;
                    DataGridInstruments.ScrollIntoView(DataGridInstruments.Items[DataGridInstruments.Items.Count - 1]);
                    DataGridInstruments.SelectedIndex = DataGridInstruments.Items.Count - 1;

                    if (selectedInstrument == 0 || selectedInstrument == -1) selectedInstrument = 1;
                }
                catch { }
                return;
            }

            if (tabControl1.SelectedIndex == 1 && functionTabIsVisible)
            {
                try
                {
                    dataRow = dtInstrumentFunctions.NewRow();
                    dataRow["IDInst"] = selectedInstrument;

                    dataRows = dtInstrumentFunctions.Select("IDInst=" + selectedInstrument);
                    dataRow["IDFct"] = dataRows.Length + 1;

                    dtInstrumentFunctions.Rows.Add(dataRow);
                    dtInstrumentFunctions.AcceptChanges();
                    DatagridFunction.ItemsSource = dtInstrumentFunctions.Select("IDInst=" + selectedInstrument, "IDInst ASC, IDFct ASC");
                }
                catch { }
                return;
            }
        }

        private void Button_Copy_Click(object sender, RoutedEventArgs e)
        {
            CopyListBoxToClipboard();
        }

        private void Button_Del_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                if (selectedInstrument == -1) return;

                for (int i = 0; i < dtInstruments.Rows.Count; i++)
                {
                    dataRow = dtInstruments.Rows[i];

                    if (Convert.ToInt32(dataRow["IDInst"]) == selectedInstrument)
                    {
                        dtInstruments.Rows.Remove(dataRow);
                        dtInstruments.AcceptChanges();
                        DataGridInstruments.ItemsSource = dtInstruments.DefaultView;
                        dtInstrumentFunctions.AcceptChanges();
                        DatagridFunction.ItemsSource = dtInstrumentFunctions.Select("IDInst=" + selectedInstrument, "IDInst ASC, IDFct ASC");
                    }
                }
            }

            if (tabControl1.SelectedIndex == 1 && functionTabIsVisible)
            {
                if (selectedFunction == -1) return;
                if (selectedInstrument == -1) return;

                for (int i = 0; i < dtInstrumentFunctions.Rows.Count; i++)
                {
                    dataRow = dtInstrumentFunctions.Rows[i];

                    if (Convert.ToInt32(dataRow["IDInst"]) == selectedInstrument && Convert.ToInt32(dataRow["IDFct"]) == selectedFunction)
                    {
                        dtInstrumentFunctions.Rows.Remove(dataRow);
                        dtInstrumentFunctions.AcceptChanges();
                        DatagridFunction.ItemsSource = dtInstrumentFunctions.Select("IDInst=" + selectedInstrument, "IDInst ASC, IDFct ASC");
                    }
                }
            }

            if ((tabControl1.SelectedIndex == 2 && functionTabIsVisible) || (tabControl1.SelectedIndex == 1 && !functionTabIsVisible))
            {
                if (selectedSwitch == -1) return;

                for (int i = 0; i < dtSwitches.Rows.Count; i++)
                {
                    dataRow = dtSwitches.Rows[i];

                    if (Convert.ToInt32(dataRow["ID"]) == selectedSwitch)
                    {
                        dtSwitches.Rows.Remove(dataRow);
                        dtSwitches.AcceptChanges();
                        DataGridSwitches.ItemsSource = dtSwitches.DefaultView;
                    }
                }
            }

            if ((tabControl1.SelectedIndex == 3 && functionTabIsVisible) || (tabControl1.SelectedIndex == 2 && !functionTabIsVisible))
            {
                if (selectedLamp == -1) return;

                for (int i = 0; i < dtLamps.Rows.Count; i++)
                {
                    dataRow = dtLamps.Rows[i];

                    if (Convert.ToInt32(dataRow["ID"]) == selectedLamp)
                    {
                        dtLamps.Rows.Remove(dataRow);
                        dtLamps.AcceptChanges();
                        DataGridLamps.ItemsSource = dtLamps.DefaultView;
                    }
                }
            }

            if ((tabControl1.SelectedIndex == 4 && functionTabIsVisible) || (tabControl1.SelectedIndex == 3 && !functionTabIsVisible))
            {
                if (selectedAccessories == -1) return;

                for (int i = 0; i < dtAccessories.Rows.Count; i++)
                {
                    dataRow = dtAccessories.Rows[i];

                    if (Convert.ToInt32(dataRow["ID"]) == selectedAccessories)
                    {
                        dtAccessories.Rows.Remove(dataRow);
                        dtAccessories.AcceptChanges();
                        DataGridAccessories.ItemsSource = dtAccessories.DefaultView;
                    }
                }
            }

            if ((tabControl1.SelectedIndex == 5 && functionTabIsVisible) || (tabControl1.SelectedIndex == 4 && !functionTabIsVisible))
            {
                if (selectedWindows == -1) return;

                for (int i = 0; i < dtWindows.Rows.Count; i++)
                {
                    dataRow = dtWindows.Rows[i];

                    if (Convert.ToInt32(dataRow["WindowID"]) == selectedWindows)
                    {
                        dtWindows.Rows.Remove(dataRow);
                        dtWindows.AcceptChanges();
                        DataGridWindows.ItemsSource = dtWindows.DefaultView;
                    }
                }
            }
        }

        private void Button_DeleteDatabases_Click(object sender, RoutedEventArgs e)
        {
            CockpitClose();
            ResetTables();
            readFile = "";
            lastFile = "-";
            dbFilename = "";
            textBoxLightColor.Text = "95E295";
            lightOnColor = textBoxLightColor.Text;
        }

        private void Button_DetailAccessories_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridAccessories.SelectedItem == null) return;

            if (DataGridAccessories.SelectedIndex == -1) return;

            try
            {
                rowView = (DataRowView)DataGridAccessories.SelectedItem;
                selectedAccessories = Convert.ToInt32(rowView.Row.ItemArray[0].ToString());

                MessageBoxAccessories mbs = new MessageBoxAccessories(selectedAccessories);
            }
            catch (Exception ex) { ImportExport.LogMessage("Detail Accessory: " + ex.ToString()); }
        }

        private void Button_DetailInstruments_Click(object sender, RoutedEventArgs e)
        {
            ShowDetailInstruments();
        }

        private void Button_DetailLamp_Click(object sender, RoutedEventArgs e)
        {
            ShowDetailLamps();
        }

        private void Button_DetailSwitch_Click(object sender, RoutedEventArgs e)
        {
            ShowDetailSwitch();
        }

        private void Button_DetailWindow_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridWindows.SelectedItem == null) return;

            if (DataGridWindows.SelectedIndex == -1) return;

            try
            {
                rowView = (DataRowView)DataGridWindows.SelectedItem;
                selectedWindows = Convert.ToInt32(rowView.Row.ItemArray[0].ToString());

                MessageBoxWindow mbs = new MessageBoxWindow(selectedWindows);
            }
            catch (Exception ex) { ImportExport.LogMessage("Detail Panel: " + ex.ToString()); }
        }

        private void Button_Donate_Click(object sender, EventArgs e)
        {
            string url = "";

            string business = "heinz.joerg.puhlmann@googlemail.com";    // your paypal email
            string description = "Donation";                            // '%20' represents a space. remember HTML!
            string country = "DE";                                      // AU, US, etc.
            string currency = "EUR";                                    // AUD, USD, etc.

            url += "https://www.paypal.com/cgi-bin/webscr" +
                "?cmd=" + "_donations" +
                "&business=" + business +
                "&lc=" + country +
                "&item_name=" + description +
                "&currency_code=" + currency +
                "&bn=" + "PP%2dDonationsBF";

            System.Diagnostics.Process.Start(url);
        }

        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = ".ikarus",
                Filter = "IKARUS files (*.ikarus)|*.ikarus|All files (*.*)|*.*"
            };
            var result = dlg.ShowDialog();
            if (result == false) { return; }

            dbFilename = dlg.FileName.Substring(dlg.FileName.LastIndexOf("\\") + 1);
            dbFilename = dbFilename.Substring(0, dbFilename.LastIndexOf("."));

            ImportExport.LogMessage("User has loaded cockpit " + dbFilename);

            LoadConfiguration(dbFilename);
            UpdateLog();
        }

        private void Button_LoadBackGround_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog()
            {
                DefaultExt = ".png",
                Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*"
            };
            var result = dlg.ShowDialog();
            if (result == false) { return; }

            background = dlg.FileName.Substring(dlg.FileName.LastIndexOf("\\") + 1);
        }

        private void Button_LogRefresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateLog();
            ListBox1.ScrollIntoView(ListBox1.Items[ListBox1.Items.Count - 1]);
        }

        private void Button_Refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateLayout();

                refreshCockpit = true;

                if (refreshCockpit)
                {
                    CockpitShow();
                    refreshCockpit = false;
                }

                CollectionViewSource.GetDefaultView(DataGridInstruments.ItemsSource).Refresh();

                DataGridInstruments.Items.Refresh(); // Update Datagrid
                DatagridFunction.Items.Refresh();
                DataGridSwitches.Items.Refresh();
                DataGridLamps.Items.Refresh();
                DataGridAccessories.Items.Refresh();
            }
            catch { }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog()
                {
                    FileName = dbFilename,
                    DefaultExt = ".ikarus",
                    Filter = "IKARUS files (*.ikarus)|*.ikarus|All files (*.*)|*.*"
                };
                Nullable<bool> result = dlg.ShowDialog();

                if (result == false) { return; }

                dtConfig.Rows[0][4] = dlg.FileName.Substring(dlg.FileName.LastIndexOf("\\") + 1);

                dtConfig.Rows[0][5] = IPAddess.Text;
                dtConfig.Rows[0][6] = PortListener.Text;
                dtConfig.Rows[0][7] = PortSender.Text;
                dtConfig.Rows[0][8] = true;

                if (dtParameter.Rows.Count == 0)
                    dtParameter.Rows.Add(background);
                else
                    dtParameter.Rows[0][0] = background;

                try
                {
                    dtWindows.Rows[0][9] = textBoxLightColor.Text;
                    dtConfig.Rows[0][10] = checkBoxShadow.IsChecked;
                    dtConfig.Rows[0][11] = jsonChecked;
                }
                catch { }

                dsConfig.AcceptChanges();
                DatabaseResetValue();

                ImportExport.DatasetToXml(currentDirectory + "\\Config.xml", dsConfig);
                ImportExport.LogMessage("Saved file: " + "Config.xml");

                ImportExport.DatasetToXml(currentDirectory + "\\" + dlg.FileName.Substring(dlg.FileName.LastIndexOf("\\") + 1), dsInstruments);
                ImportExport.LogMessage("Saved file: " + dlg.FileName.Substring(dlg.FileName.LastIndexOf("\\") + 1));
            }
            catch (Exception f) { ImportExport.LogMessage("Save problem ... " + f.ToString()); }

            lastFile = "";
            UpdateLog();
        }

        private void Button_Show_Click(object sender, RoutedEventArgs e)
        {
            if (!cockpitWindowActiv)
            {
                lightOnColor = textBoxLightColor.Text;
                CockpitShow();
            }
            else
            {
                CockpitClose();
            }
            UpdateLog();
        }

        private void Button_ShowAll_Click(object sender, RoutedEventArgs e)
        {
            DatagridFunction.ItemsSource = dtInstrumentFunctions.DefaultView;
        }

        private void Checkbox_EditMode_Click(object sender, RoutedEventArgs e)
        {
            if (editmode)
            {
                ImportExport.LogMessage("Edit mode: OFF");

                timerstate = State.run;
                lbEditMode.Visibility = Visibility.Hidden;
                Refresh.Visibility = Visibility.Hidden;

                //selectedInstrument = -1;
                //selectedLamp = -1;
                //selectedSwitch = -1;

                editmode = false;
                FillClasses();
                CockpitShow();
            }
            else
            {
                ImportExport.LogMessage("Edit mode: ON");

                timerstate = State.stop;
                lbEditMode.Visibility = Visibility.Visible;
                Refresh.Visibility = Visibility.Visible;

                selectedInstrument = -1;
                selectedLamp = -1;
                selectedSwitch = -1;

                editmode = true;
                FillClasses();
                CockpitShow();
            }
            lastFile = "";
        }

        private static void Lights_IsChecked(bool lightsChecked)
        {
            try
            {
                for (int i = 0; i < dtWindows.Rows.Count; i++)
                {
                    if (cockpitWindowActiv) cockpitWindows[i].UpdateInstrumentLights(lightsChecked, dtWindows.Rows[i]["BackgroundNight"].ToString());
                }
                ImportExport.LogMessage(lightsChecked ? "Lights on" : "Lights off");
            }
            catch { }
        }

        private void CheckBox_Lights_Checked(object sender, RoutedEventArgs e)
        {
            Lights_IsChecked(true);
        }

        private void CheckBox_Lights_Unchecked(object sender, RoutedEventArgs e)
        {
            Lights_IsChecked(false);
        }

        private void CheckBox_Log_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxLog.IsChecked == true)
            {
                lbLogging.Visibility = Visibility.Visible;
                detailLog = true;
            }
            else
            {
                lbLogging.Visibility = Visibility.Hidden;
                detailLog = false;
            }
        }

        private void DataGridAccessories_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                rowView = (DataRowView)DataGridAccessories.SelectedItem;
                selectedAccessories = Convert.ToInt32(rowView.Row.ItemArray[0].ToString()); // ID
            }
            catch { }
        }

        private void DatagridFunction_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                dataRow = (DataRow)DatagridFunction.SelectedItem;
                selectedFunction = Convert.ToInt32(dataRow.ItemArray[1].ToString()); // IDFct
            }
            catch { }
        }

        private void DataGridInstruments_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                rowView = (DataRowView)DataGridInstruments.SelectedItem;
                selectedInstrument = Convert.ToInt32(rowView.Row.ItemArray[0].ToString()); // ID
            }
            catch { }
            //catch (Exception f) { ImportExport.LogMessage("DataGridInstruments_SelectedCellsChanged .. " + f.ToString()); }
        }

        private void DataGridLamps_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                rowView = (DataRowView)DataGridLamps.SelectedItem;
                selectedLamp = Convert.ToInt32(rowView.Row.ItemArray[0].ToString()); // ID
            }
            catch { }
        }

        private void DataGridSwitches_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                rowView = (DataRowView)DataGridSwitches.SelectedItem;
                selectedSwitch = Convert.ToInt32(rowView.Row.ItemArray[0].ToString()); // ID
            }
            catch { }
        }

        private void DataGridWindows_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                rowView = (DataRowView)DataGridWindows.SelectedItem;
                selectedWindows = Convert.ToInt32(rowView.Row.ItemArray[0].ToString()); // WindowID
            }
            catch { }
        }

        private void Main_Closed(object sender, EventArgs e)
        {
            CockpitClose();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedTab == tabControl1.SelectedIndex) return;

            buttonAdd.Visibility = Visibility.Visible;
            buttonDel.Visibility = Visibility.Visible;

            if (tabControl1.SelectedIndex == 0)
            {
                selectedTab = tabControl1.SelectedIndex;
            }

            if (tabControl1.SelectedIndex == 1 && functionTabIsVisible)
            {
                selectedTab = tabControl1.SelectedIndex;

                if (selectedInstrument != -1)
                {
                    if (selectedInstrument == 0) selectedInstrument = 1;
                    DatagridFunction.ItemsSource = dtInstrumentFunctions.Select("IDInst=" + selectedInstrument, "IDInst ASC, IDFct ASC");
                }
                else
                {
                    DatagridFunction.ItemsSource = dtInstrumentFunctions.DefaultView;
                }
            }

            if ((tabControl1.SelectedIndex == 2 && functionTabIsVisible) || (tabControl1.SelectedIndex == 1 && !functionTabIsVisible))
            {
                selectedTab = tabControl1.SelectedIndex;
            }

            if ((tabControl1.SelectedIndex == 3 && functionTabIsVisible) || (tabControl1.SelectedIndex == 2 && !functionTabIsVisible))
            {
                selectedTab = tabControl1.SelectedIndex;
            }

            if ((tabControl1.SelectedIndex == 4 && functionTabIsVisible) || (tabControl1.SelectedIndex == 3 && !functionTabIsVisible))
            {
                selectedTab = tabControl1.SelectedIndex;
            }

            if ((tabControl1.SelectedIndex == 5 && functionTabIsVisible) || (tabControl1.SelectedIndex == 4 && !functionTabIsVisible))
            {
                selectedTab = tabControl1.SelectedIndex;
            }

            if ((tabControl1.SelectedIndex == 6 && functionTabIsVisible) || (tabControl1.SelectedIndex == 5 && !functionTabIsVisible))
            {
                buttonAdd.Visibility = Visibility.Hidden;
                buttonDel.Visibility = Visibility.Hidden;

                Mouse.OverrideCursor = Cursors.Wait;

                selectedTab = tabControl1.SelectedIndex;
                UpdateLog();

                try
                {
                    ListBox1.ScrollIntoView(ListBox1.Items[ListBox1.Items.Count - 1]); // scroll to the last entries
                }
                catch { }

                Mouse.OverrideCursor = null;
            }
        }

        private void CheckBoxLogSwitches_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxLogSwitches.IsChecked == true)
            {
                switchLog = true;
            }
            else
            {
                switchLog = false;
            }
        }

        private void Capture_Click(object sender, RoutedEventArgs e)
        {
            ScreenCapture(Convert.ToInt32(Left), Convert.ToInt32(Top), Convert.ToInt32(Width), Convert.ToInt32(Height));
        }

        private void CheckBox_Shadow_Checked(object sender, RoutedEventArgs e)
        {
            shadowChecked = true;

            //if (!cockpitWindowActiv) { CockpitClose(); }

            //CockpitShow();
            //UpdateLog();
        }

        private void CheckBox_Shadow_Unchecked(object sender, RoutedEventArgs e)
        {
            shadowChecked = false;

            //if (!cockpitWindowActiv) { CockpitClose(); }

            //CockpitShow();
            //UpdateLog();
        }

        private void JSON_Checked(object sender, RoutedEventArgs e)
        {
            jsonChecked = true;
            SendJSON.Visibility = Visibility.Visible;
        }

        private void JSON_Unchecked(object sender, RoutedEventArgs e)
        {
            jsonChecked = false;
            SendJSON.Visibility = Visibility.Hidden;
        }

        private void SendJSON_Click(object sender, RoutedEventArgs e)
        {
            UDP.UDPSender(IPAddess.Text.Trim(), Convert.ToInt32(PortSender.Text), json); // send a package to DCS
        }

        #endregion
    }

    #region Databases

    public class Instrument
    {
        public int instID = 0;
        public string classname = "";
        public int windowID = 0;
        public List<InstrumentFunction> instrumentFunction = new List<InstrumentFunction> { };

        public Instrument(int _instID, string _classname, int _windowID)
        {
            instID = _instID;
            classname = _classname;
            windowID = _windowID;
        }
    }

    public class InstrumentFunction
    {
        public int argNumber = 0;
        public bool ascii = false;

        public double value = 0.0;
        public double oldValue = 0.0;
        public string asciiValue = "";
        public string oldAsciiValue = "";

        public InstrumentFunction(int _argNumber, bool _ascii)
        {
            argNumber = _argNumber;
            ascii = _ascii;
        }
    }

    public class Lamps
    {
        public int ID = 0;
        public int argNumber = 0;
        public int windowID = 0;
        public double value = 0.0;
        public double oldValue = 0.0;

        public Lamps(int _ID, int _argNumber, int _windowID)
        {
            ID = _ID;
            argNumber = _argNumber;
            windowID = _windowID;
        }
    }

    public class Switches
    {
        public int ID = 0;
        public int windowID = 0;
        public int clickabledataID = 0;
        public string classname = "";

        public int dcsID = 0;
        public int deviceID = 0;
        public int buttonID = 0;

        public double value = 0.0;
        public double oldValue = 0.0;
        public bool dontReset = false;
        public bool events = false;
        public bool sendDouble = false;
        public bool sendRelease = false;
        public bool ignoreNextPackage = false;
        public bool ignoreAllPackage = false;
        public bool doit = false;

        public Switches(int _ID, int _windowID, int _clickable, string _class)
        {
            ID = _ID;
            windowID = _windowID;
            clickabledataID = _clickable;
            classname = _class;
        }
    }

    #endregion
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Ikarus
{
    public partial class MainWindow : Window
    {
        #region Member

        public enum State
        {
            startup,
            init,
            run,
            reset,
            stop
        }
        public static State timerstate = State.startup;

        public static List<Cockpit> cockpitWindows = new List<Cockpit> { };
        public static List<Instrument> instruments = new List<Instrument> { };
        public static List<Lamps> lamps = new List<Lamps> { };
        public static List<Switches> switches = new List<Switches> { };

        public static Point cockpitKoord = new Point();
        public static CultureInfo cult = new CultureInfo("en-GB");
        //---------------------- D A T A B A S E ----------------------
        public static DataSet1 dsInstruments = new DataSet1();
        public static bool cockpitWindowActiv = false;
        public static bool refreshCockpit = false;
        public static bool refreshInstruments = false;
        public static DataSet2 dsConfig = new DataSet2();

        public static DataTable dtConfig;
        public static DataTable dtInstruments;
        public static DataTable dtInstrumentFunctions;
        public static DataTable dtParameter;
        public static DataTable dtLamps;
        public static DataTable dtSwitches;
        public static DataTable dtAccessories;
        public static DataTable dtClassnames;
        public static DataTable dtWindows;

        //---------------------- D A C --------------------------------- 
        public static Masterdata dsMaster = new Masterdata();
        public static DataTable dtMasterLamps;
        public static DataTable dtMasterSwitches;

        private static string background = "";
        public static string dbFilename = "";
        public static bool functionTabIsVisible = true;
        private static string newline = Environment.NewLine;
        private static string currentDirectory = Environment.CurrentDirectory;

        private static DataRowView rowView = null;
        private static DataRow[] dataRows = new DataRow[] { };
        public static DataRow dataRow = null;
        private static DataRow[] dataRowsInstrumentsFunction = new DataRow[] { };
        private static DataRow[] dataRowsMasterSwitches = new DataRow[] { };
        private static DataRow[] dataRowsSwitches = new DataRow[] { };

        private static int loopCounterSwitches = 0;
        private static int loopSwitches = 1; // 151
        private static int selectedIndexSwitches = 0;
        private static int selectedIndexLamps = 0;
        private static int windowID = 0;
        private static int logCount = 0;

        private static bool lStateEnabled = true;
        private static Thread udpThread = null;
        private static string portListener = "";
        public static string receivedData = "";
        private static string[] receivedItems = new string[] { };
        public static bool detailLog = false;
        public static bool dataLog = false;
        private static bool cleanupMemory = false;

        private static string readFile = "";
        private static string lastFile = "-";
        private static string searchStringForFile = "File";
        private static List<string> updateLamp = new List<string>();
        private static List<int> updateWindowID = new List<int>();

        public static bool editmode = false;

        private static string package = "";

        private static int selectedInstrument = -1;
        private static int selectedFunction = -1;
        private static int selectedSwitch = -1;
        private static int selectedLamp = -1;
        private static int selectedTab = -1;
        private static int selectedAccessories = -1;
        private static int selectedWindows = -1;
        private static int dataStackSize = 0;
        private static string lastSelectedInstrumentsClass = "";
        private static int renderTier = 0;
        public static string processNameDCS = "DCS";
        public static bool lightsChecked = false;
        public static bool isRep = false;

        //private static int timerMainLoop = 0;
        //private static double flattening = 0.00025;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentCulture = cult;
            Thread.CurrentThread.CurrentUICulture = cult;

            try
            {
                checkBoxLog.IsChecked = true;
                lbLogging.Visibility = Visibility.Visible;

                File.Text = "";
                IPAddess.Text = "127.0.0.1";
                PortListener.Text = "1625";
                portListener = PortListener.Text;
                PortSender.Text = "26027";

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

                            Main.Title = "Ikarus - ( Configured for " + dbFilename.Substring(0, dbFilename.LastIndexOf(".")) + " )";
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
                        ImportExport.XmlToDataSet(dbFilename, dsInstruments);
                        ImportExport.XmlToDataSet(dbFilename.Substring(0, dbFilename.LastIndexOf(".")) + ".xml", dsMaster);

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

                //SortTables();
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
                    isRep = true;
                }

                if (isRep)
                {
                    ImportExport.DatasetToXml(dbFilename, dsInstruments);
                    ImportExport.LogMessage("Save file: " + dbFilename);
                }

                tabControl1.SelectedIndex = 5;
                selectedTab = 0;

                //buttonAdd.Visibility = Visibility.Visible;
                lbEditMode.Visibility = Visibility.Hidden;
                Refresh.Visibility = Visibility.Hidden;

                ListBox1.ItemsSource = ImportExport.logItems;

                functionTabIsVisible = true;
                if (!functionTabIsVisible) tabControl1.Items.Remove(Function);

                detailLog = false;
                checkBoxLog.IsChecked = detailLog;
                lbLogging.Visibility = detailLog ? Visibility.Visible : Visibility.Hidden;

                cleanupMemory = false;
                checkBoxCleanMemory.IsChecked = cleanupMemory;

                DataGridInstruments.CanUserDeleteRows = true;
                DatagridFunction.CanUserDeleteRows = true;
                DataGridSwitches.CanUserDeleteRows = true;
                DataGridLamps.CanUserDeleteRows = true;
                DataGridWindows.CanUserDeleteRows = true;
            }
            catch (Exception e) { ImportExport.LogMessage("Startup problem .. " + e.ToString()); }

            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           udpThread = new Thread(new ThreadStart(StartListener));
                           udpThread.IsBackground = true;
                           udpThread.Start();
                       }));

            timerstate = State.run;
            StartTimer();

            GetTier();

            GC.Collect(0, GCCollectionMode.Forced);
        }

        private void StartTimer()
        {
            DispatcherTimer timerMain = new DispatcherTimer(DispatcherPriority.Normal);
            timerMain.Tick += timerMain_Tick;
            timerMain.Interval = TimeSpan.FromMilliseconds(100.0);
            timerMain.Start();
        }

        private void StartListener()
        {
            UDP.StartListener(Convert.ToInt16(portListener.Trim()));
        }

        //+++++++++++++++++++++++ Main loop ++++++++++++++++++++++++
        private void timerMain_Tick(object sender, EventArgs e)
        {
            //timerMainLoop++;

            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           #region switch to new file

                           //if (timerstate == State.readfile)
                           //{
                           //    if (lStateEnabled)
                           //    {
                           //        lStateEnabled = false;

                           //        //CockpitLoad();

                           //        timerstate = State.run;
                           //        lStateEnabled = true;
                           //    }
                           //}
                           #endregion

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

                                   try
                                   {
                                       dataStackSize = UDP.receivedDataStack.Count;

                                       for (int i = 0; i < dataStackSize; i++)
                                       {
                                           receivedData = UDP.receivedDataStack[0];
                                           UDP.receivedDataStack.RemoveAt(0);

                                           if (receivedData.IndexOf("Ikarus=stop") != -1)
                                           {
                                               DatabaseResetValue();
                                               FillClasses();
                                               ResetCockpit();
                                               break;
                                           }
                                           if (receivedData.IndexOf(searchStringForFile) != -1)
                                           {
                                               CockpitLoad(ref receivedData);
                                               break;
                                           }
                                           if (receivedData.IndexOf("2222=1.0") != -1) { Lights_IsChecked(true); }
                                           if (receivedData.IndexOf("2222=0.0") != -1) { Lights_IsChecked(false); }
                                       }
                                       UDP.receivedDataStack.Clear();
                                   }
                                   catch (Exception ex) { ImportExport.LogMessage("State run: " + ex.ToString()); }

                                   lStateEnabled = true;
                               }
                           }
                           #endregion

                           #region Switches Update

                           if (++loopCounterSwitches >= loopSwitches && lStateEnabled)
                           {
                               lStateEnabled = false;

                               int windowID = 0;
                               string package = "";

                               for (int n = 0; n < switches.Count; n++)
                               {
                                   try
                                   {
                                       windowID = switches[n].windowID - 1;

                                       if (switches[n].value != switches[n].oldValue)
                                       {
                                           if (!switches[n].dontReset) switches[n].oldValue = switches[n].value; //<---

                                           if (switches[n].events)
                                           {
                                               if (!switches[n].dontReset) switches[n].events = false; //<---

                                               package = "C" + switches[n].deviceID.ToString() + "," + (3000 + switches[n].buttonID).ToString() + "," + switches[n].value.ToString();

                                               UDP.UDPSender(IPAddess.Text.Trim(), Convert.ToInt32(PortSender.Text), package); //<--- send a package to DCS

                                               /*if (detailLog)*/ { ImportExport.LogMessage(switches[n].classname + " ID: " + switches[n].dcsID.ToString() + " - Send package to " + IPAddess.Text.Trim() + ":" + PortSender.Text + " - Package: " + package); }

                                               if (switches[n].sendRelease) // && switches[n].value > 0.0)
                                               {
                                                   package = "C" + switches[n].deviceID.ToString() + "," + (3000 + switches[n].buttonID).ToString() + "," + (0.0).ToString();
                                                   UDP.UDPSender(IPAddess.Text.Trim(), Convert.ToInt32(PortSender.Text), package);
                                                   /*if (detailLog)*/ { ImportExport.LogMessage(switches[n].classname + " ID: " + switches[n].dcsID.ToString() + " - Send package to " + IPAddess.Text.Trim() + ":" + PortSender.Text + " - Package: " + package); }
                                               }

                                               if (!switches[n].dontReset) //<---
                                               {
                                                   if (switches[n].classname.LastIndexOf("Button") > -1)
                                                   {
                                                       switches[n].oldValue = 0.0;
                                                       switches[n].value = 0.0;
                                                   }
                                               }
                                           }
                                           else // from DCS
                                           {
                                               if (cockpitWindowActiv && cockpitWindows[windowID] != null)
                                                   cockpitWindows[windowID].UpdateSwitches(switches[n].ID.ToString());
                                           }
                                       }
                                   }
                                   catch (Exception ex) { ImportExport.LogMessage("Switches " + (n + 1) + " Update:" + ex.ToString()); }
                               }
                               loopCounterSwitches = 0;

                               lStateEnabled = true;
                           }
                           #endregion

                           #region tools

                           //if (lStateEnabled)
                           //{
                           //lStateEnabled = false;
                           //GC.Collect(0, GCCollectionMode.Forced);

                           //    if (cleanupMemory)
                           //    {
                           //        loopCounter++;

                           //        if (loopCounter >= loopMax)
                           //        {
                           //            MemoryManagement.Reduce();
                           //            loopCounter = 0;
                           //        }
                           //    }
                           //lStateEnabled = true;
                           //}
                           #endregion
                       }));

            //timerMainLoop--;
        }
        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region memberfunctions

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
            }
            catch (Exception ex) { ImportExport.LogMessage("Cockpit closed: " + ex.ToString()); }

            cockpitWindows.Clear();
            UpdateLog();
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
                        if (cockpitWindows[i] != null) cockpitWindows[i].Close();
                    }
                }
                catch (Exception ex) { ImportExport.LogMessage("Close all old panels and load a new configuration ... " + ex.ToString()); }

                cockpitWindows.Clear();

                for (int i = 0; i < dtWindows.Rows.Count; i++)
                {
                    cockpitWindows.Add(new Cockpit(Convert.ToInt16(dtWindows.Rows[i]["WindowID"]), dtWindows.Rows[i]["Background"].ToString()));

                    cockpitWindows[i].Left = double.Parse(dtWindows.Rows[i]["PosX"].ToString().Replace(",", "."), CultureInfo.InvariantCulture);
                    cockpitWindows[i].Top = double.Parse(dtWindows.Rows[i]["PosY"].ToString().Replace(", ", "."), CultureInfo.InvariantCulture);
                    cockpitWindows[i].Height = double.Parse(dtWindows.Rows[i]["Height"].ToString().Replace(", ", "."), CultureInfo.InvariantCulture);
                    cockpitWindows[i].Width = double.Parse(dtWindows.Rows[i]["Width"].ToString().Replace(", ", "."), CultureInfo.InvariantCulture);

                    cockpitWindows[i].Show();
                    if (lightsChecked) cockpitWindows[i].UpdateInstrumentLights(lightsChecked, dtWindows.Rows[i]["BackgroundNight"].ToString());
                }

                HidePanels();

                DatagridFunction.ItemsSource = dtInstrumentFunctions.DefaultView;

                cockpitWindowActiv = true;
                buttonShow.Content = "Close Cockpit";
                ImportExport.LogMessage("Cockpit opened .. ");

                Mouse.OverrideCursor = null;

                GC.Collect(0, GCCollectionMode.Forced);
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
                            //WindowState = WindowState.Minimized;
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
                        //buffer.Append(newline);
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
            //WindowState = WindowState.Normal;

            ImportExport.LogMessage("Reset Cockpit .. ");

            for (int i = 0; i < dtInstrumentFunctions.Rows.Count; i++)
            {
                try
                {
                    dtInstrumentFunctions.Rows[i]["AsciiValue"] = "";
                    dtInstrumentFunctions.Rows[i]["OldAsciiValue"] = "";
                    dtInstrumentFunctions.Rows[i]["Value"] = 0.0;
                    dtInstrumentFunctions.Rows[i]["OldValue"] = 0.0;
                }
                catch { }
            }

            for (int i = 0; i < dtLamps.Rows.Count; i++)
            {
                try
                {
                    dtLamps.Rows[i]["Value"] = 0.0;
                    dtLamps.Rows[i]["OldValue"] = 0.0;
                }
                catch { }
            }

            for (int i = 0; i < dtSwitches.Rows.Count; i++)
            {
                try
                {
                    dtSwitches.Rows[i]["Value"] = 0.0;
                    dtSwitches.Rows[i]["OldValue"] = 0.0;
                }
                catch { }
            }
        }

        private void FillClasses()
        {
            try
            {
                instruments.Clear(); // Gauges

                for (int i = 0; i < dtInstruments.Rows.Count; i++)
                {
                    instruments.Add(new Instrument(Convert.ToInt32(dtInstruments.Rows[i]["IDInst"]), dtInstruments.Rows[i]["Class"].ToString(), Convert.ToInt32(dtInstruments.Rows[i]["WindowID"])));

                    dataRowsInstrumentsFunction = dtInstrumentFunctions.Select("IDInst=" + dtInstruments.Rows[i]["IDInst"].ToString());

                    for (int n = 0; n < dataRowsInstrumentsFunction.Length; n++)
                    {
                        instruments[i].instrumentFunction.Add(new InstrumentFunction(Convert.ToInt32(dataRowsInstrumentsFunction[n]["Arg_number"]), Convert.ToBoolean(dataRowsInstrumentsFunction[n]["Ascii"])));
                    }
                }

                lamps.Clear();

                for (int i = 0; i < dtLamps.Rows.Count; i++)
                {
                    lamps.Add(new Lamps(Convert.ToInt32(dtLamps.Rows[i]["ID"]), Convert.ToInt32(dtLamps.Rows[i]["Arg_number"]), Convert.ToInt32(dtLamps.Rows[i]["WindowID"])));
                }

                switches.Clear();

                for (int i = 0; i < dtSwitches.Rows.Count; i++)
                {
                    switches.Add(new Switches(Convert.ToInt32(dtSwitches.Rows[i]["ID"]), Convert.ToInt32(dtSwitches.Rows[i]["WindowID"]), Convert.ToInt32(dtSwitches.Rows[i]["ClickabledataID"]), dtSwitches.Rows[i]["Class"].ToString()));

                    dataRowsMasterSwitches = dtMasterSwitches.Select("ID='" + switches[i].clickabledataID.ToString() + "'");

                    if (dataRowsMasterSwitches.Length > 0)
                    {
                        switches[i].dcsID = Convert.ToInt32(dataRowsMasterSwitches[0]["DcsID"]);
                        switches[i].deviceID = Convert.ToInt32(dataRowsMasterSwitches[0]["DeviceID"]);
                        switches[i].buttonID = Convert.ToInt32(dataRowsMasterSwitches[0]["ButtonID"]);
                    }
                }
            }
            catch (Exception e) { ImportExport.LogMessage("FillClasses problem .. " + e.ToString()); }
        }

        public void GetTier()
        {
            renderTier = (RenderCapability.Tier >> 16);
            if (renderTier == 0) Tier.Text = "No graphics hardware acceleration";
            if (renderTier == 1) Tier.Text = "Partial grafics hardware acceleration";
            if (renderTier == 2) Tier.Text = "Hardware acceleration";
        }

        private static bool GrabFile(string ID, ref string gotData)
        {
            string[] receivedItems = gotData.Split(':');

            for (int n = 0; n < receivedItems.Length; n++)
            {
                if (receivedItems[n].IndexOf(ID, 0) == 0)
                {
                    readFile = receivedItems[n].Substring(receivedItems[n].IndexOf("=", 0) + 1);
                    return true;
                }
            }
            return false;
        }

        private static string GrabValue(string ID, ref string receivedData)
        {
            ID = ID + "=";

            if (receivedData.IndexOf(ID, 0) > -1) // Dirty quickcheck before loop
            {
                for (int n = 0; n < receivedItems.Length; n++)
                {
                    if (receivedItems[n].IndexOf(ID, 0) == 0)
                    {
                        return receivedItems[n].Substring(receivedItems[n].IndexOf("=", 0) + 1);
                    }
                }
            }
            return "";
        }

        public static void GrabValues(string receivedData = "")
        {
            if (!cockpitWindowActiv) { return; }

            try
            {
                string newValue = "";
                int windowID = 0;

                if (receivedData.Length < 3) { return; }
                receivedItems = receivedData.Split(':');

                #region Gauges

                for (int i = 0; i < instruments.Count; i++)
                {
                    try
                    {
                        refreshInstruments = false;

                        for (int n = 0; n < instruments[i].instrumentFunction.Count; n++)
                        {
                            if (instruments[i].instrumentFunction[n].argNumber > 0) { newValue = GrabValue(instruments[i].instrumentFunction[n].argNumber.ToString(), ref receivedData); }

                            if (newValue != "")
                            {
                                windowID = instruments[i].windowID - 1;

                                if (instruments[i].instrumentFunction[n].ascii)
                                {
                                    if (newValue != instruments[i].instrumentFunction[n].oldAsciiValue)
                                    {
                                        instruments[i].instrumentFunction[n].asciiValue = newValue;
                                        instruments[i].instrumentFunction[n].oldAsciiValue = newValue;

                                        refreshInstruments = true;
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        instruments[i].instrumentFunction[n].value = double.Parse(newValue, CultureInfo.InvariantCulture);

                                        if (instruments[i].instrumentFunction[n].value != instruments[i].instrumentFunction[n].oldValue) // &&
                                                                                                                                         //Math.Abs(instruments[i].instrumentFunction[n].value - instruments[i].instrumentFunction[n].oldValue) > flattening)
                                        {
                                            refreshInstruments = true;
                                            instruments[i].instrumentFunction[n].oldValue = instruments[i].instrumentFunction[n].value;
                                        }
                                    }
                                    catch
                                    {
                                        refreshInstruments = true;
                                        instruments[i].instrumentFunction[n].ascii = true;
                                        instruments[i].instrumentFunction[n].asciiValue = newValue;
                                        instruments[i].instrumentFunction[n].oldAsciiValue = newValue;
                                    }
                                }
                            }
                        }
                        if (refreshInstruments && cockpitWindows[windowID] != null)
                        {
                            cockpitWindows[windowID].UpdateInstruments(instruments[i].instID.ToString(), false);
                        }
                    }
                    catch (Exception e) { ImportExport.LogMessage("Gauges " + (i + 1).ToString() + " problem .. " + e.ToString()); }
                }

                #endregion

                #region Lamps

                for (int n = 0; n < lamps.Count; n++)
                {
                    try
                    {
                        if (lamps[n].argNumber > 0) { newValue = GrabValue(lamps[n].argNumber.ToString(), ref receivedData); }

                        if (newValue != "")
                        {
                            windowID = lamps[n].windowID - 1;
                            lamps[n].value = double.Parse(newValue, CultureInfo.InvariantCulture);

                            if (lamps[n].value != lamps[n].oldValue)
                            {
                                lamps[n].oldValue = lamps[n].value;

                                if (cockpitWindowActiv && cockpitWindows[windowID] != null)
                                    cockpitWindows[windowID].UpdateLamps(lamps[n].ID.ToString());
                            }
                        }
                    }
                    catch (Exception e) { ImportExport.LogMessage("Lamp " + (n + 1).ToString() + " problem .. " + e.ToString()); }
                }
                #endregion

                #region Switches

                for (int n = 0; n < switches.Count; n++)
                {
                    try
                    {
                        if (switches[n].dcsID > 0) newValue = GrabValue(switches[n].dcsID.ToString(), ref receivedData);

                        if (newValue != "")
                        {
                            switches[n].value = double.Parse(newValue, CultureInfo.InvariantCulture);
                            if (detailLog) { ImportExport.LogMessage("Got new value for switch no. " + (n + 1).ToString() + " - " + switches[n].dcsID.ToString() + "=" + newValue); }
                        }
                    }
                    catch { ImportExport.LogMessage("Error with switch no. " + (n + 1).ToString() + " clickableID = " + dtSwitches.Rows[n]["ClickabledataID"].ToString() + " - " + switches[n].dcsID.ToString() + "=" + newValue); }
                }
                #endregion
            }
            catch (Exception e) { ImportExport.LogMessage("GrabValue problem .. " + e.ToString()); }
        }

        private void HidePanels()
        {
            int visible = 0;
            int panelID = 1;

            dataRows = dtSwitches.Select("Class='SwitchPanelOffOn'");

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

                cockpitWindows[panelID - 1].Visibility = (visible == 0) ? Visibility.Hidden : Visibility.Visible;
            }
        }

        private void LoadConfiguration(string filename)
        {
            try
            {
                lastFile = filename;
                readFile = filename;
                isRep = false;

                try
                {
                    if (cockpitWindowActiv)
                    {
                        try
                        {
                            for (int i = 0; i < cockpitWindows.Count; i++)
                            {
                                if (cockpitWindows[i] != null) cockpitWindows[i].Close();
                            }
                        }
                        catch (Exception ex) { ImportExport.LogMessage("Close all old panels and load a new configuration ... " + ex.ToString()); }
                    }

                    ResetTables();
                    ImportExport.XmlToDataSet(filename + ".ikarus", dsInstruments);

                    dsMaster.Clear();
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

                    FillClasses();
                }
                catch (Exception e) { ImportExport.LogMessage("XmlToDataSet problem .. " + e.ToString()); }

                try
                {
                    background = dtParameter.Rows[0][0].ToString();
                }
                catch { }

                Main.Title = "Ikarus - ( Configured for " + filename + " )";

                //SortTables();

                if (dtWindows.Rows.Count == 0)
                {
                    dtWindows.Rows.Add(1, "Front Panel", dtConfig.Rows[0][0].ToString(), dtConfig.Rows[0][1].ToString(), dtConfig.Rows[0][2].ToString(), dtConfig.Rows[0][3].ToString(), background);
                    isRep = true;
                }

                if (isRep)
                {
                    ImportExport.DatasetToXml(filename + ".ikarus", dsInstruments);
                    ImportExport.LogMessage("Save file: " + filename + ".ikarus");
                }

                //if (functionTabIsVisible)
                //{
                //    tabControl1.Items.Remove(Function);
                //    functionTabIsVisible = false;
                //    ShowFunctionTab.IsChecked = false;
                //}

                RefreshDatagrids();
                CockpitShow();
                UpdateLog();

                package = "R";
                UDP.UDPSender(IPAddess.Text.Trim(), Convert.ToInt32(PortSender.Text), package); // send a package to DCS

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

        private void ResetCockpit()
        {
            for (int i = 0; i < instruments.Count; i++)
            {
                try
                {
                    windowID = instruments[i].windowID - 1;

                    for (int n = 0; n < instruments[i].instrumentFunction.Count; n++)
                    {
                        if (cockpitWindowActiv && cockpitWindows[windowID] != null) cockpitWindows[windowID].UpdateInstruments(instruments[i].instID.ToString(), false);
                    }
                }
                catch (Exception e) { ImportExport.LogMessage("ResetCockpit problem instruments ID = " + instruments[i].instID + " .. " + e.ToString()); }
            }

            for (int n = 0; n < lamps.Count; n++)
            {

                try
                {
                    windowID = lamps[n].windowID - 1;
                    if (cockpitWindowActiv && cockpitWindows[windowID] != null) cockpitWindows[windowID].UpdateLamps(lamps[n].ID.ToString());
                }
                catch (Exception e) { ImportExport.LogMessage("ResetCockpit problem lamps ID = " + lamps[n].ID + " .. " + e.ToString()); }
            }

            for (int n = 0; n < switches.Count; n++)
            {
                try
                {
                    windowID = switches[n].windowID - 1;
                    if (cockpitWindowActiv && cockpitWindows[windowID] != null) cockpitWindows[windowID].UpdateSwitches(switches[n].ID.ToString());
                }
                catch (Exception e) { ImportExport.LogMessage("ResetCockpit problem switches ID = " + switches[n].ID + " .. " + e.ToString()); }
            }
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

            dtLamps.Columns["ID"].AutoIncrementStep = -1;
            dtLamps.Columns["ID"].AutoIncrementSeed = -1;
            dtLamps.Columns["ID"].AutoIncrementStep = 1;
            dtLamps.Columns["ID"].AutoIncrementSeed = 1;

            dtSwitches.Columns["ID"].AutoIncrementStep = -1;
            dtSwitches.Columns["ID"].AutoIncrementSeed = -1;
            dtSwitches.Columns["ID"].AutoIncrementStep = 1;
            dtSwitches.Columns["ID"].AutoIncrementSeed = 1;

            dtAccessories.Columns["ID"].AutoIncrementStep = -1;
            dtAccessories.Columns["ID"].AutoIncrementStep = -1;
            dtAccessories.Columns["ID"].AutoIncrementStep = 1;
            dtAccessories.Columns["ID"].AutoIncrementStep = 1;

            dtWindows.Columns["WindowID"].AutoIncrementStep = -1;
            dtWindows.Columns["WindowID"].AutoIncrementStep = -1;
            dtWindows.Columns["WindowID"].AutoIncrementStep = 1;
            dtWindows.Columns["WindowID"].AutoIncrementStep = 1;
        }

        public void SelectDataGridItem(string _tablename, int impordID)
        {
            if (_tablename == "Accessories") // && selectedAccessories != _itemNo - 1)
            {
                tabControl1.SelectedIndex = functionTabIsVisible ? 4 : 3;
                selectedLamp = -1;
                selectedSwitch = -1;
                selectedInstrument = -1;

                for (int i = 0; i < dtAccessories.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dtAccessories.Rows[i]["ID"]) == impordID)
                    {
                        selectedAccessories = i;
                        DataGridAccessories.SelectedItem = DataGridAccessories.Items[i];
                        DataGridAccessories.ScrollIntoView(DataGridAccessories.Items[i]);
                    }
                }
                return;
            }
            if (_tablename == "Instruments") // && selectedInstrument != _itemNo - 1)
            {
                tabControl1.SelectedIndex = 0;
                selectedLamp = -1;
                selectedSwitch = -1;
                selectedAccessories = -1;

                for (int i = 0; i < dtInstruments.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dtInstruments.Rows[i]["IDInst"]) == impordID)
                    {
                        selectedInstrument = i;
                        DataGridInstruments.SelectedItem = DataGridInstruments.Items[i];
                        DataGridInstruments.ScrollIntoView(DataGridInstruments.Items[i]);
                    }
                }
                return;
            }
            if (_tablename == "Lamps") // && selectedLamp != _itemNo - 1)
            {
                tabControl1.SelectedIndex = functionTabIsVisible ? 3 : 2;
                selectedInstrument = -1;
                selectedSwitch = -1;
                selectedAccessories = -1;

                for (int i = 0; i < dtLamps.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dtLamps.Rows[i]["ID"]) == impordID)
                    {
                        selectedLamp = i;
                        DataGridLamps.SelectedItem = DataGridLamps.Items[i];
                        DataGridLamps.ScrollIntoView(DataGridLamps.Items[i]);
                    }
                }
                return;
            }

            if (_tablename == "Switches") // && selectedSwitch != _itemNo - 1)
            {
                tabControl1.SelectedIndex = functionTabIsVisible ? 2 : 1;
                selectedLamp = -1;
                selectedInstrument = -1;
                selectedAccessories = -1;

                for (int i = 0; i < dtSwitches.Rows.Count; i++)
                {
                    if (Convert.ToInt32(dtSwitches.Rows[i]["ID"]) == impordID)
                    {
                        selectedSwitch = i;
                        DataGridSwitches.SelectedItem = DataGridSwitches.Items[i];
                        DataGridSwitches.ScrollIntoView(DataGridSwitches.Items[i]);
                    }
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
                        isRep = true;
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
                if (refreshInstruments && cockpitWindows[windowID] != null)
                    cockpitWindows[windowID].UpdateInstruments(dtInstruments.Rows[i]["IDInst"].ToString(), true);
            }
        }

        //private void SortTables()
        //{
        //    try
        //    {
        //        dataRows = dsInstruments.Tables[0].Select("", "IDInst ASC");

        //        if (dataRows.Length > 0)
        //        {
        //            dtInstruments = dataRows.CopyToDataTable();
        //        }
        //        else
        //        {
        //            dtInstruments = dsInstruments.Tables[0];
        //        }

        //        dataRows = dsInstruments.Tables[1].Select("", "IDInst ASC, IDFct ASC");

        //        if (dataRows.Length > 0)
        //        {
        //            dtInstrumentFunctions = dataRows.CopyToDataTable();
        //        }
        //        else
        //        {
        //            dtInstrumentFunctions = dsInstruments.Tables[1];
        //        }
        //    }
        //    catch (Exception e) { ImportExport.LogMessage("Startup problem .. " + e.ToString()); }
        //}

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

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch { }
        }

        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".ikarus";
            dlg.Filter = "IKARUS files (*.ikarus)|*.ikarus|All files (*.*)|*.*";

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
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".png";
            dlg.Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*";

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

                //if (cockpitWindowActiv)
                //{
                //    cockpitWindows[0].UpdateEditor(ref dtAccessories);
                //    cockpitWindows[0].UpdateEditor(ref dtInstruments);
                //    cockpitWindows[0].UpdateEditor(ref dtLamps);
                //    cockpitWindows[0].UpdateEditor(ref dtSwitches);
                //}
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
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.FileName = dbFilename;

            dlg.DefaultExt = ".ikarus";
            dlg.Filter = "IKARUS files (*.ikarus)|*.ikarus|All files (*.*)|*.*";

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

            dsConfig.AcceptChanges();

            //CockpitReset();
            DatabaseResetValue();

            ImportExport.DatasetToXml(currentDirectory + "\\Config.xml", dsConfig);
            ImportExport.LogMessage("Saved file: " + "Config.xml");

            ImportExport.DatasetToXml(currentDirectory + "\\" + dlg.FileName.Substring(dlg.FileName.LastIndexOf("\\") + 1), dsInstruments);
            ImportExport.LogMessage("Saved file: " + dlg.FileName.Substring(dlg.FileName.LastIndexOf("\\") + 1));

            lastFile = "";
            UpdateLog();
        }

        private void Button_Show_Click(object sender, RoutedEventArgs e)
        {
            if (!cockpitWindowActiv)
            {
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

        private void CheckBox_CleanMemory_Click(object sender, RoutedEventArgs e)
        {
            if (checkBoxCleanMemory.IsChecked == true)
            {
                cleanupMemory = true;
            }
            else
            {
                cleanupMemory = false;
            }
        }

        private void Checkbox_EditMode_Click(object sender, RoutedEventArgs e)
        {
            if (timerstate == State.stop)
            {
                ImportExport.LogMessage("Edit mode: OFF");

                timerstate = State.run;
                lbEditMode.Visibility = Visibility.Hidden;
                Refresh.Visibility = Visibility.Hidden;
                editmode = false;
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

                //ShowFunctionTab.IsChecked = true;

                editmode = true;
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
                lbLogging.Visibility = System.Windows.Visibility.Visible;
                detailLog = true;
            }
            else
            {
                lbLogging.Visibility = System.Windows.Visibility.Hidden;
                detailLog = false;
            }
        }

        private void CheckBox_ShowFunctionTab_Checked(object sender, RoutedEventArgs e)
        {
            //tabControl1.Items.Insert(1, Function);
            //functionTabIsVisible = true;
        }

        private void CheckBox_ShowFunctionTab_Unchecked(object sender, RoutedEventArgs e)
        {
            //tabControl1.Items.Remove(Function);
            //functionTabIsVisible = false;
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

        //private void Main_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    //if (!MainWindow.editmode) ProzessHelper.SetFocusToExternalApp(MainWindow.processNameDCS);
        //}

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //DataGridInstruments.CanUserDeleteRows = true;
            //DatagridFunction.CanUserDeleteRows = true;
            //DataGridSwitches.CanUserDeleteRows = true;
            //DataGridLamps.CanUserDeleteRows = true;

            if (selectedTab == tabControl1.SelectedIndex) return;

            buttonAdd.Visibility = System.Windows.Visibility.Visible;
            buttonDel.Visibility = System.Windows.Visibility.Visible;

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
                ListBox1.ScrollIntoView(ListBox1.Items[ListBox1.Items.Count - 1]); // scroll to the last entries

                Mouse.OverrideCursor = null;
            }
        }

        #endregion
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

        public Switches(int _ID, int _windowID, int _clickable, string _class)
        {
            ID = _ID;
            windowID = _windowID;
            clickabledataID = _clickable;
            classname = _class;
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
}

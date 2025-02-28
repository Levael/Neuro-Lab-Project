﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using Params;
using Trajectories.TrajectoryCreators;
using VaryingValuesGenerators;
using MoogController;


namespace UniJoy
{

    /// <summary>
    /// This partial is used for events callbacks.
    /// </summary>
    public partial class GuiInterface : Form
    {
        #region MEMBERS
        /// <summary>
        /// The selected protocols path to view protocols.
        /// </summary>
        private string _protocolsDirPath;

        /// <summary>
        /// The variables read from the xlsx protocol file.
        /// </summary>
        private Variables _variablesList;

        /// <summary>
        /// The excel loader for reading data configuration.
        /// </summary>
        private ExcelProtocolConfigFileLoader _excelLoader;

        /// <summary>
        /// The dictionary of the dynamic allocated textboxes that are allocated each time the user choose different protocol.
        /// It saves the dynamic TextBox reference.
        /// The string represent the name of the varName concatinating with the attributename for each textbox.
        /// </summary>
        private Dictionary<string, Control> _dynamicAllocatedTextBoxes;

        /// <summary>
        /// Dictionary that describes all ButtonBase (checkboxes and radiobuttons) names in the gui as keys with their control as value.
        /// </summary>
        private Dictionary<string, ButtonBase> _buttonbasesDictionary;

        /// <summary>
        /// A list that holds all the titles for the variables attribute to show in the title of the table.
        /// </summary>
        private List<Label> _titlesLabelsList;

        /// <summary>
        /// Holds the AcrossVectorValuesGenerator generator.
        /// </summary>
        private VaryingValuesGeneratorBase _acrossVectorValuesGenerator;

        /// <summary>
        /// Holds the StaticValuesGenerator generator.
        /// </summary>
        private StaticValuesGenerator _staticValuesGenerator;

        /// <summary>
        /// ControlLoop interface for doing the commands inserted in the gui.
        /// </summary>
        private ControlLoop _cntrlLoop;

        /// <summary>
        /// The selected protocol file full name.
        /// </summary>
        private string _selectedProtocolFullName;

        /// <summary>
        /// The selected protocol name with no .xlsx extension and ('-') additional name.
        /// </summary>
        private string _selectedProtocolName;

        //TODO: necessary?
        /// <summary>
        /// The selected directions to give them hand REWARD (xxxxxyyy).
        /// The y-y-y is the indicators for the directions as followed by left-center-right.
        /// </summary>
        //private byte _selectedHandRewardDirections;

        /// <summary>
        /// Locker for starting and stopping button to be enabled not both.
        /// </summary>
        private object _lockerStopStartButton;

        /// <summary>
        /// Locker for the pause and resume button to be enabled not both.
        /// </summary>
        private object _lockerPauseResumeButton;

        /// <summary>
        /// Indicates if the robot was engaged or disengage.
        /// </summary>
        private bool _isEngaged;

        /// <summary>
        /// Logger for writing log information.
        /// </summary>
        private ILog _logger;
        #endregion MEMBERS

        #region CONSTRUCTORS
        /// <summary>
        /// Constructor.
        /// </summary>
        public GuiInterface(ref ExcelProtocolConfigFileLoader excelLoader)
        {
            InitializeComponent();
            _excelLoader = excelLoader;
            _variablesList = new Variables();
            _variablesList._variablesDictionary = new Dictionary<string, Variable>();
            _dynamicAllocatedTextBoxes = new Dictionary<string, Control>();
            _acrossVectorValuesGenerator = DecideVaryinVectorsGeneratorByProtocolName();
            _staticValuesGenerator = new StaticValuesGenerator();
            InitializeTitleLabels();
            ShowVaryingControlsOptions(show: false);

            InitializeCheckBoxesDictionary();

            //creating the logger to writting log file information.
            //Maayan edit
            //log4net.Config.XmlConfigurator.Configure(new FileInfo(Application.StartupPath + @"\Log4Net.config"));
            log4net.Config.XmlConfigurator.Configure();
            _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            _logger.Info("Starting program...");

            Globals._systemState = SystemState.INITIALIZED;

            //make the delegate with it's control object and their nickname as pairs of dictionaries.
            Tuple<Dictionary<string, Control>, Dictionary<string, Delegate>> delegatsControlsTuple = MakeCtrlDelegateAndFunctionDictionary();
            _cntrlLoop = new ControlLoop(delegatsControlsTuple.Item2, delegatsControlsTuple.Item1, _logger);

            try
            {
                // todo: doesn't work, may be not connected without exception
                //connect to the robot.
                MoogController.MoogController.Connect();
                _cntrlLoop.IsMoogConnected = true;
            }
            catch
            {
                _cntrlLoop.IsMoogConnected = false;

                MessageBox.Show("Cannot connect to the robot - check if robot is conncted in listen mode and also not turned off", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //reset the selected direction to be empty.
            //_selectedHandRewardDirections = 0;

            //allocate the start/stop button locker.
            _lockerStopStartButton = new object();
            //disable initially the start and stop button until makeTrials button is pressed.
            _btnStart.Enabled = false;
            _btnStop.Enabled = false;

            //allocate the pause/resume button locker.
            _lockerPauseResumeButton = new object();
            //disable initially both pause and resume buttons until makeTrials button is pressed.
            /*_btnPause.Enabled = false;
            _btnResume.Enabled = false;*/

            //enable the make trials btn.
            _btnMakeTrials.Enabled = true;

            //the start point is in the disengaged state.
            _isEngaged = false;

            //add the students names (as the setting have) to the student names combo box.
            //AddStudentsNamesToRatNamesComboBox();

            //set the default file browser protocol path directory.
            SetDefaultProtocolFileBrowserDirectory();

            //move the robot to it's home position when startup.
            //avi-insert//
            //_cntrlLoop.WriteHomePosFile();
            //_cntrlLoop.MoveRobotHomePosition();

            //create the result directory in the application path if needed.
            if (!Directory.Exists(Application.StartupPath + @"\results")) {
                Directory.CreateDirectory(Application.StartupPath + @"\results\");
            }
                

            //adding background image to the window.
            //this.BackgroundImage = Image.FromFile(Application.StartupPath + @"\unityWallpaper.jpg");
            //this._varyingControlGroupBox.BackgroundImage = Image.FromFile(Application.StartupPath + @"\unityWallpaper.jpg");
        }
        #endregion CONSTRUCTORS

        #region SELECTING_INTERFACES_FUNCTION_for_SELECTEDPROTOCOL
        /// <summary>
        /// Decide which of the VaryingVectorsGenerator to call by the protocol type.
        /// </summary>
        /// <returns>The mathed IVaryingVectorGenerator for the protocol type.</returns>
        private VaryingValuesGeneratorBase DecideVaryinVectorsGeneratorByProtocolName()
        {
            //take the name before the .xlsx and the generic name before the additional name (if added with '-' char).
            switch (_selectedProtocolName)
            {
                //TODO: add speed/distance discrimination
                case "HeadingDiscrimination":
                    return new VaryingValuesGeneratorHeadingDiscrimination();
                default:
                    return new VaryingValuesGenerator();
            }
        }

        /// <summary>
        /// Returns the ITrajectoryCreator to create trajectories with by the protocol type name.
        /// </summary>
        /// <param name="protoclName">he protocol name.</param>
        /// <returns>The ITrajectoryCreator to create trajectories with for the trials.</returns>
        private ITrajectoryCreator DecideTrajectoryCreatorByProtocolName(string protoclName)
        {
            //determine the TrajectoryCreator to call with.
            switch (protoclName)
            {
                //TODO: add speed/distance discrimination
                case "HeadingDiscrimination":
                    return new HeadingDiscrimination(_variablesList, _acrossVectorValuesGenerator._crossVaryingValsBoth, _staticValuesGenerator._staticVariableList, Properties.Settings.Default.Frequency);
                default:
                    {
                        MessageBox.Show("The protocol name is not accessed, running now the HeadingDiscrimination protocol", "Warning", MessageBoxButtons.OK);

                        return new HeadingDiscrimination(_variablesList, _acrossVectorValuesGenerator._crossVaryingValsBoth, _staticValuesGenerator._staticVariableList, Properties.Settings.Default.Frequency);
                    }
            }
        }
        #endregion SELECTING_INTERFACES_FUNCTION_for_SELECTEDPROTOCOL

        #region OUTSIDER_EVENTS_HANDLE_FUNCTION

        /// <summary>
        /// Delegate clearing the psycho online graph.
        /// </summary>
        public delegate void OnlinePsychoGraphClearDelegate();

        /// <summary>
        /// Clears the psycho online graph.
        /// </summary>
        public void OnlinePsychoGraphClear()
        {
            _onlinePsychGraphControl.Series.Clear();

            _onlinePsychGraphControl.ChartAreas.First(area => true).RecalculateAxesScale();

            _onlinePsychGraphControl.ChartAreas.First(area => true).AxisY.Maximum = 1.0;

            _onlinePsychGraphControl.ChartAreas.First(area => true).AxisX.Maximum = 100.0;

            _onlinePsychGraphControl.ChartAreas.First(area => true).AxisX.Minimum = -100.0;

            _onlinePsychGraphControl.Show();
        }

        /// <summary>
        /// Delegate setting the given point in the given series.
        /// </summary>
        /// <param name="seriesName">The series name to set it's point.</param>
        /// <param name="x">The x value of the point.</param>
        /// <param name="y">The y balue of the point.</param>
        /// <param name="newPoint">Indicated if it is a new point to add or existing point.</param>
        /// <param name="visible">Indicates if the point is visibbled on the graph.</param>
        public delegate void OnlinePsychoGraphSetPointDelegate(string seriesName, double x, double y, bool newPoint = false, bool visible = true);

        /// <summary>
        /// Setting the given point in the given series.
        /// </summary>
        /// <param name="seriesName">The series name to set the point to it.</param>
        /// <param name="x">The x value of the point.</param>
        /// <param name="y">The y value of the point.</param>
        /// <param name="newPoint">Indicates if the point is new to the chart or is an existing one.</param>
        /// <param name="visible"> Indicates if the point is visibled on th graph.</param>
        public void OnlinePsychoGraphSetPoint(string seriesName, double x, double y, bool newPoint = false, bool visible = true)
        {
            if (!(_onlinePsychGraphControl.Series.Count(series => series.Name == seriesName) > 0))
            {
                _onlinePsychGraphControl.Series.Add(seriesName);
                _onlinePsychGraphControl.Series[seriesName].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            }

            /*if (newPoint)
            {
                if (visible)
                {
                    _onlinePsychGraphControl.Series[seriesName].Points.AddXY(x, 0);
                    _onlinePsychGraphControl.Series[seriesName].Points.First(point => point.XValue == x).IsValueShownAsLabel = false;
                }
            }
            else*/
            {
                if (_onlinePsychGraphControl.Series[seriesName].Points.Count(point => point.XValue == x) > 0)
                    _onlinePsychGraphControl.Series[seriesName].Points.Remove(_onlinePsychGraphControl.Series[seriesName].Points.First(point => point.XValue == x));
                if (visible)
                {
                    _onlinePsychGraphControl.Series[seriesName].Points.AddXY(x, y);
                    _onlinePsychGraphControl.Series[seriesName].Points.First(point => point.XValue == x).IsValueShownAsLabel = true;
                    _onlinePsychGraphControl.Series[seriesName].Points.First(point => point.XValue == x).LabelFormat = "{0:0.00}";
                    //show the x axis value for all the points.
                    _onlinePsychGraphControl.ChartAreas[0].AxisX.IsInterlaced = true;
                    _onlinePsychGraphControl.ChartAreas[0].AxisX.IsLabelAutoFit = true;
                }
            }

            _onlinePsychGraphControl.ChartAreas.First(area => true).RecalculateAxesScale();
        }

        /// <summary>
        /// Delegate for setting serieses names to the chart.
        /// </summary>
        /// <param name="seriesNames">The series list to set to the graph.</param>
        public delegate void OnlinePsychoGraphSetSeriesDelegate(List<string> seriesNames);

        /// <summary>
        /// Setting the online psycho graph series by the given series names list.
        /// </summary>
        /// <param name="seriesNames">The series names list to set to the chart.</param>
        public void OnlinePsychoGraphSetSeries(List<string> seriesNames)
        {
            foreach (string seriesName in seriesNames)
            {
                _onlinePsychGraphControl.Series.Add(seriesName);
                _onlinePsychGraphControl.Series[seriesName].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            }
        }

        /// <summary>
        /// Delegate for the trial details ListView text changing.
        /// </summary>
        /// <param name="text">The name of the variable to be inserted.</param>
        /// <param name="value">The value of the parameter to  be inserted.</param>
        public delegate void ChangeCurrentTrialDetailsListViewText(string text, string value);

        /// <summary>
        /// Updates the trial details ListView with the given text.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="value">The value of the parameter to add.</param>
        private void ChangeCurrentTrialDetailsListView(string name, string value)
        {
            ListViewItem lvi = new ListViewItem(name);
            lvi.SubItems.Add(value);
            _trialDetailsListView.Items.Add(lvi);
        }

        /// <summary>
        /// Delegate for the trial details ListView text clearing.
        /// </summary>
        public delegate void ClearCurrentTrialDetailsListViewText();

        /// <summary>
        /// Clear the trial details ListView text.
        /// </summary>
        private void ClearCurrentTrialDetailsListView()
        {
            _trialDetailsListView.Items.Clear();
            _trialDetailsListView.Columns.Clear();
            _trialDetailsListView.Columns.Add("Name", "Name", 350);
            _trialDetailsListView.Columns.Add("Description", "Description", 100);
            _trialDetailsListView.View = View.Details;
        }

        /// <summary>
        /// Delegate for the trial details ListView text changing.
        /// </summary>
        /// <param name="name">The name of the variable to be inserted.</param>
        /// <param name="value">The value of the variable to be inserted.</param>
        public delegate void ChangeGlobalDetailsListViewText(string name, string value);

        /// <summary>
        /// Update the global experiment details ListView with that parameter.
        /// </summary>
        /// <param name="name">The parameter name to show.</param>
        /// <param name="value">The value of the parameter to show.</param>
        private void ChangeGlobalExperimentDetailsListView(string name, string value)
        {
            _logger.Info("Start updating details list view");

            ListViewItem lvi = new ListViewItem(name);
            lvi.SubItems.Add(value);
            _globaExperimentlInfoListView.Items.Add(lvi);

            _logger.Info("End updating details list view");
        }

        /// <summary>
        /// A delegate for clearing the global experiment details listview.
        /// </summary>
        public delegate void ClearGlobalDetailsListViewText();

        /// <summary>
        /// Clear the current global details listview.
        /// </summary>
        private void ClearGlobalExperimentDetailsListView()
        {
            _globaExperimentlInfoListView.Items.Clear();
            _globaExperimentlInfoListView.Columns.Clear();
            _globaExperimentlInfoListView.Columns.Add("Name", "Name", 190);
            _globaExperimentlInfoListView.Columns.Add("Description", "Description", 150);
            _globaExperimentlInfoListView.View = View.Details;
        }

        /// <summary>
        /// Delegate for event of finishing the experiment trials rounds.
        /// </summary>
        public delegate void FinishedAllTrialsInRoundDelegate();

        /// <summary>
        /// Handler for event of finishing the experiment trials rounds.
        /// </summary>
        public void FinishedAllTrialsRound()
        {
            _btnStop.Enabled = false;
            _btnStart.Enabled = false;
            /*_btnPause.Enabled = false;
            _btnResume.Enabled = false;*/
            _btnMakeTrials.Enabled = true;
            _btnEnagae.Enabled = false;
            _btnPark.Enabled = true;
        }

        /// <summary>
        /// Handler for rat direction response panel.
        /// </summary>
        /// <param name="data"></param>
        public delegate void SetNoldusRatResponseInteractivePanelCheckboxes(byte data);

        /// <summary>
        /// Handler for updating the interactive rat response direction panel.
        /// </summary>
        /// <param name="data"></param>
        private void SetNoldusRatResponseInteractivePanel(byte data)
        {
            //_leftNoldusCommunicationRadioButton.Checked = (data & 1) > 0;

            //_centerNoldusCommunicationRadioButton.Checked = (data & 2) > 0;

            //_rightNoldusCommunicationRadioButton.Checked = (data & 4) > 0;

            //_leftHandRewardCheckBox.Show();

            //_centerHandRewardCheckBox.Show();

            //_rightHandRewardCheckBox.Show();
        }

        /// <summary>
        /// Collect all needed controls and their delegates for the ControlLoop.
        /// </summary>
        /// <returns>
        /// The both dictionaries of the delegate and it's control.
        /// The first dictionary is for control object with it's name as key.
        /// The second dictionary is for delegate object with the same name as it's key.
        /// </returns>
        private Tuple<Dictionary<string, Control>, Dictionary<string, Delegate>> MakeCtrlDelegateAndFunctionDictionary()
        {
            //create the delegate dictionary include all delegates with their nick name to find as a key in the dictionary.
            Dictionary<string, Delegate> ctrlDelegatesDic = new Dictionary<string, Delegate>();
            Dictionary<string, Control> ctrlDictionary = new Dictionary<string, Control>();

            //add the delegate for updating the text for the current trial details ListView , also add the control of the ListView with the same key name.
            ChangeCurrentTrialDetailsListViewText changeCurrentTrialTextDelegate = new ChangeCurrentTrialDetailsListViewText(ChangeCurrentTrialDetailsListView);
            ctrlDelegatesDic.Add("UpdateCurrentTrialDetailsViewList", changeCurrentTrialTextDelegate);
            ctrlDictionary.Add("UpdateCurrentTrialDetailsViewList", _trialDetailsListView);

            //add the delegate for clearing the current trial details listview.
            ClearCurrentTrialDetailsListViewText clearCurrentTrialTextDelegate = new ClearCurrentTrialDetailsListViewText(ClearCurrentTrialDetailsListView);
            ctrlDelegatesDic.Add("ClearCurrentTrialDetailsViewList", clearCurrentTrialTextDelegate);
            ctrlDictionary.Add("ClearCurrentTrialDetailsViewList", _trialDetailsListView);

            //add the delegate for clearing the global details listview.
            ClearGlobalDetailsListViewText clearGlobalTextDelegate = new ClearGlobalDetailsListViewText(ClearGlobalExperimentDetailsListView);
            ctrlDelegatesDic.Add("ClearGlobaExperimentlDetailsViewList", clearGlobalTextDelegate);
            ctrlDictionary.Add("ClearGlobaExperimentlDetailsViewList", _globaExperimentlInfoListView);

            //add the delegate for updating the text for the current global experiment details ListView , also add the control of the ListView with the same key name.
            ChangeGlobalDetailsListViewText changeGlobalExperimentTextDelegate = new ChangeGlobalDetailsListViewText(ChangeGlobalExperimentDetailsListView);
            ctrlDelegatesDic.Add("UpdateGlobalExperimentDetailsListView", changeGlobalExperimentTextDelegate);
            ctrlDictionary.Add("UpdateGlobalExperimentDetailsListView", _globaExperimentlInfoListView);

            //add the delegates for the Noldus rat response interactive panel checkboxes.
            SetNoldusRatResponseInteractivePanelCheckboxes setNoldusRatResponseInteractivePanelCheckboxesDelegate = new SetNoldusRatResponseInteractivePanelCheckboxes(SetNoldusRatResponseInteractivePanel);
            ctrlDelegatesDic.Add("SetNoldusRatResponseInteractivePanel", setNoldusRatResponseInteractivePanelCheckboxesDelegate);
            //ctrlDictionary.Add("SetNoldusRatResponseInteractivePanel", _centerHandRewardCheckBox);

            //add the delegate for event indicates finshing all rounds in trial experiment.
            FinishedAllTrialsInRoundDelegate finishedAlltrialRoundDelegate = new FinishedAllTrialsInRoundDelegate(FinishedAllTrialsRound);
            ctrlDelegatesDic.Add("FinishedAllTrialsRound", finishedAlltrialRoundDelegate);
            ctrlDictionary.Add("FinishedAllTrialsRound", _btnStop);

            //add the delegate for events changing the online psycho graph for the experiment results.
            ctrlDictionary.Add("OnlinePsychoGraph", _onlinePsychGraphControl);

            OnlinePsychoGraphClearDelegate onlinePsychoGraphClearDelegate = new OnlinePsychoGraphClearDelegate(OnlinePsychoGraphClear);
            ctrlDelegatesDic.Add("OnlinePsychoGraphClear", onlinePsychoGraphClearDelegate);

            OnlinePsychoGraphSetPointDelegate onlinePsychoGraphSetPointDelegate = new OnlinePsychoGraphSetPointDelegate(OnlinePsychoGraphSetPoint);
            ctrlDelegatesDic.Add("OnlinePsychoGraphSetPoint", onlinePsychoGraphSetPointDelegate);

            OnlinePsychoGraphSetSeriesDelegate onlinePsychoGraphSetSeriesDelegate = new OnlinePsychoGraphSetSeriesDelegate(OnlinePsychoGraphSetSeries);
            ctrlDelegatesDic.Add("OnlinePsychoGraphSetSeries", onlinePsychoGraphSetSeriesDelegate);

            //add the delegate for clearing the selected rat name.
            //ResetSelectedRatNameComboboxDelegate resetSelectedRatNameComboboxDelegate = new ResetSelectedRatNameComboboxDelegate(ResetSelectedRatNameCombobox);
            //ctrlDelegatesDic.Add("ResetSelectedRatNameCombobox", resetSelectedRatNameComboboxDelegate);
            //ctrlDictionary.Add("ResetSelectedRatNameCombobox", _comboBoxSelectedRatName);

            //add the delegate for clearing the selected student name.
            //ResetSelectedStudentNameComboboxDelegate resetSelectedStudentNameComboboxDelegate = new ResetSelectedStudentNameComboboxDelegate(ResetSelectedStudentNameComboBox);
            //ctrlDelegatesDic.Add("ResetSelectedStudentNameCombobox", resetSelectedStudentNameComboboxDelegate);
            //ctrlDictionary.Add("ResetSelectedStudentNameCombobox", _comboBoxStudentName);

            //return both dictionaries.
            return new Tuple<Dictionary<string, Control>, Dictionary<string, Delegate>>(ctrlDictionary, ctrlDelegatesDic);
        }
        #endregion OUTSIDER_EVENTS_HANDLE_FUNCTION

        #region BIGGER_ONLINE_PSYCHO_GRAPH_EVENTS
        /// <summary>
        /// Event handler when double clicking on the graph in order to open it in a new big window.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The args.</param>
        private void _onlinePsychGraphControl_Click(object sender, EventArgs e)
        {
            //The new window form to show the bigger graph on.
            Form visualizationForm = new Form();

            //adding the psych graph on it.
            visualizationForm.Controls.Add(_onlinePsychGraphControl);

            //maximize the graph.
            visualizationForm.Size = new Size(1600, 1000);
            _onlinePsychGraphControl.Size = new Size(1500, 900);

            //deleting the event fromk the click event list in order to not make a recurssion when clicking again and again. 
            _onlinePsychGraphControl.DoubleClick -= _onlinePsychGraphControl_Click;

            //showing the new big form window.
            visualizationForm.Show();

            //make the foem unclosable until the main form is closed.
            visualizationForm.FormClosing += visualizationForm_FormClosing;
        }

        /// <summary>
        /// Event for closing the bigger onlinePsychoGrsph form.
        /// </summary>
        /// <param name="sender">The bigger graph form.</param>
        /// <param name="e">The args.</param>
        void visualizationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
            }
        }
        #endregion BIGGER_ONLINE_PSYCHO_GRAPH_EVENTS

        #region GLOBAL_EVENTS_HANDLE_FUNCTIONS
        /// <summary>
        /// Closing the guiInterface window event handler.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">args</param>
        private void GuiInterface_Close(object sender, EventArgs e)
        {
            _excelLoader.CloseExcelProtocolConfigFileLoader();

            //TODO: Do I need this?
            //turn off the robot servos.
            //avi-insert//
            //_motocomController.SetServoOff();
            MoogController.MoogController.Disconnect();

            //TODO: Do I need this?
            //close the connection with the led strip.
            //_ledController.CloseConnection();
            //_ledController2.CloseConnection();

            //stop the control loop.
            if (_cntrlLoop != null)
            {
                _cntrlLoop.Stop();
            }
        }
        #endregion GLOBAL_EVENTS_HANDLE_FUNCTIONS

        #region PROTOCOL_GROUPBOX_FUNCTION
        /// <summary>
        /// Event handler for clicking the protocol browser buttom.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">args.</param>
        private void protocolBrowserBtn_Click(object sender, EventArgs e)
        {
            if (_protocolsFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                _protocolsDirPath = _protocolsFolderBrowser.SelectedPath;
                _protocolsComboBox.Items.Clear();
                AddFilesToComboBox(_protocolsComboBox, _protocolsDirPath);
            }
        }

        /// <summary>
        /// Handler for selecting protocol in the protocols combo box.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">args/</param>
        private void _protocolsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //update the name of the selected protocol.
            _selectedProtocolFullName = _protocolsComboBox.SelectedItem.ToString();
            //delete the extension and the additional name from the full name to get the short name.
            _selectedProtocolName = _selectedProtocolFullName.Split('.')[0].Split('-')[0];

            //_protocolsComboBox.SelectedItem = _selectedProtocolFullName;
            //_protocolsComboBox.SelectedItem = _protocolsComboBox.Items[0];
            SetVariables(_protocolsDirPath + "\\" + _selectedProtocolFullName);
            ShowVariablesToGui();
        }

        /// <summary>
        /// Sets the default file protocol directory.
        /// </summary>
        public void SetDefaultProtocolFileBrowserDirectory()
        {
            _protocolsDirPath = this._protocolsFolderBrowser.SelectedPath;
            AddFilesToComboBox(_protocolsComboBox, _protocolsDirPath);
        }

        /// <summary>
        /// Add the files ends with .xlsx extension to the protocol ComboBox.
        /// </summary>
        private void AddFilesToComboBox(ComboBox comboBox, string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                string[] filesEntries = Directory.GetFiles(dirPath);

                foreach (string file in filesEntries)
                {
                    if (Path.GetExtension(file).Equals(".xlsx"))
                    {
                        _protocolsComboBox.Items.Add(Path.GetFileName(file));
                    }
                }

            }

            if (_protocolsComboBox.Items.Count > 0)
            {
                _protocolsComboBox.SelectedItem = _protocolsComboBox.Items[0];
                SetVariables(_protocolsDirPath + "\\" + _protocolsComboBox.Items[0].ToString());
                //that was deleted because it show the variables already in the two lines before.
                //ShowVariablesToGui();
            }
        }

        /// <summary>
        /// Sets the variables in the chosen xslx file and stote them in the class members.
        /// </summary>
        private void SetVariables(string dirPath)
        {
            _excelLoader.ReadProtocolFile(dirPath, ref _variablesList);
        }

        /// <summary>
        /// Save the variables in the gui to an excell sheet parameters file.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The args.</param>
        private void _btnSaveProtocol_Click(object sender, EventArgs e)
        {
            _excelLoader.WriteProtocolFile(_protocolsDirPath + @"\" + _textboxNewProtocolName.Text.ToString(), _variablesList, _buttonbasesDictionary);
        }
        #endregion PROTOCOL_GROUPBOX_FUNCTION
        

        #region EXPERIMENT_RUNNING_CHANGING_FUNCTION
        /// <summary>
        /// Function handler for starting the experiment tirlas. 
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">args.</param>
        private void _btnStart_Click(object sender, EventArgs e)
        {
            //if everything is ok -> start the control loop
            if (IsReadyToStart())
            {
                lock (_lockerStopStartButton)
                {
                    #region ENABLE_DISABLE_BUTTONS
                    _btnStart.Enabled = false;
                    _btnMakeTrials.Enabled = false;
                    _btnStop.Enabled = true;
                    _btnPark.Enabled = false;
                    #endregion


                    //if already running - ignore.
                    if (!Globals._systemState.Equals(SystemState.RUNNING))
                    {
                        //update the system state.
                        Globals._systemState = SystemState.RUNNING;

                        //add the static variable list of double type values.
                        _staticValuesGenerator.SetVariables(_variablesList);

                        ITrajectoryCreator trajectoryCreator = DecideTrajectoryCreatorByProtocolName(_selectedProtocolName);

                        _cntrlLoop.NumOfRepetitions = int.Parse(_numOfRepetitionsTextBox.Text);
                        _cntrlLoop.ProtocolFullName = _selectedProtocolFullName.Split('.')[0];//delete the.xlsx extension from the protocol file name.
                        _cntrlLoop.Start(_variablesList, _acrossVectorValuesGenerator._crossVaryingValsBoth, _staticValuesGenerator._staticVariableList, Properties.Settings.Default.Frequency, trajectoryCreator);
                    }
                }
            }
        }

        /// <summary>
        /// Check all parameters needed before the control loop execution.
        /// </summary>
        /// <returns>True or false if can execute the control loop.</returns>
        private bool IsReadyToStart()
        {
            if (!_isEngaged) {
                MessageBox.Show("Error - Not engaged", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Make trails for the selected varying variables.
        /// </summary>
        /// <param name="sender">The sender oButtom object.</param>
        /// <param name="e">Args.</param>
        private void _makeTrials_Click(object sender, EventArgs e)
        {
            //Check if both the num of staicases and withinstairs is 1 or 0.
            #region STATUS_NUM_OF_OCCURENCES
            int withinStairStstusOccurences = NumOfSpecificStatus("WithinStair");
            int acrossStairStatusOccurences = NumOfSpecificStatus("AcrossStair");
            if (withinStairStstusOccurences != acrossStairStatusOccurences || acrossStairStatusOccurences > 1)
            {
                MessageBox.Show("Cannot start the experiment.\n The number of Withinstairs should be the same as AccrossStairs and both not occurs more than 1!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //do nothing else.
                return;
            }
            #endregion STATUS_NUM_OF_OCCURENCES

            //decide which creator to use depends on the protocol name.
            _acrossVectorValuesGenerator = DecideVaryinVectorsGeneratorByProtocolName();

            //if there ar no errors middwile. Generate crrossvals and run the control loop.
            _acrossVectorValuesGenerator.SetVariablesValues(_variablesList);

            ClearVaryingListBox();

            //make the varyingCrossVals matrix.
            _acrossVectorValuesGenerator.MakeVaryingMatrix();

            //add the crossVaryingVals to the listbox.
            AddVaryingMatrixToVaryingListBox(_acrossVectorValuesGenerator._crossVaryingValsBoth);

            //show the list box controls(add , remove , etc...)
            ShowVaryingControlsOptions(show: true);

            //show the start button
            _btnStart.Enabled = true;
        }

        /// <summary>
        /// Handler for stop experiment button clicked.
        /// </summary>
        /// <param name="sender">The stop button object.</param>
        /// <param name="e">The args.</param>
        private void _btnStop_Click(object sender, EventArgs e)
        {
            //update the system state.
            //Globals._systemState = SystemState.STOPPED;
            lock (_lockerStopStartButton)
            {
                //stop the control loop.
                _cntrlLoop.Stop();

                #region ENABLE_DISABLE_BUTTONS
                _btnStop.Enabled = false;
                _btnStart.Enabled = true;
                /*_btnPause.Enabled = false;
                _btnResume.Enabled = false;*/
                _btnPark.Enabled = true;
                /*_btnMoveRobotSide.Enabled = true;*/
                #endregion
            }
        }

        /// <summary>
        /// Handler for pause experiment button clicked.
        /// </summary>
        /// <param name="sender">The pause button object.</param>
        /// <param name="e">The args.</param>
        private void _btnPause_Click(object sender, EventArgs e)
        {
            lock (_lockerPauseResumeButton)
            {
                #region ENABLE_DISABLE_BUTTONS
                /*_btnPause.Enabled = false;
                _btnResume.Enabled = true;*/
                _btnPark.Enabled = true;
                _btnEnagae.Enabled = false;
                #endregion

                Globals._systemState = SystemState.PAUSED;
                _cntrlLoop.Pause();
            }
        }

        /// <summary>
        /// Handler for resume experiment button clicked.
        /// </summary>
        /// <param name="sender">The pause button object.</param>
        /// <param name="e">The args.</param>
        private void _btnResume_Click(object sender, EventArgs e)
        {
            lock (_lockerPauseResumeButton)
            {
                #region ENABLE_DISABLE_BUTTONS
                /*_btnPause.Enabled = true;
                _btnResume.Enabled = false;*/
                _btnPark.Enabled = false;
                _btnEnagae.Enabled = false;
                _btnStop.Enabled = true;
                /*_btnMoveRobotSide.Enabled = false;*/
                #endregion

                Globals._systemState = SystemState.RUNNING;
                _cntrlLoop.Resume();
            }
        }

        /// <summary>
        /// Function handler for parking the robot.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Args.</param>
        private void _btnPark_Click(object sender, EventArgs e)
        {
            // WHAT THE **** IS IT
            lock (_lockerPauseResumeButton)
            {
                lock (_lockerPauseResumeButton)
                {

                    #region DISABLE_BUTTONS
                    bool isBtnStartEnabled = _btnStart.Enabled;
                    bool isBtnStopEnabled = _btnStop.Enabled;
                    /*bool isBtnPauseEnabled = _btnPause.Enabled;*/

                    _btnStart.Enabled = false;
                    _btnStop.Enabled = false;
                    /*_btnResume.Enabled = false;
                    _btnPause.Enabled = false;*/
                    _btnEnagae.Enabled = false;
                    _btnPark.Enabled = false;
                    /*_btnMoveRobotSide.Enabled = false;*/
                    #endregion

                    MoogController.MoogController.Disengage();

                    _isEngaged = false;

                    #region ENABLE_BUTTONS_BACK
                    _btnStart.Enabled = isBtnStartEnabled;
                    _btnStop.Enabled = isBtnStopEnabled;
                    /*_btnResume.Enabled = false;
                    _btnPause.Enabled = isBtnPauseEnabled;*/
                    _btnEnagae.Enabled = true;
                    _btnPark.Enabled = true;
                    /*_btnMoveRobotSide.Enabled = true;*/
                    #endregion
                }
            }

        }

        /// <summary>
        /// Handler for Engaging the robot to it's start point.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Args.</param>
        private void _btnEnagae_Click(object sender, EventArgs e)
        {
            lock (_lockerStopStartButton)
            {
                lock (_lockerPauseResumeButton)
                {
                    #region DISABLE_BUTTONS
                    bool isBtnStartEnabled = _btnStart.Enabled;
                    bool isBtnStopEnabled = _btnStop.Enabled;
                    /*bool isBtnPauseEnabled = _btnPause.Enabled;*/

                    _btnStart.Enabled = false;
                    _btnStop.Enabled = false;
                    /*_btnResume.Enabled = false;
                    _btnPause.Enabled = false;*/
                    _btnEnagae.Enabled = false;
                    _btnPark.Enabled = false;
                    /*_btnMoveRobotSide.Enabled = false;*/
                    #endregion

                    try
                    {
                        MoogController.MoogController.Engage();
                    }
                    catch
                    {
                        MessageBox.Show("Cannot set the servos on - check if robot is conncted in play mode and also not turned off", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    
                    _isEngaged = true;

                    #region ENABLE_BUTTONS_BACK
                    _btnStart.Enabled = isBtnStartEnabled;
                    _btnStop.Enabled = isBtnStopEnabled;
                    //if paused and then parked and engaged in the middle of the experiment.
                    /*if (_isEngaged && !_btnStop.Enabled && !_btnStart.Enabled) _btnResume.Enabled = true;
                    _btnPause.Enabled = isBtnPauseEnabled;*/
                    _btnEnagae.Enabled = true;
                    _btnPark.Enabled = true;
                    #endregion
                }
            }
        }
        #endregion

        #region VARYING_LISTBOX_FUNCTIONS
        /// <summary>
        /// Adding the generated cross varying values to the varying listbox.
        /// </summary>
        /// <param name="varyingCrossValsBoth">The cross generated varying values to add to the listbox.</param>
        private void AddVaryingMatrixToVaryingListBox(List<Dictionary<string, double>> varyingCrossValsBoth)
        {
            //collect the titles for the listbox columns to a list.
            string listBoxTitleLineText = "";
            List<string> niceNameList = new List<string>();
            foreach (string varName in varyingCrossValsBoth.ElementAt(0).Keys)
            {
                //TODO: Moog
                string varNiceName = _variablesList._variablesDictionary[varName]._description["nice_name"].MoogParameter;
                niceNameList.Add(varNiceName);
            }

            //add the titles for the listbox columns
            listBoxTitleLineText = string.Join("\t", niceNameList);
            _varyingListBox.Items.Add(listBoxTitleLineText);

            //enable horizonal scrolling.
            _varyingListBox.HorizontalScrollbar = true;

            //set the display member and value member for each item in the ListBox thta represents a Dictionary values in the varyingCrossVals list.
            //_varyingListBox.DisplayMember = "_text";
            //_varyingListBox.ValueMember = "_listIndex";



            //add all varying cross value in new line in the textbox.
            int index = 0;

            foreach (Dictionary<string, double> varRowDictionaryItem in varyingCrossValsBoth)
            {
                string listBoxLineText = string.Join("\t", varRowDictionaryItem.Values);

                //VaryingItem varyItem = new VaryingItem();
                //varyItem._text = listBoxLineText;
                //varyItem._listIndex = index;

                index++;
                _varyingListBox.Items.Add(listBoxLineText);
            }
        }

        /// <summary>
        /// Creates a string to string inner dictionary inside of string to list of doubles.
        /// </summary>
        /// <param name="MoogVaryingCrossVals">The crossVaryingValues of the both MoogParameter and landscapeHouseParameter.</param>
        /// <returns>
        /// A list of dictionaries.
        /// Each item in the dictionary describes a raw of trial for the varying.
        /// Each item is a dictionary of string to string.
        /// The first string (key) is for the variable name.
        /// The second string (value) is for the value of the both ratHouseValue and the landscapeHouseValue if enabled or only the first for the key variable string.
        /// </returns>
        private List<Dictionary<string, string>> CrossVaryingValuesToBothParameters(List<Dictionary<string, List<double>>> MoogVaryingCrossVals)
        {
            //The list to be returned.
            List<Dictionary<string, string>> crossVaryingBothParmeters = new List<Dictionary<string, string>>();

            //The string builder to build string with.
            StringBuilder sBuilder = new StringBuilder();

            //run over all lines in the crossVals for the ratHouseValues.
            foreach (Dictionary<string, List<double>> varRatHouseRowItem in MoogVaryingCrossVals)
            {
                //make a row to add to the returned list.
                Dictionary<string, string> varyingBothRowItem = new Dictionary<string, string>();

                //The string for a variable in the current row.
                string itemBothParameterString = "";

                //run over all the variables in the row.
                foreach (string varName in varRatHouseRowItem.Keys)
                {
                    //clear the string builder for the new variable in the current row.
                    sBuilder.Clear();

                    //check if the value for the variable in the current line is set to tbot the ratHouseValue and the lanscapeHouseValue.
                    if (varRatHouseRowItem[varName].Count() > 1)
                    {
                        itemBothParameterString = BracketsAppender(sBuilder, varRatHouseRowItem[varName].ElementAt(0).ToString(),
                            varRatHouseRowItem[varName].ElementAt(1).ToString());
                    }

                    else
                    {
                        itemBothParameterString = varRatHouseRowItem[varName].ElementAt(0).ToString();
                    }

                    //add the variable string description for the current line (of the both) for the dictionary describes that line.
                    varyingBothRowItem.Add(varName, itemBothParameterString);
                }

                //add the line (the description of varying trial) to the list of lines (trials).
                crossVaryingBothParmeters.Add(varyingBothRowItem);
            }

            return crossVaryingBothParmeters;
        }

        /// <summary>
        /// Clears the varying list box.
        /// </summary>
        private void ClearVaryingListBox()
        {
            _varyingListBox.Items.Clear();
        }

        /// <summary>
        /// Handler for adding combination to the varying cross values.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">args.</param>
        private void _addVaryingCombination_Click(object sender, EventArgs e)
        {
            //get the cross varying values list.
            List<Dictionary<string, double>> crossVaryingVals = _acrossVectorValuesGenerator._crossVaryingValsBoth;

            //make a dictionary with the variable name as key and the belong textbox as value.
            Dictionary<string, TextBox> varNameToTextboxDictionary = new Dictionary<string, TextBox>();

            //Showing the new little form to get the inputs for the new combination.
            ShowControlAddingVaryingForm(crossVaryingVals, varNameToTextboxDictionary);
        }

        /// <summary>
        /// Showing the new tittle form of the textboxes for the varyin variables to get the input from.
        /// </summary>
        /// <param name="crossVaryingVals">The crossVaryingVals list for all the trials.</param>
        /// <param name="varNameToTextboxDictionary">The dictionary map for the variable string (key) to the variable representing textbox(value).</param>
        /// <returns></returns>
        private Form ShowControlAddingVaryingForm(List<Dictionary<string, double>> crossVaryingVals, Dictionary<string, TextBox> varNameToTextboxDictionary)
        {
            //show thw new little form for the desired variables.
            Form littleTempForm = new Form();

            int leftOffset = 0;
            int topOffset = 0;
            int width = 140;
            int height = 14;
            //add an input textbox with label show the name for each varying parameter.
            foreach (string varName in crossVaryingVals.ElementAt(0).Keys)
            {
                //TODO: Moog
                string varNiceName = _variablesList._variablesDictionary[varName]._description["nice_name"].MoogParameter;

                Label varyingAttributeLabel = new Label();
                varyingAttributeLabel.Text = varName;
                varyingAttributeLabel.Top = topOffset += 35;
                varyingAttributeLabel.Left = leftOffset;
                varyingAttributeLabel.Height = height;
                varyingAttributeLabel.Width = width;
                littleTempForm.Controls.Add(varyingAttributeLabel);

                TextBox varyingAttributeTextBox = new TextBox();
                varyingAttributeTextBox.Top = topOffset;
                varyingAttributeTextBox.Left = leftOffset + width + 20;
                littleTempForm.Controls.Add(varyingAttributeTextBox);

                //add the variable name with the belonged textbox.
                varNameToTextboxDictionary.Add(varName, varyingAttributeTextBox);
            }

            //handle the confirm adding new combination.
            Button confirmCombinationAdding = new Button();
            littleTempForm.Controls.Add(confirmCombinationAdding);
            confirmCombinationAdding.Top = topOffset;
            confirmCombinationAdding.Left = leftOffset + width;
            confirmCombinationAdding.Click += new EventHandler((sender2, e2) => confirmVaryingCombinationAdding_Click(sender2, e2, varNameToTextboxDictionary));

            //show the dialog (need to be after adding controls) with the parent as the main windows frozed behind.
            littleTempForm.ShowDialog(this);

            //retutn the little form;
            return littleTempForm;
        }

        /// <summary>
        /// Handler for clicking on the confirm buttom for adding the combination added in the little window.
        /// </summary>
        /// <param name="sender">The buttom object.</param>
        /// <param name="e">args.</param>
        /// <param name="varNameToTextboxDictionary">The dictionary map for the variable string (key) to the variable representing textbox(value).</param>
        private void confirmVaryingCombinationAdding_Click(object sender, EventArgs e, Dictionary<string, TextBox> varNameToTextboxDictionary)
        {
            //creating dictionary for variable name as key and it's value as a value.
            Dictionary<string, string> varNameToValueDictionary = new Dictionary<string, string>();

            //converting the textboxes to strings value according to their text.
            foreach (string varName in varNameToTextboxDictionary.Keys)
            {
                varNameToValueDictionary.Add(varName, varNameToTextboxDictionary[varName].Text);
            }

            //confirm if the input is spelled well.
            if (!CheckVaryingListBoxProperInput(varNameToValueDictionary))
                return;

            //add the new combination after checking the spelling for the input.
            AddNewVaryngCombination(varNameToValueDictionary);

            Button clickedButtom = sender as Button;
            Form littleWindow = clickedButtom.Parent as Form;

            //close the adding combination little window.
            littleWindow.Close();
        }

        /// <summary>
        /// Adding new line (trial) of varying combination to the varyingCrossVals and to the listbox.
        /// </summary>
        /// <param name="varNameToValueDictionary">
        /// The item that describes the trial by a dictionary.
        /// The key os for the variable name.
        /// The value is for the value of that variable include ratHouseParameter
        /// </param>
        private void AddNewVaryngCombination(Dictionary<string, string> varNameToValueDictionary)
        {
            Dictionary<string, double> varNameToValueDictionaryDoubleListVersion = new Dictionary<string, double>();
            foreach (string varName in varNameToValueDictionary.Keys)
            {
                varNameToValueDictionaryDoubleListVersion.Add(varName, double.Parse(varNameToValueDictionary[varName]));
            }

            _acrossVectorValuesGenerator._crossVaryingValsBoth.Add(varNameToValueDictionaryDoubleListVersion);

            string listBoxLineText = string.Join("\t", varNameToValueDictionary.Values);
            _varyingListBox.Items.Add(listBoxLineText);
        }

        /// <summary>
        /// Handler for removing selected varting combination from varying cross values.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">args.</param>
        private void _removeVaryingCombination_Click(object sender, EventArgs e)
        {
            int selectedIndex = _varyingListBox.SelectedIndex;
            if (selectedIndex > 0)
            {
                //take the selected item.
                // TODO: solve the NullException
                VaryingItem selectedCombination = _varyingListBox.SelectedItem as VaryingItem;

                //set the _acrossVectorValuesGenerator cross varying values
                //_acrossVectorValuesGenerator._crossVaryingVals.RemoveAt(selectedCombination._listIndex);

                _acrossVectorValuesGenerator._crossVaryingValsBoth.RemoveAt(_varyingListBox.SelectedIndex - 1);

                //update also the gui varying listbox.
                //_varyingListBox.Items.Remove(selectedCombination);
                _varyingListBox.Items.RemoveAt(_varyingListBox.SelectedIndex);

                //ReduceIndexesFromNumberedIndex(selectedCombination._listIndex, _varyingListBox);

                //if (selectedCombination._listIndex > 0)
                _varyingListBox.SelectedItem = (_varyingListBox.Items[selectedCombination._listIndex - 1] as VaryingItem);

                if (selectedIndex > 1)
                {
                    _varyingListBox.SelectedIndex = selectedIndex - 1;
                }
            }
        }

        /// <summary>
        /// Reduces the indexes of the value of the VaryingItem in the varyingListBox to be matched with the _crossVaryingVals list.
        /// </summary>
        /// <param name="beginIndex">The index to begin reducing it's value index.</param>
        /// <param name="varyingListBox">Yhe varying listbox to reduce it's indexes.</param>
        private void ReduceIndexesFromNumberedIndex(int beginIndex, ListBox varyingListBox)
        {
            for (int index = beginIndex; index < varyingListBox.Items.Count; index++)
            {
                VaryingItem varyItem = varyingListBox.Items[index] as VaryingItem;
                varyItem._listIndex--;
            }
        }

        /// <summary>
        /// Detemines ig the varying control items are visible according to the input. show.
        /// </summary>
        /// <param name="show">If to show the varying control items.</param>
        private void ShowVaryingControlsOptions(bool show)
        {
            this._varyingListBox.Visible = show;
            /*this._addVaryingCobination.Visible = show;
            this._removeVaryingCombination.Visible = show;*/
        }

        /// <summary>
        /// Function handler for changing the variable from the Gui according to the textboxes input when leaving the textbox.
        /// </summary>
        /// <param name="sender">The textbox sender object have been changed.</param>
        /// <param name="e">args.</param>
        /// <param name="varName">The variable name in the variables dictionary to update according to the textbox.</param>
        private void VariableTextBox_TextBoxLeaved(object sender, EventArgs e, string varName, string varAttibuteName)
        {
            TextBox tb = sender as TextBox;

            CheckProperInputSpelling(tb.Text, varName, varAttibuteName);

            //if a left textbox updated and need to equalize the right checkbox also.
            UpdateRightTextBoxesAvailability(_checkBoxRightAndLeftSame.Checked);
        }

        /// <summary>
        /// Function handler for status variable combobox changed.
        /// </summary>
        /// <param name="sender">The combobox object that was changed.</param>
        /// <param name="e">The args.</param>
        /// <param name="varName">The var name it's combobox changed.</param>
        private void statusCombo_SelectedIndexChanged(object sender, EventArgs e, string varName)
        {
            ComboBox cb = sender as ComboBox;
            string selectedIndex = "";

            //decide which index in the status list was selected.
            selectedIndex = StatusIndexByNameDecoder(cb.SelectedItem.ToString());

            //update the status in the variables dictionary.
            //TODO: Moog
            _variablesList._variablesDictionary[varName]._description["status"].MoogParameter = selectedIndex;

            //Check if both he num of staicases and withinstairs is 1 or 0.
            #region STATUS_NUM_OF_OCCURENCES
            int withinStairStstusOccurences = NumOfSpecificStatus("WithinStair");
            int acrossStairStatusOccurences = NumOfSpecificStatus("AcrossStair");
            if (withinStairStstusOccurences != acrossStairStatusOccurences || acrossStairStatusOccurences > 1)
            {
                MessageBox.Show("The number of Withinstairs is the same as AccrossStairs and both not occurs more than 1!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            #endregion STATUS_NUM_OF_OCCURENCES

            #region TEXTBOXES_FREEZING_NEW_STATUS
            //update the gui textboxes freezing according to the new status.
            foreach (string attribute in _variablesList._variablesDictionary[varName]._description.Keys)
            {
                if (!attribute.Equals("status"))
                {
                    if (_dynamicAllocatedTextBoxes.ContainsKey(varName + attribute))
                        FreezeTextBoxAccordingToStatus((TextBox)_dynamicAllocatedTextBoxes[varName + attribute], varName, attribute.Equals("parameters"));
                }
            }
            #endregion TEXTBOXES_FREEZING_NEW_STATUS

            //change the parametes attribute textbox for the changed status variable.
            #region PRAMETERS_TEXTBOX_CHANGE_TEXT_SHOW
            SetParametersTextBox(varName, new StringBuilder());
            #endregion PRAMETERS_TEXTBOX_CHANGE_TEXT_SHOW
        }

        /// <summary>
        /// Handle event if the number of repetitions for the experiments changed dynamically.
        /// </summary>
        /// <param name="sender">The textbox sendr.</param>
        /// <param name="e">The args</param>
        private void _numOfRepetitionsTextBox_TextChanged(object sender, EventArgs e)
        {
            _cntrlLoop.NumOfRepetitions = int.Parse((sender as TextBox).Text.ToString());
        }

        /// <summary>
        /// Handler for changing the number of repetition input text.
        /// </summary>
        /// <param name="sender">The checkbox control.</param>
        /// <param name="e">Args.</param>
        private void _numOfRepetitionsTextBox_Leave(object sender, EventArgs e)
        {
            //check if represents an integer number.
            if (!IntegerChecker(_numOfRepetitionsTextBox.Text.ToString()))
            {
                MessageBox.Show("Not an integer number entered - returnnig to 1 as default", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //put the default number.
                _numOfRepetitionsTextBox.Text = "1";
            }
        }
        #endregion VARYING_LISTBOX_FUNCTIONS

        #region AUTOS
        /*/// <summary>
        /// Event for changing the AutoStart status.
        /// </summary>
        /// <param name="sender">The checkbox.</param>
        /// <param name="e">The param.</param>
        private void _autoStartcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //todo::we need to activate it again.
            /*if (_checkBoxAutoStart.Checked)
                _cntrlLoop.AutoStart = true;
            else#1#
                _cntrlLoop.AutoStart = false;
        }*/
        #endregion AUTOS

        #region MODES

        /// <summary>
        /// Handler for event turnning on/off the SoundOn for error choice.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Args.</param>
        private void _checkboxErrorSoundOn_CheckedChanged(object sender, EventArgs e)
        {
            _cntrlLoop.EnableErrorSound = (sender as CheckBox).Checked;
        }

        /// <summary>
        /// Handler for event turnning on/off the right + left parameters must be the same.
        /// </summary>
        /// <param name="sender">The sender CheckBox.</param>
        /// <param name="e">The args.</param>
        private void _checkBoxRightAndLeftSame_CheckedChanged(object sender, EventArgs e)
        {
            _cntrlLoop.EnableRightLeftMustEquals = (sender as CheckBox).Checked;

            //make the textboxes for all pairs of right and left to be the same (disable the right textboxes and make thier values the same as the left textboxes).
            UpdateRightTextBoxesAvailability((sender as CheckBox).Checked);
        }

        /// <summary>
        /// Update and disabled all Right textboxes according to the equalization of the right textboxes and the left checkboxes.
        /// </summary>
        /// <param name="equals">If right checkboxes should equal the left checkboxes.</param>
        public void UpdateRightTextBoxesAvailability(bool equals)
        {
            UpdateRightCheckBoxAvailability("REWARD_RIGHT_DELAY", "REWARD_LEFT_DELAY", equals);
            UpdateRightCheckBoxAvailability("REWARD_RIGHT_DURATION", "REWARD_LEFT_DURATION", equals);
            UpdateRightCheckBoxAvailability("REWARD_RIGHT_DELAY_SC", "REWARD_LEFT_DELAY_SC", equals);
            UpdateRightCheckBoxAvailability("REWARD_RIGHT_DURATION_SC", "REWARD_LEFT_DURATION_SC", equals);
            UpdateRightCheckBoxAvailability("FLICKER_RIGHT", "FLICKER_LEFT", equals);
        }

        /// <summary>
        /// Update a right textbox with the given name to be equals and disabled according to equals parameter.
        /// </summary>
        /// <param name="checkboxRightName">The right textbox to be disabled and equaled to the left textbox.</param>
        /// <param name="cehckBoxLeftName">The left textbox to be equals to.</param>
        /// <param name="equals">Indicate if equals and disable or to enable.</param>
        public void UpdateRightCheckBoxAvailability(string checkboxRightName, string cehckBoxLeftName, bool equals)
        {
            if (_dynamicAllocatedTextBoxes.Keys.Contains(checkboxRightName + "parameters"))
            {
                if (equals)
                {
                    _dynamicAllocatedTextBoxes[checkboxRightName + "parameters"].Enabled = false;
                    _dynamicAllocatedTextBoxes[checkboxRightName + "parameters"].Text = _dynamicAllocatedTextBoxes[cehckBoxLeftName + "parameters"].Text;
                    //TODO: Moog
                    _variablesList._variablesDictionary[checkboxRightName]._description["parameters"].MoogParameter = _variablesList._variablesDictionary[cehckBoxLeftName]._description["parameters"].MoogParameter;
                }
                else
                {
                    _dynamicAllocatedTextBoxes[checkboxRightName + "parameters"].Enabled = true;
                }
            }
        }

        #endregion MODES

        #region PARAMETERS_GROUPBOXFUNCTIONS
        /// <summary>
        /// Showing the variables from the readen excel file to the Gui with option to change them.
        /// </summary>
        private void ShowVariablesToGui()
        {
            int top = 100;
            int left = 10;
            int width = 130;
            int height = 14;
            int eachDistance = 150;

            //clear dynamic textboxes and labels from the last uploaded protocol before creating the new dynamic controls..
            ClearDynamicControls();

            //add the labels title for each column.
            AddVariablesLabelsTitles(ref top, left, width, height, eachDistance);

            //filter only the variables where the status is not -1 (not for the checkboxes for the gui).
            //TODO: Moog
            foreach (string varName in _variablesList._variablesDictionary.Keys.Where(name => int.Parse(_variablesList._variablesDictionary[name]._description["status"].MoogParameter) != -1))
            {
                ShowVariableLabel(_variablesList._variablesDictionary[varName]._description["nice_name"].MoogParameter,
                    top,
                    left,
                    width + 40,
                    height,
                    eachDistance,
                    _variablesList._variablesDictionary[varName]._description["tool_tip"].MoogParameter);

                ShowVariableAttributes(varName,
                    top,
                    left,
                    width,
                    height,
                    eachDistance,
                    750,
                    _variablesList._variablesDictionary[varName]._description["color"].MoogParameter);

                top += 35;
            }

            //reset checkboxes statuses before matching them to the protocol file.
            foreach (ButtonBase item in _buttonbasesDictionary.Values)
            {
                if (item is CheckBox)
                {
                    (item as CheckBox).Checked = false;
                }
                else if (item is RadioButton)
                {
                    (item as RadioButton).Checked = false;
                    (item as RadioButton).Enabled = false;
                }

                //todo: add exception if not of these types/
            }

            //filter only the variables where the status is  -1 (for the checkboxes for the gui).
            //TODO: Moog
            IEnumerable<string> variablesList = _variablesList._variablesDictionary.Keys.Where(name => int.Parse(_variablesList._variablesDictionary[name]._description["status"].MoogParameter) == -1);
            /*
            foreach (string varName in variablesList)
            {
                if (_buttonbasesDictionary[varName] is RadioButton)
                {
                    (_buttonbasesDictionary[varName] as RadioButton).Checked = false;

                    if (int.Parse(_variablesList._variablesDictionary[varName]._description["parameters"]
                            .MoogParameter) == 1)
                    {
                        (_buttonbasesDictionary[varName] as RadioButton).Checked = true;
                    }
                }
                else if (_buttonbasesDictionary[varName] is CheckBox)
                {
                    (_buttonbasesDictionary[varName] as CheckBox).Checked = false;

                    if (int.Parse(_variablesList._variablesDictionary[varName]._description["parameters"]
                            .MoogParameter) == 1)
                    {
                        (_buttonbasesDictionary[varName] as CheckBox).Checked = true;
                    }
                }
            }
            */

            //update if right equals to left according to the checkbox status.
            UpdateRightTextBoxesAvailability(_checkBoxRightAndLeftSame.Checked);
        }

        /// <summary>
        /// Show the label of the variable name in the gui variable list.
        /// </summary>
        /// <param name="varName">The variable name to show.</param>
        /// <param name="top">The top place offset.</param>
        /// <param name="left">The left place offset.</param>
        /// <param name="width">The width of the label.</param>
        /// <param name="height">The height of the label.</param>
        /// <param name="eachDistance">The distance between each label.</param>
        /// <param name="toolTipString">The tooltipper string to add to the label.</param>
        public void ShowVariableLabel(string varName, int top, int left, int width, int height, int eachDistance, string toolTipString = "")
        {
            //create the new label to show on the gui.
            Label newLabel = new Label();

            //add the label on thr gui.
            _dynamicParametersPanel.Controls.Add(newLabel);
            newLabel.Name = varName;
            newLabel.Text = varName;
            newLabel.Width = width - 35;
            newLabel.Height = height;
            newLabel.Top = top;
            newLabel.Left = left;

            //add the tooltip help for the label.
            _guiInterfaceToolTip.SetToolTip(newLabel, toolTipString);

            //also , add the label to the dynamic control list.
            _dynamicAllocatedTextBoxes.Add(newLabel.Text.ToString() + "Label", newLabel);
        }

        /// <summary>
        /// Showing a single variable with all it's attributes.
        /// </summary>
        /// <param name="varName">The variable name.</param>
        /// <param name="top">The offset from the top of the window.</param>
        /// <param name="left">The offset from the left of the window for the DropDownList of each variable.</param>
        /// <param name="width">The width for each textbox in the line of the attribute.</param>
        /// <param name="height">The height for each textbox in the line of the attribute.</param>
        /// <param name="eachDistance">The distance between each textbox of the same attribute in the same line.</param>
        /// <param name="offset">The offset for each textbox of the attribute from the left.</param>
        /// <param name="color">The color to be in the background of wach textbox.</param>
        private void ShowVariableAttributes(string varName, int top, int left, int width, int height, int eachDistance, int offset, string color)
        {
            //string builder for making the text to show to the gui.
            StringBuilder sBuilder = new StringBuilder();

            #region STATUS_COMBOBOX
            //add the status ComboBox.
            ComboBox statusCombo = new ComboBox();
            statusCombo.Left = left + offset;
            statusCombo.Top = top;
            statusCombo.Items.Add("Const");
            statusCombo.Items.Add("Static");
            statusCombo.Items.Add("Varying");
            statusCombo.Items.Add("AcrossStair");
            statusCombo.Items.Add("WithinStair");
            statusCombo.Items.Add("Random");
            statusCombo.Items.Add("Vector");

            //Handle event when a status of a variable is changed.
            statusCombo.SelectedIndexChanged += new EventHandler((sender, args) => statusCombo_SelectedIndexChanged(sender, args, varName));

            //decide which items on the ComboBox is selected according to the data in the excel sheet.
            //TODO: Moog
            switch (_variablesList._variablesDictionary[varName]._description["status"].MoogParameter)
            {
                case "0":
                    statusCombo.SelectedText = "Const";
                    break;

                case "1":
                    statusCombo.SelectedText = "Static";
                    break;

                case "2":
                    statusCombo.SelectedText = "Varying";
                    break;

                case "3":
                    statusCombo.SelectedText = "AcrossStair";
                    break;

                case "4":
                    statusCombo.SelectedText = "WithinStair";
                    break;

                case "5":
                    statusCombo.SelectedText = "Random";
                    break;
                case "6":
                    statusCombo.SelectedText = "Vector";
                    break;
            }

            //add the status ComboBox to the gui.
            _dynamicParametersPanel.Controls.Add(statusCombo);
            _dynamicAllocatedTextBoxes.Add(varName + "status", statusCombo);
            #endregion STATUS_COMBOBOX

            offset -= eachDistance;

            #region INCREMENT_TEXTBOX
            //add the low bound textbox.
            TextBox incrementTextBox = new TextBox();
            incrementTextBox.Left = offset;
            incrementTextBox.Top = top;
            incrementTextBox.Width = width;
            //add the color to the textbox
            incrementTextBox.BackColor = Color.FromName(color);

            //function to change the variable list dictionary according to changes when leave the textbox.
            incrementTextBox.LostFocus += new EventHandler((sender, e) => VariableTextBox_TextBoxLeaved(sender, e, varName, "increament"));

            //freezing the textbox according to the status
            FreezeTextBoxAccordingToStatus(incrementTextBox, varName, false);

            //show the _MoogParameter.
            //TODO: Moog
            string lowBoundTextVal = string.Join(",", _variablesList._variablesDictionary[varName]._description["increament"].MoogParameter);
            incrementTextBox.Text = lowBoundTextVal;

            _dynamicParametersPanel.Controls.Add(incrementTextBox);
            _dynamicAllocatedTextBoxes.Add(varName + "increament", incrementTextBox);
            #endregion INCREMENT_TEXTBOX

            offset -= eachDistance;

            #region HIGHBOUND_TEXTBOX
            //add the low bound textbox.
            TextBox highBoundTextBox = new TextBox();
            highBoundTextBox.Left = offset;
            highBoundTextBox.Top = top;
            highBoundTextBox.Width = width;
            //add the colr to the textbox
            highBoundTextBox.BackColor = Color.FromName(color);

            //function to change the variable list dictionary according to changes when leave the textbox.
            highBoundTextBox.LostFocus += new EventHandler((sender, e) => VariableTextBox_TextBoxLeaved(sender, e, varName, "high_bound"));

            //freezing the textbox according to the status
            FreezeTextBoxAccordingToStatus(highBoundTextBox, varName, false);

            //show the _MoogParameter.
            //TODO: Moog
            string highBoundTextVal = string.Join(",", _variablesList._variablesDictionary[varName]._description["high_bound"].MoogParameter);
            highBoundTextBox.Text = highBoundTextVal;

            _dynamicParametersPanel.Controls.Add(highBoundTextBox);
            this._dynamicAllocatedTextBoxes.Add(varName + "high_bound", highBoundTextBox);
            #endregion HIGHBOUND_TEXTBOX

            offset -= eachDistance;

            #region LOWBOUND_TEXTBOX
            //add the low bound textbox.
            TextBox lowBoundTextBox = new TextBox();
            lowBoundTextBox.Left = offset;
            lowBoundTextBox.Top = top;
            lowBoundTextBox.Width = width;
            //add the color to the textbox
            lowBoundTextBox.BackColor = Color.FromName(color);

            //function to change the variable list dictionary according to changes when leave the textbox.
            lowBoundTextBox.LostFocus += new EventHandler((sender, e) => VariableTextBox_TextBoxLeaved(sender, e, varName, "low_bound"));

            //freezing the textbox according to the status
            FreezeTextBoxAccordingToStatus(lowBoundTextBox, varName, false);

            //show the _MoogParameter.
            //TODO: Moog
            lowBoundTextVal = string.Join(",", _variablesList._variablesDictionary[varName]._description["low_bound"].MoogParameter);
            lowBoundTextBox.Text = lowBoundTextVal;

            _dynamicParametersPanel.Controls.Add(lowBoundTextBox);
            _dynamicAllocatedTextBoxes.Add(varName + "low_bound", lowBoundTextBox);
            #endregion LOWBOUND_TEXTBOX

            offset -= eachDistance;

            #region PARMETERS_VALUE_TEXTBOX
            //add the low bound textbox.
            TextBox parametersTextBox = new TextBox();
            parametersTextBox.Left = offset;
            parametersTextBox.Top = top;
            parametersTextBox.Width = width;
            //add name to the control in order to get it from the list if needed.
            parametersTextBox.Name = "parameters";


            //print the parameter in the gui according to the representation of each status.
            //TODO: Moog
            switch (_variablesList._variablesDictionary[varName]._description["status"].MoogParameter)
            {
                case "0":   //const
                case "1":   //static
                case "6":   //vector
                    //show the _MoogParameter.
                    parametersTextBox.Text = string.Join(",", _variablesList._variablesDictionary[varName]._description["parameters"].MoogParameter); ;
                    break;

                case "2":   //varying
                case "3":   //acrossstair
                case "4":   //withinstair
                case "5":
                    //show the _MoogParameter.
                    string lowboundTextVal = string.Join(",", _variablesList._variablesDictionary[varName]._description["low_bound"].MoogParameter);
                    string highboundTextVal = string.Join(",", _variablesList._variablesDictionary[varName]._description["high_bound"].MoogParameter);
                    string increasingTextVal = string.Join(",", _variablesList._variablesDictionary[varName]._description["increament"].MoogParameter);

                    parametersTextBox.Text = ThreeStagesRepresentation(sBuilder, lowboundTextVal, increasingTextVal, highboundTextVal);
                    break;

            }

            //add the color to the texctbox
            parametersTextBox.BackColor = Color.FromName(color);

            _dynamicParametersPanel.Controls.Add(parametersTextBox);
            _dynamicAllocatedTextBoxes.Add(varName + "parameters", parametersTextBox);

            //freezing the textbox according to the status
            FreezeTextBoxAccordingToStatus(parametersTextBox, varName, true);

            //function to change the variable list dictionary according to changes when leave the textbox.
            parametersTextBox.LostFocus += new EventHandler((sender, e) => VariableTextBox_TextBoxLeaved(sender, e, varName, "parameters"));

            #endregion PARMETERS_VALUE_TEXTBOX

            offset -= eachDistance;
        }

        /// <summary>
        /// Clears the dynamic allocated textboxes and labels due to the previous loaded protocol..
        /// </summary>
        private void ClearDynamicControls()
        {
            foreach (Control ctrl in _dynamicAllocatedTextBoxes.Values)
            {
                _dynamicParametersPanel.Controls.Remove(ctrl);
            }

            _dynamicAllocatedTextBoxes.Clear();
        }

        /// <summary>
        /// Initializes the checkboxes dictionary with names as key with the control as value.
        /// </summary>
        private void InitializeCheckBoxesDictionary()
        {
            _buttonbasesDictionary = new Dictionary<string, ButtonBase>();

            _buttonbasesDictionary.Add("ERROR_SOUND", _checkboxErrorSoundOn);
            _buttonbasesDictionary.Add("FIXATION_ONLY", _checkBoxFixationOnly);
            _buttonbasesDictionary.Add("RIGHT_LEFT_PARAMETERS_EQUALS", _checkBoxRightAndLeftSame);
        }

        /// <summary>
        /// Initialize the title labels with their text and add them to the controls.
        /// </summary>
        private void InitializeTitleLabels()
        {
            //Make all the titles labels.
            _titlesLabelsList = new List<Label>();
            Label parametersLabel = new Label();
            Label lowBoundLabel = new Label();
            Label highBoundLabel = new Label();
            Label incrementLabel = new Label();
            Label statusLabel = new Label();

            //Add their text attribute.
            parametersLabel.Text = "Parameter";
            lowBoundLabel.Text = "Low Bound";
            highBoundLabel.Text = "High Bound";
            incrementLabel.Text = "Increment";
            statusLabel.Text = "Status";

            //Add them to the list of titles labels.
            _titlesLabelsList.Add(parametersLabel);
            _titlesLabelsList.Add(lowBoundLabel);
            _titlesLabelsList.Add(highBoundLabel);
            _titlesLabelsList.Add(incrementLabel);
            _titlesLabelsList.Add(statusLabel);
        }

        /// <summary>
        /// Adding titles lables for the attributes of the parameters.
        /// </summary>
        /// <param name="top">The offset from the top of the window.</param>
        /// <param name="left">The offset from the left of the window for the DropDownList of each variable.</param>
        /// <param name="width">The width for each label in the line of the titles.</param>
        /// <param name="height">The height for each label in the line of the titles.</param>
        /// <param name="eachDistance">The distance between each label in the titles line.</param>
        private void AddVariablesLabelsTitles(ref int top, int left, int width, int height, int eachDistance)
        {
            //add offset for the first.
            left += eachDistance;

            //Place each title label control.
            foreach (Label lbl in _titlesLabelsList)
            {
                lbl.Top = top;
                lbl.Left = left;
                lbl.Width = width;
                lbl.Height = height;
                left += eachDistance;

                //add the label to the gui.
                _dynamicParametersPanel.Controls.Add(lbl);
            }

            //increase the top for the reference.
            top += 35;
        }

        /// <summary>
        /// Add manipulation to the partA and partB for showing them in the gui as in the excel file.
        /// </summary>
        /// <param name="sBuilder">The stream builder to buld the string with.</param>
        /// <param name="partA">The first part to put to the string.</param>
        /// <param name="partB">The second part to put to the string.</param>
        /// <returns>[partA][partB]</returns>
        private string BracketsAppender(StringBuilder sBuilder, string partA, string partB)
        {
            sBuilder.Clear();
            sBuilder.Append("[");
            sBuilder.Append(partA);
            sBuilder.Append("][");
            sBuilder.Append(partB);
            sBuilder.Append("]");

            return sBuilder.ToString();
        }

        /*/// <summary>
        /// Split [x][y]... to list of {x, y, ...}
        /// </summary>
        /// <param name="value">The string to be splitted.</param>
        /// <returns>The list of splitted strings.</returns>
        private List<string> BracketsSplitter(string value)
        {
            //split for each element as the brackets should.
            List<string> splittedList = value.Split('[').ToList();

            //The first splitted value is "" , so drop it.
            splittedList.RemoveAt(0);

            //drop the ']' brackets which appears in the end.
            for (int i = 0; i < splittedList.Count; i++)
            {
                splittedList[i] = splittedList.ElementAt(i).Substring(0, splittedList.ElementAt(i).Length - 1);
            }

            //return the splitted list.
            return splittedList;
        }*/

        /*/// <summary>
        /// Converts a list of strings to a list of doubles.
        /// </summary>
        /// <param name="strList">The string list.</param>
        /// <returns>The converted double list.</returns>
        private List<double> ConvertStringListToDoubleList(List<string> strList)
        {
            //The double list to be returned.
            List<double> doubleList = new List<double>();

            //converts each element in the string list to double and insert to the double list.
            foreach (string stringVal in strList)
            {
                doubleList.Add(int.Parse(stringVal));
            }

            //return the converted list.
            return doubleList;
        }*/

        /// <summary>
        /// Creates a string for the gui with 3 steps.
        /// </summary>
        /// <param name="sBuilder">The string writer.</param>
        /// <param name="lowPart">The low bound.</param>
        /// <param name="increasingPart">Increment.</param>
        /// <param name="highPart">The high bound.</param>
        /// <returns>The string to the gui by [low:inc:high].</returns>
        private string ThreeStagesRepresentation(StringBuilder sBuilder, string lowPart, string increasingPart, string highPart)
        {
            sBuilder.Clear();
            sBuilder.Append(lowPart);
            sBuilder.Append(":");
            sBuilder.Append(increasingPart);
            sBuilder.Append(":");
            sBuilder.Append(highPart);
            return sBuilder.ToString();
        }

        /// <summary>
        /// Freezing a textbox according to it's status.
        /// </summary>
        /// <param name="textBox">The textbox to freeze or not.</param>
        /// <param name="varName">The variable name for chaecing the status for the textbox. </param>
        /// <param name="parametersTextbox">Is the textbox describe a parameters attribute textbox or vector attribute or const attribute values. </param>
        private void FreezeTextBoxAccordingToStatus(TextBox textBox, string varName, bool parametersTextbox)
        {
            //if const disabled textbox and break no matter what.
            //TODO: Moog
            if (_variablesList._variablesDictionary[varName]._description["status"].MoogParameter == "0")
            {
                textBox.Enabled = false;
                return;
            }

            //decide which items on the ComboBox is selected according to the data in the excel sheet.
            //TODO: Moog
            switch (_variablesList._variablesDictionary[varName]._description["status"].MoogParameter)
            {
                case "1":
                case "6":
                    textBox.Enabled = false;
                    break;

                case "2":
                case "3":
                case "4":
                    textBox.Enabled = true;
                    break;
            }

            //reverse the result.
            if (parametersTextbox)
            {
                textBox.Enabled = !textBox.Enabled;
            }
        }

        /// <summary>
        /// Checks the input spelling to be proper and update the dictionary according to that.
        /// </summary>
        /// <param name="attributeValue">The attribute value to check.</param>
        /// <param name="varName">The var name to attributed updated according to the new value if the input is proper.</param>
        /// <param name="attributeName">The attribute of the variable to be pdated if the input was proper.</param>
        /// <returns></returns>
        private Param CheckProperInputSpelling(string attributeValue, string varName, string attributeName)
        {
            Param par = new Param();

            //if one attribute only (can be a scalar either a vector).

            //split each vector of data for robot to a list of components.
            par.MoogParameter = attributeValue;

            //if the input can be only a scalar
            //TODO: Moog
            if (_variablesList._variablesDictionary[varName]._description["status"].MoogParameter != "6")
            {
                //if the input for one value contains more than one dot for precison dot or chars that are not digits.
                //if true , update the values in the variables dictionary.
                if (DigitsNumberChecker(par.MoogParameter))
                {
                    _variablesList._variablesDictionary[varName]._description[attributeName] = par;

                    SetParametersTextBox(varName, new StringBuilder());
                }

                //show the previous text to the changed textbox (taken from the variable list dictionary).
                else
                {
                    //refresh according to the last.
                    ShowVariablesToGui();
                }
            }
            //if the input can be a scalar either a vector.
            else
            {
                if (VectorNumberChecker(par.MoogParameter))
                {
                    _variablesList._variablesDictionary[varName]._description[attributeName] = par;

                    SetParametersTextBox(varName, new StringBuilder());
                }
                else
                {
                    //refresh according to the last.
                    ShowVariablesToGui();
                }
            }

            return par;
        }

        /// <summary>
        /// Checking ig the input for the new adding varying vector is proper.
        /// </summary>
        /// <param name="toCheckVector">The list of variables with values to be checked.</param>
        /// <returns>True or false if the input is propper.</returns>
        private bool CheckVaryingListBoxProperInput(Dictionary<string, string> toCheckVector)
        {
            //check if each attribute is according to both parameters if needed and the brackets also.
            //TODO: Moog
            foreach (string varName in toCheckVector.Keys)
            {
                int firstLeftBracketIndex = toCheckVector[varName].IndexOf('[');
                int firstRightBracketIndex = toCheckVector[varName].IndexOf(']');
                string varNiceName = _variablesList._variablesDictionary[varName]._description["nice_name"].MoogParameter;

                //if there are brackets for a scalar.
                if (firstRightBracketIndex > -1 || firstLeftBracketIndex > -1)
                {
                    MessageBox.Show("Error : variable " + varNiceName + " has brackets but is a scalar!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                //if not represent a number.
                if (!DigitsNumberChecker(toCheckVector[varName]))
                {
                    MessageBox.Show("Error : variable " + varNiceName + " has [x][y] syntax error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            //if everything is o.k return true.
            return true;
        }

        /// <summary>
        /// Check if vector input is spelled propperly.
        /// </summary>
        /// <param name="str">The string vector to be checked.</param>
        /// <returns>If the string vector is properlly spelled.</returns>
        private bool VectorNumberChecker(string str)
        {
            int x1 = str.Count(x => x == ' ') + 1;
            int y1 = str.Split(' ').Count();
            if (str.Where(x => (x < '0' || x > '9') && x != ' ' && x != '-' && x != '.').Count() > 0)
            {
                MessageBox.Show("Warnning : Vector can include only scalar and spaces.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }
            else if (!DigitsNumberChecker(str.Split(' ').ToList()))
            {
                MessageBox.Show("Warnning : Vector include to much spaces chars.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if all items in the list can represent numbers.
        /// </summary>
        /// <param name="lst">The input list.</param>
        /// <returns>True if all list items can be considered as numbers , False otherwise.</returns>
        private bool DigitsNumberChecker(List<string> lst)
        {
            bool returnVal = true;

            foreach (string value in lst)
            {
                //if the input for one value contains more than one dot for precison dot or chars that are not digits.
                returnVal = returnVal & DigitsNumberChecker(value);
            }

            return returnVal;
        }

        /// <summary>
        /// Check if a string can represent a number or not.
        /// </summary>
        /// <param name="str">The input string to be a number.</param>
        /// <returns>True if the string can be a number , False otherwise.</returns>
        private bool DigitsNumberChecker(string str)
        {
            if (str == "")
                return false;

            //can starts with negative sign.
            if (str.StartsWith("-"))
            {
                str = str.Substring(1, str.Length - 1);
            }

            if (str.Where(x => (x < '0' || x > '9') && x != '.').Count() > 0 || str.Count(x => x == '.') > 1)
            {
                MessageBox.Show("Warnning : cannot include more than one dot for precision or characters that are not digits.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if a given string represent a integer.
        /// </summary>
        /// <param name="number">The given string.</param>
        /// <returns>True or False if an integer representation for the string.</returns>
        private bool IntegerChecker(string number)
        {
            if (number.Count(c => (c < '0' || c > '9')) > 0)
                return false;
            return true;
        }

        /// <summary>
        /// Returns the num of statusVal statuses in the variable list.
        /// </summary>
        /// <param name="statusVal">The status to check it's occurence number.</param>
        /// <returns>The num of statusVal in the variable list.</returns>
        private int NumOfSpecificStatus(string statusVal)
        {
            //TODO: Moog
            statusVal = StatusIndexByNameDecoder(statusVal);
            return _variablesList._variablesDictionary.Count(variable =>
                _variablesList._variablesDictionary[variable.Key]._description["status"].MoogParameter == statusVal);
        }

        /// <summary>
        /// Get the index (number) of a status by the status name.
        /// </summary>
        /// <param name="statusValueByName">The status value by name.</param>
        /// <returns>The status value by index(number).</returns>
        private string StatusIndexByNameDecoder(string statusValueByName)
        {
            switch (statusValueByName)
            {
                case "Const":
                    return "0";
                case "Static":
                    return "1";
                case "Varying":
                    return "2";
                case "AcrossStair":
                    return "3";
                case "WithinStair":
                    return "4";
                case "Random":
                    return "5";
                case "Vector":
                    return "6";
            }

            return "4";
        }

        /// <summary>
        /// Changing the text for the parameters textbox for the varName variable.
        /// </summary>
        /// <param name="varName">The variable to change it's parameters textbox.</param>
        private void SetParametersTextBox(string varName, StringBuilder sBuilder)
        {
            //find the relevant parameters attribute textbox according to the variable name.
            TextBox parametersTextBox = _dynamicAllocatedTextBoxes[varName + "parameters"] as TextBox;

            //print the parameter in the gui according to the representation of each status.
            //TODO: Moog
            switch (_variablesList._variablesDictionary[varName]._description["status"].MoogParameter)
            {
                case "0":   //const
                case "1":   //static
                case "6":
                    //show the _MoogParameter.
                    string parametersTextVal = string.Join(",", _variablesList._variablesDictionary[varName]._description["parameters"].MoogParameter);
                    parametersTextBox.Text = parametersTextVal;
                    break;

                case "2":   //varying
                case "3":   //acrossstair
                case "4":   //withinstair
                case "5":   //ramdom
                    //show the _MoogParameter.
                    string lowboundTextVal = string.Join(",", _variablesList._variablesDictionary[varName]._description["low_bound"].MoogParameter);
                    string highboundTextVal = string.Join(",", _variablesList._variablesDictionary[varName]._description["high_bound"].MoogParameter);
                    string increasingTextVal = string.Join(",", _variablesList._variablesDictionary[varName]._description["increament"].MoogParameter);

                    parametersTextBox.Text = ThreeStagesRepresentation(sBuilder, lowboundTextVal, increasingTextVal, highboundTextVal);
                    break;

            }
        }
        #endregion

        

        private void _dynamicParametersPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }

    public class VaryingItem
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public VaryingItem()
        {

        }

        /// <summary>
        /// The text the item have to show.
        /// </summary>
        public string _text
        {
            get;
            set;
        }

        /// <summary>
        //
        /// The index in the varying cross vals list to be referenced to.
        /// </summary>
        public int _listIndex
        {
            get;
            set;
        }
    }
    
}

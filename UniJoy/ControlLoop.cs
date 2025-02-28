﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Trajectories;
using Trajectories.TrajectoryCreators;
using Params;
using WMPLib;
using log4net;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using MathNet.Numerics.Distributions;
using UnityVR;
using Newtonsoft.Json;
using MoogController;
using SimpleTCP;
using System.IO;

using UnijoyData.Shared.Commands;
using UniJoy.Network;
using UnijoyData.Shared.Data;
using InputOutputDeviceHandlers.EventHandlers.EventWriters;
using InputOutputDeviceHandlers.EventHandlers.EventTypes;
using InputOutputDeviceHandlers.UserInputs;
using InputOutputDeviceHandlers.UserInputs.GuiButtonsInput;

namespace UniJoy
{
    /// <summary>
    /// This class is the main program control loop.
    /// It calls all the needed other interfaces to make what's needed to be created.
    /// The function is called by the GuiInterface after the statButton is clicked.
    /// </summary>
    public class ControlLoop
    {
        //const bool UPDATE_GLOBAL_DETAILS_LIST_VIEW = false;

        #region ATTRIBUTES
        /// <summary>
        /// The trajectory creator interface for making the trajectory for each trial.
        /// </summary>
        //TODO: why no references?? needed?
        private ITrajectoryCreator _trajectoryCreator;

        /// <summary>
        /// The trajectory creation 
        /// </summary>
        private TrajectoryCreatorHandler _trajectoryCreatorHandler;

        /// <summary>
        /// The variables readen from the xlsx protocol file.
        /// </summary>
        private Variables _variablesList;

        /// <summary>
        /// Final list holds all the current cross varying vals by dictionary of variables with values for each line(trial) for both ratHouseParameters.
        /// </summary>
        private List<Dictionary<string, double>> _crossVaryingVals;

        /// <summary>
        /// The static variables list in double value presentation.
        /// The string is for the variable name.
        /// </summary>
        private Dictionary<string, List<double>> _staticVariablesList;

        /// <summary>
        /// The numbers of samples for each trajectory.
        /// </summary>
        private int _frequency;

        /// <summary>
        /// The name of the selected protocol.
        /// </summary>
        public string ProtocolFullName;

        /// <summary>
        /// The current varying trial combination that should be selected to make the trajectory from.
        /// </summary>
        private int _currentVaryingTrialIndex;

        /// <summary>
        /// The total number of trials for the experiment should have.
        /// </summary>
        private int _totalNumOfTrials;

        /// <summary>
        /// The total number of correct answers(head stability during the trial movement plus correct decision).
        /// </summary>
        private int _totalCorrectAnswers;

        /// <summary>
        /// The total number of entrance to the center during timeout time with stability during the duration time.
        /// </summary>
        private int _totalHeadStabilityInCenterDuringDurationTime;

        /// <summary>
        /// The total success states for a trial (correct or wrong answer but some answer).
        /// </summary>
        private int _totalChoices;

        /// <summary>
        ///The total number of head fixation breaks during the duration time.
        /// </summary>
        private int _totalHeadFixationBreaks;

        /// <summary>
        /// The total number of head fixation breaks only during the start delay time.
        /// </summary>
        private int _totalHeadFixationBreaksStartDelay;

        /// <summary>
        /// The varying index selector for choosing the current combination index.
        /// </summary>
        private VaryingIndexSelector _varyingIndexSelector;

        /// <summary>
        /// The current repetition index number.
        /// </summary>
        private int _repetitionIndex;

        /// <summary>
        /// The current stickOn index number.
        /// </summary>
        private int _stickOnNumberIndex;

        /// <summary>
        /// Includes all the current trial timings and delays.
        /// </summary>
        private TrialTimings _currentTrialTimings;

        // WTF?
        /// <summary>
        /// The current trial trajectories.
        /// The first element in the tuple is the ratHouseTrajectory.
        /// The second element in the tuple is the landscapeHouseTrajectory.
        /// </summary>
        private Tuple<Trajectory, Trajectory> _currentTrialTrajectories;

        /// <summary>
        /// The current trial stimulus type.
        /// </summary>
        private int _currentTrialStimulusType;

        /// <summary>
        /// A random object for random numbers.
        /// </summary>
        private Random _timingRandomizer;

        //todo::replace this controller with the responsebox/joystick.
        /// <summary>
        /// Controller for the rat Noldus responses.
        /// </summary>
        //private RatResponseController _ratResponseController;

        //todo::replace that with EEG controller.
        /// <summary>
        /// Controller for writing events for the EEG.
        /// </summary>
        IEventWriter<UnijoyEvent> _evenstWriter;

        /// <summary>
        /// Indicated if the control loop should not make another trials.
        /// </summary>
        private bool _stopAfterTheEndOfTheCurrentTrial;

        /// <summary>
        /// Indicated if the control loop should paused until resume button is pressed.
        /// </summary>
        private bool _pauseAfterTheEndOfTheCurrentTrial;

        /// <summary>
        /// Describes the delegate for a control with it's nick name.
        /// </summary>
        private Dictionary<string, Delegate> _mainGuiControlsDelegatesDictionary;

        /// <summary>
        /// Describes the control object with it's nick name.
        /// </summary>
        private Dictionary<string, Control> _mainGuiInterfaceControlsDictionary;

        /// <summary>
        /// Timer initialize each trial start and reset onlt at he end of the trial.
        /// </summary>
        private Stopwatch _controlLoopTrialTimer;

        /// <summary>
        /// A dictionary include a key for the event name and a double for the time of the event since the start of the trial. Each trial the dictionary cleared.
        /// </summary>
        private Dictionary<string, double> _trialEventRealTiming;

        /// <summary>
        /// The current rat sampling response come from the Noldus.
        /// The sampling rate is readen from solution settings configuration.
        /// </summary>
        private PressType _currentResponse;

        /// <summary>
        /// The rat decision about the current trial stimulus direction.
        /// </summary>
        private PressType _currentRatDecision;

        /// <summary>
        /// The decision the rat should choose.
        /// </summary>
        private PressType _correctDecision;

        /// <summary>
        /// The selected rat name (that makes the experiment).
        /// </summary>
        public string TesteeName { get; set; }

        /// <summary>
        /// The name of the student that makes the experiment.
        /// </summary>
        public string ResearcherName { get; set; }

        /// <summary>
        /// The number of repetitions for the varying set.
        /// </summary>
        public int NumOfRepetitions { get; set; }

        /// <summary>
        /// The number of stick on number for each specific value in the optional values for rounds (must devide NumOfRepetitions).
        /// CHANGE LATER
        /// </summary>
        // todo 
        public int NumOfStickOn = 1;
        //public int NumOfStickOn { get; set; }

        public bool IsMoogConnected { get; set; }

        /// <summary>
        /// The SavedDataMaker object to create new result file for each experiment.
        /// </summary>
        private SavedDataMaker _savedExperimentDataMaker;

        /// <summary>
        /// Logger for writing log information.
        /// </summary>
        private ILog _logger;

        /// <summary>
        /// The online psycho graph maker object to control the psycho chart.
        /// </summary>
        private OnlinePsychGraphMaker _onlinePsychGraphMaker;

        /// <summary>
        /// The task executing the robot forward motion until finished.
        /// </summary>
        private Task _robotMotionTask;

        /// <summary>
        /// Indicated if whether to wait for a rat to enter it's head to the center automatically skipping this step.
        /// </summary>
        public bool AutoStart { get; set; }

        /// <summary>
        /// Indicates whethear to enable error sound for a wrong choice.
        /// </summary>
        public bool EnableErrorSound { get; set; }

        //todoo::wtf it is??
        /// <summary>
        /// Indicates if to enable that right parameters values and left parameters values must be equals.
        /// </summary>
        public bool EnableRightLeftMustEquals { get; set; }

        /*/// <summary>
        /// Indicates the autos options that are commanded in the real time (when the code use it at the conditions and not only if the user change it betweens).
        /// </summary>
        public AutosOptions _autosOptionsInRealTime { get; set; }*/

        /// <summary>
        /// Indicates the special modes that are commanded in the real time.
        /// </summary>
        public SpecialModes _specialModesInRealTime { get; set; }

        /// <summary>
        /// Indicates the sounds modes that are commanded in the real time.
        /// </summary>
        public SoundsMode _soundsMode { get; set; }

        /// <summary>
        /// Dictionary represent a sound name and it's file path.
        /// </summary>
        private Dictionary<string, string> _soundPlayerPathDB;

        //Maayan Edit
        //todoo::there is the rat sample or something like that...
        private IUserInputController _remoteController;

        /// <summary>
        /// The UnityEngine command interface to send commands with.
        /// </summary>
        private UnityCommandsSender _unityCommandsSender;

        #endregion ATTRIBUTES

        #region CONTRUCTOR
        /// <summary>
        /// Default constructor
        /// <param name="ctrlDelegatesDic">The controls delegate names and objects.</param>
        /// <param name="mainGuiInterfaceControlsDictionary">The name of each main gui needed control and it's reference.</param>
        /// <param name="logger">The program logger for logging into log file.</param>
        /// </summary>
        public ControlLoop(
            Dictionary<string, Delegate> ctrlDelegatesDic,
            Dictionary<string, Control> mainGuiInterfaceControlsDictionary,
            ILog logger)
        {
            _trajectoryCreatorHandler = new TrajectoryCreatorHandler();

            //copy the logger reference to writing lof information
            _logger = logger;

            _evenstWriter = new LptEventWriter(0);
            //_remoteController = new ThundermasterJoysticUserInputController(_logger);
            //_remoteController = new KeyBoardUserInputController();

            _remoteController = new WindowButtonsInput();
            _stopAfterTheEndOfTheCurrentTrial = false;

            //init the trial events details.
            _trialEventRealTiming = new Dictionary<string, double>();
            _controlLoopTrialTimer = new Stopwatch();

            //initialize the savedDataMaker object once.
            _savedExperimentDataMaker = new SavedDataMaker();

            //initialize the controls changed by the controlLoop and their invoking functions.
            _mainGuiControlsDelegatesDictionary = ctrlDelegatesDic;
            _mainGuiInterfaceControlsDictionary = mainGuiInterfaceControlsDictionary;

            //initialize the online psycho graph.
            _onlinePsychGraphMaker = new OnlinePsychGraphMaker
            {
                ClearDelegate = _mainGuiControlsDelegatesDictionary["OnlinePsychoGraphClear"],
                SetSeriesDelegate = _mainGuiControlsDelegatesDictionary["OnlinePsychoGraphSetSeries"],
                SetPointDelegate = _mainGuiControlsDelegatesDictionary["OnlinePsychoGraphSetPoint"],
                ChartControl = _mainGuiInterfaceControlsDictionary["OnlinePsychoGraph"] as Chart
            };
            
            /*Task.Run(() => TryConnectToUnityEngine());*/
        }
        #endregion CONTRUCTOR

        #region FUNCTIONS

        #region GUI_COMMANDS
        /// <summary>
        /// Transfer the control from the main gui to the control loop until a new gui event is handled by the user.
        /// </summary>
        public void Start(Variables variablesList, List<Dictionary<string, double>> crossVaryingList, Dictionary<string, List<double>> staticVariablesList, int frequency, ITrajectoryCreator trajectoryCreatorName)
        {
            //initialize variables.
            _variablesList = variablesList;
            _crossVaryingVals = crossVaryingList;
            _staticVariablesList = staticVariablesList;
            _frequency = frequency;

            //initialize counters and varying selector.
            _totalNumOfTrials = _crossVaryingVals.Count();
            _varyingIndexSelector = new VaryingIndexSelector(_totalNumOfTrials);
            _totalCorrectAnswers = 0;
            _totalHeadStabilityInCenterDuringDurationTime = 0;
            _totalChoices = 0;
            _totalHeadFixationBreaks = 0;
            _totalHeadFixationBreaksStartDelay = 0;

            //reset repetition index and stickOnNumber (would be reset in the loop immediately because the condition == NumOfStickOn).
            _repetitionIndex = 0;
            _stickOnNumberIndex = NumOfStickOn;

            _timingRandomizer = new Random();

            //set the trajectory creator name to the given one that should be called in the trajectoryCreatorHandler.
            //also , set the other properties.
            _trajectoryCreatorHandler.SetTrajectoryAttributes(trajectoryCreatorName, _variablesList, _crossVaryingVals, _staticVariablesList, _frequency);

            //create a new results file for the new experiment.
            _savedExperimentDataMaker.CreateControlNewFile(ProtocolFullName);
            Task.Run(() => {
                _logger.Info("Saving experiment const parameters to the result file (once).");
                _savedExperimentDataMaker.SaveExperimentDataToFile(new ExperimentData()
                {
                    ApplicationVersionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                    ProtocolName = ProtocolFullName,
                    TesteeName = TesteeName,
                    ResearcherName = ResearcherName,
                    NumOfRepetitions = NumOfRepetitions,
                    /*AutosOptions = _autosOptionsInRealTime,*/
                    SpecialModes = _specialModesInRealTime,
                    SoundsMode = _soundsMode
                });
            });

            //clear and initialize the psycho online graph.
            _onlinePsychGraphMaker.Clear();
            _onlinePsychGraphMaker.VaryingParametrsNames = GetVaryingVariablesList();
            _onlinePsychGraphMaker.HeadingDireactionRegion = new Region
            {
                LowBound = double.Parse(_variablesList._variablesDictionary["HEADING_DIRECTION"]._description["low_bound"].MoogParameter),
                Increament = double.Parse(_variablesList._variablesDictionary["HEADING_DIRECTION"]._description["increament"].MoogParameter),
                HighBound = double.Parse(_variablesList._variablesDictionary["HEADING_DIRECTION"]._description["high_bound"].MoogParameter)
            };
            _onlinePsychGraphMaker.InitSerieses();


            //run the main control loop function in other thread from the main thread (that handling events and etc).
            _stopAfterTheEndOfTheCurrentTrial = false;
            Globals._systemState = SystemState.RUNNING;
            Task.Run(() => MainControlLoop());
        }

        /// <summary>
        /// Stop the MainControlLoop function.
        /// </summary>
        public void Stop()
        {
            //Globals._systemState = SystemState.STOPPED;
            _stopAfterTheEndOfTheCurrentTrial = true;
        }

        /// <summary>
        /// Pause the MainControlLoop function.
        /// </summary>
        public void Pause()
        {
            _pauseAfterTheEndOfTheCurrentTrial = true;
        }

        /// <summary>
        /// Resume the MainControlLoop.
        /// </summary>
        public void Resume()
        {
            _pauseAfterTheEndOfTheCurrentTrial = false;

            Task.Run(() => MainControlLoop());
        }
        #endregion GUI_COMMANDS

        #region STAGES_FUNCTION
        public void MainControlLoop()
        {
            _logger.Info("Main ControlLoop begin.");

            for (; _repetitionIndex < NumOfRepetitions / NumOfStickOn;)
            {
                //while all trial are not executed or not come with response stage.
                while (!_varyingIndexSelector.IsFinished())
                {
                    //if system has stopped , wait for the end of the current trial ans break,
                    if (Globals._systemState.Equals(SystemState.STOPPED) || _stopAfterTheEndOfTheCurrentTrial || _pauseAfterTheEndOfTheCurrentTrial)
                    {
                        Globals._systemState = SystemState.STOPPED;

                        if (!_pauseAfterTheEndOfTheCurrentTrial)
                            EndControlLoopByStopFunction();

                        return;
                    }

                    //initialize the stickOnNumber if needed and choose a new varyingIndex.
                    if (_stickOnNumberIndex == NumOfStickOn)
                    {
                        //choose the random combination index for the current trial.
                        _currentVaryingTrialIndex = _varyingIndexSelector.ChooseRandomCombination();

                        //creates the trajectory for both robots for the current trial if not one of the training protocols.
                        _currentTrialTrajectories = _trajectoryCreatorHandler.CreateTrajectory(_currentVaryingTrialIndex);

                        //reset the stickOnNumber.
                        _stickOnNumberIndex = 0;
                    }

                    //make that current option strickOn times one after one.
                    for (; _stickOnNumberIndex < NumOfStickOn;)
                    {
                        //if system has stopped , wait for the end of the current trial ans break,
                        if (Globals._systemState.Equals(SystemState.STOPPED) || _stopAfterTheEndOfTheCurrentTrial || _pauseAfterTheEndOfTheCurrentTrial)
                        {
                            Globals._systemState = SystemState.STOPPED;

                            if (!_pauseAfterTheEndOfTheCurrentTrial)
                                EndControlLoopByStopFunction();

                            return;
                        }

                        //Sending all needed data to all interfaces
                        PreTrialStage();

                        //TODOO:WAIT FOR THE START PRESS BUTTON
                        bool subjectPressedStartButtonDuringTheTimeoutDuration = WaitForStartButtonToBePressed();

                        //if the subject pressed the start button before timeOut time.
                        if (subjectPressedStartButtonDuringTheTimeoutDuration)
                        {
                            {
                                //update the state of the rat decision.
                                //todoo::chnage that _currentRatDecision
                                //_currentRatDecision = RatDecision.DurationTime;

                                //moving the robot with duration time , and checking for the stability of the head in the center.
                                if (MovingTheRobotDurationWithHeadCenterStabilityStage())
                                {
                                    //update the number of total head in the center with stability during the duration time.
                                    _totalHeadStabilityInCenterDuringDurationTime++;
                                    //_currentRatDecision = RatDecision.PassDurationTime;

                                    {
                                        //wait the rat to response to the movement during the response time.
                                        //Tuple<RatDecision, bool> decision = ResponseTimeStage();
                                        Tuple<PressType, bool> decision = ResponseTimeStage();
                                    }
                                }

                                //if fixation break
                                else
                                {
                                    //update the number of head fixation breaks.
                                    _totalHeadFixationBreaks++;
                                }
                            }
                        }

                        //sounds the beep with the missing start gead in the center.
                        else
                        {
                            AudioResponse.PlayTimeOutSound();
                            // skip this trial.
                            continue;
                        }

                        //the post trial stage for saving the trial data and for the delay between trials.
                        //TODOO: What changes need to be made?
                        bool duration1HeadInTheCenterStabilityStage = true; //to delete
                        if (PostTrialStage(duration1HeadInTheCenterStabilityStage))
                            _stickOnNumberIndex++;
                        else//if the the trual not succeed - choose another varting index randomly and not the same index (also , if stick number exists make it only of happens at the first time fot the stick on index).
                        {
                            if (_stickOnNumberIndex == 0)
                            {
                                //reset the current trial varying index because it was not successful.
                                _varyingIndexSelector.ResetTrialStatus(_currentVaryingTrialIndex);

                                //choose the random combination index for the current trial.
                                _currentVaryingTrialIndex = _varyingIndexSelector.ChooseRandomCombination();

                                //craetes the trajectory for both robots for the current trial if not one of the training protocols.
                                _currentTrialTrajectories = _trajectoryCreatorHandler.CreateTrajectory(_currentVaryingTrialIndex);
                            }

                        }
                    }
                }

                //reset all trials combination statuses for the next repetition.
                _varyingIndexSelector.ResetTrialsStatus();
                _repetitionIndex++;
            }

            //end of all trials and repetitions.
            EndControlLoopByStopFunction();
        }
        /// <summary>
        /// Initializes the variables , points , trajectories , random variables ,  etc.
        /// </summary>
        public void ResetVariables()
        {
            //TODOO : change the index of the trial to be identical to the trial number in the result file.
            _logger.Info("Initialization Stage of trial #" + (_totalHeadStabilityInCenterDuringDurationTime + 1));

            //determine all current trial timings and delays.
            _currentTrialTimings = DetermineCurrentTrialTimings();
            CheckDurationTimeAcceptableValue();

            //determine current stimulus type.
            _currentTrialStimulusType = DetermineCurrentStimulusType();

            //set the response to the stimulus direction as no entry to decision stage (and change it after if needed as well).
            //todo::change that _currentRatDecision
            //_currentRatDecision = RatDecision.NoEntryToResponseStage;

            //set the auto option to default values.
            /*_autosOptionsInRealTime = new AutosOptions();*/
            //initialize the trial special mode options.
            _specialModesInRealTime = new SpecialModes();
            //initialize the trial sounds mode options.
            _soundsMode = new SoundsMode();

            _specialModesInRealTime.EnableRightLeftMustEquals = EnableRightLeftMustEquals;

            //reset the trial stopwatch and add the start event trial to the trial events list and timings.
            _controlLoopTrialTimer.Restart();
            _trialEventRealTiming.Clear();
        }

        /*void SendTrialMetaDataToUnityEngine()
        {
            UnijoyTrialMetaData unijoyTrialMetaData = new UnijoyTrialMetaData()
            {
                ColorData = new ColorData()
                {
                    Red = (int)(_staticVariablesList["STAR_COLOR"][0]),
                    Green = (int)(_staticVariablesList["STAR_COLOR"][1]),
                    Blue = (int)(_staticVariablesList["STAR_COLOR"][2])
                },

                ObjectType = ObjectType.Triangle,
                Size = ((float)(_staticVariablesList["STAR_SIZE"][0]),
                                        (float)(_staticVariablesList["STAR_SIZE"][1])),
                Density = (float)(_staticVariablesList["STAR_DENSITY"][0]),

                //Source = "Unijoy",
                X = _currentTrialTrajectories.Item1.X.Select(item => (float)(item)).ToList(),
                Y = _currentTrialTrajectories.Item1.Y.Select(item => (float)(item)).ToList(),
                Z = _currentTrialTrajectories.Item1.Z.Select(item => (float)(item)).ToList(),
                RX = _currentTrialTrajectories.Item1.RX.Select(item => (float)(item)).ToList(),
                RY = _currentTrialTrajectories.Item1.RY.Select(item => (float)(item)).ToList(),
                RZ = _currentTrialTrajectories.Item1.RZ.Select(item => (float)(item)).ToList(),

                StarFieldDimension = ((float)(_staticVariablesList["STAR_VOLUME"][0]),
                                        (float)(_staticVariablesList["STAR_VOLUME"][1]),
                                        (float)(_staticVariablesList["STAR_VOLUME"][2])),
                Coherence = (int)(_staticVariablesList["STAR_MOTION_COHERENCE"][0]),

                ScreenDimension = ((float)(_staticVariablesList["SCREEN_DIMENSION"][0]),
                                        (float)(_staticVariablesList["SCREEN_DIMENSION"][1])),
                ClipPlanes = ((float)(_staticVariablesList["CLIP_PLANES"][0]),
                                        (float)(_staticVariablesList["CLIP_PLANES"][1])),
                EyeOffsets = ((float)(_staticVariablesList["EYE_OFFSETS"][0]),
                                        (float)(_staticVariablesList["EYE_OFFSETS"][1]),
                                        (float)(_staticVariablesList["EYE_OFFSETS"][2])),
                HeadCenter = ((float)(_staticVariablesList["HEAD_CENTER"][0]),
                                        (float)(_staticVariablesList["HEAD_CENTER"][1]),
                                        (float)(_staticVariablesList["HEAD_CENTER"][2]))
            };

            string serializedData = JsonConvert.SerializeObject(unijoyTrialMetaData);
            string serializedDataPath = $@"D:\UnijoyMetaData\{Guid.NewGuid()}.json";
            File.WriteAllText(serializedDataPath, serializedData);

            _unityCommandsSender.TrySendCommand(UnityEngineCommands.ReadTrialData, serializedDataPath);
        }*/

        /// <summary>
        /// Pre trial stage for sending all pre data for the current trial.
        /// </summary>
        public void PreTrialStage()
        {
            //_logger.Info("Pre trial stage begin.");

            //initialize the current time parameters and all the current trial variables.
            ResetVariables();

            //updates the gui elements as the current trial parameters.
            UpdateGuiElements();

            _trialEventRealTiming.Add("TrialBegin", _controlLoopTrialTimer.ElapsedMilliseconds);


            //send the data to the UnityEngine
            //SendTrialMetaDataToUnityEngine();

            Task preTrialWaitingTask = new Task(() =>
            {
                Thread.Sleep((int)(1000 * _currentTrialTimings.wPreTrialTime));
            });

            preTrialWaitingTask.Start();

            //Maayan edit
            //Task.WaitAll(preTrialWaitingTask, sendDataToRobotTask, SendDataToLedControllersTask);
            Task.WaitAll(preTrialWaitingTask);
        }

        /// <summary>
        /// Updates the gui elements with the current trial options.
        /// </summary>
        private void UpdateGuiElements()
        {
            //show some trial details to the gui trial details panel.
            ShowTrialDetailsToTheDetailsListView();
            //show the global experiment details for global experiment details.
            ShowGlobalExperimentDetailsListView();
        }

        public Tuple<PressType, bool> ResponseTimeStage()
        {
            _logger.Info("ResponseTimeStage begin.");
            // ~(Michael Saar)~ here the correct answer is determined. the correct choice is stored in the variable _correctDecision.
            DetermineCurrentStimulusAnswer();
            
            _remoteController.FlushBuffer();
            _currentResponse = PressType.None;
            Stopwatch sw = new Stopwatch();
            
            sw.Start();
            // wait for the press (until the response is NOT None) or until the time is up
            while (sw.ElapsedMilliseconds < (int)(1000 * _currentTrialTimings.wResponseTime) && _currentResponse == PressType.None)
            {
                // get the response (if there is one) from the remote controller.
                _currentResponse = _remoteController.SubjectChoice();
            }
            sw.Stop();
            
            // if the response is None, then the user did not respond in time.
            if (_currentResponse == PressType.None)
            {
                AudioResponse.PlayTimeOutSound();
                // todo: understand the role of this line here - why need to send the command to the unity engine? 
                //send command to UnityEngine that it should clean all it's rendered data.
                //_unityCommandsSender.TrySendCommand(UnityEngineCommands.VisualOperationCommand, VisualOperationCommand.CleanScreen);
                _logger.Info("ResponseTimeStage end. User did not respond in time.");
                return new Tuple<PressType, bool>(PressType.None, false);
            }
            
            // if the response is not None, then the user responded in time- check if the response is correct.
            _totalChoices++;
            //add the response real time to the real times dictionary.
            _trialEventRealTiming.Add("HumanDecision", _controlLoopTrialTimer.ElapsedMilliseconds);
            //get the current stimulus direction.
            double currentHeadingDirection = double.Parse(GetVariableValue("HEADING_DIRECTION"));
            // if the response is correct
            bool isCorrect = _currentResponse == _correctDecision;
            if (isCorrect)
            {
                // play the sound for correct answer.
                AudioResponse.PlayCorrectAnswerSound();
                //increase the total correct answers.
                _totalCorrectAnswers++;
                //update the psycho online graph.
                _onlinePsychGraphMaker.AddResult("Heading Direction", _currentTrialStimulusType, currentHeadingDirection, AnswerStatus.CORRECT);
            }
            else
            {
                // play the sound for wrong answer.
                AudioResponse.PlayWrongAnswerSound();
                //update the psycho online graph.
                _onlinePsychGraphMaker.AddResult("Heading Direction", _currentTrialStimulusType, currentHeadingDirection, AnswerStatus.WRONG);
            }
            // log the response data to the log file.
            _logger.Info($"ResponseTimeStage end. User responded in time. User response: {_currentResponse}. Correct answer: {_correctDecision}. Is correct: {isCorrect}.");
            return new Tuple<PressType, bool>(_currentResponse, isCorrect);
        }


        /// <summary>
        /// Moving the robot stage (if the subject pressed start in the timeOut time and was stable in the center for startDelay time).
        /// This function also, in paralleled to the robot moving, checks that the rat head was consistently in the center during the duration time of the movement time.
        /// </summary>
        /// <returns>True if the head was stable consistently in the center during the movement time.</returns>
        public bool MovingTheRobotDurationWithHeadCenterStabilityStage()
        {
            _logger.Info("Moving the robot with duration time and head center stability check stage is begin.");


            //start moving the robot according to the stimulus type.
            _logger.Info("Send Executing robot trajectory data start command");
            //TODO:DELETE
            //_robotMotionTask.Start();

            //TODO: Maayan - call the Moog to make a move
            int movementDuration = (int)(1000 * _currentTrialTimings.wDuration) + 5000; // ~(Michael Saar)~ added the 5000 
            /*
            _robotMotionTask = Task.Factory.StartNew(() =>
            {
                // write to the log file the start of movement sleeping for the duration time. // ~(Michael Saar)~
                // write also the thread id to the log file. // ~(Michael Saar)~
                _logger.Info("Started sleeping during movement." + "Thread id: " + Thread.CurrentThread.ManagedThreadId 
                             + " sleeping for: " + movementDuration); // ~(Michael Saar)~
                Thread.Sleep(movementDuration);
                _logger.Info("Finished Sleeping"); // ~(Michael Saar)~
            });
            */
            
            // forwardMovementTask
            /*if (IsMoogConnected)
            {*/
                Task forwardMovementTask;
                //Task.Run(() =>
                //{
                //for (_currentTrialTrajectories.Moog.count)
                 forwardMovementTask = Task.Factory.StartNew(() =>
                 {
                     _logger.Info("Sending to MOOG forward movement Task --begin"); // ~(Michael Saar)~
                     int currentTrialTrajectoriesSize = _currentTrialTrajectories.Item1.Count();
                     double MOTION_BASE_CENTER = -0.22077500;
                     // 2 stopwatches with pretty similar names -_-
                     Stopwatch stopwatch = new Stopwatch();
                     stopwatch.Start();
                     Stopwatch stopwatchPositions = new Stopwatch();
                     /*
                         for (int i = 0; i < currentTrialTrajectoriesSize; i++) // ~(Michael Saar)~ jump 16 points in order to get 1000H
                         {
                             //SendPosition(currentTrialTrajectory.Moog(i).X , currentTrialTrajectory.Moog(i).Y , currentTrialTrajectory.Moog(i).Z)
                             double surge = _currentTrialTrajectories.Item1[i].X;
                             double lateral = _currentTrialTrajectories.Item1[i].Y;
                             double heave = _currentTrialTrajectories.Item1[i].Z + MOTION_BASE_CENTER;
                             double rx = _currentTrialTrajectories.Item1[i].RX;
                             double ry = _currentTrialTrajectories.Item1[i].RY;
                             double rz = _currentTrialTrajectories.Item1[i].RZ;
                             MoogController.MoogController.SendPosition(surge / 100.0, heave, lateral / 100.0, rx, ry, rz);
                         }
                         */
                     // create all trajectories at once
                     double[] surge = new double[currentTrialTrajectoriesSize];
                     double[] lateral = new double[currentTrialTrajectoriesSize];
                     double[] heave = new double[currentTrialTrajectoriesSize];
                     double[] rx = new double[currentTrialTrajectoriesSize];
                     double[] ry = new double[currentTrialTrajectoriesSize];
                     double[] rz = new double[currentTrialTrajectoriesSize];

                     for (int i = 0; i < currentTrialTrajectoriesSize; i++)
                     {
                         // why /100?
                         
                         surge[i] = _currentTrialTrajectories.Item1[i].X / 100.0;
                         lateral[i] = _currentTrialTrajectories.Item1[i].Y / 100.0;
                         heave[i] = _currentTrialTrajectories.Item1[i].Z + MOTION_BASE_CENTER;
                         rx[i] = _currentTrialTrajectories.Item1[i].RX;
                         ry[i] = _currentTrialTrajectories.Item1[i].RY;
                         rz[i] = _currentTrialTrajectories.Item1[i].RZ;
                     }

                     // send all trajectories at once
                     double sumTimeSendPositions = 0;
                     stopwatchPositions.Start(); // ~(Michael Saar)~ --- DEBUG: calculate the average time of sending positions
                     double start = stopwatchPositions.ElapsedMilliseconds;
                     for (int i = 0; i < currentTrialTrajectoriesSize; i++)
                     {
                         //start = stopwatchPositions.ElapsedMilliseconds;
                         // two times because of specificity of moog
                         MoogController.MoogController.SendPosition(surge[i], heave[i], lateral[i], rx[i], ry[i], rz[i]);
                         MoogController.MoogController.SendPosition(surge[i], heave[i], lateral[i], rx[i], ry[i], rz[i]);
                         //sumTimeSendPositions += stopwatchPositions.ElapsedMilliseconds - start;
                     }

                     // get the time passed from the start of the stopwatch.
                     double timePassed = stopwatch.ElapsedMilliseconds;
                     _logger.Info("Time passed: " + timePassed);
                     _logger.Info("the average time of sending positions: " +
                                  sumTimeSendPositions /
                                  currentTrialTrajectoriesSize); // ~(Michael Saar)~ --- DEBUG: calculate the average time of sending positions
                     _logger.Info("Sending to MOOG forward movement Task --end"); // ~(Michael Saar)~
                 });
                 //});
                 
                 _trialEventRealTiming.Add("RobotEndMovingForward", _controlLoopTrialTimer.ElapsedMilliseconds);
                 
                 return true;
            /*}*/
            

            //throw new Exception();

            //write alpha omega that the stimulus start.
            /*Task.Run(() =>
            {
                WriteEventWriterStimulusBegin();
                _trialEventRealTiming.Add("StimulusStart", _controlLoopTrialTimer.ElapsedMilliseconds);
            });*/

            //execute the leds command if necessary.
            /*if (_currentTrialStimulusType == 2 ||
                _currentTrialStimulusType == 3 ||
                _currentTrialStimulusType == 4 ||
                _currentTrialStimulusType == 5)
            {

                //TODO: Maayan - call the unity visualization
                /*Task.Run(() =>
                {
                    for ()
                    {
                        SendOculusFrame(currentTrialTrajectory.Unity);
                        Position++;
                    }
                );#1#
                Task.Run(() =>
                {
                    //---data to send to the unity program (server)---
                    /*string msgToSend = "Space";
                    string jsonMsgToSend = JsonConvert.SerializeObject(msgToSend.ToArray());
                    TCPSender.SendString(jsonMsgToSend);
                    TCPSender.SendMovement(_currentTrialTrajectories.Item1);#1#

                    _unityCommandsSender.TrySendCommand(UnityEngineCommands.VisualOperationCommand , VisualOperationCommand.StartRender);
                });
            }*/

            //wait the robot task to finish the movement.
            /*if (_currentTrialStimulusType != 0)
            {
                // log the _robotMotionTask Thread id // ~(Michael Saar)~
                //_logger.Info("Waiting for _robotMotionTask to finish the movement." + "_robotMotionTask Thread id: " + _robotMotionTask.Id); // ~(Michael Saar)~
                //_robotMotionTask.Wait();
                _logger.Info("_robotMotionTask finished the movement.");
            }*/
            //TODOO: ADD THE SAME WAIT FOR THE VISUAL

            //TODOO:DELETE
            //also send the AlphaOmega that motion forward ends.
            //TODOO: Do I need this?
            //_alphaOmegaEventsWriter.WriteEvent(true, AlphaOmegaEvent.RobotEndMovingForward);
            /*_trialEventRealTiming.Add("RobotEndMovingForward", _controlLoopTrialTimer.ElapsedMilliseconds);

            //TODOO:DELETE
            //_logger.Info("End MovingTheRobotDurationWithHeadCenterStabilityStage with AutoFixation = " + AutoFixation + ".");
            //return the true state of the heading in the center stability during the duration time or always true when AutoFixation.
            //return headInCenterAllTheTime;
            return true;*/
        }

        /// <summary>
        /// Wait for the subject to press the start button in order to start the movement.
        /// </summary>
        /// <returns>True if the subject pressed on the start button during the limit of the timeoutTime.</returns>
        //public bool WaitForHeadEntranceToTheCenterStage()
        public bool WaitForStartButtonToBePressed()
        {
            //Start beep. Now waiting for the subject to press start button
            AudioResponse.PlayStartTrialSound();

            //TODOO: Do I need this?
            //write the beep start event to the AlphaOmega.
            //_alphaOmegaEventsWriter.WriteEvent(true, AlphaOmegaEvent.AudioStart);
            _trialEventRealTiming.Add("AudioStart", _controlLoopTrialTimer.ElapsedMilliseconds);

            _logger.Info("Waiting for start button to be pressed.");

/*#if UPDATE_GLOBAL_DETAILS_LIST_VIEW
            //update the global details listview with the current stage.
            _mainGuiInterfaceControlsDictionary["UpdateGlobalExperimentDetailsListView"].BeginInvoke(
            _mainGuiControlsDelegatesDictionary["UpdateGlobalExperimentDetailsListView"], "Current Stage", "Waiting for rat to start trial");
#endif*/

            //if autoStart is checked than should not wait for the subject to press start for starting.
            /*_autosOptionsInRealTime.AutoStart = AutoStart;
            if (AutoStart)
            {
                _logger.Info("AutoStart is On so no wait.");

                return true;
            }*/

            //stopwatch for the start button response timeout.
            Stopwatch sw = new Stopwatch();
            sw.Start();

            _remoteController.FlushBuffer();
            while ((int)sw.Elapsed.TotalMilliseconds < (int)(_currentTrialTimings.wTimeOutTime * 1000))
            {
                if (_remoteController.IsStartButtonPressed())
                {
                    /*_trialEventRealTiming.Add("HeadEnterCenter", _controlLoopTrialTimer.ElapsedMilliseconds);*/

                    return true;
                }

                //sample the signal indicating if the subject pressed start only 60 time per second (because the refresh rate of the signal is that frequency).
                Thread.Sleep((int)(Properties.Settings.Default.NoldusRatReponseSampleRate));
            }

            return false;
        }

        /// <summary>
        /// The post trial time - analyzing the response , saving all the trial data into the results file.
        /// <param name="duration1HeadInTheCenterStabilityStage">Indicated if one of the robots ot both moved due to duration 1 head in the center stability stage.</param>
        /// <returns>True if trial succeed otherwise returns false.</returns>
        /// </summary>
        public bool PostTrialStage(bool duration1HeadInTheCenterStabilityStage) // duration1HeadInTheCenterStabilityStage is always true
        {
            _logger.Info("PostTrialStage begin.");

            //update the global details listview with the current stage.

/*#if UPDATE_GLOBAL_DETAILS_LIST_VIEW
            _mainGuiInterfaceControlsDictionary["UpdateGlobalExperimentDetailsListView"].BeginInvoke(
            _mainGuiControlsDelegatesDictionary["UpdateGlobalExperimentDetailsListView"], "Current Stage", "Post trial time.");
#endif*/
            //need to get the robot backwards only if there was a rat entrance that trigger thr robot motion.
            Task moveRobotHomePositionTask;
            /*if (!duration1HeadInTheCenterStabilityStage || !IsMoogConnected)
            {
                _logger.Info("Backward not executed because duration1HeadInTheCenterStabilityStage = false. Calling NullFunction().");
                moveRobotHomePositionTask = Task.Factory.StartNew(() => NullFunction());
            }
            else
            {*/
                //send the AlphaOmega that motion backward starts.
                //TODOO: Do I need this?
            _evenstWriter.WriteEvent(UnijoyEvent.MoogStartMovingBackward);
            _trialEventRealTiming.Add("RobotStartMovingBackward", _controlLoopTrialTimer.ElapsedMilliseconds);


            _logger.Info("Creating backward trajectory.");
            //TODOO: Do I need this? yes
            Tuple<Trajectory, Trajectory> returnTrajectory = _trajectoryCreatorHandler.CreateTrajectory(_currentVaryingTrialIndex, true);
            _logger.Info("Finish creating backward trajectory.");

            moveRobotHomePositionTask = Task.Factory.StartNew(() =>
            {
                //for (_currentTrialTrajectories.Moog.count)
                _logger.Info("Backward started.");

                int rtuenTrajectorySize = returnTrajectory.Item1.Count();
                   
                // ~(Michael Saar)~ --- DEBUG ---
                    
                for (int i = 0; i < rtuenTrajectorySize; i += 1) // ~(Michael Saar)~ increment by 6 in order to imitate 1000Hz behavior over 2.5 seconds
                {
                    //SendPosition(currentTrialTrajectory.Moog(i).X , currentTrialTrajectory.Moog(i).Y , currentTrialTrajectory.Moog(i).Z)
                    double MOTION_BASE_CENTER = -0.22077500;
                    int size = _currentTrialTrajectories.Item1.X.Count - 1;
                    double surge = _currentTrialTrajectories.Item1[size].X + returnTrajectory.Item1[i].X;
                    double lateral = _currentTrialTrajectories.Item1[size].Y + returnTrajectory.Item1[i].Y;
                    double heave = returnTrajectory.Item1[i].Z + MOTION_BASE_CENTER;
                    double rx = returnTrajectory.Item1[i].RX;
                    double ry = returnTrajectory.Item1[i].RY;
                    double rz = returnTrajectory.Item1[i].RZ;
                    MoogController.MoogController.SendPosition(surge / 100.0, heave, lateral / 100.0, rx, ry, rz);
                    MoogController.MoogController.SendPosition(surge / 100.0, heave, lateral / 100.0, rx, ry, rz);
                }
                // send all 1000 positions at once without loop
                _logger.Info("Backward ended.");
            });
            //}
            
            // ---DEBUG LOG: why is there a sleep here? // ~(Michael Saar)~ 
            //_logger.Info("before post trial 5000 milliseconds sleep.");
            //todoo::what is that magic number??
            //Thread.Sleep(5000);
            //_logger.Info("after post trial 5000 milliseconds sleep.");

            bool trialSucceed = true;

            if (!_currentRatDecision.Equals(RatDecison.PassDurationTime))
            {
                //reset status of the current trial combination index if there was no response stage at all.
                //todoo::check if ResetTrialStatus
                _varyingIndexSelector.ResetTrialStatus(_currentVaryingTrialIndex);
                trialSucceed = false;
            }

            //save the data into the result file only if the trial is within success trials (that have any stimulus)
            /*if (!_currentRatDecision.Equals(RatDecision.NoEntryToResponseStage))
            {*/
                Task.Run(() =>
                {
                    _logger.Info("Saving trial# " + (_totalHeadStabilityInCenterDuringDurationTime + _totalHeadFixationBreaks) + " to the result file.");
                    _savedExperimentDataMaker.SaveTrialDataToFile(new TrialData()
                    {
                        TrialNum = _totalHeadStabilityInCenterDuringDurationTime + _totalHeadFixationBreaks,
                        //todo::change that _currentRatDecision
                        //RatDecision = _currentRatDecision,
                        TrialEventsTiming = _trialEventRealTiming
                    });
                });
            /*}*/

            _logger.Info("before post trial (int)(_currentTrialTimings.wPostTrialTime * 1000) sleep."); // ~(Michael Saar)~
            Thread.Sleep((int)(_currentTrialTimings.wPostTrialTime * 1000));
            _logger.Info("before post trial (int)(_currentTrialTimings.wPostTrialTime * 1000) sleep."); // ~(Michael Saar)~

            //wait the maximum time of the postTrialTime and the going home position time.
            //TODOO: Do I need this?
            moveRobotHomePositionTask.Wait();
            //also send the AlphaOmega that motion backwards ends.
            //todoo:: check need
            //_alphaOmegaEventsWriter.WriteEvent(true, AlphaOmegaEvent.RobotEndMovingBackward);

            //throw new Exception();

            _logger.Info("PostTrialStage ended. TrialSucceed = " + trialSucceed + ".");
            //todoo::check what is trial succedd in Moog terms - no response is OK?
            return trialSucceed;
        }
        
        #endregion STAGES_FUNCTION

        #region STAGES_ADDIION_FUNCTION
        /*/// <summary>
        /// Writing the stimulus type to the AlphaOmega according to the current stimulus type.
        /// </summary>
        private void WriteEventWriterStimulusBegin()
        {
            _logger.Info("Writing AlphaOmega stimulus event start");

            //todoo::change _alphaOmegaEventsWriter to EEG and update this writing.
            switch (_currentTrialStimulusType)
            {
                case 0://none
                    break;
                case 1://vistibular only.
                    _evenstWriter.WriteEvent(UnijoyEvent.StimulusStart1);
                    break;

                case 2://visual only.
                    _evenstWriter.WriteEvent(UnijoyEvent.StimulusStart2);
                    break;

                case 3://vistibular and visual both.
                    _evenstWriter.WriteEvent(UnijoyEvent.StimulusStart3);
                    break;

                default://if there is no motion , make a delay of waiting the duration time (the time that should take the robot to move).
                    break;
            }
        }*/

        //TODOO: Maayan - determine whether the subject answer was correct
        /// <summary>
        /// Determine the current trial correct side.
        /// </summary>
        public void DetermineCurrentStimulusAnswer()
        {
            //if stim tupe 0 chose uniformly random side.
            if (GetVariableValue("STIMULUS_TYPE") == "0")
            {
                _correctDecision = (Bernoulli.Sample(0.5) == 1) ? PressType.Right : PressType.Left;
            }

            //get the current stimulus direction.
            double currentHeadingDirection = double.Parse(GetVariableValue("HEADING_DIRECTION"));

            //determine the current stimulus direaction.
            PressType currentStimulationSide = (currentHeadingDirection == 0) ? (PressType.Center) : ((currentHeadingDirection > 0) ? (PressType.Right) : PressType.Left);

            _correctDecision = currentStimulationSide;
        }

        /// <summary>
        /// Return true if the duration time is according to the rules of the duration time variable.
        /// </summary>
        private void CheckDurationTimeAcceptableValue()
        {
            if (_currentTrialTimings.wDuration > 1.0)
            {
                MessageBox.Show("The current trial timing for robot movement is bigger than 1 second - Arduino doesnt have enough memory for more than 10 frames per second.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                throw new ArgumentException("The current trial timing for robot movement is bigger than 1 second - Arduino doesnt have enough memory for more than 10 frames per second.",
                    "_currentTrialTimings.wDuration");
            }

            if (_currentTrialTimings.wDuration * 1 - double.Parse(_currentTrialTimings.wDuration.ToString("0.0")) != 0)
            {
                MessageBox.Show("The current trial timing for robot movement is not a 0.1 multiplication than 1 second.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                throw new ArgumentException("The current trial timing for robot movement is not a 0.1 multiplication than 1 second.",
                    "_currentTrialTimings.wDuration");
            }
        }

        #endregion STAGES_ADDIION_FUNCTION

        #region GUI_CONTROLS_FUNCTIONS
        /// <summary>
        /// Show global experiment parameters.
        /// </summary>
        private void ShowGlobalExperimentDetailsListView()
        {
            //clear the past global details in the global details listview.
            _mainGuiInterfaceControlsDictionary["ClearGlobaExperimentlDetailsViewList"].BeginInvoke(
            _mainGuiControlsDelegatesDictionary["ClearGlobaExperimentlDetailsViewList"]);

            //update the number of trials count.
            _mainGuiInterfaceControlsDictionary["UpdateGlobalExperimentDetailsListView"].BeginInvoke(
                _mainGuiControlsDelegatesDictionary["UpdateGlobalExperimentDetailsListView"], "Trial # (count)", ((_totalHeadStabilityInCenterDuringDurationTime + _totalHeadFixationBreaks + _totalHeadFixationBreaksStartDelay + 1)).ToString());

            //update the number of total correct head in center with stabilty during duration time.
            _mainGuiInterfaceControlsDictionary["UpdateGlobalExperimentDetailsListView"].BeginInvoke(
                _mainGuiControlsDelegatesDictionary["UpdateGlobalExperimentDetailsListView"], "Success Trials", (_totalChoices).ToString());

            //update the number of total correct answers.
            _mainGuiInterfaceControlsDictionary["UpdateGlobalExperimentDetailsListView"].BeginInvoke(
                _mainGuiControlsDelegatesDictionary["UpdateGlobalExperimentDetailsListView"], "Correct Answers", (_totalCorrectAnswers).ToString());

            //update the number of total failure trial during duration time.
            _mainGuiInterfaceControlsDictionary["UpdateGlobalExperimentDetailsListView"].BeginInvoke(
                _mainGuiControlsDelegatesDictionary["UpdateGlobalExperimentDetailsListView"], "Failure Trials", (_totalHeadStabilityInCenterDuringDurationTime + _totalHeadFixationBreaks + _totalHeadFixationBreaksStartDelay - _totalChoices).ToString());

            //update the number of total fixation breaks.
            _mainGuiInterfaceControlsDictionary["UpdateGlobalExperimentDetailsListView"].BeginInvoke(
                _mainGuiControlsDelegatesDictionary["UpdateGlobalExperimentDetailsListView"], "Fixation Breaks", (_totalHeadFixationBreaks + _totalHeadFixationBreaksStartDelay).ToString());

            //update the number of left trials.
            _mainGuiInterfaceControlsDictionary["UpdateGlobalExperimentDetailsListView"].BeginInvoke(
                _mainGuiControlsDelegatesDictionary["UpdateGlobalExperimentDetailsListView"], "Remaining Trials", (_varyingIndexSelector.CountRemaining()).ToString());

            //update the number of total correct head in center with stabilty during duration time.
            _mainGuiInterfaceControlsDictionary["UpdateGlobalExperimentDetailsListView"].BeginInvoke(
                _mainGuiControlsDelegatesDictionary["UpdateGlobalExperimentDetailsListView"], "No choices", (_totalHeadStabilityInCenterDuringDurationTime - _totalChoices).ToString());
        }

        /// <summary>
        /// Show the current trial dynamic details to the ListView.
        /// </summary>
        public void ShowTrialDetailsToTheDetailsListView()
        {
            Dictionary<string, double> currentTrialDetails = _crossVaryingVals[_currentVaryingTrialIndex];
            _mainGuiInterfaceControlsDictionary["ClearCurrentTrialDetailsViewList"].BeginInvoke(
            _mainGuiControlsDelegatesDictionary["ClearCurrentTrialDetailsViewList"]);

            foreach (string varName in currentTrialDetails.Keys)
            {
                string currentParameterDetails;
                //only ratHouseParameter
                currentParameterDetails = "[" + currentTrialDetails[varName].ToString() + "]";

                _mainGuiInterfaceControlsDictionary["UpdateCurrentTrialDetailsViewList"].BeginInvoke(
                _mainGuiControlsDelegatesDictionary["UpdateCurrentTrialDetailsViewList"], varName, currentParameterDetails);
            }
        }
        #endregion GUI_CONTROLS_FUNCTIONS

        #region ADDITIONAL_FUNCTIONS
        /// <summary>
        /// This function is called if the control loop asked to be ended.
        /// </summary>
        public void EndControlLoopByStopFunction()
        {
            Globals._systemState = SystemState.FINISHED;

            //todo::check if to send here clear command to unity engine/Moog.

            //reset the repetition index.
            _repetitionIndex = 0;

            //show the final global experiment info (show it the last time)
            ShowGlobalExperimentDetailsListView();

            //Close and release the current saved file.
            _savedExperimentDataMaker.CloseFile();

            //#if UPDATE_GLOBAL_DETAILS_LIST_VIEW
            //raise an event for the GuiInterface that the trials round is over.
            _mainGuiInterfaceControlsDictionary["FinishedAllTrialsRound"].BeginInvoke(_mainGuiControlsDelegatesDictionary["FinishedAllTrialsRound"]);

            // ???
            //choose none rat in the selected rat
            _mainGuiInterfaceControlsDictionary["ResetSelectedRatNameCombobox"].BeginInvoke(_mainGuiControlsDelegatesDictionary["ResetSelectedRatNameCombobox"]);
            //#endif
        }

        /*public void TryConnectToUnityEngine()
        {
            int numOfRetries = 0;

            _unityCommandsSender = new UnityCommandsSender(8910);
            
            /*
            while (!_unityCommandsSender.TryStart())
            {

                MessageBox.Show($"Could not connect to the UnityEngine. Retries:{numOfRetries}", "Error");

                numOfRetries++;
            }
            #1#
        }*/

        /*/// <summary>
        /// Load all mp3 files that the MediaPlayer object should use.
        /// </summary>
        private void LoadAllSoundPlayers()
        {
            _soundPlayerPathDB.Add("CorrectAnswer", Application.StartupPath + @"\SoundEffects\correct sound effect.wav");
            _soundPlayerPathDB.Add("WrongAnswer", Application.StartupPath + @"\SoundEffects\Wrong Buzzer Sound Effect (Raised pitch 400 percent and -3db).wav");
            _soundPlayerPathDB.Add("Ding", Application.StartupPath + @"\SoundEffects\Ding Sound Effects (raised pitch 900 percent).wav");
            _soundPlayerPathDB.Add("MissingAnswer", Application.StartupPath + @"\SoundEffects\Wrong Buzzer Sound Effect (Raised pitch 400 percent and -3db).wav");
            _soundPlayerPathDB.Add("Ding-Left", Application.StartupPath + @"\SoundEffects\Ding Sound Effects - Left (raised pitch 900 percent).wav");
            _soundPlayerPathDB.Add("Ding-Right", Application.StartupPath + @"\SoundEffects\Ding Sound Effects - Right (raised pitch 900 percent).wav");
        }*/

        /// <summary>
        /// Determine the current trial stimulus type bt the stimulus type variable status.
        /// </summary>
        /// <returns>The stimulus type.</returns>
        public int DetermineCurrentStimulusType()
        {
            string stimulusTypeStatus = _variablesList._variablesDictionary["STIMULUS_TYPE"]._description["status"].MoogParameter;
            switch (stimulusTypeStatus)
            {
                case "1"://static
                    return int.Parse(_variablesList._variablesDictionary["STIMULUS_TYPE"]._description["parameters"].MoogParameter);
                case "2"://varying
                case "6"://Vector
                    return (int)(_crossVaryingVals[_currentVaryingTrialIndex]["STIMULUS_TYPE"]);
            }
            return 0;
        }

        /// <summary>
        /// Detrmines all current tiral timings and delays acoording the time types statuses.
        /// </summary>
        /// <returns>Return the TrialTimings struct contains all the timings types.</returns>
        public TrialTimings DetermineCurrentTrialTimings()
        {
            TrialTimings currentTrialTimings;
            currentTrialTimings.wStartDelay = DetermineTimeByVariable("START_DELAY");

            currentTrialTimings.wPreTrialTime = DetermineTimeByVariable("PRE_TRIAL_TIME");

            currentTrialTimings.wPostTrialTime = DetermineTimeByVariable("POST_TRIAL_TIME");

            currentTrialTimings.wTimeOutTime = DetermineTimeByVariable("TIMEOUT_TIME");

            currentTrialTimings.wResponseTime = DetermineTimeByVariable("RESPONSE_TIME");

            currentTrialTimings.wDuration = DetermineTimeByVariable("STIMULUS_DURATION");

            return currentTrialTimings;
        }

        /// <summary>
        /// determine the current trial of the input type time by it's status (random , static , etc...).
        /// </summary>
        /// <param name="timeVarName">The time type to be compute.</param>
        /// <returns>The result time according to the type of the time.</returns>
        public double DetermineTimeByVariable(string timeVarName)
        {

            //detrmine variable value by the status of the time type.
            string timeValue = GetVariableValue(timeVarName);

            //if not found - it is random type varriable.
            if (timeValue == string.Empty)
            {
                double lowTime = double.Parse(_variablesList._variablesDictionary[timeVarName]._description["low_bound"].MoogParameter);
                double highTime = double.Parse(_variablesList._variablesDictionary[timeVarName]._description["high_bound"].MoogParameter);
                return RandomTimeUniformly(lowTime, highTime);
            }

            return double.Parse(timeValue);
        }

        private List<string> GetVaryingVariablesList()
        {
            List<string> varyingVariablesNames = new List<string>();

            foreach (string item in _variablesList._variablesDictionary.Keys)
            {
                if (_variablesList._variablesDictionary[item]._description["status"].MoogParameter[0].Equals("2"))
                {
                    varyingVariablesNames.Add(_variablesList._variablesDictionary[item]._description["nice_name"].MoogParameter[0].ToString());
                }
            }

            return varyingVariablesNames;
        }

        /// <summary>
        /// Determines the asked variable value at the current stage by it's status (not include random types).
        /// </summary>
        /// <param name="parameterName">The parameter name to get the value for.</param>
        /// <returns>The value of the parameter at the current trial.</returns>
        public string GetVariableValue(string parameterName)
        {
            try
            {
                //detrmine the status of the variable type.
                string variableStatus = _variablesList._variablesDictionary[parameterName]._description["status"].MoogParameter;


                //decide the time value of the time type according to it's status.
                switch (variableStatus)
                {
                    case "0"://const
                    case "1"://static
                        return _variablesList._variablesDictionary[parameterName]._description["parameters"].MoogParameter;

                    case "2"://varying
                    case "6":
                        return _crossVaryingVals[_currentVaryingTrialIndex][parameterName].ToString("000000.00000000");

                    default:
                        return string.Empty;

                }
            }

            catch
            {
                MessageBox.Show("Error", "The parameter " + parameterName + " is not in the excel sheet.", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return string.Empty;
            }
        }

        /// <summary>
        /// Random a double (4 precision) value uniformly by the given bounds.
        /// </summary>
        /// <param name="lowTime">The low bound.</param>
        /// <param name="highTime">The high bound.</param>
        /// <returns>The random 4 double precision in the bounded range.</returns>
        public double RandomTimeUniformly(double lowTime, double highTime)
        {
            //we cannot really have a randon double number because in uniform countinious distrbution the probability for any value is 0.
            //so mutiplt it by 1000 , make a random number , and party it by 1000 (4 digits precision).
            int lowTimeInteger = (int)(lowTime * 1000);
            int highTimeInteger = (int)(highTime * 1000);

            //get the random integer (the doubled rand time in 4 digits precison).
            int randTimeInteger = _timingRandomizer.Next(lowTimeInteger, highTimeInteger);

            //return the result.
            return (double)(randTimeInteger) / 1000;
        }

        /// <summary>
        /// Null bumper function.
        /// </summary>
        private void NullFunction()
        {
        }
        
        #endregion ADDITIONAL_FUNCTIONS
        
        #endregion FUNCTIONS

        #region STRUCT_ENUMS
        /// <summary>
        /// Struct contains all the trial timings.
        /// </summary>
        public struct TrialTimings
        {
            /// <summary>
            /// The delay time between the rat head center and the trial start time.
            /// </summary>
            public double wStartDelay;

            /// <summary>
            /// The pre trial time before start beep and trial starts.
            /// </summary>
            public double wPreTrialTime;

            /// <summary>
            /// The duration to wait between the end of the previous trial and the beginning of the next trial.
            /// </summary>
            public double wPostTrialTime;

            /// <summary>
            /// The time after the beep of the trial begin and the time the rat can response with head to the center in order to begin the movement.
            /// </summary>
            public double wTimeOutTime;

            /// <summary>
            /// The time the rat have to response (with head to the left or to the right) after the rewardCenter (ig get).
            /// </summary>
            public double wResponseTime;

            /// <summary>
            /// The robot movement duration.
            /// </summary>
            public double wDuration;
        };


        /// <summary>
        /// Enum for the human stimulus decision.
        /// </summary>
        public enum HumanDecision
        {
            NoResponse = 0,
            Left = 1,
            Right = 2,
            Up = 3,
            Down = 4
        };

        /// <summary>
        /// Enum for the rat stimulus decision.
        /// </summary>
        public enum RatDecison
        {
            /// <summary>
            /// The rat decision could not happened due to no entrance to the response stage (because no stability or entrance to the center).
            /// </summary>
            NoEntryToResponseStage = -1,

            /// <summary>
            /// The rat doesn't decide anything (it's head was not in any of the sides).
            /// </summary>
            NoDecision = 0,

            /// <summary>
            /// The rat decide about the center.
            /// </summary>
            Center = 7,

            /// <summary>
            /// The rat decide about the left side.
            /// </summary>
            Left = 1,

            /// <summary>
            /// The rat decide about the right side.
            /// </summary>
            Right = 2,

            /// <summary>
            /// The rat decide about the left side.
            /// </summary>
            Up = 3,

            /// <summary>
            /// The rat decide about the right side.
            /// </summary>
            Down = 4,

            /// <summary>
            /// The rat passed the duration time fixation (movement) as a state.
            /// </summary>
            PassDurationTime = 5,

            /// <summary>
            /// DurationTime state.
            /// </summary>
            DurationTime = 6
        };

        #endregion STRUCT_ENUMS


        
    }
}

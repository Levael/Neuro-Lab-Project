using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniJoy
{
    /// <summary>
    /// This class is called at the beginning and the end of each trial.
    /// At the beginning in order to store the current trial parameters.
    /// At the end of the trial in order to store the responses and etc results.
    /// At the end of each trial in the experiment it saves the whole stored data into an txt file.
    /// </summary>
    class SavedDataMaker
    {
        /// <summary>
        /// The current saving file StreamWriter to save the file with.
        /// </summary>
        private StreamWriter _currentSavedFileStreamWriter;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SavedDataMaker()
        {
            _currentSavedFileStreamWriter = null;
        }

        public void SaveExperimentDataToFile(ExperimentData experimentData)
        {
            //create a new stringBuilder for line filling in the new created results file.
            StringBuilder lineBuilder = new StringBuilder();
            
            //append the application version number.
            lineBuilder.Append("Application version number:");
            lineBuilder.Append(experimentData.ApplicationVersionNumber);
            _currentSavedFileStreamWriter.WriteLine(lineBuilder.ToString());
            lineBuilder.Clear();
            
            //append the protocol full name
            lineBuilder.Append("Protocol name:");
            lineBuilder.Append(experimentData.ProtocolName);
            _currentSavedFileStreamWriter.WriteLine(lineBuilder.ToString());
            lineBuilder.Clear();

            //append the rat name.
            lineBuilder.Append("Testee name:");
            lineBuilder.Append(experimentData.TesteeName);
            _currentSavedFileStreamWriter.WriteLine(lineBuilder.ToString());
            lineBuilder.Clear();

            //append the student name.
            lineBuilder.Append("Researcher name:");
            lineBuilder.Append(experimentData.ResearcherName);
            _currentSavedFileStreamWriter.WriteLine(lineBuilder.ToString());
            lineBuilder.Clear();
            
            //append the stick number value.
            lineBuilder.Append("NumOfRepetitions:");
            lineBuilder.Append(experimentData.NumOfRepetitions);
            _currentSavedFileStreamWriter.WriteLine(lineBuilder.ToString());
            lineBuilder.Clear();
            
            //appends all static variables names and values.
            // TODO bug here
            foreach (string paramName in experimentData.StaticVariables.Keys)
            {
                lineBuilder.Append(paramName);
                lineBuilder.Append(":");
                lineBuilder.Append(experimentData.StaticVariables[paramName]);
                lineBuilder.Append(" ");

                _currentSavedFileStreamWriter.WriteLine(lineBuilder.ToString());
                lineBuilder.Clear();
            }

            //append all varying variables names and values.
            foreach (string paramName in experimentData.VaryingVariables.Keys)
            {
                lineBuilder.Append(paramName);
                lineBuilder.Append(":");
                lineBuilder.Append(experimentData.VaryingVariables[paramName]);

                _currentSavedFileStreamWriter.WriteLine(lineBuilder.ToString());
                lineBuilder.Clear();
            }
            
            //append the auto's option statuses in the current trial.
            lineBuilder.Append(experimentData.AutosOptions.ToString());
            _currentSavedFileStreamWriter.WriteLine(lineBuilder.ToString());
            lineBuilder.Clear();

            //append the special modes options in the current trial.
            lineBuilder.Append(experimentData.SpecialModes.ToString());
            _currentSavedFileStreamWriter.WriteLine(lineBuilder.ToString());
            lineBuilder.Clear();

            //append the sounds modes options in the current trial.
            lineBuilder.Append(experimentData.SoundsMode.ToString());
            _currentSavedFileStreamWriter.WriteLine(lineBuilder.ToString());
            lineBuilder.Clear();
            
            //flush the text to be written immediately.
            _currentSavedFileStreamWriter.Flush();
        }

        /// <summary>
        /// Save (write) a trial in the experiment to the current created new file.
        /// </summary>
        /// <param name="trialData">The trial data struct to written to the file.</param>
        public void SaveTrialDataToFile(TrialData trialData)
        {
            //create a new stringBuilder for line filling in the new created results file.
            StringBuilder lineBuilder = new StringBuilder();

            //append the new trial number.
            lineBuilder.Append("Trial:");
            lineBuilder.Append(trialData.TrialNum);
            _currentSavedFileStreamWriter.WriteLine(lineBuilder.ToString());
            lineBuilder.Clear();

            //append the rat decision for the stimulus direction.
            lineBuilder.Append("Human Decision:");
            lineBuilder.Append(trialData.HumanDecision);
            _currentSavedFileStreamWriter.WriteLine(lineBuilder.ToString());
            lineBuilder.Clear();

            //save all trial events with their real time timings.
            SaveTheRealTimingEvents(trialData.TrialEventsTiming);

            //flush the text to be written immediately.
            _currentSavedFileStreamWriter.Flush();
        }

        /// <summary>
        /// Saving all real times events and values dictionary to the results file.
        /// </summary>
        /// <param name="eventRealTimingDictionary">The real time events name - time dictionary.</param>
        private void SaveTheRealTimingEvents(Dictionary<string, double> eventRealTimingDictionary)
        {
            StringBuilder lineBuilder = new StringBuilder();

            foreach (KeyValuePair<string, double> eventTimePair in eventRealTimingDictionary)
            {
                lineBuilder.Append(eventTimePair.Key + "RealTime");
                lineBuilder.Append(":");
                lineBuilder.Append(eventTimePair.Value.ToString());

                _currentSavedFileStreamWriter.WriteLine(lineBuilder.ToString());

                lineBuilder.Clear();
            }
        }

        /// <summary>
        /// Close and release the current saved file.
        /// </summary>
        public void CloseFile()
        {
            if (_currentSavedFileStreamWriter != null) _currentSavedFileStreamWriter.Dispose();
        }

        /// <summary>
        /// Create a new experiment result file to save in it new experiment data.
        /// <param name="protocolName">The protocol name for the current experiment.</param>
        /// </summary>
        public void CreateControlNewFile(string protocolName)
        {
            //Creates a directory with the rat name and the day in it if not exists.
            CreateNewDirectory(protocolName);

            //create a new results file for the new experiment.
            _currentSavedFileStreamWriter = File.CreateText(@"C:\results\" + protocolName + @"\" + DateTime.Now.ToString("yyyy_MM_dd") + @"\" + DateTime.Now.ToString("yyyy_MM_dd_HH-mm") + ".txt");
        }

        /// <summary>
        /// Creates a directory with the rat name abd the day in it if not exists.
        /// </summary>
        /// <param name="ratName">The rat name.</param>
        private void CreateNewDirectory(string ratName)
        {
            //create a rat directory if there is no rat directory with it's name.
            if (!Directory.Exists(@"C:\results\" + ratName))
            {
                Directory.CreateDirectory(@"C:\results\" + ratName);
            }

            if (!Directory.Exists(@"C:\results\" + ratName + @"\" + DateTime.Now.ToString("yyyy_MM_dd")))
            {
                Directory.CreateDirectory(@"C:\results\" + ratName + @"\" + DateTime.Now.ToString("yyyy_MM_dd"));
            }
        }
    }

    /// <summary>
    /// Class for data to be saved into file only once at the beginning of the experiment
    /// </summary>
    class ExperimentData
    {
        /// <summary>
        /// The application version number.
        /// </summary>
        public string ApplicationVersionNumber { get; set; }
        
        /// <summary>
        /// The name of the testee being experiment.
        /// </summary>
        public String TesteeName { get; set; }
        
        /// <summary>
        /// The student name that makes the experiment.
        /// </summary>
        public string ResearcherName { get; set; }
        
        /// <summary>
        /// The running protocol file name.
        /// </summary>
        public string ProtocolName { get; set; }
        
        /// <summary>
        /// The global number of repetitions for each varying parameter.
        /// </summary>
        public int NumOfRepetitions { get; set; }
        
        /// <summary>
        /// The static variables value for one trial.
        /// </summary>
        public Dictionary<string, List<double>> StaticVariables { get; set; }

        /// <summary>
        /// The varying variables value for one trial.
        /// </summary>
        public Dictionary<string, double> VaryingVariables { get; set; }

        /*/// <summary>
        /// The timings variables for one trial.
        /// </summary>
        public ControlLoop.TrialTimings TimingsVariables { get; set; }*/
        
        /// <summary>
        /// AutoOption class for all Autos values.
        /// </summary>
        public AutosOptions AutosOptions { get; set; }

        /// <summary>
        /// SpecialModes object for all special modes values.
        /// </summary>
        public SpecialModes SpecialModes { get; set; }

        /// <summary>
        /// SoundsModes object for all sounds modes values.
        /// </summary>        
        public SoundsMode SoundsMode { get; set; }
    }

    /// <summary>
    /// TrialData compact class.
    /// </summary>
    class TrialData
    {
        /// <summary>
        /// The rat decision for the stimulus direction.
        /// </summary>
        public ControlLoop.HumanDecision HumanDecision { get; set; }

        /// <summary>
        /// The trial number in the experiment.
        /// </summary>
        public int TrialNum { get; set; }

        /*/// <summary>
        /// The stick on number.
        /// </summary>
        public int StickOnNumber { get; set; }*/

        /// <summary>
        /// A dictionary include a key for the event name and a double for the time of the event since the start of the trial. Each trial the dictionary cleared.
        /// </summary>
        public Dictionary<string, double> TrialEventsTiming { get; set; }

        /*/// <summary>
        /// The total time in the experiment a manual reward was given.
        /// </summary>
        public double TotalHabdRewardTime { get; set; }*/
    }

    /// <summary>
    /// AutosOptions data class.
    /// </summary>
    public class AutosOptions
    {
        /// <summary>
        /// Ctor initialize all properties to false.
        /// </summary>
        public AutosOptions()
        {
            AutoReward = false;
            AutoFixation = false;
            AutoStart = false;
            AutoRewardSound = false;
        }

        /// <summary>
        /// AutoReward state at the current trial.
        /// </summary>
        public bool AutoReward { get; set; }

        /// <summary>
        /// AutoFixation state at the current trial.
        /// </summary>
        public bool AutoFixation { get; set; }

        /// <summary>
        /// AutoStart state at the current trial.
        /// </summary>
        public bool AutoStart { get; set; }

        /// <summary>
        /// AutoRewardSound state at the current trial.
        /// </summary>
        public bool AutoRewardSound { get; set; }

        /// <summary>
        /// Return a string represent the object.
        /// </summary>
        /// <returns>The string represents the obhect.</returns>
        public override string ToString()
        {
            return
                "AutoReward:" + AutoReward + "\r\n" +
                "AutoFixation:" + AutoFixation + "\r\n" +
                "AutoStart:" + AutoStart + "\r\n" +
                "CenterRewardSound:" + AutoRewardSound;
        }
    }

    /// <summary>
    /// A class describes all special modes options.
    /// </summary>
    public class SpecialModes
    {
        /// <summary>
        /// Constructor initalize all properties to false.
        /// </summary>
        public SpecialModes()
        {
            FixationOnly = false;
            SecondChoice = false;
            EnableRightLeftMustEquals = false;
            RRInverse = false;
        }

        /// <summary>
        /// Indicate fixation only mode for success (not with choices).
        /// </summary>
        public bool FixationOnly { get; set; }

        /// <summary>
        /// Indicates ability for second choice (to correct the answer).
        /// </summary>
        public bool SecondChoice { get; set; }

        /// <summary>
        /// Indicates if the right and the left parameters should be the same.
        /// </summary>
        public bool EnableRightLeftMustEquals { get; set; }

        /// <summary>
        /// Indicates if in case of Random Heading Direction , the descision was converted to be true.
        /// </summary>
        public bool RRInverse { get; set; }

        public override string ToString()
        {
            return
                "FixationOnly:" + FixationOnly + "\r\n" +
                "SecondChoice:" + SecondChoice + "\r\n" +
                "RR Inverse:" + RRInverse + "\r\n" +
                "EnableRightLeftMustEquals:" + EnableRightLeftMustEquals;
        }
    }

    /// <summary>
    /// A class describes all sounds modes options.
    /// </summary>
    public class SoundsMode
    {
        public SoundsMode()
        {
            BreakFixationSoundOn = false;
            ErrorChoiceSoundOn = false;
            EnableGoCueSound = false;
            EnableCueSoundInBothSide = false;
            EnableCueSoundInCorrectSide = false;
        }

        /// <summary>
        /// Indicates break fixation sound is on.
        /// </summary>
        public bool BreakFixationSoundOn { get; set; }

        /// <summary>
        /// Indicates if error sound is on/off when a wrong choice occured.
        /// </summary>
        public bool ErrorChoiceSoundOn { get; set; }

        /// <summary>
        /// Indicates if go cue sound is enabled (can be one of the correct side cue or the both sides option).
        /// </summary>
        public bool EnableGoCueSound { get; set; }

        /// <summary>
        /// Indicates to enable sound onlt at the correct side.
        /// </summary>
        public bool EnableCueSoundInCorrectSide { get; set; }

        /// <summary>
        /// Indicate if to enable clue sounds on both the side no matter what is the correct side.
        /// </summary>
        public bool EnableCueSoundInBothSide { get; set; }

        public override string ToString()
        {
            return
                "BreakFixationSoundOn:" + BreakFixationSoundOn + "\r\n" +
                "ErrorChoiceSoundOn:" + ErrorChoiceSoundOn + "\r\n" +
                "EnableGoCueSound:" + EnableGoCueSound + "\r\n" +
                "EnableCueSoundInBothSide:" + EnableCueSoundInBothSide + "\r\n" +
                "EnableCueSoundCorrectSide:" + EnableCueSoundInCorrectSide;
        }
    }

}

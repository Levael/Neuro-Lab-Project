using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Params;
using Trajectories.TrajectoryCreators;

namespace Trajectories
{
    /// <summary>
    /// This class is called in each trial in order to create the trajectories to the current trial
    /// according to the current trial paramaters.
    /// </summary>
    public class TrajectoryCreatorHandler
    {
        #region ATTRIBUTES
        /// <summary>
        /// The trajectory creator name (the name of the type for making the trajectory).
        /// </summary>
        private string _trajectoryCreatorName;

        /// <summary>
        /// The index of the current varying trial in the random indexed varying vector.
        /// </summary>
        private int _varyingCurrentIndex = 0;

        /// <summary>
        /// The variables read from the xlsx protocol file.
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
        private Dictionary<string, List<double>> _staticVals;

        /// <summary>
        /// The numbers of samples for each trajectory.
        /// </summary>
        private int _frequency;

        /// <summary>
        /// The TrajectoryCreator object decided by the trajectoryName.
        /// </summary>
        private ITrajectoryCreator _trajectoryCreator;
        #endregion ATTRIBUTES

        #region CONSTRUCTORS
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="matlab">The Matlab computations handler object.</param>
        public TrajectoryCreatorHandler()
        {
            //reset the varying index to read from.
            _varyingCurrentIndex = 0;
        }
        #endregion CONSTRUCTORS

        #region FUNCTIONS
        /// <summary>
        /// Setting new trajectory attribute according to a new user input.
        /// </summary>
        /// <param name="trajectoryCreator">The trajectoryCreator class to make and call in order to deliver the trajectory.</param>
        /// <param name="variableList">The variables read from the xlsx protocol file.</param>
        /// <param name="crossVaryingVals">Final list holds all the current cross varying vals by dictionary of variables with values for each line(trial) for both ratHouseParameters.</param>
        /// <param name="staticVariables">The static variables list in double value presentation.</param>
        /// <param name="frequency">The numbers of samples for each trajectory.</param>
        public void SetTrajectoryAttributes(ITrajectoryCreator trajectoryCreator, Variables variableList, List<Dictionary<string, double>> crossVaryingVals, Dictionary<string, List<double>> staticVariables, int frequency)
        {
            //set the variables.
            _variablesList = variableList;
            _crossVaryingVals = crossVaryingVals;
            _staticVals = staticVariables;
            _frequency = frequency;

            //arguments for the ITrajectoryCreator constructor.
            object[] args = new object[4];
            args[0] = _variablesList;
            args[1] = _crossVaryingVals;
            args[2] = _staticVals;
            args[3] = _frequency;

            _trajectoryCreator = trajectoryCreator;
        }

        /// <summary>
        /// Create a trajectory the landscapeHouseTrajectory for the control loop.
        /// </summary>
        /// <returns>The landscapeHouseTrajectory.</returns>
        public Tuple<Trajectory, Trajectory> CreateTrajectory(int index = 0 , bool returnHomeCommand = false)
        {
            return _trajectoryCreator.CreateTrialTrajectory(index, returnHomeCommand);
        }
        #endregion FUNCTIONS
    }
}

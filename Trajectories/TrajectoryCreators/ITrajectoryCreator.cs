using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace Trajectories.TrajectoryCreators
{
    /// <summary>
    /// Describes all base functions and members that protocol trajectory creators should implement.
    /// </summary>
    public interface ITrajectoryCreator
    {
        /// <summary>
        /// Generating a vector of sampled gaussian cdf with the given attributes.
        /// </summary>
        /// <param name="duration">The duration for the trajectory.</param>
        /// <param name="sigma">The number of sigmas for the trajectory in the generated gaussian cdf.</param>
        /// Image as example (larger number -> smoother graph):
        /// https://en.wikipedia.org/wiki/Normal_distribution#/media/File:Normal_Distribution_CDF.svg
        /// <param name="magnitude">The magnitude of the trajectory.</param>
        /// <param name="frequency">The number of samples for the gaussian cdf to the trajectory.</param>
        /// <returns>
        /// The sampled gaussian cdf trajectory.
        /// The vector length is as the given frequency.
        /// </returns>
        Vector<double> GenerateGaussianSampledCDF(double duration, double sigma, double magnitude, int frequency);

        /// <summary>
        /// Read the current trial needed parameters and insert them to the object members.
        /// </summary>
        void ReadTrialParameters(int index);

        /// <summary>
        /// Computes the trajectory tuple (for the MoogTrajectory and for the VRTrajectory).
        /// </summary>
        /// <param name="index">The index from the crossVaryingList to take the attributes of he varying variables from.</param>
        /// <returns>The trajectory tuple (for the MoogTrajectory and for the VRTrajectory). </returns>
        Tuple<Trajectory, Trajectory> CreateTrialTrajectory(int index = 0 , bool returnHomeCommand = false);
    }
}

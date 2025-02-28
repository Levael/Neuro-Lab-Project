﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Params;

namespace VaryingValuesGenerators
{
    public abstract class VaryingValuesGeneratorBase
    {
        #region ATTRIBUTES
        /// <summary>
        /// Dictionary holds all static variables.
        /// </summary>
        protected Variable _staticVariables;

        /// <summary>
        /// Dictionary holds all varying variables.
        /// </summary>
        protected Variables _varyingVariables;

        /// <summary>
        /// Dictionary holds all acrossStair variables.
        /// </summary>
        protected Variables _acrossStairVariables;

        /// <summary>
        /// Dictionary holds all withinStair variables.
        /// </summary>
        protected Variables _withinStairVariables;

        /// <summary>
        /// Initial Dictionary (nor updated later) describes all the vectors sorted by all trials for each variables by index for the varying values generator.
        /// The vector for each key string (name  of variable) is the paralleled vector for the other strings (variables).
        /// Each paralleled index in all vector describes a vector for one trial.
        /// </summary>
        protected Dictionary<string, Vector<double>> _varyingVectorDictionary;

        /// <summary>
        ///Initial Matrix (not updated later) holds all the generated varying vectors for the experiment. Each row in the matrix represent a varying trial vector.
        /// The num of the columns should be the number of the trials.
        /// </summary>
        protected Matrix<double> _varyingMatrix;

        /// <summary>
        /// Final list holds all the current cross varying vals by dictionary of variables with values for each line(trial) for both ratHouseParameters and landscapeHouseParameters.
        /// </summary>
        public List<Dictionary<string, double>> _crossVaryingValsBoth;
        #endregion ATTRIBUTES

        #region FUNCTIONS
        /// <summary>
        /// Sets the variables dictionary into new variables dictionaries ordered by statuses.
        /// </summary>
        /// <param name="vars"></param>
        public abstract void SetVariablesValues(Variables vars);

        /// <summary>
        /// Creates all the varying vectors the trial in the experiment would use.
        /// </summary>
        public abstract void MakeTrialsVaringVectors();

        /// <summary>
        /// Getting the list of all varying vector. Each vector is represented by dictionary of variable name and value.
        /// </summary>
        /// <returns>Returns list in the size of generated varying vectors. Each vector represents by the name of the variable and it's value.</returns>
        public abstract List<Dictionary<string, double>> MakeVaryingMatrix();

        /// <summary>
        /// Creates varying vectors list according to the varying vectors variables(the list include each variable as a vector with no connection each other).
        /// </summary>
        public abstract Dictionary<string, Vector<double>> MakeSeperatedVaryingVectorsList();

        /// <summary>
        /// Creates a vector include values from the selected bounds.
        /// </summary>
        /// <param name="lowBound">The low bound to start with.</param>
        /// <param name="highBound">The high bound to end with.</param>
        /// <param name="increment">The increment between each element in the generated vector.</param>
        /// <returns>The generated vector from the input bounds.</returns>
        public Vector<double> CreateVectorFromBounds(double lowBound, double highBound, double increment)
        {
            // example: [-15, 15] with step 5 = {-15, -10, -5, 0, 5, 10, 15} (3+1+3)
            Vector<double> createdVector = Vector<double>.Build.Dense((int)((highBound - lowBound) / increment + 1));
            int index = 0;

            while (lowBound <= highBound)
            {
                createdVector.At(index, lowBound);
                lowBound += increment;
                index++;
            }

            return createdVector;
        }

        /// <summary>
        /// Converts a string numbers array to a double Vector.
        /// </summary>
        /// <param name="vector">The string vector nums.</param>
        /// <returns>The double vector.</returns>
        public Vector<double> CreateVectorFromStringVector(string vector)
        {
            string[] seperatedValues = vector.Split(' ');

            double[] seperatedDoubleValues = new double[seperatedValues.Length];

            for (int i = 0; i < seperatedValues.Length; i++)
            {
                seperatedDoubleValues[i] = double.Parse(seperatedValues[i]);
            }

            return Vector<double>.Build.Dense(seperatedDoubleValues);
        }

        #endregion FUNCTIONS
    }
}

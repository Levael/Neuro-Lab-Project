using System.Collections.Generic;
using System.Linq;

namespace Params
{
    /// <summary>
    /// Class hold a dictionary with all variables names as keys and all variables Variable(attribute list + parameters) as value.
    /// </summary>
    public class Variables
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Variables()
        {

        }

        /// <summary>
        /// The dictionary holds each variable name as a key , and the Variable itself as a value.
        /// </summary>
        public Dictionary<string, Variable> _variablesDictionary
        {
            get;
            set;
        }

        /// <summary>
        /// Filter the variables by a status and returns them.
        /// </summary>
        /// <param name="status">The status to be filtered.</param>
        /// <returns>The filtered Variable dictionary.</returns>
        public Variables FilterVariablesByStatus(string status)
        {
            return FilterVariablesByStatuses(new string[] { status });
        }

        public Variables FilterVariablesByStatuses(string[] statuses)
        {
            Variables filteredVariables = new Variables();

            filteredVariables._variablesDictionary = new Dictionary<string, Variable>();

            foreach (string varName in _variablesDictionary.Keys)
            {
                if (statuses.Count(s => s == _variablesDictionary[varName]._description["status"].MoogParameter) == 1)
                {
                    filteredVariables._variablesDictionary[varName] = new Variable(_variablesDictionary[varName]);
                }
            }

            return filteredVariables;
        }
    }
}

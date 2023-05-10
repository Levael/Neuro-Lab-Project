namespace UniJoy
{
    /// <summary>
    /// Globals variables for the program control.
    /// </summary>
    public static class Globals
    {
        /// <summary>
        /// The system state (running , stopped , paused etc.
        /// </summary>
        public static SystemState _systemState;
    }
    
    /// <summary>
    /// Enum describes the system states.
    /// </summary>
    public enum SystemState
    {
        /// <summary>
        /// The system is running now.
        /// </summary>
        RUNNING = 0,

        /// <summary>
        /// The system has been stopped by the user.
        /// </summary>
        STOPPED = 1,

        /// <summary>
        /// The system has been paused by the user.
        /// </summary>
        PAUSED = 2,

        /// <summary>
        /// The system is now warmed up.
        /// </summary>
        INITIALIZED = 4,

        /// <summary>
        /// The current experiment (all trials) over, waiting for the next command.
        /// </summary>
        FINISHED = 5
    }
}

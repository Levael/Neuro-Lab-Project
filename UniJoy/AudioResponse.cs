using System;
using System.Threading.Tasks;

namespace UniJoy
{
    public static class AudioResponse
    {
        private const int CORRECT_RESP_FREQ = 2200;     // sound, not MOOG freq...
        private const int INCORRECT_RESP_FREQ = 1000;
        private const int TIMOUT_FREQ = 400;
        private const int PLAYING_SOUND_DURATION = 500; // in milliseconds
        private const int START_TRIAL_FREQ = 1800;
        
        private static void PlaySound(int freq, int duration) {
            // play sound in a new thread
            Task.Run(() => Console.Beep(freq, duration));
        }
        
        // may be regular 
        public static void PlayStartTrialSound() {
            PlaySound(START_TRIAL_FREQ, PLAYING_SOUND_DURATION - 300);
        }
        
        public static void PlayTimeOutSound() {
            PlaySound(TIMOUT_FREQ, PLAYING_SOUND_DURATION);
        }
        
        public static void PlayWrongAnswerSound() {
            // play the sound in a new thread.
            PlaySound(INCORRECT_RESP_FREQ, PLAYING_SOUND_DURATION);
        }

        public static void PlayCorrectAnswerSound() {
            PlaySound(CORRECT_RESP_FREQ, PLAYING_SOUND_DURATION);
        }
        
        // old version
        
        /*/// <summary>
        /// Object for handles WindowsMediaPlayer controls to play mp3 files.
        /// </summary>
        private WindowsMediaPlayer _windowsMediaPlayer;*/
        
        /*_soundPlayerPathDB = new Dictionary<string, string>();
            LoadAllSoundPlayers();*/
        /*_windowsMediaPlayer = new WindowsMediaPlayer();*/
        
        /*_windowsMediaPlayer = new WindowsMediaPlayer();*/
        
        /*Task.Run(() => {
            _windowsMediaPlayer.URL = _soundPlayerPathDB["WrongAnswer"];
            _windowsMediaPlayer.controls.play();
         });*/
        
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
    }
}
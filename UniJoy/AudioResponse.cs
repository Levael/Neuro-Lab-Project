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
        
        private static void PlaySound(int freq, int duration) {
            // play sound in a new thread
            Task.Run(() =>
            {
                Console.Beep(freq, duration);
            });
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
    }
}
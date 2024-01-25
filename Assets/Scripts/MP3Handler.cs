using UnityEngine;
using NAudio.Wave;
using System.Threading;
using System.IO;
using System.Collections;

public class MP3Handler : MonoBehaviour
{
    public static MP3Handler instance;
    private Thread audioThread;
    private WaveOutEvent waveOut;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(this); }
    }
    private void Update()
    {
        if (waveOut == null) return;

        waveOut.Volume = SettingsManager.instance.playerSettings.musicVolume;
    }

    public IEnumerator PlaySong(string fileName)
    {
        string filePath = Application.persistentDataPath + "/Songs/" + fileName + ".mp3";
        float bpm = MidiInput.currentSettings.bpm;
        if (!File.Exists(filePath)) { Debug.LogError($"MP3 File \"{fileName}\" not found at path {filePath}"); yield break; }
        audioThread = new Thread(() => ReadMP3File(filePath));
        yield return new WaitUntil(() => GameManager.instance.songTime >= 0);
        audioThread.Start();


    }

    // Function to read the MP3 file
    void ReadMP3File(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("File path is empty or null.");
            return;
        }

        // Create a new WaveFileReader with the provided path
        using (var reader = new Mp3FileReader(path))
        {
            // Create a buffer to read audio data
            byte[] buffer = new byte[4096];

            // Create a WaveOutEvent to play the audio
            waveOut = new WaveOutEvent();

            // Set the WaveOutEvent's WaveStream to the Mp3FileReader
            waveOut.Init(reader);

            // Start playing the audio
            waveOut.Play();

            // Wait for the audio to finish playing
            while (waveOut.PlaybackState == PlaybackState.Playing)
            {
                // You can perform other tasks here while the audio is playing
            }
        }
    }

    // Called when the application loses or gains focus
    public void StopMusic()
    {
        if (audioThread != null)
        {
            waveOut.Stop();
            waveOut.Dispose();
        }
        if (audioThread != null && audioThread.IsAlive)
        {
            audioThread.Abort();
        }
    }

    void OnDestroy()
    {

        // Stop and dispose of the WaveOutEvent and Mp3FileReader when the script is destroyed
        if (waveOut != null)
        {
            waveOut.Stop();
            waveOut.Dispose();
        }

        // Abort the audio playback thread

    }
}


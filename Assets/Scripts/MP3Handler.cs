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
    Coroutine songDemoCoroutine;

    [SerializeField] float songDemoLength;
    [SerializeField] float songDemoFadeDuration;
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
    }
    public void SetVolume(float newVolume)
    {
        if (waveOut == null) return;
        waveOut.Volume = newVolume / 100;
    }
    public IEnumerator PlaySong(string filePath)
    {
        StopMusic();
        if (!File.Exists(filePath)) { Debug.LogError($"MP3 File not found at path {filePath}"); yield break; }
        audioThread = new Thread(() => ReadMP3File(filePath));
        yield return new WaitUntil(() => GameManager.instance.songTime >= 0);
        audioThread.Start();



    }

    public void StartSongDemo(string filePath)
    {
        if (songDemoCoroutine != null)
        {
            StopCoroutine(songDemoCoroutine);
        }
        songDemoCoroutine = StartCoroutine(LoadSongDemo(filePath));
    }
    public IEnumerator LoadSongDemo(string filePath)
    {
        StopMusic();
        if (!File.Exists(filePath))
        {
            Debug.LogError($"MP3 File not found at path {filePath}");
            yield break;
        }

        // Start playing the song
        yield return audioThread = new Thread(() => ReadMP3File(filePath));
        audioThread.Start();
        while (waveOut == null)
        {
            yield return null;
        }

        // Calculate the actual duration of the song being played
        float actualSongDuration = GetSongDuration(filePath);

        // Calculate fade durations based on the actual song duration
        float fadeInDuration = Mathf.Min(actualSongDuration / 2, songDemoFadeDuration);
        float fadeOutDuration = Mathf.Min(actualSongDuration / 2, songDemoFadeDuration);

        float timeElapsed = 0;

        // Fade in
        float currentVolume = PlayerSettings.musicVolume / 100;
        while (timeElapsed <= fadeInDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / fadeInDuration;
            waveOut.Volume = Mathf.Lerp(0f, currentVolume, t);
            yield return null;
        }

        // Wait for the middle part of the song
        float temp = Mathf.Clamp(songDemoLength - (2 * songDemoFadeDuration), 0, 5); // make this shorter when song length is shorter than 2x songdemofade.
        if (temp > 0)
        {
            yield return new WaitForSecondsRealtime(temp);
        }
        currentVolume = PlayerSettings.musicVolume / 100;

        // Fade out
        timeElapsed = 0;
        while (timeElapsed <= fadeOutDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / fadeOutDuration;
            waveOut.Volume = Mathf.Lerp(currentVolume, 0f, t);
            yield return null;
        }
        currentVolume = PlayerSettings.musicVolume / 100;
        SetVolume(currentVolume);
        StopMusic();

    }
    private float GetSongDuration(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            Debug.LogError("File path is invalid or does not exist.");
            return 0f;
        }

        using (Mp3FileReader reader = new Mp3FileReader(filePath))
        {
            return (float)reader.TotalTime.TotalSeconds;
        }
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
        if (songDemoCoroutine != null)
        {
            StopCoroutine(songDemoCoroutine);
            SetVolume(PlayerSettings.musicVolume);
        }
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





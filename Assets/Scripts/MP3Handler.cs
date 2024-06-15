using UnityEngine;
using NAudio.Wave;
using System.Threading;
using System.IO;
using System.Collections;
/// <summary>
/// Handles reading and playback of .MP3 Files.
/// </summary>
public class MP3Handler : MonoBehaviour
{
    public static MP3Handler instance;
    /// <summary>
    /// Thread used for audio playback.
    /// </summary>
    private Thread audioThread;
    /// <summary>
    /// WaveOut used to read and play MP3 Audio.
    /// </summary>
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
    }
    /// <summary>
    /// set volume of Mp3 Playback.
    /// </summary>
    public void SetVolume(float newVolume)
    {
        if (waveOut == null) return;
        waveOut.Volume = newVolume / 100;

    }
    /// <summary>
    /// Start parsing and playing the mp3 file.
    /// </summary>
    public IEnumerator PlaySong(string filePath)
    {
        StopMusic();
        if (!File.Exists(filePath)) { Debug.LogError($"MP3 File not found at path {filePath}"); yield break; }
        audioThread = new Thread(() => ReadMP3File(filePath));
        yield return new WaitUntil(() => GameManager.instance.songTime >= 0);
        try { audioThread.Start(); } catch { Debug.Log("Error Starting thread."); }
    }
   
    /// <summary>
    /// read Mp3 file at "Path" to play back through NAudio.
    /// </summary>
    /// <param name="path"></param>
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

    /// <summary>
    /// Stops all music playing from NAudio.
    /// </summary>
    public void StopMusic()
    {
        if (audioThread != null)
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
            }
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





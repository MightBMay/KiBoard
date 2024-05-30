using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPiano : MonoBehaviour
{
    public static SpawnPiano instance;
    [SerializeField] GameObject pianoWhiteTilePrefab;
    [SerializeField] GameObject keyParticlePrefab;
    /// <summary>
    /// array of sprites for each key of the piano.
    /// </summary>
    [SerializeField] SpriteRenderer[] spriterenderers = new SpriteRenderer[88];
    /// <summary>
    /// colour when incorrectly pressed/timed.
    /// </summary>
    [SerializeField] static Color enabledColour = new Color(255, 0, 0, 128);
    /// <summary>
    /// colour when perfectly timed
    /// </summary>
    [SerializeField] static Color perfectColour = new Color(255, 0, 195, 128);
    /// <summary>
    /// colour when pressed with good timing.
    /// </summary>
    [SerializeField] static Color goodColour = new Color(0.15f, 1, .5f, 128);
    /// <summary>
    /// colour when pressed with okay timing.
    /// </summary>
    [SerializeField] static Color okayColour = new Color(0, 9, 255, 128);
    /// <summary>
    /// Lane colour 1.
    /// </summary>
    [SerializeField] Color lane1;
    /// <summary>
    /// lane colour 2.
    /// </summary>
    [SerializeField] Color lane2;

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(instance.gameObject); }
    }
    /* unused, used to spawn piano tiles and lanes during runtime.
        void spawnPiano()
        {
            for (int i = 0; i < 88; i++)
            {
                // create piano tile, set position, and colour every other one.
                Transform tile = Instantiate(pianoWhiteTilePrefab, transform.position + Vector3.right * 0.2045455f * (i), Quaternion.identity).transform;
                tile.SetParent(transform.parent);
                spriterenderers[i] = tile.GetComponent<SpriteRenderer>();
                spriterenderers[i].color = GetDefaultKeyColour(i);
                var lane = tile.GetChild(0).GetComponent<SpriteRenderer>();
                lane.color = i % 2 == 0 ? lane1 : lane2;
                lane.transform.position += (Vector3.up * lane.transform.localScale.y/2);


            }
        }*/
    /// <summary>
    /// gets the index of a specific keys sprite renderer.
    /// </summary>
    /// <param name="sr"></param>
    /// <returns></returns>
    public int GetIndexOfSpriteRenderer(SpriteRenderer sr)
    {
        return Array.IndexOf(spriterenderers, sr);
    }
    /// <summary>
    /// gets default colour for a key based on key number.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public Color GetDefaultKeyColour(int i)
    {
        return CheckBlackNote(i + 1) ? Color.black : Color.white;
    }
    /// <summary>
    /// resets colours of all keys on the keyboard.
    /// </summary>
    public void ClearAllKeyColours()
    {
        for (int i = 0; i < 88; i++)
        {
            spriterenderers[i].color = GetDefaultKeyColour(i);
        }
    }
    /// <summary>
    /// is the note at index I a black note?
    /// </summary>
    static bool CheckBlackNote(int i)
    {
        int value = i % 12;   
        if(GameSettings.usePiano) return value == 2 || value == 5 || value == 7 || value == 10 || (value == 0 && i != 0);
        return value == 0 || value == 2 ||value == 5 || value == 7 || value == 10 ;

    }
    /// <summary>
    /// Updates the colours of keys when notes are pressed based off of the timing score.
    /// </summary>
    /// <param name="noteNumber"> note number of the note pressed.</param>
    /// <param name="enabled"> is the note being enabled or disabled?</param>
    /// <param name="timingScore"> timing score of the note.</param>
    public void UpdateKeyColours(int noteNumber, bool enabled, string timingScore = "")
    {
        if (GameSettings.usePiano)
        {
            spriterenderers[noteNumber].color = GetKeyColour(noteNumber, enabled, timingScore);
        }
        else
        {
            spriterenderers[(noteNumber-3 )% 12].color = GetKeyColour(noteNumber, enabled, timingScore);
        }
    }
    /// <summary>
    /// Get colour of a key based off of timing score.
    /// </summary>
    public static Color GetKeyColour(int keyNum, string timingScore = "")
    {
        switch (timingScore)
        {
            case "Perfect":
                return perfectColour;

            case "Good":
                return goodColour;

            case "Okay":
                return okayColour;

            default:
                return enabledColour;

        }

    }
    /// <summary>
    /// Get colour of a key based off of timing score taking in to account notes being released.
    /// </summary>

    public static Color GetKeyColour(int keyNum, bool isKeyEnabled, string timingScore = "")
    {

        if (!isKeyEnabled)
        {
            if (CheckBlackNote(keyNum + 1)) { return Color.black; }
            else { return Color.white; }
        }
        switch (timingScore)
        {
            case "Perfect":
                return perfectColour;

            case "Good":
                return goodColour;

            case "Okay":
                return okayColour;
            default:
                return enabledColour;

        }

    }

    /// <summary>
    /// spawn particle effect at pressed key.
    /// </summary>
    /// <param name="keyNum">key number to spawn the particles at.</param>
    /// <param name="score"> timing score used for colouring the particles.</param>
    public void SpawnKeyParticle(int keyNum, string score = "")
    {
        ParticleSystem particle;
        if (GameSettings.usePiano) { particle = Instantiate(keyParticlePrefab, spriterenderers[keyNum].transform).GetComponent<ParticleSystem>(); }
        else { particle = Instantiate(keyParticlePrefab, spriterenderers[(keyNum-3 )%12].transform).GetComponent<ParticleSystem>(); }
        particle.transform.position += Vector3.up / 2;
        ParticleSystem.MainModule main = particle.main;
        main.startColor = GetKeyColour(keyNum, score);
        particle.Play();
        Destroy(particle, 5f);
    }
}

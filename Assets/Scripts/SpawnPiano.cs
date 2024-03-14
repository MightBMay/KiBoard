using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPiano : MonoBehaviour
{
    public static SpawnPiano instance;
    [SerializeField] GameObject pianoWhiteTilePrefab, keyParticlePrefab;
    [SerializeField] SpriteRenderer[] spriterenderers = new SpriteRenderer[88];
    [SerializeField] static Color enabledColour = new Color(255, 0, 0, 128), perfectColour = new Color(255, 0, 195, 128), goodColour = new Color(0.15f, 1, .5f, 128), okayColour = new Color(0, 9, 255, 128);
    [SerializeField] Color lane1, lane2;

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

    public int GetIndexOfSpriteRenderer(SpriteRenderer sr)
    {
        return Array.IndexOf(spriterenderers, sr);
    }
    public Color GetDefaultKeyColour(int i)
    {
        return CheckBlackNote(i + 1) ? Color.black : Color.white;
    }
    public void ClearAllKeyColours()
    {
        for (int i = 0; i < 88; i++)
        {
            spriterenderers[i].color = GetDefaultKeyColour(i);
        }
    }

    static bool CheckBlackNote(int i)
    {
        return i % 12 == 2 || i % 12 == 5 || i % 12 == 7 || i % 12 == 10 || (i % 12 == 0 && i != 0);
    }

    public void UpdateKeyColours(int noteNumber, bool enabled, string timingScore = "")
    {
        spriterenderers[noteNumber].color = GetKeyColour(noteNumber, enabled, timingScore);
    }

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

    public void SpawnKeyParticle(int keyNum, string score = "")
    {
        var particle = Instantiate(keyParticlePrefab, spriterenderers[keyNum].transform).GetComponent<ParticleSystem>();
        particle.transform.position += Vector3.up / 2;
        ParticleSystem.MainModule main = particle.main;
        main.startColor = GetKeyColour(keyNum, score);
        particle.Play();
        Destroy(particle, 5f);
    }
}

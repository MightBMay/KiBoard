using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SongVersionMenu : MonoBehaviour
{
    public static SongVersionMenu instance;
    [SerializeField] GameObject songVersionPrefab;
    [SerializeField] Transform itemHolder;
    [SerializeField] GameObject versionMenu;
    [SerializeField] List<Button> itemList = new List<Button>();
    [SerializeField] List<MiniFileGroup> files = new List<MiniFileGroup>();
    List<string> allPaths = new List<string>();
    public Button selectedButton;
    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(this); }
    }

    public void OpenMenu(FileGroup fileGroup)
    {
        // Clear the itemList
        itemList.ForEach(item => { Destroy(item.gameObject); });
        itemList.Clear();

        // Set the versionMenu to active
        versionMenu.SetActive(true);

        // Combine MIDI and JSON file paths into one list
        allPaths.Clear();
        allPaths.AddRange(fileGroup.MidiFiles);
        allPaths.AddRange(fileGroup.JsonFiles);

        // Group files with the same name but different extensions
        files.Clear();
        foreach (string path in allPaths)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            MiniFileGroup miniFileGroup = files.FirstOrDefault(file => file.fileName == fileName);
            if (miniFileGroup != null)
            {
                miniFileGroup.AddPath(path);
            }
            else
            {
                miniFileGroup = new MiniFileGroup(fileName);
                miniFileGroup.AddPath(path);
                files.Add(miniFileGroup);
            }
        }

        // Instantiate SongVersionItem objects
        foreach (MiniFileGroup miniFileGroup in files)
        {
            SongVersionItem svi = Instantiate(songVersionPrefab, itemHolder).GetComponent<SongVersionItem>();
            svi.SetValues(miniFileGroup.midiPath, miniFileGroup.jsonPath);
            itemList.Add(svi.GetComponent<Button>());
        }
        if(selectedButton != null)selectedButton.interactable = true;
        selectedButton = itemList[0];
        selectedButton.interactable = false;
    }

    [System.Serializable]
    private class MiniFileGroup
    {
        public string fileName;
        public string midiPath;
        public string jsonPath;

        internal MiniFileGroup(string filename)
        {
            this.fileName = filename;
        }

        public void AddPath(string path)
        {
            string extension = Path.GetExtension(path);
            switch (extension)
            {
                case ".mid":
                    midiPath = path;
                    break;
                case ".json":
                    jsonPath = path;
                    break;
                default:
                    break;
            }
        }
    }
}

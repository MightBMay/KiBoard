using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
public class SongList : MonoBehaviour
{
    public static SongList instance;
    [SerializeField] GameObject songItemPrefab;
    SongItem[] songItemList;
    public SongItem currentlySelected;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }
    }
    public void SelectItem(SongItem item)
    {
        if (currentlySelected != null) { DeselectItem(); }
        currentlySelected = item;
        currentlySelected.GetComponentInChildren<Button>().interactable = false;
    }

    public void DeselectItem()
    {
        
        currentlySelected.GetComponentInChildren<Button>().interactable = true;
        currentlySelected = null;
    }
    public void SpawnSongItems(List<FileGroup> fileGroups)
    {
        songItemList = new SongItem[fileGroups.Count];
        for (int i = 0; i < fileGroups.Count; i++)
        {
            var group = fileGroups[i];
            SongItem songItem = Instantiate(songItemPrefab, transform).GetComponent<SongItem>();
            songItem.gameObject.name = songItem.songName.text = group.FileName;
            //songItem.songDuration.text = "duration";
            songItem.fileGroup = group;
            songItem.GetComponentInChildren<RawImage>().texture = group.LoadImageFromFile(group.PngFile);
            string str = group.CheckFileGroupContents();
            songItem.songContains.text = str;
            if (str.Contains("Midi AND Json file not found")) { songItem.GetComponent<Button>().interactable = false; }
            songItemList[i] = songItem;

        }
    }
}

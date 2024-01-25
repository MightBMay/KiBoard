using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SongList : MonoBehaviour
{
    [SerializeField] GameObject songItemPrefab;
    SongItem[] songItemList;

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

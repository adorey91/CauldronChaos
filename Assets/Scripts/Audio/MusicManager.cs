using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public struct Song
{
    public enum SongType
    {
        MainMenuMusic,
        ManagementMusic,
        PracticeMusic,
        GameplayMusic
    }

    public SongType type;
    public AudioClip song;
}

public class MusicManager : MonoBehaviour
{
    [Header("Music Output")]
    [SerializeField] private AudioSource musicSource;

    [Header("Songs")]
    [SerializeField] private List<Song> songs;

    public void PlayMusic(Song.SongType songType)
    {
        for (int i = 0; i < songs.Count; i++)
        {
            if (songs[i].type == songType)
            {
                musicSource.clip = songs[i].song;
                musicSource.Play();
            }
        }
    }
}

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
        None,
        GameplayMusic
    }

    public SongType type;
    public AudioClip song;
}

public class MusicManager : MonoBehaviour
{
    [Header("Music Output")]
    public AudioSource musicSource;

    [Header("Songs")]
    [SerializeField] private List<Song> songs;
    private Song.SongType currentSong = Song.SongType.None;

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
    }

    public void PlayMusic(Song.SongType songType)
    {
        for (int i = 0; i < songs.Count; i++)
        {
            if (songs[i].type == songType)
            {
                currentSong = songs[i].type;
                if (musicSource != null)
                {
                    musicSource.clip = songs[i].song;
                    musicSource.Play();
                }
            }
        }
    }

    public Song.SongType GetCurrentMusic()
    {
        return currentSong;
    }
}

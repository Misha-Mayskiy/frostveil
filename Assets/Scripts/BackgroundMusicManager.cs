using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public AudioClip[] musicTracks; // Массив с треками
    public AudioSource audioSource; // Источник аудио

    private int currentTrackIndex = -1; // Индекс текущего трека

    void Start()
    {
        currentTrackIndex = Random.Range(0, musicTracks.Length);

        if (musicTracks.Length == 0)
        {
            Debug.LogWarning("No music tracks assigned!");
            return;
        }

        audioSource = GetComponent<AudioSource>();

        PlayNextTrack(); // Запускаем первый трек
    }

    void Update()
    {
        // Если текущий трек завершен, переключаемся на следующий
        if (!audioSource.isPlaying)
        {
            PlayNextTrack();
        }
    }

    void PlayNextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Length; // Переходим к следующему треку
        audioSource.clip = musicTracks[currentTrackIndex];
        audioSource.Play();
    }

    // Вы можете добавить метод для изменения громкости через ползунок:
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}

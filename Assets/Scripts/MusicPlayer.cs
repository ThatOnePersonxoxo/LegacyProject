using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] tracks;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayRandomTrack();
        Debug.Log("MusicPlayer started in: " + gameObject.scene.name);
    }

    void PlayRandomTrack()
    {
        int index = Random.Range(0, tracks.Length);

        audioSource.clip = tracks[index];
        audioSource.loop = true;
        audioSource.Play();
    }
}


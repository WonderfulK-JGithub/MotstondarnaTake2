using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedAudioManager : MonoBehaviour
{
    public static AdvancedAudioManager current;

    [SerializeField] GameObject audioPrefab;

    List<AudioSource> currentSounds = new List<AudioSource>();
    Dictionary<AudioClip, int> currentCounts = new Dictionary<AudioClip, int>();

    public AudioClip[] audioClips;

    private void Awake()
    {
        current = this;
    }

    private void Update()
    {
        for (int i = 0; i < currentSounds.Count; i++)
        {
            if (!currentSounds[i].isPlaying)
            {
                currentCounts[currentSounds[i].clip]--;
                Destroy(currentSounds[i].gameObject);
                currentSounds.RemoveAt(i);
                i--;

            }
        }
    }

    public AudioSource PlayLoopedSound(AudioClip clip)
    {
        AudioSource newAudio = Instantiate(audioPrefab).GetComponent<AudioSource>();
        newAudio.clip = clip;
        newAudio.loop = true;

        newAudio.Play();

        return newAudio;
    }

    public void CancelAndPlaySound(AudioClip clip, int limit = 0)
    {
        for (int i = 0; i < currentSounds.Count; i++)
        {
            if(currentSounds[i].clip == clip)
            {
                if(currentCounts[clip] >= limit)
                {
                    Destroy(currentSounds[i].gameObject);
                    currentCounts[currentSounds[i].clip]--;
                    currentSounds.RemoveAt(i);
                    i--;
                }
                
            }
        }

        AudioSource newAudio = Instantiate(audioPrefab).GetComponent<AudioSource>();
        newAudio.clip = clip;
        newAudio.Play();

        if (currentCounts.ContainsKey(clip)) currentCounts[clip]++;
        else currentCounts[clip] = 1;

        currentSounds.Add(newAudio);
    }

    public void PlayUnderLimit(AudioClip clip, int limit = 1)
    {
        if(currentCounts.ContainsKey(clip) && currentCounts[clip] < limit)
        {
            AudioSource newAudio = Instantiate(audioPrefab).GetComponent<AudioSource>();
            newAudio.clip = clip;
            newAudio.Play();

            if (currentCounts.ContainsKey(clip)) currentCounts[clip]++;
            else currentCounts[clip] = 1;

            currentSounds.Add(newAudio);
        }

        
    }
}


public enum AUDIO
{
    LASER,
    PIN,
}
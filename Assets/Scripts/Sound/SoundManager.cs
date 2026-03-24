using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
[RequireComponent(typeof(AudioSource)),ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    
    AudioSource audioSource;
    private static SoundManager instance;
    public  SoundStruct[] soundStructs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
    }


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    
    public static void PlayRandomSoundFromType(SoundType soundType, float intensity)
    {
        AudioClip[] clips = instance.soundStructs[(int)soundType].audioClip;
        int randomint = UnityEngine.Random.Range(0, clips.Length-1);

        if (!instance.audioSource.IsUnityNull())
        {
            instance.audioSource.PlayOneShot(clips[randomint], intensity);
        }
        
        
        
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));    
        Array.Resize(ref soundStructs, names.Length);
        for (int i = 0;i < names.Length; i++)
        {
            soundStructs[i].name = names[i];
        }
    }
#endif
}



[System.Serializable]
public struct SoundStruct
{
     public  string name;
    public AudioClip[] audioClip;
}
[System.Serializable]
public enum SoundType
{
    Mine,
    Chop,
    Gather,
    Build,
    Walk,
    BuildOver,
    BuildStep,
    Attack,
    Die,
    Falling
}

using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    public static float volumn = 1f;
    public enum Sound
    {
        PlayerWalk,
        PlayerSprint,
        PlayerCrouch,
        PlayerJump,
        Landing,
        BedBounce,
        SlowMotion,
        Congratulation,
        SadMusic,
        Theme
    }
    private static Dictionary<Sound, float> soundTimerDictionary;

    private static GameObject soundGameObject;
    private static AudioSource audioSource;

    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.PlayerWalk] = 0;
        soundTimerDictionary[Sound.PlayerSprint] = 0;
        soundTimerDictionary[Sound.PlayerCrouch] = 0;
    }
    public static void PlaySound(Sound sound)
    {      
        if (CanPlaySound(sound))
        {
            if (soundGameObject == null)
            {
                soundGameObject = new GameObject();
                audioSource = soundGameObject.AddComponent<AudioSource>();
            }
            audioSource.volume = volumn;
            audioSource.PlayOneShot(GetAudioClip(sound));
        }
    }
    private static bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            default:
                return true;
            case Sound.PlayerWalk:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerFootStepTimerMax = 0.4f;
                    if (lastTimePlayed + playerFootStepTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case Sound.PlayerSprint:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerSprintTimerMax = 0.25f;
                    if (lastTimePlayed + playerSprintTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case Sound.PlayerCrouch:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerCrouchTimerMax = 0.5f;
                    if (lastTimePlayed + playerCrouchTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }


        }
    }
    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.Instance.soundAudioClips)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        return null;
    }
    public static float GetSoundLength(Sound sound)
    {
        AudioClip audioClip = GetAudioClip(sound);
        return (audioClip != null) ? audioClip.length : 0;
    }
    public static void StopSound()
    {
        audioSource.Stop();
    }

}

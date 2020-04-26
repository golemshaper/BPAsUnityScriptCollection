using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("VolumeControls:")]
    [SerializeField] float masterVolume = 1f;
    [SerializeField] float sfxVolume = 1f;
    [SerializeField] float musicVolume = 1f;
    [SerializeField] int NumberOfChannels = 20;
    AudioSource[] sfxPlayers;
    [SerializeField] AudioSource musicPlayerA;
    [SerializeField] AudioSource musicPlayerB;
    private static AudioManager instance;
    
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                //singleton creation.
                GameObject audioManagerGo = new GameObject();
                audioManagerGo.name = "$AudioManger";
                audioManagerGo.AddComponent<AudioManager>();
                instance = audioManagerGo.GetComponent<AudioManager>();

            }
            return instance;
        }
        set
        {
            instance = value;
        }
    }

    public float MasterVolume
    {
        get
        {
            return masterVolume;
        }

        set
        {
            masterVolume = value;
        }
    }

    public float SfxVolume
    {
        get
        {
            return sfxVolume;
        }

        set
        {
            sfxVolume = value;
        }
    }

    public float MusicVolume
    {
        get
        {
            return musicVolume;
        }

        set
        {
            musicVolume = value;
        }
    }
    public AudioClip GetCurrentMusic()
    {
        if (musicPlayerA == null) return null;
        if (musicPlayerB == null) return null;

        if (useAudioPlayerA)
        {
            return musicPlayerA.clip;
        }
        return musicPlayerB.clip;
    }
    void Awake()
    {
        //add sfx player:
        //  sfxPlayer = this.gameObject.AddComponent<AudioSource>();
        //  sfxPlayer.volume =  SfxVolume * MasterVolume;
        //add music players: (2 so we can crossfade)
        CreateSFXPlayers(NumberOfChannels);
         if(musicPlayerA==null)  musicPlayerA = this.gameObject.AddComponent<AudioSource>();
        musicPlayerA.volume = MusicVolume * MasterVolume;
        if (musicPlayerB == null) musicPlayerB = this.gameObject.AddComponent<AudioSource>();
        musicPlayerB.volume = MusicVolume * MasterVolume;
        instance = this;
    }
    void CreateSFXPlayers(int numberOfChannels)
    {
        sfxPlayers = new AudioSource[numberOfChannels];
        for(int i=0; i< numberOfChannels;i++)
        {
            sfxPlayers[i]= this.gameObject.AddComponent<AudioSource>();
        }
    }
    public void PlaySfx(AudioClip clip)
    {
        
        PlaySfx(clip, 1f);
    }
    public void PlaySfx(AudioClip clip,Vector3 pos,float volume)
    {
        AudioSource.PlayClipAtPoint(clip, pos, volume * sfxVolume);
       
    }
    public void PlaySfx(AudioClip[] clip, Vector3 pos, float volume)
    {
        if (clip == null) return;
        if (clip.Length == 0) return;
        int selectSfx = GameManager.instance.RandomMain().Next(0, clip.Length);
        PlaySfx(clip[selectSfx],pos,volume);
    }
    public void PlaySfx(AudioClip[] clip, Vector3 pos)
    {
        if (clip == null) return;
        if (clip.Length == 0) return;
        int selectSfx = GameManager.instance.RandomMain().Next(0, clip.Length);
        PlaySfx(clip[selectSfx], pos, 1f);
    }
    public void PlaySfx(AudioClip clip, Vector3 pos)
    {
        PlaySfx(clip, pos, 1f);
    }
    int currentChannel = 0;
    public void PlaySfx(AudioClip clip, float volume)
    {
        PlaySfx(clip, volume, false);

    }
    public void PlaySfx(AudioClip clip,float volume,bool onlyOneAtATime)
    {
        if (clip == null) return;
        if (onlyOneAtATime)
        {
            foreach(AudioSource a in sfxPlayers)
            {
                if (a.clip == clip) return;//don't try to play this sound.
            }
        }
        sfxPlayers[currentChannel].clip = clip;
        sfxPlayers[currentChannel].volume =  SfxVolume* volume * MasterVolume;
        sfxPlayers[currentChannel].loop = false;
        sfxPlayers[currentChannel].Play();
        currentChannel++;
        if (currentChannel >= sfxPlayers.Length) currentChannel = 0;
    }
    public void PlaySfx(AudioClip clip, float volume, bool onlyOneAtATime,bool loop)
    {
        if (clip == null) return;
        if (onlyOneAtATime)
        {
            foreach (AudioSource a in sfxPlayers)
            {
                if (a.clip == clip) return;//don't try to play this sound.
            }
        }
        sfxPlayers[currentChannel].clip = clip;
        sfxPlayers[currentChannel].volume = SfxVolume * MasterVolume;
        sfxPlayers[currentChannel].loop = loop;
        sfxPlayers[currentChannel].Play();
        currentChannel++;
        if (currentChannel >= sfxPlayers.Length) currentChannel = 0;
    }
    public void StopSFX(AudioClip clip)
    {
        foreach (AudioSource a in sfxPlayers)
        {
            if (a.clip == clip)
            {
                a.loop = false;
                a.Stop();
            }
        }
    }
    public void PlaySfx(AudioClip[] clip)
    {
        if (clip == null) return;
        if (clip.Length == 0) return;
        int selectSfx = GameManager.instance.RandomMain().Next(0, clip.Length);
        PlaySfx(clip[selectSfx]);
    }
    public void PlaySfx(AudioClip[] clip,float volume)
    {
        int selectSfx = GameManager.instance.RandomMain().Next(0, clip.Length);
        if (clip == null) return;
        if (clip.Length == 0) return;

        PlaySfx(clip[selectSfx], volume);
    }
    public void PlaySfx(AudioClip[] clip, float volume, bool onlyOneAtATime)
    {
        int selectSfx = GameManager.instance.RandomMain().Next(0, clip.Length);
        PlaySfx(clip[selectSfx], volume, onlyOneAtATime);
    }
    bool useAudioPlayerA = false;
    public void PlayMusic(AudioClip music)
    {
        PlayMusic(music,1f);
    }
    public void PlayMusic(AudioClip music,float modVolume)
    {
        if (useAudioPlayerA)
        {
            if (musicPlayerA.clip == music) return;
        }
        else
        {
            if (musicPlayerB.clip == music) return;
        }
        useAudioPlayerA = !useAudioPlayerA;
        
        if(useAudioPlayerA)
        {
            //change this later once cross fading is built
            targetVolume = modVolume * MasterVolume * MusicVolume;
           // musicPlayerA.volume = modVolume * MasterVolume * musicVolume;
            musicPlayerA.clip = music;
            musicPlayerA.loop = true;
            musicPlayerA.Play();
            
        }
        else
        {
            //change this later once cross fading is built
            targetVolume = modVolume * MasterVolume * MusicVolume;
           // musicPlayerB.volume = modVolume * MasterVolume * musicVolume;
            musicPlayerB.clip = music;
            musicPlayerB.loop = true;
            musicPlayerB.Play();
      
        }
    }
    public void Update()
    {
        if (useAudioPlayerA)
        {
            musicPlayerA.volume = Mathf.Lerp(musicPlayerA.volume, targetVolume, crossfadeSpeed * Time.deltaTime);
            musicPlayerB.volume = Mathf.Lerp(musicPlayerB.volume, 0f, crossfadeSpeed * Time.deltaTime);

        }
        else
        {
            musicPlayerB.volume = Mathf.Lerp(musicPlayerB.volume, targetVolume, crossfadeSpeed * Time.deltaTime);
            musicPlayerA.volume = Mathf.Lerp(musicPlayerA.volume, 0f, crossfadeSpeed * Time.deltaTime);
        }
    }
    const float crossfadeSpeed = 2f;
    float targetVolume = 0f;
}

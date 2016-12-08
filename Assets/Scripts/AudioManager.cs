using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour, IGameManager {
    [SerializeField] private AudioSource soundSource;//播放任何2D音乐用的 hierarchy中为Audio, 播放按键按下时的声音
    [SerializeField] private AudioSource music1Source;//播放场景中的背景音乐
    [SerializeField] private AudioSource music2Source;//用于切换背景音乐时的渐入渐出
    [SerializeField] private string introBGMusic;
    [SerializeField] private string levelBGMusic;
    private AudioSource _activeMusic;
    private AudioSource _inactiveMusic;
    public float crossFadeRate = 1.5f;
    private bool _crossFading;//正在淡入淡出时用于避免bug的开关

    private float _musicVolume;

    public float musicVolume { //音乐的声音
        get {
            return _musicVolume;
        }
        set {
            _musicVolume = value;
            if (music1Source != null && !_crossFading) {
                music1Source.volume = _musicVolume;
                music2Source.volume = _musicVolume;
            }
        }
    }

    public bool musicMute {
        get {
            if (music1Source != null) {
                return music1Source.mute;
            }
            return false;
        }
        set {
            if (music1Source != null) {
                music1Source.mute = value;
                music2Source.mute = value;
            }
        }
    }

    public ManagerStatus status {
        get;
        private set;
    }
    
    public float soundVolume {
        get {
            return AudioListener.volume;
        }
        set {
            AudioListener.volume = value;
        }
    }

    public bool soundMute {
        get {
            return AudioListener.pause;
        }
        set {
            AudioListener.pause = value;
        }
    }

    private NetworkService _network;

    public void Startup(NetworkService service) {
        Debug.Log("AudioManager starting...");
        _network = service;

        music1Source.ignoreListenerVolume = true;
        music1Source.ignoreListenerPause = true;
        music2Source.ignoreListenerVolume = true;
        music2Source.ignoreListenerPause = true;

        soundVolume = 1f;
        musicVolume = 1f;

        _activeMusic = music1Source;//初始化 1激活
        _inactiveMusic = music2Source;

        status = ManagerStatus.Started;
    }

    public void PlaySound(AudioClip clip) {
        soundSource.PlayOneShot(clip);
    }

    public void PlayIntroMusic() {
        PlayMusic(Resources.Load("Music/" + introBGMusic) as AudioClip);
    }

    public void PlayLevelMusic() {
        PlayMusic(Resources.Load("Music/" + levelBGMusic) as AudioClip);
    }

    private void PlayMusic(AudioClip clip) {
        if (_crossFading) {
            return;
        }
        StartCoroutine(CrossFadeMusic(clip));//启用协程 切换音乐
        Debug.Log("1");
    }

    private IEnumerator CrossFadeMusic(AudioClip clip)  {
        _crossFading = true;
        _inactiveMusic.clip = clip;
        _inactiveMusic.volume = 0;
        _inactiveMusic.Play();
        Debug.Log("2");

        float counter = 0f;
        float scaleRate = crossFadeRate * _musicVolume;
        while (_activeMusic.volume > 0) {
            counter++;
            Debug.Log(5*counter);
            _activeMusic.volume -= scaleRate * Time.deltaTime;
            _inactiveMusic.volume += scaleRate * Time.deltaTime;
            yield return null;//暂停一帧 现在停止这个方法，然后在下一帧中从这里重新开始！
            Debug.Log(3*counter);
        }
        Debug.Log("4");
        AudioSource temp = _activeMusic;
        _activeMusic = _inactiveMusic;
        _activeMusic.volume = _musicVolume;
        _inactiveMusic = temp;
        _inactiveMusic.Stop();

        _crossFading = false;
    }

    public void StopMusic() {
        _activeMusic.Stop();
        _inactiveMusic.Stop();
    }
}

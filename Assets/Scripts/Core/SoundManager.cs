using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;
    
    
    
    
    [SerializeField] private List<SoundData> _sounds;
    
    
    
    
    private Dictionary<string, SoundData> _soundDict;
    private float _musicVolume = 1f;
    private float _sfxVolume = 1f;
    
    
    public static SoundManager Instance {get; private set;}
    
    private void Awake() {
        if (Instance != null) {
            Debug.LogError("Duplicate sound manager detected");
            return;
        }
        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        // Создаем словарь айдишник - звук
        _soundDict = new Dictionary<string, SoundData>();
        foreach (var sound in _sounds) {
            _soundDict[sound.id] = sound;
        }
        
        LoadSettings();
        ApplyVolume();
    }
    
    private void LoadSettings() {
        _musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        _sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    private void ApplyVolume() {
        _musicSource.volume = _musicVolume;
        _sfxSource.volume = _sfxVolume;
        
        // Звуки лупоу
        foreach (var src in _loopSources.Values)
            if (src != null)
                src.volume = _sfxVolume;
    }
    
    
    private Dictionary<string, float> _lastPlayTimes = new Dictionary<string, float>();
    private float _sfxCooldown = 0.05f; // минимальный интервал между одинаковыми звуками

    public void PlaySFX(string id) {
        if (id == "") { // Заглушка
            return;
        }
        if (!_soundDict.TryGetValue(id, out SoundData data)) {
            Debug.Log("Не найден " + id);
            return;
        }

        float lastTime;
        if (_lastPlayTimes.TryGetValue(id, out lastTime)) {
            if (Time.time - lastTime < _sfxCooldown) return; // слишком рано, пропускаем
        }

        _lastPlayTimes[id] = Time.time;
        _sfxSource.PlayOneShot(data.audioClip, data.volume * _sfxVolume);
    }
    
    
    private readonly Dictionary<string, AudioSource> _loopSources = new();
    
    public void PlayLoopSfx(string id) {
        if (!_soundDict.TryGetValue(id, out SoundData data)) {
            Debug.Log("Не найден " + id);
        }
        if (_loopSources.ContainsKey(id))
            return; // уже играет
        
        var src = gameObject.AddComponent<AudioSource>();
        src.clip = data.audioClip;
        src.volume = data.volume;
        src.loop = true;
        src.Play();

        _loopSources[id] = src;
    }
    
    public void StopLoopSfx(string id) {
        if (_loopSources.TryGetValue(id, out var src)) {
            src.Stop();
            Destroy(src);
            _loopSources.Remove(id);
        }
    }

    public void StopAllLoopSfx() {
        foreach (var s in _loopSources.Values) {
            if (s) Destroy(s);
        }
        _loopSources.Clear();
    }


    
    
    public void PlayMusic(AudioClip clip, bool loop = true) {
        _musicSource.clip = clip;
        _musicSource.loop = loop;
        _musicSource.Play();
    }
    
    public void StopMusic() => _musicSource.Stop();
    
    
    
    public void SetMusicVolume(float value) {
        _musicVolume = value;
        _musicSource.volume = _musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
        ApplyVolume();
    }
    
    public void SetSFXVolume(float value) {
        _sfxVolume = value;
        _sfxSource.volume = _sfxVolume;
        PlayerPrefs.SetFloat("SFXVolume", _sfxVolume);
        ApplyVolume();
    }


    

    
    public void MuteMusic(bool mute) {
        _musicSource.mute = mute;
        PlayerPrefs.SetFloat("MusicVolume", mute ? _musicVolume : 0f);
    }
    
    public void MuteSFX(bool mute) {
        _sfxSource.mute = mute;
        PlayerPrefs.SetFloat("SFXVolume", mute ? _sfxVolume : 0f);

    }
}

using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraShake : MonoBehaviour {
    [Header("Noise Profiles")]
    [SerializeField] private NoiseSettings _normalNoise;  // Handheld Tele
    [SerializeField] private NoiseSettings _shakeNoise;   // 6D Shake


    public static CameraShake Instance;
    private CinemachineBasicMultiChannelPerlin _perlin;
    private float _shakeTimer;

    private float _startAmplitude;
    private float _startFrequency;

    private float _targetAmplitude;
    private float _targetFrequency;



    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        var camera = GetComponent<CinemachineVirtualCamera>();
        _perlin = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        // Ќачальные значени€ - плавное прокачивание
        _startAmplitude = _perlin.m_AmplitudeGain;
        _startFrequency = _perlin.m_FrequencyGain;
    }

    public void Shake(float intensity, float time) {
        _perlin.m_NoiseProfile = _shakeNoise;
        _perlin.m_AmplitudeGain = intensity;
        _perlin.m_FrequencyGain = .5f;

        StartCoroutine(ShakeRoutine(time, intensity));
    }

    private IEnumerator ShakeRoutine(float duration, float amplitude) {
        float timer = duration;
        while (timer > 0f) {
            timer -= Time.deltaTime;
            yield return null;
        }

        _perlin.m_NoiseProfile = _normalNoise;
        float t = 0f;
        float restoreDuration = 3f;
        float startAmp = _perlin.m_AmplitudeGain;
        float startFreq = _perlin.m_FrequencyGain;

        while (t < restoreDuration) {
            t += Time.deltaTime;
            float lerp = t / restoreDuration;
            _perlin.m_AmplitudeGain = Mathf.Lerp(startAmp, _startAmplitude, lerp);
            _perlin.m_FrequencyGain = Mathf.Lerp(startFreq, _startFrequency, lerp);
            yield return null;
        }

        _perlin.m_AmplitudeGain = _startAmplitude;
        _perlin.m_FrequencyGain = _startFrequency;
    }

}

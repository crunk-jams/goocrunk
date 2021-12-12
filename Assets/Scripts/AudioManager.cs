using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource bg1AudioSource;
    [SerializeField] private AudioSource bg2AudioSource;
    [SerializeField] private float fadeTime;
    [SerializeField] private AudioSource SFX;
    [SerializeField] private AudioClip ShootSFX;
    [SerializeField] private AudioSource chargeSource;
    [SerializeField] private AudioSource menuAudioSource;
    [SerializeField] private AudioClip hitCheckPointSFX;
    [SerializeField] private AudioClip hitPortalSFX;
    [SerializeField] private AudioClip hitTargetSFX;

    private bool bg1IsActive; 

     [SerializeField] private AudioClip[] bgAudioClips;
    private int currentAudioClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        bg1IsActive = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        fadeInAudio = StartCoroutine(setAudioToFadeInMenu());
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            bg1IsActive = true;
            fadeInAudio = StartCoroutine(setAudioToFadeInMenu());
        }
        else
        {
            StopCoroutine(fadeInAudio);
            StartCoroutine(setAudioToFadeOutMenu());
        }
    }

    Coroutine fadeInAudio;
    IEnumerator setAudioToFadeInMenu()
    {
        var currentBG1Vol = bg1AudioSource.volume;
        var currentBG2Vol = bg2AudioSource.volume;
        menuAudioSource.Play();
        var fadeTimer = 0.0f;
        while (fadeTimer < 3)
        {
            bg1AudioSource.volume = Mathf.Lerp(currentBG1Vol, 0, fadeTimer/3 );
            bg2AudioSource.volume = Mathf.Lerp(currentBG2Vol, 0, fadeTimer/3);
            menuAudioSource.volume = Mathf.Lerp(0, 1, fadeTimer/3);
            fadeTimer += Time.deltaTime;
            yield return null;
        }
        bg1AudioSource.volume = 0;
        bg2AudioSource.volume = 0;
        bg1AudioSource.Stop();
        bg2AudioSource.Stop();
        menuAudioSource.volume = 1;
    }
    IEnumerator setAudioToFadeOutMenu()
    {
        var currentMenuVol = menuAudioSource.volume;
        var fadeTimer = 0.0f;
        while (fadeTimer < 1)
        {
            menuAudioSource.volume = Mathf.Lerp(currentMenuVol, 0, fadeTimer);
            fadeTimer += Time.deltaTime;
            yield return null;
        }
        menuAudioSource.Stop();
        menuAudioSource.volume = 0;
    }



    public void ChangedLevel(int newLevel)
    {
        if (currentAudioClip == newLevel)
        {
            return;
        }
        if (newLevel >= bgAudioClips.Length)
        {
            if (!bg1AudioSource.isPlaying && !bg2AudioSource.isPlaying)
            {
                newLevel = 15;
            }
            else
            {
                return;
            }
        }

        currentAudioClip = newLevel;
        StartCoroutine(setAudioToFade(currentAudioClip));
    }

    IEnumerator setAudioToFade(int newLevel)
    {
        bg1IsActive = !bg1IsActive;
        var fadeInAudioSource = bg1IsActive ? bg1AudioSource : bg2AudioSource;
        fadeInAudioSource.clip = bgAudioClips[newLevel];
        fadeInAudioSource.Play();
        var fadeOutAudioSource = !bg1IsActive ? bg1AudioSource : bg2AudioSource;
        var fadeTimer = 0.0f;
        while (fadeTimer < fadeTime)
        {
            fadeInAudioSource.volume = Mathf.Lerp(0, 1, fadeTimer / fadeTime);
            fadeOutAudioSource.volume = Mathf.Lerp(1, 0, fadeTimer / fadeTime);

            fadeTimer += Time.deltaTime;
            yield return null;
        }
        fadeInAudioSource.volume = 1;
        fadeOutAudioSource.volume = 0;
        fadeOutAudioSource.Stop();
    }

    public void FireShot(float pullback)
    {
        chargeSource.Stop();
        SFX.pitch = pullback * 1.5f *Random.Range(0.9f, 1.0f);
        SFX.PlayOneShot(ShootSFX);
    }

    public void ChargeShot(float pullback)
    {
        chargeSource.pitch = pullback * 0.85f;
        if (!chargeSource.isPlaying)
        {
            chargeSource.Play();
        }
    }
    public void HitCheckPoint()
    {
        SFX.pitch = Random.Range(0.85f, 1.15f);
        SFX.PlayOneShot(hitCheckPointSFX);
    }
    public void HitPortal()
    {
        SFX.pitch = Random.Range(0.6f, 0.7f);
        SFX.PlayOneShot(hitPortalSFX);
    }
    public void HitTarget()
    {
        SFX.pitch = Random.Range(0.3f, 0.4f);
        SFX.PlayOneShot(hitTargetSFX);
    }

}

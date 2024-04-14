using UnityEngine;
using UnityEngine.UI;

public class GameMusic : MonoBehaviour
{
    public static GameMusic instance;
    public AudioClip MenuMusic;
    public AudioClip EarthMusic;
    public AudioClip NightmareMusic;
    public AudioClip MirrorMusic;
    public AudioClip AsylumMusic;
    public AudioClip BasicAttackSound;
    public AudioClip BasicBombAttackSound;
    public AudioClip BasicBombExplosionSound;
    public AudioClip EnemyDieSound;
    public AudioClip PlayerDieSound;
    public AudioClip ItemPickUpSound;
    public AudioClip WalkingSound;
    public AudioClip ShieldSound;
    public AudioSource effectMP3;
    public AudioSource musicMP3;
    public AudioSource frequentMP3;
    public Text worldName;

    public bool isShieldActive = false;

    public void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
        {
            Debug.Log("[Error]: GameMusic Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    public void Start()
    {
        PlayMenuMusic();
    }
    public void FixedUpdate()
    {
        if (MenuManager.isSoundActivated == false)
        {
            musicMP3.Stop();
            effectMP3.Stop();
            frequentMP3.Stop();
        }

        //THE MUSIC PLAYS ENDLESSLY
        if (musicMP3.isPlaying == false && MenuManager.isSoundActivated == true)
            musicMP3.Play();

        else if (FindObjectOfType<PlayerController>() != null) //IF THE LOCAL PLAYER (YOU) IS CREATED, WE CAN START PLAYING LAND MUSICS
        {
            //MUSIC OF EARTH [DISTANCE -100.0 <= X < 100]
            if (FindObjectOfType<PlayerController>().transform.position.x >= -100.0f
            && FindObjectOfType<PlayerController>().transform.position.x < 100.0f)
            {
                if (!worldName.text.Equals("E A R T H"))
                    worldName.text = "E A R T H";
                if (musicMP3.clip != EarthMusic)
                    PlayEarthMusic();
            }
            //MUSIC OF NIGHTMARE [DISTANCE 100.0 <= X < 300]
            else if (FindObjectOfType<PlayerController>().transform.position.x >= 100.0f
            && FindObjectOfType<PlayerController>().transform.position.x < 300.0f)
            {
                if (!worldName.text.Equals("N I G H T M A R E"))
                    worldName.text = "N I G H T M A R E";
                if (musicMP3.clip != NightmareMusic)
                    PlayNightmareMusic();
            }
            //MUSIC OF MIRROR [DISTANCE 300.0 <= X < 500]
            else if (FindObjectOfType<PlayerController>().transform.position.x >= 300.0f
            && FindObjectOfType<PlayerController>().transform.position.x < 500.0f)
            {
                if (!worldName.text.Equals("M I R R O R"))
                    worldName.text = "M I R R O R";
                if (musicMP3.clip != MirrorMusic)
                    PlayMirrorMusic();
            }
            //MUSIC OF ASYLUM [DISTANCE 500.0 <= X < 700]
            else if (FindObjectOfType<PlayerController>().transform.position.x >= 500.0f
            && FindObjectOfType<PlayerController>().transform.position.x < 700.0f)
            {
                if (!worldName.text.Equals("A S Y L U M"))
                    worldName.text = "A S Y L U M";
                if (musicMP3.clip != AsylumMusic)
                    PlayAsylumMusic();
            }

            if (isShieldActive == true)
                PlayShieldSound();
        }

    }

    public void PlayMenuMusic()
    {
        if (MenuManager.isSoundActivated == false)
            return;

        musicMP3.volume = 1.0f;
        musicMP3.clip = MenuMusic;
    }
    public void PlayEarthMusic()
    {
        if (MenuManager.isSoundActivated == false)
            return;

        musicMP3.volume = 0.8f;
        musicMP3.clip = EarthMusic;
    }
    public void PlayNightmareMusic()
    {
        if (MenuManager.isSoundActivated == false)
            return;

        musicMP3.volume = 0.6f;
        musicMP3.clip = NightmareMusic;
    }
    public void PlayMirrorMusic()
    {
        if (MenuManager.isSoundActivated == false)
            return;

        musicMP3.volume = 1f;
        musicMP3.clip = MirrorMusic;
    }
    public void PlayAsylumMusic()
    {
        if (MenuManager.isSoundActivated == false)
            return;

        musicMP3.volume = 1f;
        musicMP3.clip = AsylumMusic;
    }
    public void PlayBasicAttackSound()
    {
        if (MenuManager.isSoundActivated == false)
            return;

        effectMP3.PlayOneShot(BasicAttackSound, 0.3f);
    }
    public void PlayEnemyDieSound()
    {
        if (MenuManager.isSoundActivated == false)
            return;

        effectMP3.PlayOneShot(EnemyDieSound, 0.8f);
    }
    public void PlayPlayerDieSound()
    {
        if (MenuManager.isSoundActivated == false)
            return;

        effectMP3.PlayOneShot(PlayerDieSound, 1f);
    }
    public void PlayBasicBombAttackSound()
    {
        if (MenuManager.isSoundActivated == false)
            return;

        if (musicMP3.clip == MirrorMusic)
            effectMP3.PlayOneShot(BasicBombAttackSound, 0.01f);
        else
            effectMP3.PlayOneShot(BasicBombAttackSound, 1f);
    }
    public void PlayBasicBombExplosionSound()
    {
        if (MenuManager.isSoundActivated == false)
            return;

        if (musicMP3.clip == MirrorMusic)
            effectMP3.PlayOneShot(BasicBombExplosionSound, 0.05f);
        else
            effectMP3.PlayOneShot(BasicBombExplosionSound, 0.5f);
    }
    public void PlayItemPickUpSound()
    {
        if (MenuManager.isSoundActivated == false)
            return;

        effectMP3.PlayOneShot(ItemPickUpSound, 1f);
    }
    public void PlayWalkingSound()
    {
        if (MenuManager.isSoundActivated == false)
            return;

        if (frequentMP3.clip != WalkingSound)
        {
            frequentMP3.clip = WalkingSound;
            frequentMP3.volume = 0.05f;
        }
        if (frequentMP3.isPlaying == false)
            frequentMP3.Play();
    }

    public void PlayShieldSound()
    {
        if (MenuManager.isSoundActivated == false)
            return;

        if (frequentMP3.clip != ShieldSound)
        {
            frequentMP3.clip = ShieldSound;
            frequentMP3.volume = 0.5f;
        }
        if (frequentMP3.isPlaying == false)
            frequentMP3.Play();
    }
}

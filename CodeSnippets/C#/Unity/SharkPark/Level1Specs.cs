using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1Specs : MonoBehaviour {

    public Camera mainCamera;
    public GameObject boundaries;
    public int level2Entry;
    public int level3Entry;
    public int level4Entry;
    public int gameCompletionEntry;
    public GameObject boatMan;
    public GameObject ItemBoatSpawn;
    public GameObject SmallBombSpawn;
    public GameObject subMan;
    public GameObject mermaids;
    public GameObject cthulhu;
    public AudioSource level1Music;
    public GameObject level2MusicOB;
    public AudioSource level2Music;
    public GameObject level3MusicOB;
    public AudioSource level3Music;
    public GameObject level4MusicOB;
    public AudioSource level4Music;
    public GameObject scoreTextOb;
    public GameObject solarSoundIntro;
    public GameObject solarSoundOutroAmbient;
    public GameObject cthulhuAmbientTransitionSound;
    public float playerInvincibleTransitionTime;
    public GameObject reverseSpell;
    //public AudioSource level2MusicSource;
    //public AudioClip level2MusicClip;

    public static float frameCount;
    public static float totalTimePassed;
    public static float completedGameTime;
    public static bool level2Achieved;
    public static bool level3Achieved;
    public static bool level4Achieved;
    public static int repeatingPoints;
    public static bool gameCompleted = false;
    public static string playerNameForPlaythrough;
    public static Animator cameraAnimator;

    private Animator boundariesAnimator;

    // Use this for initialization
    void Start () {
        if (gameCompleted)
        {
            gameCompleted = false;
            ScoreScript.round += 1;
        }
        else
        {
            ScoreScript.scoreValue = 0;
            ScoreScript.round = 1;
        }

        frameCount = 0;
        totalTimePassed = 0;
        repeatingPoints = 0;
        completedGameTime = 0;
        ScoreScript.doublePointsModifierOn = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
        level2Achieved = false;
        level3Achieved = false;
        level4Achieved = false;
        cameraAnimator = mainCamera.GetComponent<Animator>();
        boundariesAnimator = boundaries.GetComponent<Animator>();
        ItemBoatSpawn.SetActive(true);
        SmallBombSpawn.SetActive(true);
        boatMan.SetActive(true);
        cameraAnimator.SetBool("Retry", false);
    }
	
	// Update is called once per frame
	void Update () {
        frameCount = Time.frameCount;
        totalTimePassed += Time.deltaTime;
        completedGameTime = Time.time;

        if (repeatingPoints >= level2Entry && !level2Achieved)
        {
            Player.playerTransitioningToNewLevel = true;
            level2Achieved = true;
            repeatingPoints = 0;
            cameraAnimator.SetBool("Stage2", true);
            boundariesAnimator.SetBool("Stage2", true);
            StartCoroutine(SetActiveLvl2(subMan, 11f));
            StartCoroutine(FadeLevelMusicSound(level1Music));
            StartCoroutine(PlayerNoLongerTransitioning(playerInvincibleTransitionTime, false));
        }

        if (repeatingPoints >= level3Entry && level2Achieved && !level3Achieved)
        {
            Player.playerTransitioningToNewLevel = true;
            level3Achieved = true;
            repeatingPoints = 0;
            cameraAnimator.SetBool("Stage3", true);
            boundariesAnimator.SetBool("Stage3", true);
            StartCoroutine(FadeLevelMusicSound(level2Music));
            StartCoroutine(SetActiveLvl3(mermaids, 12f));
            StartCoroutine(PlayerNoLongerTransitioning(playerInvincibleTransitionTime, false));
        }

        if (repeatingPoints >= level4Entry && level2Achieved && level3Achieved && !level4Achieved)
        {
            Player.playerTransitioningToNewLevel = true;
            level4Achieved = true;
            repeatingPoints = 0;
            cameraAnimator.SetBool("Stage4", true);
            boundariesAnimator.SetBool("Stage4", true);
            StartCoroutine(FadeLevelMusicSound(level3Music));
            StartCoroutine(SetActiveLvl4(cthulhu, 16f));
            StartCoroutine(PlayerNoLongerTransitioning(playerInvincibleTransitionTime, false));
        }

        if (repeatingPoints >= gameCompletionEntry && level4Achieved)
        {
            gameCompleted = true;
            StartCoroutine(SetActiveObject(2f, reverseSpell, true));
            StartCoroutine(NewRound(8f));
        }

        if (Player.playerDamaged)
        {
            StartCoroutine(FadeLevelMusicSound(level1Music));
            StartCoroutine(FadeLevelMusicSound(level2Music));
            StartCoroutine(FadeLevelMusicSound(level3Music));
            StartCoroutine(FadeLevelMusicSound(level4Music));
            StartCoroutine(SetActiveScore(6f));
        }

        if (ForceDown.inForce)
            StartCoroutine(TriggerForceOff(1f));
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Player.playerDamaged = false;
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
        frameCount = 0;
        totalTimePassed = 0;
        ScoreScript.doublePointsModifierOn = false;
        Force.inForce = false;
        ForceDown.inForce = false;
        level2Achieved = false;
        Player.scoreAdded = false;

        if(cameraAnimator != null)
            cameraAnimator.SetBool("Stage2", false);

        if(boundariesAnimator != null)
            boundariesAnimator.SetBool("Stage2", false);

        if(!gameCompleted)
            ScoreScript.scoreValue = 0;
    }

    void MusicSoundTransitions()
    {
        if (level2Achieved && !level3Achieved)
            solarSoundIntro.SetActive(true);
        else if (level2Achieved && level3Achieved && !level4Achieved)
        {
            solarSoundIntro.SetActive(false);
            solarSoundOutroAmbient.SetActive(true);
        }
        else if (level2Achieved && level3Achieved && level4Achieved)
        {
            solarSoundIntro.SetActive(false);
            solarSoundOutroAmbient.SetActive(false);
            cthulhuAmbientTransitionSound.SetActive(true);
        }
    }

    IEnumerator SetActiveLvl2(GameObject gObject, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        gObject.SetActive(true);
        level2MusicOB.SetActive(true);
        ItemBoatSpawn.SetActive(false);
        SmallBombSpawn.SetActive(false);
        boatMan.SetActive(false);
    }

    IEnumerator SetActiveLvl3(GameObject gObject, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        gObject.SetActive(true);
        level3MusicOB.SetActive(true);
        solarSoundOutroAmbient.SetActive(false);
    }

    IEnumerator SetActiveLvl4(GameObject gObject, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        gObject.SetActive(true);
        level4MusicOB.SetActive(true);
        cthulhuAmbientTransitionSound.SetActive(false);
    }

    IEnumerator FadeLevelMusicSound(AudioSource music)
    {
        while (music.volume > 0.01f)
        {
            music.volume -= 0.1f * Time.deltaTime * 2;
            yield return null;
        }

        music.volume = 0;
        music.Stop();

        MusicSoundTransitions();
    }

    IEnumerator TriggerForceOff(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Force.inForce = false;
        ForceDown.inForce = false;
    }

    IEnumerator SetActiveScore(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        scoreTextOb.SetActive(false);
    }

    IEnumerator PlayerNoLongerTransitioning(float delayTime, bool isTransitioning)
    {
        yield return new WaitForSeconds(delayTime);
        Player.playerTransitioningToNewLevel = isTransitioning;
    }

    IEnumerator SetActiveObject(float delayTime, GameObject gObject, bool active)
    {
        yield return new WaitForSeconds(delayTime);
        gObject.SetActive(active);
    }

    IEnumerator NewRound(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        level4Achieved = false;
        level3Achieved = false;
        level2Achieved = false;
        SceneManager.LoadScene("Level1");
    }
}

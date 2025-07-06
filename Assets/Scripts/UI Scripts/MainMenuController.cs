using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public string sceneToLoad = "GameScene";
    public Animator ufoAnimator;

    [Header("Crash Effects and Sound")]
    public GameObject crashEffectPrefab;   // Crash effect prefab
    public AudioClip crashSound;           // Crash sound effect
    public float crashEffectDelay = 0f;    // Delay before playing effect (typically matches animation duration)
    public ParticleSystem explosionEffect1;
    public ParticleSystem explosionEffect2;
    public ParticleSystem explosionEffect3;
    public ParticleSystem explosionEffect4;

    [Header("Intro Dialogue Audio")] // Helpful header for organization in the Inspector
    public AudioClip fatherDialogue; // Father's dialogue audio
    public AudioClip childDialogue;  // Child's dialogue audio
    public AudioClip HiDialogue;     // Initial greeting dialogue audio

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void StartGame()
    {
        if (ufoAnimator != null)
        {
            // 1️⃣ Trigger crash animation
            ufoAnimator.SetTrigger("StartCrash");

            // 2️⃣ Play crash sound immediately
            if (crashSound != null)
            {
                audioSource.PlayOneShot(crashSound);
            }

            // 3️⃣ Start coroutine: wait for animation then play effects
            StartCoroutine(HandleCrashSequence());
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    IEnumerator HandleCrashSequence()
    {
        // Wait for animation duration
        yield return new WaitForSeconds(crashEffectDelay);

        // Play visual effects
        if (crashEffectPrefab != null)
        {
            Debug.Log("Instantiate crash effect prefab: " + crashEffectPrefab.name);
            Instantiate(crashEffectPrefab, ufoAnimator.transform.position, Quaternion.identity);
            explosionEffect1.gameObject.SetActive(true);
            explosionEffect2.gameObject.SetActive(true);
            explosionEffect3.gameObject.SetActive(true);
            explosionEffect4.gameObject.SetActive(true);
        }

        if (fatherDialogue != null && childDialogue != null)
        {
            Debug.Log("Playing intro dialogue...");
            // Play initial greeting
            audioSource.clip = HiDialogue;
            audioSource.Play();
            yield return new WaitForSeconds(HiDialogue.length);

            // Play father's dialogue
            audioSource.clip = fatherDialogue;
            audioSource.Play();
            yield return new WaitForSeconds(fatherDialogue.length);

            // Play child's dialogue
            audioSource.clip = childDialogue;
            audioSource.Play();
            yield return new WaitForSeconds(childDialogue.length);

            Debug.Log("Intro dialogue finished.");
        }
        else if (fatherDialogue == null || childDialogue == null)
        {
            Debug.LogWarning("Father or Child dialogue AudioClips are not assigned! Skipping dialogue playback.");
        }

        // Wait a few more seconds for effects to finish
        yield return new WaitForSeconds(9f);

        // Load the game scene
        SceneManager.LoadScene(sceneToLoad);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

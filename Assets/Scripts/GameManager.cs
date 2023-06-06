using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum World
    {
        Doors, Ocean, Tub, Park, Forest, Final
    }

    public bool isGameActive;
    public bool isInTransit;

    public GameObject shadow;
    public Camera mainCamera;

    public World world;
    public World newWorld;

    public bool isDoorOpen;

    //Left, Right, Bottom, Top
    public List<float> playerBounds;
    public List<float> cameraBounds;

    public UIManager uiManager;
    public Transition transition;
    public AudioManager audioManager;

    public GameObject compMarks;

    public GameObject bubbleParticle;

    public GameObject firePrefab;
    public GameObject fireWhooshPrefab;

    public GameObject gameCompleteUI;
    private PlayerController playerController
    {
        get { return GameObject.Find("Player").GetComponent<PlayerController>(); }
    }

    private bool isHooked;

    private GameObject hook;
    private GameObject chest;

    private GameObject faucet;
    private GameObject faucetButton;
    private GameObject water;

    private GameObject glass;

    private GameObject exMark;

    private bool isFinalUnlocked;
    private List<bool> completionStatuses = new List<bool>();

    private Grain grain;
    void Start()
    {
        isGameActive = true;

        playerBounds = new List<float>()
        {
            -1.85f, 1.85f, -1, 1
        };

        world = World.Doors;

        for (int i = 0; i < compMarks.transform.childCount; i++)
        {
            completionStatuses.Add(compMarks.transform.GetChild(i).gameObject.activeSelf);
        }

        if (!PlayerPrefs.HasKey("isFinalUnlocked"))
        {
            Save();
        }
        StartCoroutine(Load());

        mainCamera.GetComponent<PostProcessVolume>().sharedProfile.TryGetSettings(out grain);

        //debug
    }
    private void FixedUpdate()
    {
        isDoorOpen = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !isInTransit)
        {
            if (isGameActive)
            {
                PauseGame();
            }
            else if (!isGameActive)
            {
                if (uiManager.currentUI == UIManager.UI.Paused)
                {
                    UnpauseGame();
                }
                else
                {
                    uiManager.OpenPaused();
                }
            }
        }
        if (isGameActive)
        {
            if (world == World.Doors)
            {
                DoorsUpdate();
            }
            else if (world == World.Ocean)
            {
                OceanUpdate();
            }
            else if (world == World.Tub)
            {
                TubUpdate();
            }
            else if (world == World.Park)
            {
                ParkUpdate();
            }
            else if (world == World.Forest)
            {
                ForestUpdate();
            }
            else if (world == World.Final)
            {
                FinalUpdate();
            }
        }

        //debug
        /*if (Input.GetKeyDown(KeyCode.K))
        {
            for (int i = 0; i < completionStatuses.Count; i++)
            {
                completionStatuses[i] = true;
            }
        }*/
    }
    private void DoorsUpdate()
    {
        playerController.GetComponent<Animator>().SetBool("isBlinking", false);
        if (Input.GetButtonDown("Interact"))
        {
            if (isDoorOpen)
            {
                StartCoroutine(SwitchWorld());
                audioManager.PlaySound("Door Enter");
            }
            else
            {
                playerController.GetComponent<Animator>().SetBool("isBlinking", true);
            }
        }
    }
    private void OceanUpdate()
    {
        hook = GameObject.Find("Hook");
        chest = GameObject.Find("Chest");

        if (Input.GetButtonDown("Interact"))
        {
            if (playerController.GetComponent<BoxCollider2D>().IsTouching(hook.GetComponent<BoxCollider2D>()))
            {
                if (playerController.canMoveHorizontally)
                {
                    playerController.transform.position = new Vector2(hook.transform.position.x, hook.transform.position.y);
                    playerController.canMoveHorizontally = false;
                    isHooked = true;
                    audioManager.PlaySound("Hook Sound");
                }
                else
                {
                    playerController.canMoveHorizontally = true;
                    isHooked = false;
                    if (chest.GetComponent<BoxCollider2D>().IsTouching(hook.GetComponent<BoxCollider2D>()))
                    {
                        StartCoroutine(ReelHook());
                        hook.transform.position = new Vector2(hook.transform.position.x, chest.transform.position.y);
                        hook.GetComponent<SpriteRenderer>().sortingOrder = -1;
                        audioManager.PlaySound("Hook Sound");
                    }
                }
            }
            else
            {
                audioManager.PlaySound("Bubble Sound", 0.8f, 1.2f);
                if (!playerController.GetComponent<SpriteRenderer>().flipX)
                {
                    Instantiate(bubbleParticle, playerController.transform.position +
                        new Vector3(playerController.GetComponent<BoxCollider2D>().size.x / 2, 0, 0), Quaternion.identity);
                }
                else
                {
                    Instantiate(bubbleParticle, playerController.transform.position -
                        new Vector3(playerController.GetComponent<BoxCollider2D>().size.x / 2, 0, 0), Quaternion.identity);
                }
            }
        }
        if (isHooked)
        {
            hook.transform.position = playerController.transform.position;
        }
    }
    private IEnumerator ReelHook()
    {
        float reelSpeed = 2;
        yield return new WaitForSeconds(1);
        while (hook.transform.position.y < 8)
        {
            hook.transform.position += new Vector3(0, reelSpeed * Time.deltaTime);
            chest.transform.position += new Vector3(0, reelSpeed * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(CompleteWorld(World.Ocean));
        yield break;
    }
    private void TubUpdate()
    {
        faucet = GameObject.Find("Faucet").transform.GetChild(0).gameObject;
        faucetButton = GameObject.Find("Faucet").transform.GetChild(1).gameObject;
        water = GameObject.Find("Water");

        if (Input.GetButtonDown("Interact"))
        {
            if (playerController.GetComponent<BoxCollider2D>().IsTouching(faucet.GetComponent<BoxCollider2D>()) &&
                faucetButton.transform.position.y != 0.21f)
            {
                faucetButton.transform.position = new Vector3(faucetButton.transform.position.x, 0.21f, faucetButton.transform.position.z);
                StartCoroutine(RaiseWater());
                audioManager.PlaySound("Faucet Sound");
                audioManager.PlaySound("Bathtub Running Water");
            }
            else
            {
                audioManager.PlaySound("Duck Squeak");
            }
        }
        if (Mathf.Abs(playerController.transform.position.x) > 5 || playerController.transform.position.y < -1)
        {
            playerController.GetComponent<Rigidbody2D>().gravityScale = 10;
        }
        else
        {
            playerController.GetComponent<Rigidbody2D>().gravityScale = 5;
        }
        if (playerController.transform.position.y < -7 && playerController.GetComponent<SpriteRenderer>().color.a >= 1)
        {
            StartCoroutine(CompleteWorld(World.Tub));
        }
    }
    private IEnumerator RaiseWater()
    {
        while (water.transform.position.y < -1.1f)
        {
            water.transform.position += new Vector3(0, 0.06f * Time.deltaTime);
            yield return null;
        }
        audioManager.StopSound("Bathtub Running Water");
        yield break;
    }
    private void ParkUpdate()
    {
        glass = GameObject.Find("Glass");


        if (playerController.GetComponent<BoxCollider2D>().IsTouching(glass.GetComponent<BoxCollider2D>()) && 
            playerController.GetComponent<SpriteRenderer>().color.a >= 1)
        {
            StartCoroutine(CompleteWorld(World.Park));
            audioManager.PlaySound("Water Plop");
        }
    }
    private void ForestUpdate()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (GameObject.FindGameObjectsWithTag("Fire").Count() == 0) 
            {
                audioManager.PlaySound("Fire Crackle");
            }
            Instantiate(firePrefab, playerController.transform.position, Quaternion.identity);
            audioManager.PlaySound("Fire Whoosh");
            Instantiate(fireWhooshPrefab);
        }
    }
    private void FinalUpdate()
    {
        exMark = GameObject.Find("Exclamation Mark");

        if (Input.GetButtonDown("Interact"))
        {
            if (!playerController.GetComponent<BoxCollider2D>().IsTouching(exMark.GetComponent<BoxCollider2D>()))
            {
                StartCoroutine(FinalBlink());
            }
            else
            {
                StartCoroutine(CompleteGame());
            }
        }
        if (playerController.transform.position.x > 0)
        {
            audioManager.GetSound("Final Tone").volume = playerController.transform.position.x / 20;
            grain.intensity.value = playerController.transform.position.x / 20;
        }
    }
    private IEnumerator FinalBlink()
    {
        isGameActive = false;
        playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        StartCoroutine(transition.StartTransition());
        yield return new WaitForSeconds(0.8f);
        grain.intensity.value = 0;
        StartCoroutine(transition.EndTransition());
        yield return new WaitForSeconds(0.5f);
        isGameActive = true;
    }
    private IEnumerator CompleteGame()
    {
        isGameActive = false;
        playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        StartCoroutine(transition.StartTransition());
        gameCompleteUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        audioManager.StopSound("Final Tone");
        yield return new WaitForSeconds(2);
        gameCompleteUI.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        gameCompleteUI.transform.GetChild(1).gameObject.SetActive(true);
        yield break;
    }
    public void GameCompleted()
    {
        PlayerPrefs.DeleteAll();
        for (int i = 0; i < completionStatuses.Count; i++)
        {
            completionStatuses[i] = false;
        }
        isFinalUnlocked = false;
    }
    public IEnumerator CompleteWorld(World world)
    {
        newWorld = World.Doors;
        StartCoroutine(SwitchWorld());
        for (int i = 0; i < compMarks.transform.childCount; i++)
        {
            if (compMarks.transform.GetChild(i).name == world.ToString() + " Completion Mark")
            {
                completionStatuses[i] = true;
                break;
            }
        }
        if (!completionStatuses.Contains(false))
        {
            isFinalUnlocked = true;
        }
        yield return new WaitForSeconds(3);
        compMarks.transform.Find(world.ToString() + " Completion Mark").gameObject.SetActive(true);
    }
    private IEnumerator SwitchWorld()
    {
        isGameActive = false;
        playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        StartCoroutine(playerController.FadeOut());
        yield return new WaitForSeconds(1);
        StartCoroutine(transition.StartTransition());
        StartCoroutine(audioManager.FadeAudio("Dreaming", 1, 0));
        yield return new WaitForSeconds(1);
        grain.intensity.value = 0;
        SceneManager.UnloadSceneAsync(world.ToString());
        SceneManager.LoadScene(newWorld.ToString(), LoadSceneMode.Additive);
        compMarks.SetActive(false);
        mainCamera.orthographicSize = 1;
        switch (world)
        {
            case World.Ocean:
                audioManager.StopSound("Ocean Ambience Sound");
                break;
        }
        switch (newWorld)
        {
            case World.Doors:
                mainCamera.backgroundColor = new Color(0.07f, 0.07f, 0.07f);
                playerBounds = new List<float>()
                {
                    -1.85f, 1.85f, -1, 1
                };
                cameraBounds = new List<float>()
                {
                    -1.85f, 1.85f, 0, 0
                };
                compMarks.SetActive(true);
                break;
            case World.Ocean:
                mainCamera.backgroundColor = new Color(0, 0.2f, 0.5f);
                playerBounds = new List<float>()
                {
                    -10, 10, -5, 5
                };
                cameraBounds = new List<float>()
                {
                    -10, 10, -5.5f, 5
                };
                audioManager.PlaySound("Ocean Ambience Sound");
                break;
            case World.Tub:
                mainCamera.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
                playerBounds = new List<float>()
                {
                    -7, 7, -10f, 2f
                };
                cameraBounds = new List<float>()
                {
                    -7, 7, -10f, 2
                };
                break;
            case World.Park:
                mainCamera.backgroundColor = new Color(0.3f, 0.8f, 1f);
                playerBounds = new List<float>()
                {
                    -10, 10, -0.1f, 5
                };
                cameraBounds = new List<float>()
                {
                    -10, 10, -1f, 5
                };
                break;
            case World.Forest:
                mainCamera.backgroundColor = new Color(0.1f, 0.3f, 0);
                playerBounds = new List<float>()
                {
                    -10, 10, -10f, 10
                };
                cameraBounds = new List<float>()
                {
                    -10, 10, -10f, 10
                };
                mainCamera.orthographicSize = 2f;
                break;
            case World.Final:
                mainCamera.backgroundColor = new Color(0, 0, 0);
                playerBounds = new List<float>()
                {
                    -1, 25, -10f, 10
                };
                cameraBounds = new List<float>()
                {
                    -1, 25, -10f, 10
                };
                audioManager.PlaySound("Final Tone");
                break;
        }
        for (int i = 0; i < compMarks.transform.childCount; i++)
        {
            if (completionStatuses[i])
            {
                compMarks.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        world = newWorld;
        yield return new WaitForSeconds(1);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(world.ToString()));
        StartCoroutine(FadePlayerIn());
        if (world == World.Doors)
        {
            if (isFinalUnlocked)
            {
                GameObject.Find("Doors").transform.GetChild(GameObject.Find("Doors").transform.childCount - 1).gameObject.SetActive(true);
            }
        }
        mainCamera.transform.position = new Vector3(0, 0, mainCamera.transform.position.z);
        yield return new WaitForEndOfFrame();
        StartCoroutine(transition.EndTransition());
        if (world != World.Doors && world != World.Final)
        {
            audioManager.PlaySound("Dreaming");
            StartCoroutine(audioManager.FadeAudio("Dreaming", 1, 1));
        }
        yield return new WaitForSeconds(1);
        isGameActive = true;
        Save();
        yield break;
    }
    private IEnumerator FadePlayerIn()
    {
        yield return new WaitForEndOfFrame();
        playerController.GetComponent<SpriteRenderer>().color = new Color(playerController.GetComponent<SpriteRenderer>().color.r,
            playerController.GetComponent<SpriteRenderer>().color.g, playerController.GetComponent<SpriteRenderer>().color.b, 0);
        yield return new WaitForSeconds(1);
        StartCoroutine(playerController.FadeIn());
        yield break;
    }
    private void PauseGame()
    {
        Time.timeScale = 0;
        uiManager.OpenPaused();
        shadow.SetActive(true);
        isGameActive = false;
    }
    private void UnpauseGame()
    {
        Time.timeScale = 1;
        uiManager.ClearUI();
        shadow.SetActive(false);
        isGameActive = true;
    }
    public void WakeUp()
    {
        UnpauseGame();
        if (world != World.Doors)
        {
            newWorld = World.Doors;
            StartCoroutine(SwitchWorld());
        }
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    private void Save()
    {
        PlayerPrefs.SetInt("isFinalUnlocked", isFinalUnlocked ? 1 : 0);
        for (int i = 0; i < compMarks.transform.childCount; i++)
        {
            PlayerPrefs.SetInt("door" + i + "Unlocked", completionStatuses[i] ? 1 : 0);
        }
        PlayerPrefs.SetFloat("sfxVolume", audioManager.sfxVolume);
        PlayerPrefs.SetFloat("bgmVolume", audioManager.bgmVolume);
        PlayerPrefs.Save();
    }
    private IEnumerator Load()
    {
        yield return new WaitForEndOfFrame();
        isFinalUnlocked = PlayerPrefs.GetInt("isFinalUnlocked") == 1;
        for (int i = 0; i < compMarks.transform.childCount; i++)
        {
            completionStatuses[i] = PlayerPrefs.GetInt("door" + i + "Unlocked") == 1;
            if (completionStatuses[i])
            {
                compMarks.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        audioManager.UpdateSFXVolume(PlayerPrefs.GetFloat("sfxVolume"));
        audioManager.UpdateBGMVolume(PlayerPrefs.GetFloat("bgmVolume"));
        if (audioManager.sfxVolume == 0)
        {
            audioManager.UpdateSFXVolume(0.001f);
        }
        if (audioManager.bgmVolume == 0)
        {
            audioManager.UpdateBGMVolume(0.001f);
        }
        //yield return new WaitForSeconds(0.1f);
        if (isFinalUnlocked)
        {
            GameObject.Find("Doors").transform.GetChild(GameObject.Find("Doors").transform.childCount - 1).gameObject.SetActive(true);
        }
        yield break;
    }
    private void OnApplicationQuit()
    {
        Save();
    }
}

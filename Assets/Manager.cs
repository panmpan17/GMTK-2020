using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Manager : MonoBehaviour
{
    public static Manager ins;
    private static bool tutorialShowed = false;

    [SerializeField]
    private Transform nearsetHouse1, nearsetHouse2;
    // [SerializeField]
    // private SpriteRenderer tutorialSprite;
    [SerializeField]
    private Text tutorialText;
    [SerializeField]
    private Transform focusMask;
    private Villiger tutorialVilliger;
    private Guard tutorialGuard;
    private int tutorialState = 0;
    // [SerializeField]
    // private Sprite[] tutorilSprites;
    [SerializeField]
    private string[] tutorialTexts;
    [SerializeField]
    private Transform tutorialTree;

    [SerializeField]
    private Villiger[] villigerPrefab;
    private List<Villiger> villigerPrefabs;
    [SerializeField]
    private int villigerInitlNum;
    [SerializeField]
    private int villigerMaxNum;
    private int villigerNum, villigerSpawned;
    [SerializeField]
    private float spawnTime, maxSpawnTime;
    private float spawnTimer, targetSpawnTime;

    [SerializeField]
    private Guard guardPrefab;
    [SerializeField]
    private int guardNum;

    [SerializeField]
    private Transform[] houses;
    public PlayerController Player;

    [SerializeField]
    private GameObject warningIcon;
    [SerializeField]
    private Transform warningIconFill;
    [SerializeField]
    private float warningTime;
    private float warningTimer;
    [System.NonSerialized]
    public bool Warning;

    [SerializeField]
    private GameObject gameOverAnim, winImage;

    [SerializeField]
    private AudioSource basicBGM, scaryBGM;

    public Villiger PickVilligerPrefab() {
        if (villigerPrefabs == null || villigerPrefabs.Count == 0)
            villigerPrefabs = new List<Villiger>(villigerPrefab);
    
        int index = Random.Range(0, villigerPrefabs.Count);
        Villiger v = villigerPrefabs[index];
        villigerPrefabs.RemoveAt(index);
        return v;
    }

    private void Awake() {
        ins = this;

        List<Transform> houseList = new List<Transform>(houses);

        if (!tutorialShowed) {
            houseList.Remove(nearsetHouse1);
            tutorialVilliger = Instantiate<Villiger>(PickVilligerPrefab());
            tutorialVilliger.transform.position = nearsetHouse1.position;
            tutorialVilliger.gameObject.SetActive(true);
            villigerNum++;
            villigerSpawned++;
            villigerInitlNum--;

            houseList.Remove(nearsetHouse2);
            tutorialGuard = Instantiate<Guard>(guardPrefab);
            tutorialGuard.transform.position = nearsetHouse2.position;
            tutorialGuard.gameObject.SetActive(true);
            guardNum--;

            Time.timeScale = 0;
            focusMask.transform.position = Player.transform.position + Vector3.up;

            tutorialText.gameObject.SetActive(true);
            tutorialText.text = tutorialTexts[tutorialState];
            tutorialText.transform.position = GetComponent<Camera>().WorldToScreenPoint(focusMask.position + new Vector3(0, 2));
        }
        else {
            tutorialText.gameObject.SetActive(false);
            focusMask.parent.gameObject.SetActive(false);
        }

        for (int i = 0; i < villigerInitlNum; i++) {
            Villiger villiger = Instantiate<Villiger>(PickVilligerPrefab());
            int index = Random.Range(0, houseList.Count);
            villiger.transform.position = houseList[index].position;
            houseList.RemoveAt(index);
            villiger.gameObject.SetActive(true);
            villigerNum++;
            villigerSpawned++;
        }

        for (int i = 0; i < guardNum; i++)
        {
            Guard guard = Instantiate<Guard>(guardPrefab);
            int index = Random.Range(0, houseList.Count);
            guard.transform.position = houseList[index].position;
            houseList.RemoveAt(index);
            guard.gameObject.SetActive(true);
        }

        spawnTimer = Random.Range(spawnTime, maxSpawnTime);

        warningIcon.SetActive(false);
    }

    private void Update() {
        if (!tutorialShowed) {
            switch (tutorialState) {
                case 0:
                    if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
                    {
                        tutorialState++;
                        Vector3 pos = GetComponent<Camera>().ScreenToWorldPoint(warningIcon.transform.position);
                        pos.z = 0;
                        pos.x -= 2;
                        focusMask.transform.position = pos;

                        tutorialText.text = tutorialTexts[tutorialState];
                        tutorialText.transform.position = GetComponent<Camera>().WorldToScreenPoint(focusMask.position + new Vector3(3f, -2));
                    }
                    break;
                case 1:
                    if (Input.anyKeyDown || Input.GetMouseButtonDown(0)) {
                        tutorialState++;
                        focusMask.transform.position = tutorialVilliger.transform.position + Vector3.up;

                        tutorialText.text = tutorialTexts[tutorialState];
                        tutorialText.transform.position = GetComponent<Camera>().WorldToScreenPoint(focusMask.position + new Vector3(0, 2));
                    }
                    break;
                case 2:
                    if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
                    {
                        tutorialState++;
                        focusMask.transform.position = tutorialGuard.transform.position + Vector3.up;

                        tutorialText.text = tutorialTexts[tutorialState];
                        tutorialText.transform.position = GetComponent<Camera>().WorldToScreenPoint(focusMask.position + new Vector3(0, 2));
                    }
                    break;
                case 3:
                    if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
                    {
                        tutorialState++;
                        focusMask.transform.position = tutorialTree.transform.position + Vector3.up;

                        tutorialText.text = tutorialTexts[tutorialState];
                        tutorialText.transform.position = GetComponent<Camera>().WorldToScreenPoint(focusMask.position + new Vector3(2, 2));
                    }
                    break;
                case 4:
                    if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
                    {
                        tutorialState++;
                        Vector3 pos = GetComponent<Camera>().ScreenToWorldPoint(warningIcon.transform.position);
                        pos.z = 0;
                        focusMask.transform.position = pos;
                        warningIcon.gameObject.SetActive(true);

                        tutorialText.text = tutorialTexts[tutorialState];
                        tutorialText.transform.position = GetComponent<Camera>().WorldToScreenPoint(focusMask.position + new Vector3(1, -2));
                    }
                    break;
                case 5:
                    if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
                    {
                        tutorialState++;
                        focusMask.transform.position = Player.transform.position + Vector3.up;

                        tutorialText.text = tutorialTexts[tutorialState];
                        tutorialText.transform.position = GetComponent<Camera>().WorldToScreenPoint(focusMask.position + new Vector3(0, 2));
                    }
                    break;
                case 6:
                    if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
                    {
                        tutorialText.gameObject.SetActive(false);
                        warningIcon.gameObject.SetActive(false);
                        tutorialShowed = true;
                        focusMask.parent.gameObject.SetActive(false);
                        Time.timeScale = 1;
                    }
                    break;
            }
        }//tutorialTree

        if (Warning) {
            warningTimer -= Time.deltaTime;
            warningIconFill.localScale = new Vector3(1, warningTimer / warningTime, 1);
            if (warningTimer <= 0) {
                Warning = false;
                warningIcon.SetActive(false);

                basicBGM.Play();
                basicBGM.volume = 0.1f;
                StartCoroutine(LerpSound(basicBGM, 1f, 0.3f));
                StartCoroutine(LerpSound(scaryBGM, 1f, 0f, delegate {
                    scaryBGM.Stop();
                }));
            }
        }

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0) {
            spawnTimer = Random.Range(spawnTime, maxSpawnTime);

            if (villigerSpawned < villigerMaxNum) {
                List<Transform> houseList = new List<Transform>(houses);

                while (houseList.Count > 0) {
                    int index = Random.Range(0, houseList.Count);

                    if (Mathf.Abs(houseList[index].position.x - Player.transform.position.x) > 8) {
                        Villiger villiger = Instantiate<Villiger>(PickVilligerPrefab());
                        villiger.transform.position = houseList[index].position;
                        villiger.gameObject.SetActive(true);
                        villigerNum++;
                        villigerSpawned++;
                        break;
                    }

                    houseList.RemoveAt(index);
                }
            }
        }
    }

    public void RemoveVilliger(Villiger villiger) {
        Destroy(villiger.gameObject);

        if (--villigerNum <= 0)
            StartCoroutine(Waitin());
    }

    public IEnumerator Waitin() {
        yield return new WaitForSeconds(2.5f);
        Time.timeScale = 0;
        winImage.SetActive(true);
    }

    public void ResetWarning() {
        Warning = true;
        warningTimer = warningTime;
        warningIcon.SetActive(true);
        warningIconFill.localScale = Vector3.one;
    }

    public void GameOver() {
        Player.GameOver();
        gameOverAnim.SetActive(true);
        Time.timeScale = 0;
    }

    public void Replay() {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartChasing() {
        StartCoroutine(LerpSound(basicBGM, 1f, 0, delegate {
            basicBGM.Pause();
        }));
        scaryBGM.Play();
        scaryBGM.volume = 0.1f;
        StartCoroutine(LerpSound(scaryBGM, 1f, 0.3f));
    }

    public IEnumerator LerpSound(AudioSource source, float sec, float targetVol, System.Action complete=null) {
        float time = 0;

        float originalVol = source.volume;
        while (time < sec) {
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
            source.volume = Mathf.Lerp(originalVol, targetVol, time / sec);
        }
        source.volume = targetVol;
        complete?.Invoke();
        // yield
    }
}

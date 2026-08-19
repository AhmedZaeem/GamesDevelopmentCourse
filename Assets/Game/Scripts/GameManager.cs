using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject zombiePrefab;
    public PlayerController player;
    public Image healthBar;
    public Text killsText;
    public GameObject gameOverPanel;

    public float spawnRate = 4f;
    public float spawnDistance = 16f;
    public int maxZombies = 6;
    public int startZombies = 3;

    int kills;
    float timer;
    bool over;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        killsText.text = "Kills: 0";
        gameOverPanel.SetActive(false);

        for (int i = 0; i < startZombies; i++)
            Spawn();
    }

    void Update()
    {
        healthBar.fillAmount = (float)player.health / player.maxHealth;

        if (over)
        {
            if (Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            return;
        }

        timer += Time.deltaTime;

        if (timer >= spawnRate)
        {
            timer = 0f;

            if (FindObjectsByType<Zombie>().Length < maxZombies)
                Spawn();
        }
    }

    void Spawn()
    {
        Vector2 offset = Random.insideUnitCircle.normalized * spawnDistance;
        Vector3 pos = player.transform.position + new Vector3(offset.x, 0f, offset.y);
        pos.y = 0.1f;

        Instantiate(zombiePrefab, pos, Quaternion.identity);
    }

    public void AddKill()
    {
        kills++;
        killsText.text = "Kills: " + kills;
    }

    public void GameOver()
    {
        over = true;
        gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

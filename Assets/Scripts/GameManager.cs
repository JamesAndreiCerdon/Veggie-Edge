using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // Add this line for scene management

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Blade blade;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private Image fadeImage;

    public int score { get; private set; } = 0;
    public int currentLevel { get; private set; } = 1; // Track current level
    public int fruitsSliced { get; private set; } = 0; // Track number of fruits sliced
    public int fruitsToNextLevel { get; private set; } = 50; // Fruits required for next level

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        Time.timeScale = 1f;

        ClearScene();

        blade.enabled = true;
        spawner.enabled = true;

        score = 0;
        fruitsSliced = 0; // Reset sliced fruits
        scoreText.text = score.ToString();
    }

    private void ClearScene()
    {
        Fruit[] fruits = FindObjectsOfType<Fruit>();

        foreach (Fruit fruit in fruits)
        {
            Destroy(fruit.gameObject);
        }

        Bomb[] bombs = FindObjectsOfType<Bomb>();

        foreach (Bomb bomb in bombs)
        {
            Destroy(bomb.gameObject);
        }
    }

    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();

        fruitsSliced++; // Increase fruits sliced each time score is updated

        // Check if player has sliced enough fruits to level up
        if (fruitsSliced >= fruitsToNextLevel)
        {
            LevelUp();
        }

        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);

        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }
    }

    private void LevelUp()
    {
        currentLevel++; // Increment level
        fruitsSliced = 0; // Reset fruits sliced for next level

        // Optionally increase difficulty (e.g., increase fruit spawn rate or speed)
        IncreaseDifficulty();

        // Optional: Display level up message or handle transitions
        Debug.Log("Level Up! Now on Level " + currentLevel);

        // Load the next scene (Level 2, Level 3, etc.)
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        // Load the next scene. The scene name will be "Level X", where X is the current level.
        string nextLevel = "Level " + currentLevel;
        SceneManager.LoadScene(nextLevel);
    }

    // Example function to increase difficulty after leveling up
    private void IncreaseDifficulty()
    {
        // Example: Increase spawn speed, or introduce new fruit types, etc.
        spawner.IncreaseSpawnRate(currentLevel); // Assuming spawner has this method
    }

    public void Explode()
    {
        blade.enabled = false;
        spawner.enabled = false;

        StartCoroutine(ExplodeSequence());
    }

    private IEnumerator ExplodeSequence()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        // Fade to white
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.clear, Color.white, t);

            Time.timeScale = 1f - t;
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);

        NewGame();

        elapsed = 0f;

        // Fade back in
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.white, Color.clear, t);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }
    }
}

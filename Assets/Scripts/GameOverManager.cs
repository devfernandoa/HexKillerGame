using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverCanvas;
    public GameObject UICanvas;
    public Button saveButton; // Reference to the Save button
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TMP_InputField nameInputField;

    private const string GetHighScoresUrl = "https://hex-killer.vercel.app/api/get-highscores";
    private const string SaveHighScoreUrl = "https://hex-killer.vercel.app/api/save-highscore";

    public AudioSource gameMusic; // Reference to the game music AudioSource
    public AudioClip gameOverSound; // Game over sound effect
    public float fadeDuration = 0.2f; // Duration of the music fade-out

    private void Start()
    {
        UICanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
    }

    public void ShowGameOverScreen()
    {
        // Stop spawning and destroy objects
        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        SpawnerColetavel collectableSpawner = FindObjectOfType<SpawnerColetavel>();

        if (enemySpawner != null)
        {
            enemySpawner.StopSpawning();
        }
        if (collectableSpawner != null)
        {
            collectableSpawner.StopSpawning();
        }

        DestroyAllObjectsWithTag("Enemy");
        DestroyAllObjectsWithTag("Coletavel");
        DestroyAllObjectsWithTag("Player");

        // Fetch and display high scores
        StartCoroutine(FetchHighScores());

        // Display the player's score
        scoreText.text = "Score: " + ScoreManager.Instance.GetScore();

        // Display the player's survival time
        timerText.text = "Survival Time: " + Mathf.RoundToInt(XpManager.Instance.globalTimer) + "s";

        // Show the game over screen
        UICanvas.SetActive(false);
        gameOverCanvas.SetActive(true);

        // Fade out the game music
        StartCoroutine(FadeOutGameMusic());
    }

    private IEnumerator FadeOutGameMusic()
    {
        float startVolume = gameMusic.volume;

        // Fade out the game music
        while (gameMusic.volume > 0)
        {
            gameMusic.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        // Stop the game music
        gameMusic.Stop();

        // Play the game over sound
        AudioSource.PlayClipAtPoint(gameOverSound, Camera.main.transform.position);
    }


    private IEnumerator FetchHighScores()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(GetHighScoresUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                HighScoreList highScoreList = JsonUtility.FromJson<HighScoreList>(json);
                UpdateHighScoreDisplay(highScoreList.highscores);
            }
            else
            {
                Debug.LogError("Failed to fetch high scores: " + request.error);
            }
        }
    }

    private void UpdateHighScoreDisplay(List<HighScoreEntry> highscores)
    {
        highScoreText.text = "High Scores:\n";

        for (int i = 0; i < highscores.Count; i++)
        {
            highScoreText.text += $"{i + 1}. {highscores[i].name} - {highscores[i].score}\n";
        }
    }

    public void OnSaveButtonClicked()
    {
        string playerName = nameInputField.text;
        int playerScore = ScoreManager.Instance.GetScore();
        Debug.Log("Player name: " + playerName);
        Debug.Log("Player score: " + playerScore);

        if (!string.IsNullOrEmpty(playerName))
        {
            saveButton.interactable = false; // Disable the Save button to prevent multiple submissions
            StartCoroutine(SaveHighScore(playerName, playerScore));
        }
    }

    private IEnumerator SaveHighScore(string name, int score)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", name);
        form.AddField("score", score.ToString());

        using (UnityWebRequest request = UnityWebRequest.Post(SaveHighScoreUrl, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("High score saved successfully!");
                StartCoroutine(FetchHighScores()); // Refresh the high score display
            }
            else
            {
                saveButton.interactable = true; // Re-enable the Save button
                Debug.LogError("Failed to save high score: " + request.error);
            }
        }
    }

    public void OnRestartButtonClicked()
    {
        // Reset the PowerUpManager before reloading the scene
        if (PowerUpManager.Instance != null)
        {
            PowerUpManager.Instance.ResetManager();
        }

        // Reset other persistent managers if needed
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnQuitButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void DestroyAllObjectsWithTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
    }
}

[System.Serializable]
public class HighScoreEntry
{
    public string name;
    public int score;
}

[System.Serializable]
public class HighScoreList
{
    public List<HighScoreEntry> highscores;
}
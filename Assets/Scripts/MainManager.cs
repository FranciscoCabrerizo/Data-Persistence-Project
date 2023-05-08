using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text BestScoreText;
    public TextMeshProUGUI playerText;
    public string playerName;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    [System.Serializable]
    class bestScoreData {
        public int bestScore_points;
        public string bestScore_name;
    }
    private int bestScore;
    private string playerBestScore;
    private bool m_GameOver = false;

    
    // Start is called before the first frame update
    void Start()
    {
        if (NameManager.instance != null) {
            SetName(NameManager.instance.name);
        }

        LoadBestScore();
        BestScoreText.text = $"Best Score: {playerBestScore} : {bestScore}";

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void SetName(string name) {
        playerName = name;
        playerText.text = $"Player: {name}";
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        if (m_Points > bestScore) {
            bestScore = m_Points;
            playerBestScore = playerName;
        }
        SetBestScore(playerBestScore, bestScore);
        m_GameOver = true;
        GameOverText.SetActive(true);
        SaveBestScore();
    }

    void SetBestScore(string name, int score) {
        BestScoreText.text = $"Best Score: {name} : {score}";
    }

    void SaveBestScore() {
        bestScoreData data = new bestScoreData();
        data.bestScore_name = playerBestScore;
        data.bestScore_points = bestScore;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    void LoadBestScore() {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path)) {
            string json = File.ReadAllText(path);
            bestScoreData data = JsonUtility.FromJson<bestScoreData>(json);

            bestScore = data.bestScore_points;
            playerBestScore = data.bestScore_name;
        } else {
            bestScore = 0;
            playerBestScore = "Name";
        }
    }
}

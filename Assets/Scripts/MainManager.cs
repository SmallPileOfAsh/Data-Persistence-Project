using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text HighScoreText;
    public Text ScoreText;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    
    // Start is called before the first frame update
    void Start()
    {
        SetupUI();

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

    void SetupUI()
    {
        UpdateScoreName();
        UpdateHighScore();  
    }

    void AddPoint(int point)
    {
        m_Points += point;
        UpdateScoreName();
        
    }

    //Update High Score Text
    private void UpdateHighScore()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.highScore == 0)
            {
                HighScoreText.text = "Best Score: None: 0";
            }
            else
            {
                HighScoreText.text = "Best Score: " + GameManager.Instance.highScoreName + ": " + GameManager.Instance.highScore;
            }
        }
        else
        {
            HighScoreText.text = HighScoreText.text = "Best Score: UNKNOWN: UNKNOWN";
        }
        
    }

    //Update Score Text
    void UpdateScoreName()
    {
        if (GameManager.Instance != null && GameManager.Instance.playerName != "")
        {
            ScoreText.text = GameManager.Instance.playerName + "'s Score: " + m_Points;
        }
        else
        {
            ScoreText.text = "???'s " + "Score: " + m_Points;
        }
    }

    //Handle Gameover
    public void GameOver()
    {
        if (GameManager.Instance != null)
        {
            if (m_Points > GameManager.Instance.highScore)
        {
                GameManager.Instance.highScore = m_Points;
                GameManager.Instance.highScoreName = GameManager.Instance.playerName;
                GameManager.Instance.SaveHighScore();
            }
        }
        m_GameOver = true;
        GameOverText.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class TetrisManager : MonoBehaviour
{
    public int score;
    private TetrisGrid grid;
    [SerializeField] GameObject gameOverText;
    [SerializeField] TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start()
    {
        grid = FindAnyObjectByType<TetrisGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGameOver();
        scoreText.text = "score:" + score;
    }

    public void CalculateScore(int linesCleared)
    {
        switch(linesCleared)
        {
            case 1: score += 100;
                break;
             case 2 : score += 200; 
                break;
            case 3:
                score += 400;
                break;
            case 4:
                score += 800;
                break;
        }
    }

    public void CheckGameOver()
    {
        for (int i = 0; i < grid.width; i++)
        {
            if (grid.IsCellOccupied(new Vector2Int(i, grid.height - 1)))
            {
                Debug.Log("Game Over");
                gameOverText.SetActive(true);
                Invoke("ReloadScene", 3);
            }
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene("Tetris");
    }
}

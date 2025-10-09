using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState { WaitingToStart, Playing, GameOver }
    public GameState CurrentState { get; private set; } = GameState.WaitingToStart;
    public float ElapsedTime => m_ElapsedTime;


    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private TextMeshProUGUI TimeSurvived;
    [SerializeField] private TextMeshProUGUI DistanceRolled;
    [SerializeField] private TextMeshProUGUI SnowmanHealed;
    [SerializeField] private TextMeshProUGUI EnemiesKilled;
    [SerializeField] private TextMeshProUGUI SnowballsThrowed;
    [SerializeField] private BallDistanceTracker DistanceTracker;
    [SerializeField] private SnowMan SnowManObject;
    [SerializeField] private SnowShooter SnowShooterObject;

    private int m_EnemiesDefeated;
    private float m_ElapsedTime;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        GameOverPanel.SetActive(false);
    }

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if (CurrentState != GameState.GameOver)
            m_ElapsedTime += Time.deltaTime;
    }

    public void EnemyDefeated()
    {
        m_EnemiesDefeated++;
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
         Cursor.visible = false;
    }

    public void GameOver()
    {
        if (CurrentState == GameState.GameOver) return;
        CurrentState = GameState.GameOver;
        ShowGameOver();
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void ShowGameOver()
    {
        GameOverPanel.SetActive(true);
        TimeSurvived.text = FormatTime(m_ElapsedTime);
        EnemiesKilled.text = m_EnemiesDefeated.ToString();
        DistanceRolled.text = DistanceTracker.TotalDistance.ToString();
        SnowmanHealed.text = SnowManObject.TotalHealed.ToString();
        SnowballsThrowed.text = SnowShooterObject.TotalBallsThrowed.ToString();
    }
    
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes}:{secs:00}";
    }
}

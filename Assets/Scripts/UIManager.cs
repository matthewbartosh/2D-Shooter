using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _ammoText;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartGameText;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Sprite[] _livesSprites;
    [SerializeField]
    private GameObject[] _shieldSprites;
    [SerializeField]
    private float _flickerTime = 0.5f;
    private GameManager _gameManager;
   

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _ammoText.text = "Ammo: " + 15 + " / " + 15;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("GameManager is NULL.");
        }
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateAmmo(int playerAmmo)
    {
        _ammoText.text = "Ammo: " + playerAmmo + " / " + 15;
    }

    public void UpdateLives(int currentLives)
    {
        _livesImg.sprite = _livesSprites[currentLives];
    }

    public void GameOverUI()
    { 
        _gameManager.GameOver();
        StartCoroutine(GameOverFlicker());
        _restartGameText.gameObject.SetActive(true);
    }

    IEnumerator GameOverFlicker()
    {
        while (true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(_flickerTime);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(_flickerTime);
        }
    }

    public void ShieldLife(int _shieldHealth) // 3 Shield points, 2 shield points, 1 shield point, 0-default no shields
    {
        _shieldSprites[_shieldHealth].SetActive(true);
        _shieldSprites[_shieldHealth + 1].SetActive(false);
    }
}

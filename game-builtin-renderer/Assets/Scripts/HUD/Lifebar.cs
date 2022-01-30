using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Lifebar : MonoBehaviour
{
    [SerializeField] int _lives;
    [SerializeField] int _lifeMax;
    [SerializeField] int _lifeTotal;
    [SerializeField] GameObject[] _lifeSegments;
    [SerializeField] GameObject _lifeTextMesh;
    [SerializeField] GameObject _maxTextMesh;
    [SerializeField] GameObject _livesRemaining;

    // Start is called before the first frame update
    void Start()
    {
        if(_lifeMax > 20)
        {
            _lifeMax = 20;
        }

        _lifeTotal = _lifeMax;
        
        GGJ2022.EnemyAI.PlayerState.OnGainedALife += OneUp;
        GGJ2022.EnemyAI.PlayerState.OnDied += Death;
        GGJ2022.EnemyAI.PlayerState.OnResetHealth += ResetHealth;
        GGJ2022.EnemyAI.PlayerState.OnDamaged += TakeDamage;

        UpdateLifeBar();
        UpdateHealth();
    }

    void OneUp()
    {
        _lives++;
        UpdateLives();
    }

    void Death()
    {
        _lives--;
        UpdateLives();
    }

    void ResetHealth(int _newTotalHealth)
    {
        _lifeMax = _lifeTotal = _newTotalHealth;
        UpdateLifeBar();
        UpdateHealth();
    }

    void TakeDamage(int _incomingDamage)
    {
        if(_lifeTotal >= 0)
        {
            _lifeTotal -= _incomingDamage;
        }
        if(_lifeTotal < 0)
        {
            _lifeTotal = 0;
        }

        UpdateLifeBar();
        UpdateHealth();
    }

    public void UpdateLifeBar()
    {
        for(int i = 0; i < _lifeMax; i++)
        {
            if(i < _lifeTotal)
            {
                _lifeSegments[i].SetActive(true);
            }
            else
            {
                _lifeSegments[i].SetActive(false);            }
        }
    }

    public void UpdateHealth()
    {
        _lifeTextMesh.GetComponent<TextMeshProUGUI>().text = "" + _lifeTotal;
        _maxTextMesh.GetComponent<TextMeshProUGUI>().text = "/ " + _lifeMax;
    }

    public void UpdateLives()
    {
        _livesRemaining.GetComponent<TextMeshProUGUI>().text = "x" + _lives;
    }
}

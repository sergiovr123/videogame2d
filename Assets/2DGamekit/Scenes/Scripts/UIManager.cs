using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText = null;
    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Puntaje:" + 10.0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScore(double playerScore){
        _scoreText.text = "Puntaje:" + playerScore;
    }
}

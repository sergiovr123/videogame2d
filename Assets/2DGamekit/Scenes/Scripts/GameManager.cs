using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))] 
public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioClip m_correctSound = null;
    [SerializeField] private AudioClip m_incorrectSound = null;
    [SerializeField] private Color m_correctColor = Color.black;
    [SerializeField] private Color m_incorrectColor = Color.black;
    [SerializeField] private float m_waitTime = 0.0f;

    private QuizDB m_quizDB = null;
    private QuizUI m_quizUI = null;
    private AudioSource m_audioSource = null;
    private Double _score=10;
    private Double minimum_score=0;
    private UIManager _uiManager;
    private DateTime startDate ;
    private DateTime endDate ;

    private void Start() {
        _uiManager = GameObject.FindObjectOfType<UIManager>();
        m_quizDB = GameObject.FindObjectOfType<QuizDB>();
        m_quizUI = GameObject.FindObjectOfType<QuizUI>();
        m_audioSource = GetComponent<AudioSource>();
        startDate=DateTime.UtcNow.AddHours(-5);
        NextQuestion();
    }

    private void NextQuestion(){
        m_quizUI.Construct(m_quizDB.GetRandom(),GiveAnswer);
    }

    private void GameOver(){
        SceneManager.LoadScene(2);
    }

    private void GiveAnswer(OptionButton optionButton){
        StartCoroutine(GiveAnswerRoutine(optionButton));
    }

    private IEnumerator GiveAnswerRoutine(OptionButton optionButton){
        if(m_audioSource.isPlaying)
            m_audioSource.Stop();

        m_audioSource.clip = optionButton.Option.correct? m_correctSound: m_incorrectSound;
        optionButton.SetColor(optionButton.Option.correct?m_correctColor:m_incorrectColor);
        m_audioSource.Play();

        yield return new WaitForSeconds(m_waitTime);
        if(optionButton.Option.correct){
            endDate=DateTime.UtcNow.AddHours(-5);
            SaveProgress();
        }else{
            _score-=2.5;
            if(_score.CompareTo(minimum_score)<0 || _score.CompareTo(minimum_score)==0){
                GameOver();
            }else{
                _uiManager.UpdateScore(_score);
                NextQuestion();
            }
        }
    }
    private void SaveProgress(){
        // URL of the endpoint
        string url = "localhost:4040/v1/sessions-game";

        // JSON data you want to send
        string jsonData = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{{\"startDate\": \"{0}\", \"finalDate\": \"{1}\", \"userId\": \"12345678\", \"result\": \"{2}\"}}", startDate.ToUniversalTime().ToString("o"),endDate.ToUniversalTime().ToString("o"),_score);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        // Create a new UnityWebRequest, set method to POST and set the uploadHandler to the appropriate data
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and attach a callback
        StartCoroutine(SendRequest(request));
        GameOver();
    }

    IEnumerator SendRequest(UnityWebRequest request)
    {
        // Send the request itself
        yield return request.SendWebRequest();

        // Handle response
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    
    }

}

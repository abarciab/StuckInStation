using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager i;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] Fade fade;
    [SerializeField] MusicPlayer music;

    [HideInInspector] public TrainController CurrentTrain;
    private CameraController _cameraController;

    private void Awake()
    {
        i = this;
    }

    private void Start()
    {
        _cameraController = FindObjectOfType<CameraController>();   
        fade.Hide();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _cameraController.IsFollowingPlayer) TogglePause();
    }

    void TogglePause()
    {
        if (Time.timeScale == 0) Resume();
        else Pause();
    }

    
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        AudioManager.i.Resume();
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        AudioManager.i.Pause();
    }

    [ButtonMethod]
    public void LoadMenu()
    {
        Resume();
        StartCoroutine(FadeThenLoadScene(0));
    }

    [ButtonMethod]
    public void EndGame()
    {
        Resume();
        StartCoroutine(FadeThenLoadScene(2));
    }

    IEnumerator FadeThenLoadScene(int num)
    {
        fade.Appear(); 
        music.FadeOutAllCurrent(fade.fadeTime);
        yield return new WaitForSeconds(fade.fadeTime + 0.5f);
        Destroy(AudioManager.i.gameObject);
        SceneManager.LoadScene(num);
    }

}

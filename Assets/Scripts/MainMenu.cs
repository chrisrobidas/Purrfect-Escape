using System;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] 
    private GameObject _mainMenu;
    
    [SerializeField] 
    private GameObject _exitMenu;
    
    [SerializeField]
    private InputController _inputControllerScript;
    
    [SerializeField]
    private GameObject _cmVcam1;

    public bool _isPlaying;

    private void Start()
    {
        _isPlaying = false;
    }

    private void Update()
    {
        if (_isPlaying && Input.GetKeyDown(KeyCode.Escape))
        {
            if (_exitMenu.activeSelf)
            {
                HideExitMenu();
            }
            else
            {
                ShowExitMenu();
            }
        }
    }

    public void Play()
    {
        _mainMenu.SetActive(false);
        _cmVcam1.SetActive(true);
        _inputControllerScript.WakeUpCat();
        _isPlaying = true;
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void ShowExitMenu()
    {
        _inputControllerScript.StopCat();
        _cmVcam1.SetActive(false);
        _exitMenu.SetActive(true);
    }

    public void HideExitMenu()
    {
        _exitMenu.SetActive(false);
        _inputControllerScript.ResumeCat();
        _cmVcam1.SetActive(true);
    }
}

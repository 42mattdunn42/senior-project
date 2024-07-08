using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    public AudioSource buttonClick;
    private GameManager gm;



    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    public void Quit()
    {
        buttonClick.Play();
        Application.Quit();
    }
    public void LoadFightScene() //loads the first fight scene initially
    {
        gm.playFightSound();
        gm.LoadFightScene();
        gm.firstLoad = false;
    }
    public void LoadNextFight()
    {
        gm.playFightSound();
        gm.LoadFightScene();
    }
    public void LoadMainMenu()
    {
        buttonClick.Play();
        SceneManager.LoadScene("MainMenu");
    }
}

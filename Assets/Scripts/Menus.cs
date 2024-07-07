using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    private GameManager gm;
    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void LoadFightScene() //loads the first fight scene initially
    {
        SceneManager.LoadScene("FightScene");
        gm.firstLoad = false;
    }
    public void LoadNextFight()
    {
        gm.LoadFightScene();
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

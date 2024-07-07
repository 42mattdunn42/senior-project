using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    private GameObject HUD;
    private GameObject canvas;

    void Awake()
    {
        // Assign the current GameObject (HUDManager script is attached to) to 'HUD'
        HUD = gameObject;

        // Find the Canvas GameObject in the scene
        canvas = GameObject.Find("Canvas"); // Adjust this to find your Canvas GameObject by name
    }

    public void UnparentHUD()
    {
        // Check if the HUD has a parent (i.e., it's not already unparented)
        if (HUD.transform.parent != null)
        {
            // Unparent the HUD from its current parent (Canvas)
            HUD.transform.SetParent(null);
        }
    }

    public void DisableHUDChildren()
    {
        // Loop through all children of the HUD and deactivate them
        foreach (Transform child in HUD.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void EnableHUDChildren()
    {
        // Loop through all children of the HUD and deactivate them
        foreach (Transform child in HUD.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    public void ReparentHUDToCanvas()
    {
        // Find the Canvas GameObject in case it has changed
        canvas = GameObject.Find("Canvas");

        // Reparent the HUD under the Canvas GameObject
        HUD.transform.SetParent(canvas.transform);
    }
}

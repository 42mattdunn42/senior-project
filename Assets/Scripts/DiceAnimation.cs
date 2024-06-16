using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceAnimation : MonoBehaviour
{
    private Animator animator;
    private DiceRoller roller;
    private int diceIndex;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("No Animator found on " + gameObject.name);
        }
        diceIndex = int.Parse(this.name[this.name.Length - 1].ToString());
        roller = GameObject.FindGameObjectWithTag("Roller").GetComponent<DiceRoller>();
    }

    public void AnimateRoll()
    {
        animator.SetTrigger("OnRoll");
    }

    public void SetFace()
    {
        roller.SetFace(diceIndex);
    }
}

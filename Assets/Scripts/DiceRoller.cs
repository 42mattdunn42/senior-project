using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour
{
    public List<Button> dice;
    public List<int> faces;
    public int[] results { get; private set; }
    public List<DiceAnimation> animators;
    public bool[] rerollEligibility;
    int maxRerolls;
    bool allowSameReroll = false;

    public void SetDice(List<Button> dice) { this.dice = dice; }
    public List<Button> GetDice() { return this.dice; }
    public void SetNumFaces(List<int> faces) { this.faces = faces; }
    public List<int> GetFaces() { return this.faces; }
    private FightManager fm;

    public List<AudioSource> diceRolls;
    public List<AudioSource> dieRolls;

    private void Start()
    {
        fm = GameObject.FindGameObjectWithTag("FightManager").GetComponent<FightManager>();
        foreach(Button die in dice)
        {
            animators.Add(die.GetComponent<DiceAnimation>());
        }
        rerollEligibility = new bool[dice.Count];
    }
    /// <summary>
    /// Uses the list of TextMeshPro and numFaces to roll dice and display to the user
    /// </summary>
    /// <returns>An int array of the results of the roll(s)</returns>
    public void Roll()
    {
        diceRolls[UnityEngine.Random.Range(0, diceRolls.Count)].Play();
        ResetColor();
        int[] output = new int[dice.Count];
        for (int i = 0; i < dice.Count; i++)
        {
            output[i] = (UnityEngine.Random.Range(0, faces[i]) + 1);
            animators[i].AnimateRoll();
        }
        this.results =  output;
        fm.CalculateDamage();
    }

    /// <summary>
    /// Sets all dice faces to ""
    /// </summary>
    public void ClearDice()
    {
        foreach(Button die in dice)
        {
            die.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    /// <summary>
    /// Rerolls the specified die
    /// </summary>
    /// <param name="die_number"></param>
    public void ReRoll(int die_number)
    {
        if (rerollEligibility[die_number] && maxRerolls > 0)
        {
            dieRolls[UnityEngine.Random.Range(0, dieRolls.Count)].Play();
            if (!allowSameReroll)
            {
                rerollEligibility[die_number] = false;
            }
            ResetColor();
            dice[die_number].gameObject.transform.GetChild(0).gameObject.SetActive(false);
            animators[die_number].AnimateRoll();
            int temp = (UnityEngine.Random.Range(0, faces[die_number]) + 1);
            results[die_number] = temp;
            fm.CalculateDamage();
            maxRerolls--;
        }
    }

    // These are needed for the dice if they are buttons.
    /// <summary>
    /// Rerolls only die 0
    /// </summary>
    public void ReRoll_Die0()
    {
        ReRoll(0);
    }
    /// <summary>
    /// Rerolls only die 1
    /// </summary>
    public void ReRoll_Die1()
    {
        ReRoll(1);
    }
    /// <summary>
    /// Rerolls only die 2
    /// </summary>
    public void ReRoll_Die2()
    {
        ReRoll(2);
    }
    /// <summary>
    /// Rerolls only die 3
    /// </summary>
    public void ReRoll_Die3()
    {
        ReRoll(3);
    }
    /// <summary>
    /// Rerolls only die 4
    /// </summary>
    public void ReRoll_Die4()
    {
        ReRoll(4);
    }


    public void ColorLargeStraight()
    {
        for (int i = 0; i < dice.Count; i++)
        {
            dice[i].GetComponent<Image>().color = Color.red;
        }
    }
    public void ColorSmallStraight(int min)
    {
        Dictionary<int, bool> colored = new Dictionary<int, bool>();
        for (int i = 0; i < dice.Count; i++)
        {
            try
            {
                colored.Add(results[i], false);
            }
            catch { }
        }
        for (int i = 0; i < dice.Count; i++)
        {
            if (!(results[i] < min) && !(results[i] > min + 3) && !colored[results[i]])
            {
                dice[i].GetComponent<Image>().color = Color.red;
                colored[results[i]] = true;
            }
            else
            {
                dice[i].GetComponent<Image>().color = Color.gray;
            }
        }
    }
    public void ColorXOfAKind(int val)
    {
        for(int i = 0; i < dice.Count; i++)
        {
            if (results[i] == val)
            {
                dice[i].GetComponent<Image>().color = Color.red;
            }
            else
            {
                dice[i].GetComponent<Image>().color = Color.gray;
            }
        }
    }
    public void ColorFullHouse(int val)
    {
        for (int i = 0; i < dice.Count; i++)
        {
            if (results[i] == val)
            {
                dice[i].GetComponent<Image>().color = Color.red;
            }
            else
            {
                dice[i].GetComponent<Image>().color = Color.magenta;
            }
        }
    }
    public void ResetColor()
    {
        foreach(Button die in dice)
        {
            die.GetComponent<Image>().color = Color.white;
        }
    }

    /// <summary>
    /// Sets an individual dice to the desired value
    /// </summary>
    /// <param name="diceIndex"></param>
    /// <param name="val"></param>
    public void SetDiceValue(int diceIndex, int val)
    {
        //dice[diceIndex].GetComponentInChildren<TextMeshProUGUI>().text = val.ToString();
        SetFace(diceIndex, val);
        fm.CalculateDamage();  // color dice
    }

    /// <summary>
    /// Sets the number of faces for all dice to 6
    /// </summary>
    public void ResetDiceNumbers()
    {
        foreach(Button die in dice)
        {
            foreach (var tmp in die.GetComponentsInChildren<TextMeshProUGUI>())
            {
                if (tmp.name == "NumFaces")
                {
                    tmp.text = "6";
                }
            }
        }
        for(int i = 0; i < faces.Count; i++)
        {
            faces[i] = 6;
        }
    }

    /// <summary>
    /// Sets the number of faces for a die at the given index
    /// </summary>
    /// <param name="diceIndex"></param>
    /// <param name="numFaces"></param>
    public void SetDiceNumber(int diceIndex, int numFaces)
    {
        faces[diceIndex] = numFaces;
        foreach (var tmp in dice[diceIndex].GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (tmp.name == "NumFaces")
            {
                tmp.text = numFaces.ToString();
            }
        }
    }

    /// <summary>
    /// Used to show the results after animating
    /// </summary>
    /// <param name="index"></param>
    public void SetFace(int index)
    {
        foreach (var tmp in dice[index].GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (tmp.name == "Text (TMP)")
            {
                tmp.text = results[index].ToString();
            }
        }
    }

    /// <summary>
    /// Sets the face of a die at the index to the value
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    public void SetFace(int index, int value)
    {
        foreach (var tmp in dice[index].GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (tmp.name == "Text (TMP)")
            {
                tmp.text = value.ToString();
            }
        }
        results[index] = value;
    }

    /// <summary>
    /// Sets all values of rerollEligibility to true (this array allows a die to be rerolled when its index is true) and sets the max number of rerolls.
    /// </summary>
    public void allowRerolls(int maxRolls, bool allowSameRerolls)
    {
        for (int i = 0; i < rerollEligibility.Length; i++)
        {
            rerollEligibility[i] = true;
        }
        maxRerolls = maxRolls;
        allowSameReroll = allowSameRerolls;
    }
}

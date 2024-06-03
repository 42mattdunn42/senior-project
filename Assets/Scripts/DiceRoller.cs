using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour
{
    public List<Button> dice;
    public List<int> faces;
    public int[] results { get; private set; }

    public void SetDice(List<Button> dice) { this.dice = dice; }
    public List<Button> GetDice() { return this.dice; }
    public void SetNumFaces(List<int> faces) { this.faces = faces; }
    public List<int> GetFaces() { return this.faces; }
    private FightManager fm;

    private void Start()
    {
        fm = GameObject.FindGameObjectWithTag("FightManager").GetComponent<FightManager>();
    }
    /// <summary>
    /// Uses the list of TextMeshPro and numFaces to roll dice and display to the user
    /// </summary>
    /// <returns>An int array of the results of the roll(s)</returns>
    public void Roll()
    {
        ResetColor();
        int[] output = new int[dice.Count];
        for (int i = 0; i < dice.Count; i++)
        {
            output[i] = (Random.Range(0, faces[i]) + 1);
            dice[i].GetComponentInChildren<TextMeshProUGUI>().text = output[i].ToString();

        }
        this.results =  output;
        fm.CalculateDamage();
    }

    /// <summary>
    /// Sets all dice faces to zero
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
        ResetColor();
        int temp = (Random.Range(0, faces[die_number]) + 1);
        dice[die_number].GetComponentInChildren<TextMeshProUGUI>().text = temp.ToString();
        results[die_number] = temp;
        fm.CalculateDamage();
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
}

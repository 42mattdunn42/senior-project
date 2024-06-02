using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    public List<TextMeshProUGUI> dice;
    public List<int> faces;
    public int[] results { get; private set; }

    public void SetDice(List<TextMeshProUGUI> dice) { this.dice = dice; }
    public List<TextMeshProUGUI> GetDice() { return this.dice; }
    public void SetNumFaces(List<int> faces) { this.faces = faces; }
    public List<int> GetFaces() { return this.faces; }

    /// <summary>
    /// Uses the list of TextMeshPro and numFaces to roll dice and display to the user
    /// </summary>
    /// <returns>An int array of the results of the roll(s)</returns>
    public void Roll()
    {
        int[] output = new int[dice.Count];
        for (int i = 0; i < dice.Count; i++)
        {
            output[i] = (Random.Range(0, faces[i]) + 1);
            dice[i].text = output[i].ToString();

        }
        this.results =  output;
    }

    /// <summary>
    /// Sets all dice faces to zero
    /// </summary>
    public void ClearDice()
    {
        foreach(TextMeshProUGUI die in dice)
        {
            die.text = "";
        }
    }

    /// <summary>
    /// Rerolls the specified die
    /// </summary>
    /// <param name="die_number"></param>
    public void ReRoll(int die_number)
    {
        int temp = (Random.Range(0, faces[die_number]) + 1);
        dice[die_number].text = temp.ToString();
        results[die_number] = temp;
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
}

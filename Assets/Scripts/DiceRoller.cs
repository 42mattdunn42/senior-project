using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    public List<TextMeshProUGUI> dice;
    public List<int> faces;
    public int[] results { get; private set; }

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

    public void SetDice(List<TextMeshProUGUI> dice) { this.dice = dice; }
    public List<TextMeshProUGUI> GetDice() { return this.dice; }
    public void SetNumFaces(List<int> faces) { this.faces = faces; }
    public List<int> GetFaces() { return this.faces; }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class TextScrambler {

    /// <summary>
    /// Transition from one text to another over time
    /// </summary>
    /// <param name="monoBehaviour"></param>
    /// <param name="initial">text to transition from</param>
    /// <param name="final">text to transition to</param>
    /// <param name="time">time in seconds to transition</param>
    /// <param name="assignFunction">assign the output to your text. example: (result)=>{myText.text = result;}</param>
    public static void Scramble(this MonoBehaviour monoBehaviour, string initial, string final, float time, Action<string> assignFunction)
    {
        monoBehaviour.StartCoroutine(ScrambleCoroutine(initial, final, time, assignFunction));
    }

    private static IEnumerator ScrambleCoroutine(string initial, string final, float time, Action<string> assignFunction)
    {
        string bigger = initial.Length > final.Length ? initial : final;
        var waitTime = new WaitForSeconds(time / bigger.Length);
        List<int> positions = new List<int>(bigger.Length);
        for (int i = 0; i < bigger.Length; i++)
        {
            positions.Add(i);
        }
        StringBuilder stringBuilderInitial = new StringBuilder(initial);

        while(positions.Count > 0)
        {
            int posIdx = UnityEngine.Random.Range(0, positions.Count);
            int pos = positions[posIdx];
            if(pos < stringBuilderInitial.Length)
            {
                if(pos < final.Length)
                {
                    stringBuilderInitial[pos] = final[pos];
                }
                else
                {
                    stringBuilderInitial.Remove(pos, 1);
                    if(posIdx < positions.Count - 1)
                    {
                        for (int i = posIdx + 1; i < positions.Count; i++)
                        {
                            positions[i]--;
                        }
                    }
                }
            }
            else
            {
                stringBuilderInitial.Append(' ', pos + 1 - stringBuilderInitial.Length);
                stringBuilderInitial[pos] = final[pos];
            }
            positions.RemoveAt(posIdx);
            assignFunction(stringBuilderInitial.ToString());
            yield return waitTime;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPlaytime : MonoBehaviour
{
    public double totalPlaytime;
    public static TrackPlaytime instance;
    public void SetTotalPlaytime(double value)
    {
        totalPlaytime = value;
    }
    private void Awake()
    {
        instance = this;
        StartCoroutine(TrackPlaytimeRoutine());
    }
    public IEnumerator TrackPlaytimeRoutine()
    {
        while (true)
        {
            const float timerInterval = 0.5f; //count by half seconds
            //wait for 1 second ignoring timescale.
            yield return StartCoroutine(WaitForRealSeconds(timerInterval));

            totalPlaytime += timerInterval;

        }
    }
    public static IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }
}
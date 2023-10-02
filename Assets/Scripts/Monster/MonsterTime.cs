using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterTime : MonoBehaviour
{
    public GameObject monster;
    public float interval;
    private float timer = 0.0f;
    public float startSecond = 5.0f;
    public float endSecond = 10.0f;

    private void Awake()
    {
        monster.SetActive(false);
        interval = Random.Range(startSecond, endSecond);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > interval&&monster!=null)
        {
            monster.SetActive(true);
            timer = 0.0f;
            
        }
    }
}

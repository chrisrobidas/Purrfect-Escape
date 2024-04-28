using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keygen : MonoBehaviour
{
    void Start()
    {
        Bottle[] bottles = FindObjectsOfType<Bottle>();
        int randomBottle = Random.Range(1, bottles.Length);
        bottles[randomBottle].ContainsKey = true;
    }
}

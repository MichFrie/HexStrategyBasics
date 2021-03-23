using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    public UnitStats unitStats;

    public void Damage()
    {
        unitStats.strength--;
    }
}


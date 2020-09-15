using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Units", menuName ="ScriptableObjects/UnitMenu")]
public class Unit : ScriptableObject
{
    public int strength;
    public int attackPower;
    public int defenseValue;
    public int id;
}



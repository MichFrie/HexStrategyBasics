using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Officer", menuName = "Create new Officer")]
public class OfficerStats : ScriptableObject
{
    public string officerName;
    public int experience;
    public int competence;

    public int movementPoints;
}

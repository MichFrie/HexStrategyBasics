using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitStatsDisplay : MonoBehaviour
{
    public UnitStats unitStats;

    public Text unitName;
    public Text unitStrength;
    public Text unitMovementPoints;

    public void DisplayUnitStatsOnGui()
    {
        this.unitName.text = $"Name: {this.unitStats.name}";
        this.unitStrength.text = $"Strength: {(this.unitStats.strength).ToString()}";
        this.unitMovementPoints.text = $"Movement Points: {this.unitStats.movementPoints}";
    }
}

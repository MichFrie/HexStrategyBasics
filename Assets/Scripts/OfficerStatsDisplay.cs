using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfficerStatsDisplay : MonoBehaviour
{
    public OfficerStats officerStats;

    public Text officerName;
    public Text officerExperience;
    public Text officerCompetence;

    public void DisplayOfficerStatsOnGui()
    {
        this.officerName.text = $"Name: {(this.officerStats.officerName).ToString()}";
        this.officerExperience.text = $"Experience: {this.officerStats.experience}";
        this.officerCompetence.text = $"Competence: {this.officerStats.competence}";
    }

}

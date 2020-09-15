using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTest : MonoBehaviour
{

    public Unit unit;
    public UnitStats unitStats;
    public UnitCreation unitCreation;

    private void Start()
    {
        unitStats = GetComponent<UnitStats>();
    }

    private void Update()
    {
        Attack();
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Attack");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                unitStats.strength = 0;
               
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet // »ó¼Ó!!
{
    public Transform target;
    NavMeshAgent navi;

    void Awake()
    {
        navi = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        navi.SetDestination(target.position);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : Selectable
{
    [Header("Shooting")]
    public Transform FirePosition;
    public GameObject BulletPrefab;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }
}

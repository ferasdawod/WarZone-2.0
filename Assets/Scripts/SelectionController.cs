using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public void OnSelectUnit(RaycastHit hit)
    {
        var selectable = hit.collider.GetComponent<Selectable>();
        if (selectable && selectable.hasAuthority)
        {
            selectable.ToggleSelection();
        }
    }
}

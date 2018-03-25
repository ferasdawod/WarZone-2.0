using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Selectable : NetworkBehaviour
{
    [Serializable]
    public class SelectionOptions
    {
        public Material NormalMaterial;
        public Material SelectedMaterial;
        public MeshRenderer TargetRenderer;
        public bool SelectionState;
    }

    public SelectionOptions SelectionSettings;
    private bool _lastSelectionState;

    public bool IsSelected { get { return SelectionSettings.SelectionState; } }

    protected virtual void Awake()
    {
        if (!SelectionSettings.SelectedMaterial) Debug.LogError("SelectedMaterial Not set", this);
        if (!SelectionSettings.TargetRenderer) Debug.LogError("TargetRenderer Not set", this);

        if (!SelectionSettings.NormalMaterial)
        {
            SelectionSettings.NormalMaterial = SelectionSettings.TargetRenderer.material;
        }

        SelectionSettings.SelectionState = _lastSelectionState = false;
    }

    protected virtual void Update()
    {
        if (_lastSelectionState != SelectionSettings.SelectionState)
        {
            SelectionSettings.TargetRenderer.material = SelectionSettings.SelectionState ? SelectionSettings.SelectedMaterial : SelectionSettings.NormalMaterial;
            _lastSelectionState = SelectionSettings.SelectionState;
        }
    }

    public void ToggleSelection()
    {
        SelectionSettings.SelectionState = !SelectionSettings.SelectionState;
    }

    public void SetSelectionState(bool isSelected)
    {
        SelectionSettings.SelectionState = isSelected;
    }
}
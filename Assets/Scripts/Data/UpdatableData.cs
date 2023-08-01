using UnityEditor;
using UnityEngine;

public class UpdatableData : ScriptableObject
{
    public event System.Action OnValueUpdated;
    public bool autoUpdate;


    protected virtual void OnValidate()
    {
        if (autoUpdate)
        {
            NotifyOfUpdatedValues();
        }
    }
    public void NotifyOfUpdatedValues()
    {
        OnValueUpdated?.Invoke();
    }



}

#if UNITY_EDITOR


[CustomEditor(typeof(UpdatableData),true)]
public class UpdatableDataCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UpdatableData data = (UpdatableData)target;

        if (GUILayout.Button("Update"))
        {
            data.NotifyOfUpdatedValues();
        }
    }
}
#endif
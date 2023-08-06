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
            UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
        }
    }
    public void NotifyOfUpdatedValues()
    {
        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
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
            EditorUtility.SetDirty(target);
        }
    }
}
#endif
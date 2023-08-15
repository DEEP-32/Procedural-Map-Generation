using FirstProceduralGeneration;
using UnityEngine;

[CreateAssetMenu(fileName ="HeightMapSettings",menuName = "Data/HeightMapSettings")]
public class HeightMapSettings : UpdatableData
{
    public NoiseSetting noisesetting;

    public bool useFalloff;
    public float heightMultiplier;
    public AnimationCurve heightCurve;

    public float minHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get { return heightMultiplier * heightCurve.Evaluate(1); }
    }

#if UNITY_EDITOR
    override protected void OnValidate()
    {
       noisesetting.ValidateValues();

        base.OnValidate();
    }
#endif
}

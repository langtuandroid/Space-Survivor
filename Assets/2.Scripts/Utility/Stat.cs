using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField]
    private float baseValue = 0;

    [SerializeField]
    private bool minValue = false;
    [SerializeField]
    private float MinValue = 0;

    [SerializeField]
    private bool maxValue = false;
    [SerializeField]
    private float MaxValue = 1000;

    [SerializeField]
    private List<float> FloatModifiers = new List<float>();
    [SerializeField]
    private List<float> PercentModifiers = new List<float>();

    public float GetFinalStatValue()
    {
        float finalValue = baseValue;
        FloatModifiers.ForEach(x => finalValue += x);

        float totalPercent = 1;

        PercentModifiers.ForEach(x => totalPercent += x);

        finalValue = finalValue * totalPercent;

        if (maxValue)
            finalValue = Mathf.Clamp(finalValue, 0, MaxValue);

        if (minValue)
            finalValue = Mathf.Clamp(finalValue, MinValue, finalValue);

        return finalValue;
    }

    public int GetFinalStatValueAsInt()
    {
        float finalValue = baseValue;
        FloatModifiers.ForEach(x => finalValue += x);

        float totalPercent = 1;

        PercentModifiers.ForEach(x => totalPercent += x);

        finalValue = finalValue * totalPercent;

        if (maxValue)
            finalValue = Mathf.Clamp(finalValue, 0, MaxValue);

        if (minValue)
            finalValue = Mathf.Clamp(finalValue, MinValue, finalValue);

        return Mathf.RoundToInt(finalValue);
    }

    public void AddFloatModifier(float modifier)
    {
        if (modifier != 0)
            FloatModifiers.Add(modifier);
    }

    public void RemoveIntModifier(int modifier)
    {
        if (modifier != 0)
            FloatModifiers.Remove(modifier);
    }

    public void AddPercentModifier(float modifier)
    {
        if (modifier != 0)
            PercentModifiers.Add(modifier);
    }

    public void RemovePercentModifier(float modifier)
    {
        if (modifier != 0)
            PercentModifiers.Remove(modifier);
    }

    public void ClearFloatModifier()
    {
        FloatModifiers.Clear();
    }

    public void ClearPercentModifier()
    {
        PercentModifiers.Clear();
    }

    public void ClearAllModifier()
    {
        FloatModifiers.Clear();
        PercentModifiers.Clear();
    }

    public void SetBaseValue(float value)
    {
        baseValue = value;
    }

    public float GetBaseValue()
    {
        return baseValue;
    }
}

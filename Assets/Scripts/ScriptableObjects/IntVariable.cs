using UnityEngine;

[CreateAssetMenu(fileName = "IntVariable", menuName = "ScriptableObjects/IntVariable", order = 1)]
public class IntVariable : Variable<int>
{
    public override void SetValue(int value)
    {
        _value = value;
    }

    // Overload
    public void SetValue(IntVariable value)
    {
        SetValue(value.Value);
    }

    public void Add(int amount)
    {
        _value += amount;
    }

    public void Subtract(int amount)
    {
        _value -= amount;
    }
}
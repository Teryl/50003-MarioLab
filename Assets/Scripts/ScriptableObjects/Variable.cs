using UnityEngine;

public abstract class Variable<T> : ScriptableObject
{
    [SerializeField]
    protected T _value;

    public T Value
    {
        get { return _value; }
        set { SetValue(value); }
    }

    public abstract void SetValue(T value);
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableSO<T> : ScriptableObject
{
    public event Action<ValueDataChanged<T>> onValueChanged;

    [SerializeField] protected T m_RuntimeValue;
    public virtual T Value
    {
        get => m_RuntimeValue;
        set
        {
            var oldValue = m_RuntimeValue;
            m_RuntimeValue = value;
            if (!IsEquals(oldValue, m_RuntimeValue))
                onValueChanged?.Invoke(new ValueDataChanged<T>(oldValue, value));
        }
    }

    protected virtual bool IsEquals(T valueA, T valueB)
    {
        return EqualityComparer<T>.Default.Equals(valueA, valueB);
    }
}

public struct ValueDataChanged<T>
{
    public T oldValue { get; set; }
    public T newValue { get; set; }

    public ValueDataChanged(T oldValue, T newValue)
    {
        this.oldValue = oldValue;
        this.newValue = newValue;
    }
}

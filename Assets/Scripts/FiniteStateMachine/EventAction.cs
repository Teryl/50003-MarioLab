using System;

public enum ActionType
{
    Attack = 0,
    Default = -1
}

[Serializable]
public struct EventAction
{
    public Action action;
    public ActionType type;
}
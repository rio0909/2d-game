using System;

[Serializable]
public class DialogueChoice
{
    public string text;
    public Action onSelect;

    public DialogueChoice(string text, Action onSelect)
    {
        this.text = text;
        this.onSelect = onSelect;
    }
}

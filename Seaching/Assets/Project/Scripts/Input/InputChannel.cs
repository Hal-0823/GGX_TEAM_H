using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// InputActionの切り替えを通知するチャネル
/// </summary>
[CreateAssetMenu(menuName = "Events/Input Channel")]
public class InputChannel : ScriptableObject
{
    public UnityAction OnRequestDialogueControl;
    public UnityAction OnRequestPlayerControl;

    public void SwitchToDialogue()
    {
        OnRequestDialogueControl?.Invoke();
    }

    public void SwitchToPlayer()
    {
        OnRequestPlayerControl?.Invoke();
    }
}
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "CutSceneData", menuName = "CutSceneData", order = 1)]
public class CutSceneData : ScriptableObject, IEnumerable<CutSceneAction>
{
    public string cutSceneName;

    public List<CutSceneAction> cutSceneActions;

    public IEnumerator<CutSceneAction> GetEnumerator()
    {
        return cutSceneActions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return cutSceneActions.GetEnumerator();
    }
}

[System.Serializable]
public class CutSceneAction{
    public string name;
    [TextArea] public string text;
    // Existing SOs can store a sprite together with the legacy clear flag.
    [HideInInspector] public bool clearSprite;
    [LabelText("MySide")] public bool isLeft;
    public Sprite sprite;
    public bool callFunction;
    public string clazz;
    public string method;

    public CutSceneActionType Type => !callFunction ? CutSceneActionType.Dialogue :
        method == "WaitUntil" || method == "WaitUntilAndHide"
            ? CutSceneActionType.Wait : CutSceneActionType.Function;

    public Sprite DisplaySprite => clearSprite ? null : sprite;
}

public enum CutSceneActionType
{
    Dialogue,
    Function,
    Wait
}

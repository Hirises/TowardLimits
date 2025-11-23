using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
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
    public bool clearSprite;
    [Label("MySide")] public bool isLeft;
    public Sprite sprite;
    public bool callFunction;
    public string clazz;
    public string method;
}

using UnityEngine;
using UnityEngine.UI;

public class MultiImageTargetGraphics : MonoBehaviour
{
    [SerializeField] private Graphic[] targetGraphics;

    public Graphic[] GetTargetGraphics => targetGraphics;
#if UNITY_EDITOR
    [ContextMenu("Get All Graphics")]
    void GetAllGraphics()
    {
        targetGraphics = this.GetComponentsInChildren<Graphic>();
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
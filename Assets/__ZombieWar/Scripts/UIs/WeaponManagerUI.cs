using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerWeaponEventCode
{
    /// <summary>
    /// Raised when weapon of player has changed.
    /// <para><typeparamref name="WeaponSO"/>: Changed weapon</para>
    /// </summary>
    OnWeaponChanged,
    /// <summary>
    /// Raised when weapon select button just clicked.
    /// <para><typeparamref name="WeaponSO"/>: Changed weapon</para>
    /// </summary>
    OnRequestWeaponChange,
}

public class WeaponManagerUI : MonoBehaviour
{
    [SerializeField] WeaponManagerSO weaponManagerSO;
    [SerializeField] WeaponSelectUI weaponSelectUIPrefab;
    [SerializeField] PPrefWeaponSOVariable currentUsingWeaponSO;
    [SerializeField] Button expandBtn;
    [SerializeField] Button collapseBtn;
    [SerializeField] Image expandBtnImage;
    [SerializeField] CanvasGroup collapsedUICG;
    [SerializeField] CanvasGroup expandedUICG;

    bool isExpanded = false;
    Sequence sequence;
    List<Vector2> orginSelectButtonPositions = new();
    List<RectTransform> weaponSelectUIs = new();
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void OnDestroy()
    {
        UnsubscribeButtons();
    }

    void Init()
    {
        SetupCanvasGroups();
        SetupSelectBtnUIs();
        SubscribeButtons();
    }

    void SetupCanvasGroups()
    {
        //TODO: setup canvas groups, disable expanded, enable collapsed at the beginning
        collapsedUICG.alpha = 1;
        collapsedUICG.blocksRaycasts = true;
        expandedUICG.alpha = 0;
        expandedUICG.blocksRaycasts = false;
    }

    void SetupSelectBtnUIs()
    {
        if (!weaponManagerSO) return;
        if (!expandedUICG.TryGetComponent(out VerticalLayoutGroup verticalLayoutGroup)) return;
        if (!expandedUICG.TryGetComponent(out ContentSizeFitter contentSizeFitter)) return;

        //Setup collapsed UI
        expandBtnImage.sprite = currentUsingWeaponSO.Value.WeaponIcon;

        //Setup expand UI
        verticalLayoutGroup.enabled = true;
        foreach (var weaponSO in weaponManagerSO.WeaponSOs)
        {
            var instance = Instantiate(weaponSelectUIPrefab, expandedUICG.transform);
            instance.WeaponSO = weaponSO;
            weaponSelectUIs.Add(instance.GetComponent<RectTransform>());
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(expandedUICG.GetComponent<RectTransform>());
        contentSizeFitter.enabled = false;
        verticalLayoutGroup.enabled = false;

        foreach (var selectBtn in weaponSelectUIs)
        {
            //Change anchor but keep the position
            Vector3 oldPos = selectBtn.position;
            selectBtn.anchorMin = Vector2.one * 0.5f;
            selectBtn.anchorMax = Vector2.one * 0.5f;
            selectBtn.position = oldPos;

            //Store expanded position
            orginSelectButtonPositions.Add(selectBtn.anchoredPosition);

            //Reset to collapsed position
            selectBtn.anchoredPosition = Vector2.zero;
        }
    }

    void SubscribeButtons()
    {
        collapseBtn.onClick.AddListener(HandleCollapsedButtonClicked);
        expandBtn.onClick.AddListener(HandleExpandedButtonClicked);
    }

    void UnsubscribeButtons()
    {
        collapseBtn.onClick.RemoveListener(HandleCollapsedButtonClicked);
        expandBtn.onClick.RemoveListener(HandleExpandedButtonClicked);
    }

    void HandleExpandedButtonClicked()
    {
        if (isExpanded) return;
        isExpanded = true;
        if (sequence != null)
        {
            sequence.Kill();
            sequence = null;
        }
        sequence = DOTween.Sequence();
        expandedUICG.blocksRaycasts = false;
        collapsedUICG.blocksRaycasts = false;
        sequence.Append(expandedUICG.DOFade(1, 0.5f));
        sequence.Join(collapsedUICG.DOFade(0, 0.5f));
        for (int i = 0; i < weaponSelectUIs.Count; i++)
        {
            sequence.Join(weaponSelectUIs[i].DOAnchorPos(orginSelectButtonPositions[i], 0.5f));
        }
        sequence.OnComplete(() =>
        {
            expandedUICG.blocksRaycasts = true;
        });
    }

    void HandleCollapsedButtonClicked()
    {
        if (!isExpanded) return;
        isExpanded = false;
        if (sequence != null)
        {
            sequence.Kill();
            sequence = null;
        }
        sequence = DOTween.Sequence();
        expandedUICG.blocksRaycasts = false;
        collapsedUICG.blocksRaycasts = false;
        sequence.Append(expandedUICG.DOFade(0, 0.5f));
        sequence.Join(collapsedUICG.DOFade(1, 0.5f));
        for (int i = 0; i < weaponSelectUIs.Count; i++)
        {
            sequence.Join(weaponSelectUIs[i].DOAnchorPos(Vector2.zero, 0.5f));
        }
        sequence.OnComplete(() =>
        {
            collapsedUICG.blocksRaycasts = true;
        });
    }
}

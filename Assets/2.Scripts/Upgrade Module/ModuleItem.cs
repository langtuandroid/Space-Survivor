using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using TMPro;

public class ModuleItem : MonoBehaviour
{
    public enum SlotType
    {
        inventory = 1,
        equip = 2
    }

    [field: SerializeField] public IUpgradeModule module { get; private set; }

    [Space]

    [SerializeField] private Image slotIamge;
    [SerializeField] private Image moduleImage;
    [SerializeField] private Image coverImage;
    [field: SerializeField] public Image moduleUpgradeCoverImage { get; private set; }
    [field: SerializeField] public SlotType slotType;

    [Space]

    [SerializeField] private Color defaultColor_equip;
    [SerializeField] private Color defaultColor_inventory;

    [SerializeField] private Color selectedColor;
    [SerializeField] private TextMeshProUGUI newText;

    [SerializeField] private CanvasGroup canvasGroup;



    private Sequence sequence;

    public static readonly Color32 Color_specialModule = new Color32(224, 168, 71, 255);

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => AudioManager.instance.PlaySFX("click2"));
    }

    public void OnClickThisModule()
    {
        UpgradeModuleManager.instance.OpenModuleUpgradeDetail(this);
    }

    public void InitModule(IUpgradeModule module, SlotType slotType, bool tweening = true, System.Action<ModuleItem> onclick = null, bool Lock = false)
    {
        this.module = module;
        this.slotType = slotType;

        UpdateImage(tweening);
        UnselectSlot();

        if (onclick != null)
            GetComponent<Button>().onClick.AddListener(() => onclick.Invoke(this));

        SetModuleImageColor(moduleImage, module.type);

        // print(1111);
        if (newText != null)
        {
            if (module.GetUpgradeModuleObject().isNew)
            {
                newText.gameObject.SetActive(true);
                // module.GetUpgradeModuleObject().isNew = false;
            }
            else
            {
                newText.gameObject.SetActive(false);
            }
        }

        if (canvasGroup != null)
        {
            if (Lock)
            {
                canvasGroup.DOFade(0.48f, 0.5f).OnStart(() => canvasGroup.alpha = 1f);
                GetComponent<Image>().enabled = false;
            }
            else
            {
                canvasGroup.alpha = 1f;
                GetComponent<Image>().enabled = true;
            }
        }


    }

    public void UpdateImage(bool tweening = true)
    {
        moduleImage.sprite = UpgradeModuleManager.instance.GetUpgradeModuleImage((int)module.GetUpgradeModuleObject().module);
        coverImage.sprite = UpgradeModuleManager.instance.GetUpgradeModuleCover(module.GetUpgradeModuleObject().tier);

        if (tweening)
        {
            sequence
            .Append(moduleImage.transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.15f).SetLoops(2, LoopType.Yoyo))
            .Append(coverImage.transform.DOScale(new Vector3(1.25f, 1.25f, 1.25f), 0.15f).SetLoops(2, LoopType.Yoyo))
            .SetLoops(1);
        }
    }

    public void UpgradeModuleSelect()
    {
        if (UpgradeModuleManager.instance.SelectModuleUpgradeSlot(this))
        {
            slotIamge.color = selectedColor;
        }
        else
        {
            UnselectSlot();
        }
    }

    public void UnselectSlot()
    {
        switch (slotType)
        {
            case SlotType.inventory:
                slotIamge.color = defaultColor_inventory;

                break;

            case SlotType.equip:
                slotIamge.color = defaultColor_equip;

                break;

            default:
                slotIamge.color = defaultColor_inventory;

                break;
        }
    }

    public static void OpenModuleDetail(ModuleItem item)
    {
        var detail = Resources.Load<GameObject>("UI/ModuleDetail");
        var obj = Instantiate(detail, GameManager.instance.MainUIParent);
        obj.GetComponent<ModuleDetailUI>().Init(item);
    }

    public static void SetModuleImageColor(Image image, UpgradeModuleType type)
    {
        switch (type)
        {
            case UpgradeModuleType.SpecialType:
                image.color = Color_specialModule;
                break;

            default:
                image.color = Color.white;
                break;

        }
    }
}

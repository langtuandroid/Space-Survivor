using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ModuleSortButton : MonoBehaviour
{
    [SerializeField] Button btn;
    public UpgradeModuleType type;

    public Color selectedColor;
    public Color unselectedColor;


    // Start is called before the first frame update
    void Awake()
    {
        btn = GetComponent<Button>();
    }

    public void OnClickSortButton()
    {
        UpgradeModuleManager.instance.UpdateUI(type);
        // UpgradeModuleManager.instance.GenerateInventoryModulePrefabs(sortType: type);

        EventManager.instance.CustomEvent(AnalyticsType.UI, "UI_ModuleSortButton - " + type.ToString());
    }

    public void OnSelect()
    {
        btn.GetComponent<Image>().color = selectedColor;
    }

    public void OnUnselect()
    {
        btn.GetComponent<Image>().color = unselectedColor;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ModuleEquipSlot : MonoBehaviour
{
    [field: SerializeField] public Image moduleImage {get; private set;}

    public void InitSlot(Sprite image)
    {
        moduleImage.sprite = image;

        moduleImage.gameObject.SetActive(true);
    }

    public void ClearThisSlot()
    {
        moduleImage.sprite = null;

        moduleImage.gameObject.SetActive(false);
    }
}

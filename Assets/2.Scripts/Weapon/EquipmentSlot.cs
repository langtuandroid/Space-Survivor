using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public IEquipment weaponObject;
    [SerializeField] private Image weaponImage;

    [Space]

    [SerializeField] private Transform weaponUpgradeNodeParent;
    [SerializeField] private GameObject weaponUpgradeNode;
    [SerializeField] private GameObject weaponCover;
    [SerializeField] private GameObject passiveCover;

    public WeaponCoolTimeImage weaponCoolTimeImage;

    public void SetEquipment(IEquipment weapon)
    {
        weaponObject = weapon;
        weaponImage.sprite = weapon.GetEquipmentIamge();

        //장비 타입에 맞는 커버 활성화
        switch (weapon.GetAnyEqupment())
        {
            case AnyEqupment.Weapon:
                weaponCover.SetActive(true);
                passiveCover.SetActive(false);

                break;

            case AnyEqupment.Passive:
                weaponCover.SetActive(false);
                passiveCover.SetActive(true);

                break;

            default:
                weaponCover.SetActive(true);
                passiveCover.SetActive(false);

                break;

        }
    }

    public void AddUpgradeNode()
    {
        Instantiate(weaponUpgradeNode, weaponUpgradeNodeParent);
    }
}

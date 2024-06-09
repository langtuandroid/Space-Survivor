using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AlterManager : SingletonStatic<AlterManager>
{

    public void GenerateNewModuleAlter(UpgradeModuleObject newModule)
    {
        if (newModule == null)
            return;

        var alter = Instantiate(Resources.Load<GameObject>("UI/NewModuleAlter"), GameManager.instance.MainUIParent);

        alter.GetComponentInChildren<ModuleItem>().InitModule(newModule, ModuleItem.SlotType.equip);

        alter.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1f);
        alter.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1f);

        var seq = DOTween.Sequence().SetUpdate(true);
        seq.Join(alter.GetComponent<RectTransform>().DOAnchorPosX(0, 1f).SetEase(Ease.OutCubic).OnStart(() => { }));
        seq.AppendInterval(1.5f);
        seq.Append(alter.transform.DOScale(Vector3.zero, 0.7f).SetEase(Ease.InCubic)).OnComplete(() => Destroy(alter));
    }
}

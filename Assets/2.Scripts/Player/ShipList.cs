using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ShipList", menuName = "Scriptable Object/Ship/new ShipList", order = int.MaxValue)]
public class ShipList : ScriptableObject
{
    public List<ShipObject> shipList = new List<ShipObject>();

    public ShipObject GetShipObject(string code)
    {
        for (int i = 0; i < shipList.Count; i++)
        {
            if (shipList[i].shipObjectData.shipCode == code)
                return shipList[i];
        }

        Debug.LogError("함선데이터가 플레이어한테도 없고 ShipList 에도 없습니다.");

        return null;
    }

    /// <summary>
    /// 함선 코드로 함선 몸 오브젝트 가져오기
    /// </summary>
    public GameObject GetShipBody(string code)
    {
        for (int i = 0; i < shipList.Count; i++)
        {
            if (shipList[i].shipObjectData.shipCode == code)
                return shipList[i].shipObjectData.shipBody;
        }

        Debug.LogError("함선데이터가 플레이어한테도 없고 ShipList 에도 없습니다.");

        return null;
    }
}

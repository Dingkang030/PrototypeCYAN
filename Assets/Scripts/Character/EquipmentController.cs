using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentController : MonoBehaviour {

    public List<Transform> equipSlot;
    public Transform currentEquip;

    public void ChangeEquipSlot(int index)
    {
        if (index >= equipSlot.Count)
            return;
        currentEquip.parent = null;
        currentEquip.parent = equipSlot[index];
        currentEquip.localPosition = Vector3.zero;
        currentEquip.localEulerAngles = Vector3.zero;
    }
}

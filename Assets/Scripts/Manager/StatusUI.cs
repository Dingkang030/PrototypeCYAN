using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatusUI : MonoBehaviour {
    [SerializeField]
    Image hpBar;
    void Update()
    {
        UpdateHP();
    }
    void UpdateHP()
    {
        UnitInfo currentUnit = ControlManager.Instance.GetCurrentUnit();
        if (currentUnit == null)
            return;
        if(hpBar)
            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount,currentUnit.GetHP() / currentUnit.GetMaxHP(),Time.deltaTime*2);
    }
}

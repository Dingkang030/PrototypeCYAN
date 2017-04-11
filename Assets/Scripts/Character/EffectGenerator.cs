using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectSet
{
    public string name;
    public GameObject prefab;
    public Transform attachTo;
    public Vector3 offset;
    public Vector3 eulerOffset;
    public bool useLocalEulerOffset;
    public float time=5.0f;
}

public class EffectGenerator : MonoBehaviour {

    public List<EffectSet> effectList;
    
    
	void Start () {
		
	}
	public void GenerateEffect(string name)
    {
        GenerateEffect(effectList.Find(set => set.name == name));
    }
    public void GenerateEffect(EffectSet set)
    {
        GameObject effect = GameObject.Instantiate<GameObject>(set.prefab);
        Vector3 pos = (set.attachTo != null) ? set.attachTo.position : transform.position;
        Vector3 euler = (set.attachTo != null && set.useLocalEulerOffset) ? set.attachTo.eulerAngles : transform.eulerAngles;
        effect.transform.position = pos;
        effect.transform.position += (set.attachTo != null)?set.attachTo.TransformVector(set.offset):set.offset;
        effect.transform.eulerAngles = euler + set.eulerOffset;
        GameObject.Destroy(effect, set.time);
    }
    public void GenerateEffect(int index)
    {
        EffectSet set = effectList[index];
        GenerateEffect(set);
    }

	void Update () {
		
	}
}

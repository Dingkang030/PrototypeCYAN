using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EDragon : EventBase
{
    public UnitInfo boss;
    public EventObject[] pillars;
    void Start()
    {
    }
    static void Swap(ref int a, ref int b)
    {

        int temp = b;
        b = a;
        a = temp;

    }
    protected override IEnumerator EventCoroutine()
    {
        CharacterMovement.MOVING_TYPE lastMovingType=CharacterMovement.MOVING_TYPE.ground;
        List<int> pillarOrder=new List<int>();
        for (int i = 0; i < pillars.Length; i++)
            pillarOrder.Add(i);
        for (int i = 0; i < pillars.Length*2; i++)
        {
            int j= UnityEngine.Random.Range(0, pillars.Length);
            int jj=UnityEngine.Random.Range(0, pillars.Length);
            int a = pillarOrder[j];
            int b = pillarOrder[jj];
            Swap(ref a, ref b);
            pillarOrder[j] = a;
            pillarOrder[jj] = b;
        }
        AIBase ai = boss.GetComponent<AIDragon>();
        ai.SetTargetUnit(eventPlayer);
        while (true)
        {
            if(boss.GetMovement().GetMovingType()!=lastMovingType)// On changed moving Type
            {
                lastMovingType = boss.GetMovement().GetMovingType();
                if(lastMovingType==CharacterMovement.MOVING_TYPE.flying)
                {
                    int p = pillarOrder[0];
                    Debug.Log(p);
                    pillarOrder.RemoveAt(0);
                    Material mat = pillars[p].GetComponent<MeshRenderer>().material;
                    mat.color = Color.red;
                    pillars[p].SetEnable(true);
                    pillars[p].AddEvent(
                        delegate (Transform caller)
                        {
                            if(caller.tag=="Player")
                            {
                                boss.GetMovement().SetMovingType(CharacterMovement.MOVING_TYPE.ground);
                                pillars[p].SetEnable(false);
                                mat.color = Color.black ;
                            }
                        }
                    );
                }
                else
                {

                }
            }
            yield return null;
        }
        yield return null;
    }
}

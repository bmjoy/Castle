﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    [Header("House属性")]
    public int value;                           // 城堡生命值
    public int perValue;
    public int maxValue;                        // 最大生命值
    public int lv;                              // 等级
    public int maxLv;
    public int owner;
    public static float timeDisOfAdd = 0.9f;    // 城堡生命值增加间隔
    public static float timeDisOfMove = 0.1f;   // 士兵移动时间间隔

    public GameObject player;                   // save player
    public int index;                           // houseArray index

    private float timeUse;

    void Start()
    {
        lv = 1;
        maxValue = lv * perValue;
        timeUse = timeDisOfAdd;
    }

    void Update()
    {
        timeUse -= Time.deltaTime;
        if (timeUse <= 0)
        {
            timeUse = timeDisOfAdd;
            if (value < maxValue) value++;
            else if (value > maxValue) value--;
        }
    }

    void OnTouched()
    {
        gameObject.GetComponent<MeshRenderer>().material =
            Resources.Load<Material>("Materials/New Material 3");
        for (int k = 0; k < Scene.instance.sizeOfHouse; k++)
            if (Scene.instance.g[index, k] != 0)
            {
                GameObject obj = Scene.instance.posToHouse[Scene.instance.housePosArray[k]];
                obj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/New Material 3");
            }
    }

    void UnTouched()
    {
        for (int k = 0; k < Scene.instance.sizeOfHouse; k++)
        {
            GameObject obj = Scene.instance.posToHouse[Scene.instance.housePosArray[k]];
            obj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/New Material 1");
        }
    }


    public IEnumerator DelayToInvokeDo(GameObject lastObj, int val)
    {
        var lastObjScript = lastObj.GetComponent<House>();
        if (lastObjScript.value <= val)
        {
            yield break;
        }

        Soldier script = player.GetComponent<Soldier>();
        script.dstHouse = gameObject;
        script.path = Scene.instance.houseRoadPath[lastObjScript.index, index];
        if (script.path == null) yield break;
        script.basePos = lastObj.transform.position;
        script.now = 0;
        script.owner = lastObjScript.owner;
        Instantiate(player, transform.position, new Quaternion());
        lastObjScript.value--;

        yield return new WaitForSeconds(timeDisOfMove);
        StartCoroutine(DelayToInvokeDo(lastObj, val));
    }


    void JustAttack(GameObject lastObj)
    {
        int val = lastObj.GetComponent<House>().value / 2; // 每次移动一半
        if (lastObj.GetComponent<House>().owner == owner && value + val > maxValue) val = maxValue - value;
        StartCoroutine(DelayToInvokeDo(lastObj, lastObj.GetComponent<House>().value - val));
    }

    private void OnGUI()
    {
        Vector2 mScreen = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 mPoint = new Vector2(mScreen.x, Screen.height - mScreen.y);

        if (lv < maxLv && value >= maxValue)
        {
            if (GUI.Button(new Rect(mPoint.x, mPoint.y + 3, 30, 20), "UP"))
            {
                value -= maxValue;
                maxValue = ++lv * perValue;
            }
            GUI.Label(new Rect(mPoint.x - 40, mPoint.y + 3, 40, 20), value.ToString() + "/" + maxValue.ToString());
        }
        else
        {
            GUI.Label(new Rect(mPoint.x, mPoint.y + 3, 40, 20), value.ToString() + "/" + maxValue.ToString());
        }
    }
}
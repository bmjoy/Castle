﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    public List<Vector2Int> path;   // 士兵要走的路程
    public int now;                 // 当前路程索引
    public Vector3 basePos;         // 起点位置
    public House srcHouseScript;
    public GameObject dstHouse;
    public int owner;

    private float disOfMove;
    private float timeUse;          // 行走间隔 House.timeDisOfMove

    void Start()
    {
        disOfMove = srcHouseScript.timeDisOfMove;
        timeUse = disOfMove;
        gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Scene.instance.colorTable[owner]);
    }

    void Update()
    {
        if (Global.instance.isStop) return;
        timeUse -= Time.deltaTime;
        if (timeUse < 0)
        {
            timeUse = disOfMove;
            transform.position = new Vector3(path[now].x * Scene.instance.allScale, path[now].y * Scene.instance.allScale, basePos.z + 1);
            now++;
            if (now >= path.Count)
            {
                var script = dstHouse.GetComponent<House>();
                if (script.owner != owner)
                {
                    if (owner == Global.instance.owner)
                    {
                        if (Global.instance.buffOfAttack >= Random.value)
                        {
                            script.value -= 2;
                            Global.instance.killS += 2;
                            Effect.instance.red(script);
                            Debug.Log("deep attack");
                        }
                        else
                        {
                            Global.instance.killS++;
                            script.value--;
                        }
                    }
                    else
                    {
                        script.value--;
                        if (script.owner == Global.instance.owner) Global.instance.lostS++;
                    }
                    if (script.value <= 0)
                    {
                        if (script.owner == Global.instance.owner || owner == Global.instance.owner)
                        {
                            Effect.instance.shake();
                            if (owner == Global.instance.owner) Global.instance.killCas++;
                            if (script.owner == Global.instance.owner) Global.instance.lostCas++;
                        }
                        script.owner = owner;
                        script.lv = 1;
                        script.initHouse();
                    }
                }
                else script.value++;
                DestroyImmediate(gameObject);
                return;
            }
        }
    }
}
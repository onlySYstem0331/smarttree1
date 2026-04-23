using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class color_change_2 : MonoBehaviour
{
    public float DelayTime = 0.1f;
    public GameObject Knownable;   //结点ui物体绑定

    public string nodeText;    //结点核心数据


    public bool If_Choose;       //颜色形态
    public Color Color_Choose_Yes = Color.red;
    public Color Color_Choose_No = Color.white;
    public bool IfCanUse = true;
    public int TestNumber = 0;

    private TMP_Text uiText;

    public List<color_change> color_Changes = new List<color_change>();


    void Start()
    {
        If_Choose = false;
        // 核心：先找父物体Canvas，再找它的子物体Knowledge
        Transform parentTransform = GameObject.Find("Canvas").transform;
        Knownable = parentTransform.Find("Knowledge").gameObject;

       if (Knownable != null)
        {
            // 1. 先找到名为 sy 的子物体
            Transform syTrans = Knownable.transform.Find("sy");
            
            // 2. 从 sy 身上获取 Text 组件
            if (syTrans != null)
            {
                uiText = syTrans.GetComponent<TMP_Text>();
            }
        }
    }

    void Update()
    {
        master();
    }

    /// <summary>
    /// 用于处理点击的一系列方法;
    /// 目前已经实现的功能有:
    /// 前驱节点的限制
    /// 颜色的切换
    /// 简单的debug;
    /// 
    /// 将要实现的功能:
    /// 对应的字幕动画播报
    /// 对应的UI 也就是知识点的展示;
    /// </summary>
    public void master()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null)
            {

                //foreach (var change in color_Changes)
                //{
                //    if (change.If_Choose == false)
                //    {
                //        TestNumber++;
                //    }
                //}
                //IfCanUse = (TestNumber >= 0) ? false : true;
                //TestNumber = 0;

                //if (hit.transform == transform && !IfCanUse)
                //{
                //    string prefabpath = "Prefab/Fail_tip";
                //    string Parent_Name = "Canvas";
                //    Vector3 position = new Vector3(0, -200, 0);
                //    GameObject temp = Animation_Control.InstantiatePrefabAsChild(prefabpath, position, Parent_Name);
                //}

                // 只对当前被点击的对象进行操作  
                if (hit.transform == transform && IfCanUse)
                {
                    //这里实际效果可以做一些修改,可能一个知识点不存在打开又关闭
                    this.If_Choose = !this.If_Choose;

                    SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

                    // 检查 SpriteRenderer 组件是否存在
                    if (spriteRenderer != null)
                    {
                        if (If_Choose)
                        {
                            spriteRenderer.color = Color_Choose_Yes;
                            Unlock_Next();
                            Debug.Log("成功解锁,展示对应知识点");
                            Knownable.SetActive(true);

                            //  核心：把知识点文本设置到UI上（安全不报错）
                            if (uiText != null)
                            {
                                uiText.text = nodeText;
                            }
                        }
                        else
                        {
                            spriteRenderer.color = Color_Choose_No;
                            Debug.Log("已经解锁成功");
                            //这里考虑做成复习的效果,如果学生再次点击就会再次复习
                        }
                    }
                }
            }
        }
    }

    public void Unlock_Next()
    {
        if (this.IfCanUse)
        {
            foreach (var change in color_Changes)
            {
                change.gameObject.SetActive(true);
            }
        }
    }
}
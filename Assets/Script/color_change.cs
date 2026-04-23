using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class color_change : MonoBehaviour
{
    public float DelayTime = 0.1f;
    public GameObject Knowledge;

    public bool If_Choose;
    public Color Color_Choose_Yes = Color.red;
    public Color Color_Choose_No = Color.white;

    public bool IfCanUse = true;         //此功能用于后期拓展点击时间间隔
    public int Text_Count;               //用于扩展后其对应的文字片段,或者也可以使用String类型来便于扩展


    public List<GameObject> gameObjects= new List<GameObject>();


    //public List<color_change> color_Changes = new List<color_change>();



    void Start()
    {
        If_Choose = false;
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
    IEnumerator GradualCreate(float delay)
    {
        foreach (GameObject obj in gameObjects)
        {
            obj.SetActive(true);
            yield return new WaitForSeconds(delay);
        } 
    }
    public void master()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null)
            {
                /// <summary>
                /// 此处的代码废弃,为另一种模型的代码
                ///                 //foreach (var change in color_Changes)
                ///{
                ///   if (change.If_Choose == false)
                ///    {
                ///       TestNumber++;
                ///    }
                ///}
                ///IfCanUse = (TestNumber > 0) ? false : true;
                ///TestNumber = 0;
                ///
                ///if (hit.transform == transform && !IfCanUse)
                ///{
                ///    string prefabpath = "Prefab/Fail_tip";
                ///    string Parent_Name = "Canvas";
                ///    Vector3 position = new Vector3(0, -200, 0);
                ///    GameObject temp = Animation_Control.InstantiatePrefabAsChild(prefabpath, position, Parent_Name);
                ///}
                /// </summary>


                // 只对当前被点击的对象进行操作  //这里需要多看看因为不太懂;
                if (hit.transform == transform && IfCanUse)
                {
                    this.If_Choose = !this.If_Choose;
                    // 组件检测
                    SpriteRenderer spriteRenderer = hit.collider.GetComponent<SpriteRenderer>();
                    // 检查 SpriteRenderer 组件是否存在
                    if (spriteRenderer != null)
                    {
                        if (If_Choose)
                        {
                            spriteRenderer.color = Color_Choose_Yes;
                            Knowledge.gameObject.SetActive(true);   


                            UIManager.Instance.UiControl(() =>
                            {
                                StartCoroutine(GradualCreate(DelayTime));
                                Debug.Log("成功解锁,展示对应知识点");
                            });
                        }
                        else
                        {
                            spriteRenderer.color = Color_Choose_No;
                            //这里可能需要切换一下

                            Debug.Log("已经解锁成功");
                            //这里考虑做成复习的效果,如果学生再次点击就会再次复习
                        }
                    }
                }
            }
        }
    }

    public void close()
    {
        StartCoroutine(GradualCreate(DelayTime));
    }
}




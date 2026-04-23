using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelDestroy : MonoBehaviour
{
    // Start is called before the first frame update

    public float LiveTime = 0f;

    void Start()
    {
        StartCoroutine(DeleteAfterDelay(LiveTime,this.gameObject));
    }

    // Update is called once per frame
    IEnumerator DeleteAfterDelay(float delay,GameObject K)
    {
        yield return new WaitForSeconds(delay);
        // 删除当前游戏对象
        Destroy(K);
    }
}

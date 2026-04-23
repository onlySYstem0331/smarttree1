using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseControlCamera : MonoBehaviour
{

    public float dragSpeed = 0.02f; // 拖动速度

    public bool mouseIsDrag = false;
    public Vector3 mouseEndPos;
    private Camera cam;
    private Transform myCamera;

    // Start is called before the first frame update
    void Awake()
    {
        cam = GetComponent<Camera>();
        myCamera = cam.transform;
    }

    private void Update()
    {
        mouseDragCameraMove();
    }
    /// <summary>
    /// 鼠标拖动相机移动
    /// </summary>
    private void mouseDragCameraMove()
    {
        if (!mouseIsDrag && Input.GetMouseButtonDown(0) && Utility.IsMouseOverUI() == false)
        {
            mouseEndPos = Input.mousePosition;
            mouseIsDrag = true;
        }
        if (mouseIsDrag && Input.GetMouseButton(0) && Utility.IsMouseOverUI() == false)
        {
            Vector3 curPos = Input.mousePosition;
            Vector3 movePos = (curPos - mouseEndPos) * dragSpeed;
            myCamera.position -= movePos;
            mouseEndPos = curPos;
        }
        if (mouseIsDrag && Input.GetMouseButtonUp(0))
        {
            mouseIsDrag = false;
        }
    }

    public static class Utility
    {
        public static bool IsMouseOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}

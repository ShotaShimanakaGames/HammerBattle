using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderInMainCamera : MonoBehaviour
{
    [System.NonSerialized] public bool m_IsRenderInMainCamera = false;

    private void OnWillRenderObject()
    {
        if (Camera.current.name == Camera.main.name)
        {
            m_IsRenderInMainCamera = true;
        }
    }
}

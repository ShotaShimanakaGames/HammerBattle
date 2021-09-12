using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private Transform m_TraceXTarget;
    [SerializeField] private List<RenderInMainCamera> m_RendererTargetInCamera = new List<RenderInMainCamera>();
    [SerializeField] private float m_ZoomInOutSpeed;
    private float m_DefaultFOV;
    private Camera m_Camera;

    public enum E_CAMERA_STATE
    {
        ZOOM_OUT,
        ZOOM_IN,
        TRACE
    }
    private E_CAMERA_STATE m_State;

    private void Start()
    {
        m_Camera = GetComponent<Camera>();
        m_DefaultFOV = m_Camera.fieldOfView;

        m_State = E_CAMERA_STATE.TRACE;
    }

    private void Update()
    {
        switch (m_State)
        {
            case E_CAMERA_STATE.TRACE:
                iTween.MoveUpdate(gameObject, iTween.Hash("x", m_TraceXTarget.position.x));
                break;

            case E_CAMERA_STATE.ZOOM_OUT:
                bool isAllInCamera = true;
                m_RendererTargetInCamera.ForEach(e => {
                    if (e.m_IsRenderInMainCamera == false)
                    {
                        isAllInCamera = false;
                    }
                });

                if (isAllInCamera == false)
                {
                    m_Camera.fieldOfView += m_ZoomInOutSpeed;
                }
                break;

            case E_CAMERA_STATE.ZOOM_IN:
                m_Camera.fieldOfView = iTween.FloatUpdate(m_Camera.fieldOfView, m_DefaultFOV, m_ZoomInOutSpeed);
                break;

        }
    }

    public void SetState(E_CAMERA_STATE state)
    {
        m_State = state;
    }
}

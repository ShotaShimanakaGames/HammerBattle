using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SafeAreaPadding : MonoBehaviour
{
    RectTransform m_panel;
    Rect m_lastSafeArea = new Rect(0, 0, 0, 0);

    private void Start()
    {
        m_panel = GetComponent<RectTransform>();
        UpdateSafeArea();
    }

    private void Update()
    {
        UpdateSafeArea();
    }

    private void UpdateSafeArea()
    {
        Rect safeArea = Screen.safeArea;
        if (safeArea == m_lastSafeArea)
        {
            return;
        }

        m_lastSafeArea = safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        m_panel.anchorMin = anchorMin;
        m_panel.anchorMax = anchorMax;
    }
}

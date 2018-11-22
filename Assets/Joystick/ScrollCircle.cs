using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollCircle : ScrollRect
{
    public static ScrollCircle Instance { get; private set; }

    public float horizontalValue = 0;
    public float verticalValue = 0;

    protected float mRadius = 0f;

    public CanvasScaler m_CanvasScaler;
    public RectTransform m_Cursor;
    public Vector2 dragPos = Vector2.zero;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        //计算摇杆块的半径
        mRadius = (transform as RectTransform).sizeDelta.x * 0.5f;
    }

    public bool isContineMouse(Vector3 mousePos)
    {
        Vector3 canvasMousePos = Vector3.zero;
        canvasMousePos.x = m_CanvasScaler.referenceResolution.x * (mousePos.x / Screen.width) - m_CanvasScaler.referenceResolution.x / 2;
        canvasMousePos.y = m_CanvasScaler.referenceResolution.y * (mousePos.y / Screen.height) - m_CanvasScaler.referenceResolution.y / 2;

        return Vector3.Distance(m_Cursor.anchoredPosition3D, canvasMousePos) < m_Cursor.sizeDelta.x;
    }

    public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        base.OnDrag(eventData);
        var contentPostion = this.content.anchoredPosition;
        if (contentPostion.magnitude > mRadius)
        {
            contentPostion = contentPostion.normalized * mRadius;
            SetContentAnchoredPosition(contentPostion);
        }
        dragPos = eventData.position;
        horizontalValue = contentPostion.x / mRadius;
        verticalValue = contentPostion.y / mRadius;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        dragPos = Vector2.one * 10000;
        horizontalValue = 0;
        verticalValue = 0;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Teo.AutoReference;
using DBD.BaseGame;

public class BaseDragObject : BaseMonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField, Get] protected Collider2D col2D;

    [SerializeField, Get] public SpriteRenderer spriteRenderer;
    [SerializeField] protected int sortingOrder;
    [SerializeField] protected Vector3 startPosition;
    [SerializeField] protected Vector3 startRotation;
    [SerializeField] protected Vector3 startScale;
    private Vector3 spoonPosition;
    private Vector3 mouseDownPosition;
    protected Camera mainCamera;
    protected static bool isAnyDragging = false; // chỉ cho phép 1 đối tượng di chuyển
    protected bool isDragging = false;

    #region LoadComponents

    protected override void LoadComponents()
    {
        base.LoadComponents();
    }

    #endregion

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
        startRotation = transform.localEulerAngles;
        startScale = transform.localScale;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log($"datdb - pointer down ");
        isAnyDragging = true;
        isDragging = true;

        startPosition = transform.position;
        spoonPosition = transform.position;
        mouseDownPosition = GetWorldPoint(eventData.position);

        SetOrderInLayer(50);
        EffectDown();
        OnOffCol2D(false);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector3 rayPoint = GetWorldPoint(eventData.position);
        Vector3 delta = rayPoint - mouseDownPosition;
        Vector3 pos = spoonPosition + delta;

        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        transform.position = GetMousePositionClamp();
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
        isAnyDragging = false;

        OnOffCol2D(false);
        // EffectUp();
    }


    public virtual void EffectDown()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(transform.DOLocalRotate(SetRotateEffectDown(), 0.2f));
        sequence.Join(transform.DOScale(startScale * SetScaleEffectDown(), 0.2f));
    }

    public virtual Vector3 SetRotateEffectDown()
    {
        return Vector3.zero;
    }

    public virtual float SetScaleEffectDown()
    {
        return 1.2f;
    }

    public virtual void EffectUp()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(transform.DOLocalRotate(startRotation, 0.2f));
        sequence.Join(transform.DOScale(startScale, 0.2f));
        if (IsMoveToStartPosition())
        {
            sequence.Join(transform.DOMove(startPosition, 0.2f));
        }

        sequence.OnComplete(() =>
        {
            OnOffCol2D(true);
            SetOrderInLayer(sortingOrder);
        });
    }

    protected virtual bool IsMoveToStartPosition()
    {
        return true;
    }

    public virtual void SetOrderInLayer(int sortingOrder)
    {
        spriteRenderer.sortingOrder = sortingOrder;
    }

    public void OnOffCol2D(bool bl)
    {
        col2D.enabled = bl;
    }

    private Vector3 GetWorldPoint(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        return ray.GetPoint(distance);
    }

    public Vector3 GetMousePositionClamp()
    {
        Vector3 mousePosition = Vector3.zero;
        mousePosition.x = Mathf.Clamp(transform.position.x, Pos_Screen_Left_Right().x + PercentClampLeftRight().x,
            Pos_Screen_Left_Right().y - PercentClampLeftRight().y);
        mousePosition.y = Mathf.Clamp(transform.position.y, Pos_Screen_Bottom_Top().x + PercentClampBottomTop().x,
            Pos_Screen_Bottom_Top().y - PercentClampBottomTop().y);
        mousePosition.z = transform.position.z;
        return mousePosition;
    }

    public Vector3 Pos_Screen_Left_Right() // x = screenLeft | y = screenRight
    {
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float screenLeft = mainCamera.transform.position.x - cameraWidth / 2;
        float screenRight = mainCamera.transform.position.x + cameraWidth / 2;
        return new Vector3(screenLeft, screenRight);
    }

    public Vector3 Pos_Screen_Bottom_Top() // x = screenBottom | y = screenTop
    {
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float screenBottom = mainCamera.transform.position.y - cameraHeight / 2;
        float screenTop = mainCamera.transform.position.y + cameraHeight / 2;
        return new Vector3(screenBottom, screenTop);
    }

    protected virtual Vector2 PercentClampLeftRight() // x = Left | y = Right
    {
        return Vector2.zero;
    }

    protected virtual Vector2 PercentClampBottomTop() // x = Bottom  | y = Top
    {
        return Vector2.zero;
    }
}
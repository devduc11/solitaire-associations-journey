using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Teo.AutoReference;
using DBD.BaseGame;

public class BaseDragObject : BaseMonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField, Get] protected Collider2D col2D;
    [SerializeField, Get] protected BoxCollider2D boxCollider2D;

    // [SerializeField, Get] public SpriteRenderer spriteRenderer;
    [SerializeField, Get] protected Image image;
    [SerializeField] protected int sortingOrder;
    [SerializeField] protected Vector2 startPosition;
    [SerializeField] protected Vector3 startRotation;
    [SerializeField] protected Vector3 startScale;
    private Vector2 spoonPosition;
    private Vector2 mouseDownPosition;
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
        if (isAnyDragging) return;

        isAnyDragging = true;
        isDragging = true;

        startPosition = transform.position;

        // LƯU VỊ TRÍ BAN ĐẦU CỦA OBJECT
        spoonPosition = transform.position;

        // LƯU VỊ TRÍ CHUỘT LÚC BẤM
        mouseDownPosition = GetWorldPoint(eventData.position);

        EffectDown();
        OnOffCol2D(false);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector2 rayPoint = GetWorldPoint(eventData.position);
        Vector2 delta = rayPoint - mouseDownPosition;

        Vector2 targetPos2D = spoonPosition + delta;
        Vector2 clampedPos2D = GetMousePositionClamp(targetPos2D);

        // GIỮ NGUYÊN Z
        transform.position = new Vector3(
            clampedPos2D.x,
            clampedPos2D.y,
            transform.position.z
        );
        // Debug.Log($"Drag pos: {transform.position}");
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
        isAnyDragging = false;

        EffectUp();
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
        float z = transform.position.z; // GIỮ Z HIỆN TẠI

        Sequence sequence = DOTween.Sequence();
        sequence.Join(transform.DOLocalRotate(startRotation, 0.2f));
        sequence.Join(transform.DOScale(startScale, 0.2f));

        if (IsMoveToStartPosition())
        {
            sequence.Join(transform.DOMove(
                new Vector3(startPosition.x, startPosition.y, z),
                0.2f
            ));
        }

        sequence.OnComplete(() =>
        {
            OnOffCol2D(true);
        });
    }


    protected virtual bool IsMoveToStartPosition()
    {
        return true;
    }

    public void SetSizeBoxCol2D()
    {
        RectTransform rect = image.rectTransform;
        boxCollider2D.size = rect.rect.size;
    }

    public void OnOffCol2D(bool bl)
    {
        col2D.enabled = bl;
    }

    private Vector2 GetWorldPoint(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        float distance = Vector2.Distance(transform.position, Camera.main.transform.position);
        return ray.GetPoint(distance);
    }

    public Vector2 GetMousePositionClamp(Vector2 targetPos)
    {
        Vector2 mousePosition = Vector2.zero;

        mousePosition.x = Mathf.Clamp(
            targetPos.x,
            Pos_Screen_Left_Right().x + PercentClampLeftRight().x,
            Pos_Screen_Left_Right().y - PercentClampLeftRight().y
        );

        mousePosition.y = Mathf.Clamp(
            targetPos.y,
            Pos_Screen_Bottom_Top().x + PercentClampBottomTop().x,
            Pos_Screen_Bottom_Top().y - PercentClampBottomTop().y
        );

        return mousePosition;
    }


    public Vector2 Pos_Screen_Left_Right() // x = screenLeft | y = screenRight
    {
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float screenLeft = mainCamera.transform.position.x - cameraWidth / 2;
        float screenRight = mainCamera.transform.position.x + cameraWidth / 2;
        return new Vector2(screenLeft, screenRight);
    }

    public Vector2 Pos_Screen_Bottom_Top() // x = screenBottom | y = screenTop
    {
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float screenBottom = mainCamera.transform.position.y - cameraHeight / 2;
        float screenTop = mainCamera.transform.position.y + cameraHeight / 2;
        return new Vector2(screenBottom, screenTop);
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
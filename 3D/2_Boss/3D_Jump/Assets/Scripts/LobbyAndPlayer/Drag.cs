using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{

    RectTransform rect;
    public GameObject obj;
    [SerializeField] public Canvas canvas;
    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Awake()
    {
        rect = obj.GetComponent<RectTransform>();
        canvasGroup = obj.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // PointerEventData �� delta�� �󸶳� �̵��ߴ����� ��Ÿ �� �ִ� ���̴�.
        // ���� �ش� ��ü�� rectTransform�� delta��ŭ �̵����� �ش�.

        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnDrop(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }


}

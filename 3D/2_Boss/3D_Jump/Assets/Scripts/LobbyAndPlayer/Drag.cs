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
        // PointerEventData 의 delta는 얼마나 이동했는지를 나타 내 주는 것이다.
        // 따라서 해당 객체의 rectTransform을 delta만큼 이동시켜 준다.

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

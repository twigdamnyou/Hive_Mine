using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{

    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public Vector2 offset;
    public float padding;
    private Canvas canvas;

    public LayoutElement layoutElement;

    public int characterWrapLimit;

    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponentInParent<CanvasGroup>();
    }

    private void FadeIn()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f);
    }


    public void SetText(string content, string header = "")
    {
        FadeIn();
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentField.text = content;

        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            int headerLength = headerField.text.Length;
            int contentLength = contentField.text.Length;

            layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
        }

        //Vector2 position = Input.mousePosition;

        //float pivotX = position.x / Screen.width;
        //float pivotY = position.y / Screen.height;

        //rectTransform.pivot = new Vector2(pivotX, pivotY);
        //transform.position = position;

        SetTooltipPosition();
    }

    private void SetTooltipPosition()
    {
        Vector2 newPos = (Vector2)Input.mousePosition + offset;
        //newPos.z = 0f;
        float rightEdgeToScreenEdgeDistance = Screen.width - (newPos.x + rectTransform.rect.width * canvas.scaleFactor / 2) - padding;
        if (rightEdgeToScreenEdgeDistance < 0)
        {
            newPos.x += rightEdgeToScreenEdgeDistance;
        }
        float leftEdgeToScreenEdgeDistance = 0 - (newPos.x - rectTransform.rect.width * canvas.scaleFactor / 2) + padding;
        if (leftEdgeToScreenEdgeDistance > 0)
        {
            newPos.x += leftEdgeToScreenEdgeDistance;
        }
        float topEdgeToScreenEdgeDistance = Screen.height - (newPos.y + rectTransform.rect.height * canvas.scaleFactor) - padding;
        if (topEdgeToScreenEdgeDistance < 0)
        {
            newPos.y += topEdgeToScreenEdgeDistance;
        }
        transform.position = newPos;
    }
}

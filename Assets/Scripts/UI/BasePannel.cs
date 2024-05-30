using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePannel : MonoBehaviour
{
    public GameObject view;

    public string panelID;

    protected CanvasGroup canvasGroup;
    public float fadeModifier = 1f;
    public bool IsOpen { get { return view.activeInHierarchy; } }

    protected virtual void Awake()
    {
        view = transform.Find("View").gameObject;

        if (view == null)
        {
            Debug.LogError("Cant find View on: " + gameObject.name);
        }

        panelID = GetType().Name;
        //Debug.Log(panelID);

        canvasGroup = GetComponentInChildren<CanvasGroup>(true);
    }

    public virtual void Open()
    {
        view.SetActive(true);
    }

    public virtual void Close()
    {
        view.SetActive(false);
    }

    public void Toggle()
    {
        if (IsOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void FadeIn(Action callback = null)
    {
        canvasGroup.alpha = 0f;
        StartCoroutine(Fade(1f, callback));
    }

    public void FadeOut(Action callback = null)
    {
        canvasGroup.alpha = 1f;
        StartCoroutine(Fade(0f, callback));
    }

    public IEnumerator Fade(float targetValue, Action callback = null)
    {
        if (canvasGroup == null)
        {
            Debug.LogError("no canvas group found on panel " + GetType().Name);
            callback?.Invoke();

            yield break;
        }

        while (canvasGroup.alpha != targetValue)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetValue, Time.deltaTime * fadeModifier);
            yield return new WaitForEndOfFrame();
        }

        callback?.Invoke();
    }
}

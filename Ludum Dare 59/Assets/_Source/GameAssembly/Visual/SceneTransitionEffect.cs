using DG.Tweening;
using System.Collections;
using UnityEngine;

public class SceneTransitionEffect : MonoBehaviour
{
    [SerializeField] private float timeInCenter;
    [SerializeField] private float slideDuration;

    private RectTransform imageTransform;

    private void Awake()
    {
        imageTransform = GetComponent<RectTransform>();
    }

    public void DoTransition()
    {
        

        Vector2 centerPos = imageTransform.anchoredPosition;
        Vector2 endPos = new Vector2(2000f, centerPos.y);

        imageTransform.anchoredPosition = new Vector2(-2000f, centerPos.y);

        Sequence seq = DOTween.Sequence().SetUpdate(true);

        seq.Append(imageTransform.DOAnchorPos(centerPos, slideDuration))
            .Append(imageTransform.DOAnchorPos(endPos, slideDuration).SetDelay(timeInCenter));
    }

    public void HideView()
    {

        Vector2 centerPos = new Vector2(0, imageTransform.anchoredPosition.y);

        imageTransform.anchoredPosition = new Vector2(-2000f, centerPos.y);

        imageTransform.DOAnchorPos(centerPos,slideDuration).SetUpdate(true);
    }

    public void ShowView()
    {
        Vector2 endPos = new Vector2(2000f, imageTransform.anchoredPosition.y);

        imageTransform.DOAnchorPos(endPos,slideDuration).SetUpdate(true);
    }
}

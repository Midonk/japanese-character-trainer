using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class SubmissionDrawer : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _characterDisplay;
    [SerializeField] private TMPro.TMP_Text _feedbackDisplay;
    [SerializeField] private float _displacementRadius;
    [SerializeField] private float _duration = 0.25f;
    [SerializeField] private Ease _easing;

    private void Awake() 
    {
        _initialPosition = _feedbackDisplay.rectTransform.localPosition;
        _initialColor = _feedbackDisplay.color;
        var tmpColor = _initialColor;
        tmpColor.a = 0;
        _feedbackDisplay.color = tmpColor;
    }

    public void Tween()
    {
        _feedbackDisplay.text = _characterDisplay.text;
        _feedbackDisplay.color = _initialColor;
        _feedbackDisplay.rectTransform.localPosition = _initialPosition;
        _feedbackDisplay.rectTransform.rotation = Quaternion.identity;
        var randomVector = Random.insideUnitCircle.normalized * _displacementRadius;
        var spatializedVector = new Vector3(randomVector.x, randomVector.y, 0);
        var endPosition = _feedbackDisplay.rectTransform.localPosition + spatializedVector;
        var endColor = _initialColor;
        endColor.a = 0;
        _feedbackDisplay.rectTransform.DORotate(new Vector3(0, 0, Random.Range(-90f, 90f)), _duration, RotateMode.WorldAxisAdd).SetEase(_easing);
        _feedbackDisplay.rectTransform.DOLocalMove(endPosition, _duration).SetEase(_easing);
        _feedbackDisplay.DOColor(endColor, _duration);
    }

    private void OnDrawGizmosSelected() 
    {
        Handles.color = Color.blue;
        var position = Camera.main.ScreenToWorldPoint(_feedbackDisplay.rectTransform.localPosition);
        position.z = 1;
        Handles.DrawWireDisc(position, Vector3.forward, _displacementRadius / _feedbackDisplay.canvas.referencePixelsPerUnit); 
    }

    private Vector3 _initialPosition;
    private Color _initialColor;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class SubmissionDrawer : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _display;
    [SerializeField] private float _displacementRadius;

    public void Tween()
    {
        var randomVector = Random.insideUnitCircle.normalized * _displacementRadius;
        var spatializedVector = new Vector3(randomVector.x, randomVector.y, 0);
        var endPosition = _display.rectTransform.localPosition + spatializedVector;
        _display.rectTransform.DOLocalMove(endPosition, 0.25f);
    }

    private void OnDrawGizmosSelected() 
    {
        Handles.color = Color.blue;
        var position = Camera.main.ScreenToWorldPoint(_display.rectTransform.localPosition);
        position.z = 1;
        Handles.DrawWireDisc(position, Vector3.forward, _displacementRadius / _display.canvas.referencePixelsPerUnit); 
    }
}

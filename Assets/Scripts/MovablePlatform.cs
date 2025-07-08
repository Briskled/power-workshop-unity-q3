#region

using DG.Tweening;
using UnityEngine;

#endregion

public class MovablePlatform : MonoBehaviour
{
    private enum Direction
    {
        Up,
        Dpwn
    }

    [SerializeField] private float moveDuration = 3;
    [SerializeField] private float moveDistance = 5;
    [SerializeField] private Ease moveEase = Ease.InOutQuad;

    private Direction currentDirection = Direction.Up;
    private Vector3 bottomPoint;
    private Vector3 topPoint;

    private void Awake()
    {
        bottomPoint = transform.position;
        topPoint = transform.position + Vector3.up * moveDistance;
    }

    private void Start()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(topPoint, moveDuration).SetEase(moveEase));
        sequence.Append(transform.DOMove(bottomPoint, moveDuration).SetEase(moveEase));
        sequence.SetLoops(-1, LoopType.Restart);
        sequence.Play();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * moveDistance);
    }
}
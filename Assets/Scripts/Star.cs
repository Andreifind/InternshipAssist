using UnityEngine;

public class Star : MonoBehaviour
{
    private GameObject _targetPosition;
    public float moveSpeed = 5f;

    void Start()
    {
        _targetPosition = this.GetComponentInParent<LevelManager>().StarTargetPosition;
        this.transform.parent = null;
    }

    void Update()
    {
        if (_targetPosition != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, _targetPosition.transform.position, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, _targetPosition.transform.position) < 0.1f)
            {
                _targetPosition.GetComponent<StarCounter>().AddStar();
                Destroy(gameObject);
            }
        }
    }
}

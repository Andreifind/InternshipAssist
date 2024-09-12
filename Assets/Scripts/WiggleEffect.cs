using UnityEngine;

public class WiggleEffect : MonoBehaviour
{
    public float effectDuration = 0.5f;  
    public float squishAmount = 0.2f;    
    private bool _isAnimating = false;
    private float _timer = 0f;
    private Vector3 _originalScale;
    private Vector3 _detachedScale;  

    void Start()
    {
        _originalScale = transform.localScale;  
        _detachedScale = _originalScale;  
    }

    void Update()
    {
        if (_isAnimating)
        {
            _timer += Time.deltaTime;
            float t = _timer / effectDuration;

            float squishFactor = Mathf.Sin(t * Mathf.PI);  

            float xScale = _detachedScale.x * (1 + squishAmount * squishFactor);  
            float yScale = _detachedScale.y * (1 - squishAmount * squishFactor);  

            transform.localScale = new Vector3(xScale, yScale, _detachedScale.z);

            if (_timer >= effectDuration)
            {
                _isAnimating = false;
                transform.localScale = _detachedScale;  
            }
        }
    }

    public void StartWiggle()
    {
        if (_isAnimating)
            return;

        _isAnimating = true;
        _timer = 0f;

        _detachedScale = transform.localScale;
    }

    public void OnDetach()
    {
        if (_isAnimating)
            return;

        _detachedScale = transform.localScale;

        StartWiggle();
    }

    public void OnReattach()
    {
        transform.localScale = _originalScale;
        _detachedScale = _originalScale;
        StartWiggle();
    }
}

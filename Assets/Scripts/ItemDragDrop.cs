using UnityEngine;

public class ItemDragDrop : MonoBehaviour
{
    private Vector2 _startPos;
    private bool _isBeingHeld = false;
    private Transform _originalParent;
    private Vector3 _originalPosition;
    private Slot _currentSlot = null;
    private Slot _previousSlot = null;
    private WiggleEffect _wiggleEffect;

    private void Start()
    {
        if(this.transform.parent != null)
        {
            _originalParent = this.transform.parent;
            _originalPosition = this.transform.parent.localPosition;
        }
        _wiggleEffect = GetComponent<WiggleEffect>();
    }

    private void Update()
    {
        if (_isBeingHeld)
        {
            Vector2 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            this.transform.position = new Vector2(mousePos.x - _startPos.x, mousePos.y - _startPos.y);
        }
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && (this.GetComponentInParent<Slot>().Layer == 0))
        {
            Debug.Log("detach");
            if (this.transform.parent != null)
            {
                Debug.Log("detach");
                _previousSlot = this.transform.parent.GetComponent<Slot>();
                if (_previousSlot != null)
                {
                    _previousSlot.HoldTheItem(false);
                }
                this.transform.SetParent(null);
            }

            Vector2 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            _startPos = mousePos - (Vector2)this.transform.position;

            _isBeingHeld = true;
            _wiggleEffect.OnDetach();
        }
    }

    private void OnMouseUp()
    {
        _isBeingHeld = false;
        if (_currentSlot != null && !_currentSlot.IsHoldingItem && _currentSlot.Layer==0)
        {
            this.transform.SetParent(_currentSlot.transform);
            this.transform.position = _currentSlot.transform.position;
            _currentSlot.HoldTheItem(true);
            _originalParent = this.transform.parent;
            _originalPosition = this.transform.localPosition;
            _previousSlot.RefreshCollider();
        }
        else
        {
            this.transform.SetParent(_originalParent);
            this.transform.localPosition = new Vector2(0,0);
            if (_previousSlot != null)
                _previousSlot.HoldTheItem(true);
        }

        _currentSlot = null;
        _wiggleEffect.OnReattach();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Slot"))
        {
            Slot slot = other.GetComponent<Slot>();
            if (slot != null && !slot.IsHoldingItem)
            {
                _currentSlot = slot;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Slot"))
        {
            Slot slot = other.GetComponent<Slot>();
            if (slot != null && slot == _currentSlot)
            {
                _currentSlot = null;
            }
        }
    }
}

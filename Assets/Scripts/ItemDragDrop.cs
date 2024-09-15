using UnityEngine;
using System.Collections.Generic;

public class ItemDragDrop : MonoBehaviour
{
    private Vector2 _startPos;
    private bool _isBeingHeld = false;
    private Transform _originalParent;
    private Vector3 _originalPosition;
    private Slot _currentSlot = null;
    private Slot _previousSlot = null;
    private WiggleEffect _wiggleEffect;
    private Collider2D _itemCollider;

    private void Start()
    {
        if (this.transform.parent != null)
        {
            _originalParent = this.transform.parent;
            _originalPosition = this.transform.parent.localPosition;
        }

        _wiggleEffect = GetComponent<WiggleEffect>();
        _itemCollider = GetComponent<Collider2D>();
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
        if (this.GetComponentInParent<Shelf>().IsLocked == false && Time.timeScale>0)
        if (Input.GetMouseButtonDown(0) && (this.GetComponentInParent<Slot>().Layer == 0))
        {
            Debug.Log("detach");
            if (this.transform.parent != null)
            {
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

        Vector2 boxSize = _itemCollider.bounds.size;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0f);

        Slot closestSlot = FindClosestSlot(colliders);

        if (closestSlot != null && !closestSlot.IsHoldingItem && closestSlot.Layer == 0 && closestSlot.GetComponentInParent<Shelf>().IsLocked == false && Time.timeScale > 0)
        {
            this.transform.SetParent(closestSlot.transform);
            this.transform.position = closestSlot.transform.position;
            closestSlot.HoldTheItem(true);
            _originalParent = this.transform.parent;
            _originalPosition = this.transform.localPosition;

            _previousSlot?.RefreshCollider();
        }
        else
        {
            this.transform.SetParent(_originalParent);
            this.transform.localPosition = new Vector2(0, 0);
            _previousSlot?.HoldTheItem(true);
        }

        _currentSlot = null;
        _wiggleEffect.OnReattach();
    }

    private Slot FindClosestSlot(Collider2D[] colliders)
    {
        Slot closestSlot = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {
            Slot slot = collider.GetComponent<Slot>();
            if (slot != null && !slot.IsHoldingItem)
            {
                float distance = Vector2.Distance(transform.position, slot.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestSlot = slot;
                }
            }
        }

        return closestSlot;
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

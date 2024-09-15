using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainBox : MonoBehaviour
{
    private int _healthPoints = 4;
    private Rigidbody2D _lockRigidBody;
    public Shelf LockedShelf;
    public float ForceAmount = 5f;

    public float YOffset = 0.719f;

    public GameObject TopLeftChains;
    public GameObject TopRightChains;
    public GameObject BottomLeftChains;
    public GameObject BottomRightChains;
    public DisplayChainStrength Display;

    public GameObject Lock;

    private void Start()
    {
        _lockRigidBody = Lock.GetComponent<Rigidbody2D>();
        Display.ShowChainStrength(_healthPoints);
        transform.position = new Vector2(transform.position.x, transform.position.y - YOffset);
        transform.localScale = new Vector2(1.5f, 1.5f);
    }

    public void TakeDamage()
    {
        _healthPoints--;
        Display.ShowChainStrength(_healthPoints);

        switch (_healthPoints)
        {
            case 3:
                Destroy(BottomRightChains);
                ApplyForce();
                break;
            case 2:
                Destroy(TopLeftChains);
                ApplyForce();
                break;
            case 1:
                ApplyForce();
                break;
            case 0:
                Destroy(BottomLeftChains);
                Destroy(TopRightChains);
                LockedShelf.IsLocked = false;
                Destroy(gameObject,1);
                break;
            default:
                break;
        }
    }

    private void ApplyForce()
    {
        Vector3 forceDirection = Vector3.up;
        _lockRigidBody.AddForce(forceDirection * ForceAmount, ForceMode2D.Impulse);
    }
}

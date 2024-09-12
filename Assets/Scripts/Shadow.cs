using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public Vector2 Offset = new Vector2(-0.1f, -0.1f);
    public Material Mat;
    private GameObject _shadow;
    void Start()
    {
        _shadow = new GameObject("Shadow");
        _shadow.transform.parent = this.transform;
        _shadow.transform.localPosition = Offset;
        _shadow.transform.localRotation = Quaternion.identity;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        SpriteRenderer sr = _shadow.AddComponent<SpriteRenderer>();
        sr.sprite = renderer.sprite;
        sr.material = Mat;
        sr.sortingLayerName = renderer.sortingLayerName;
        sr.sortingOrder = renderer.sortingOrder - 1;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    public delegate void OnTileBeginClick(Tile tile);
    public delegate void OnTileClicked(Tile tile);
    public delegate void OnTileBeginDrag(Tile tile, PointerEventData eventData);
    public delegate void OnTileDragged(Tile tile, PointerEventData eventData);
    public delegate void OnTileEndDrag(Tile tile, PointerEventData eventData);
    public OnTileClicked onTileBeginClick;
    public OnTileClicked onTileClicked;
    public OnTileDragged onTileBeginDrag;
    public OnTileDragged onTileDragged;
    public OnTileDragged onTileEndDrag;
    private GameObject contents;
    public int x;
    public int y;

    public GameObject Contents
    {
        get
        {
            return contents;
        }
    }

    public bool HasContents
    {
        get
        {
            return contents != null;
        }
    }

    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public GameObject PushContentsFromPrefab(GameObject contentsPrefab)
    {
        GameObject newContents = Instantiate(contentsPrefab, transform);
        return PushContents(newContents);
    }

    public GameObject PushContents(GameObject newContents)
    {
        GameObject oldContents = contents;
        contents = newContents;
        if (newContents)
        {
            newContents.transform.parent = transform;
        }
        if (oldContents)
        {
            oldContents.transform.parent = null;
        }
        return oldContents;
    }

    public GameObject PushContentsAndMoveToCenter(GameObject newContents, float time, LeanTweenType easing = LeanTweenType.easeInOutQuad)
    {
        GameObject oldContents = null;
        if (contents != newContents)
        {
            oldContents = PushContents(newContents);
        }
        LeanTween.moveLocal(newContents, Vector3.zero, time).setEase(easing);
        return oldContents;
    }

    public float GetDistance(Tile otherTile)
    {
        return Mathf.Sqrt(Mathf.Pow(x - otherTile.x, 2f) + Mathf.Pow(y - otherTile.y, 2f));
    }

    public Vector2 GetDirection(Tile otherTile)
    {
        return new Vector2(otherTile.x - x, otherTile.y - y);
    }

    public bool IsContentCentered()
    {
        return (!HasContents || Vector3.zero.Equals(contents.transform.localPosition));
    }

    public void Clear()
    {
        if (HasContents)
        {
            Destroy(contents);
        }
        Destroy(gameObject);
    } 

    public void Block()
    {
        if (HasContents)
        {
            Animator animator = contents.GetComponent<Animator>();
            if (animator && AnimatorUtils.HasParameter("Blocked", animator))
            {
                animator.SetTrigger("Blocked");
            }
        }
    }

    void Update()
    {
        if (HasContents)
        {
            Animator animator = contents.GetComponent<Animator>();
            if (animator && AnimatorUtils.HasParameter("IsMoving", animator))
            {
                animator.SetBool("IsMoving", !IsContentCentered());
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onTileClicked != null)
        {
            onTileClicked(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Do nothing
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (onTileDragged != null)
        {
            onTileDragged(this, eventData);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (onTileBeginDrag != null)
        {
            onTileBeginDrag(this, eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (onTileEndDrag != null)
        {
            onTileEndDrag(this, eventData);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (onTileBeginClick != null)
        {
            onTileBeginClick(this);
        }
    }
}

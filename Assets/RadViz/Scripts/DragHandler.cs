using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
	public bool dragOnSurfaces = true;

	private GameObject m_DraggingIcon;
	private RectTransform m_DraggingPlane;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		Debug.Log("in begin" + eventData);
		var canvas = FindInParents<Canvas>(gameObject);
		if ( canvas == null )
			return;

		// We have clicked something that can be dragged.
		// What we want to do is create an icon for this.
		m_DraggingIcon = new GameObject("icon");

		m_DraggingIcon.transform.SetParent(canvas.transform, false);
		m_DraggingIcon.transform.SetAsLastSibling();

		var image = m_DraggingIcon.AddComponent<Image>();

		image.sprite = GetComponent<Image>().sprite;
		image.SetNativeSize();

		if ( dragOnSurfaces )
			m_DraggingPlane = transform as RectTransform;
		else
			m_DraggingPlane = canvas.transform as RectTransform;

		SetDraggedPosition(eventData);
	}

	public void OnDrag(PointerEventData data)
	{
		Debug.Log("in OnDrag" + data);
		if ( m_DraggingIcon != null )
			SetDraggedPosition(data);
	}

	private void SetDraggedPosition(PointerEventData data)
	{
		Debug.Log("in SetDraggedPosition" + data);
		if ( dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null )
			m_DraggingPlane = data.pointerEnter.transform as RectTransform;

		var rt = m_DraggingIcon.GetComponent<RectTransform>();
		Vector3 globalMousePos;
		if ( RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos) )
		{
			rt.position = globalMousePos;
			rt.rotation = m_DraggingPlane.rotation;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		Debug.Log("in OnEndDrag" + eventData);
		if ( m_DraggingIcon != null )
			Destroy(m_DraggingIcon);
	}

	static public T FindInParents<T>(GameObject go) where T : Component
	{
		if ( go == null )
			return null;
		var comp = go.GetComponent<T>();

		if ( comp != null )
			return comp;

		Transform t = go.transform.parent;
		while ( t != null && comp == null )
		{
			comp = t.gameObject.GetComponent<T>();
			t = t.parent;
		}
		return comp;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log("in OnPointerDown" + eventData);
		throw new System.NotImplementedException();
	}
}

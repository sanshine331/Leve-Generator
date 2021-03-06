﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
	public Vector2 Position { get { return transform.position; } }

	public int Width { get { return _width; } }

	public int Height { get { return _height; } }

	public int ID { get { return _id; } set { _id = value; } }

	public Rect Rect { get { return _rect; } }

	public float rectMinX { get { return Rect.x - Rect.width / 2 + 0.5f; } }

	public float rectMaxX { get { return Rect.x + Rect.width / 2 - 0.5f; } }

	public float rectMinY { get { return Rect.y - Rect.height / 2 + 0.5f; } }

	public float rectMaxY { get { return Rect.y + Rect.height / 2 - 0.5f; } }
	// so the point is on grid

	public Collider2D Collider { get { return _collider; } }

	private List<HallWay> hallways = new List<HallWay> ();


	private int _width;
	private int _height;
	private int _id;
	private Rect _rect;
	private BoxCollider2D _collider;
	private Rigidbody2D _body2d;

	//	private Cell[,] cells;

	void Awake ()
	{
		_collider = gameObject.GetComponent<BoxCollider2D> ();
		_body2d = gameObject.GetComponent<Rigidbody2D> ();
	}

	public void SetPosition (Vector2 pos)
	{
		transform.position = pos;
	}

	public void SetSize (int width, int height)
	{
		_width = width;
		_height = height;

		// set collider
		// larger gap
		_collider.size = new Vector2 (width, height);

		// set cells
//		cells = new Cell[width, height];

		Debug.Log ("created ```````");
	}

	void OnTriggerStay2D (Collider2D other)
	{
		float xDir = other.transform.position.x - transform.position.x;
		float yDir = other.transform.position.y - transform.position.y;

		float xDiff = Mathf.Abs (xDir);
		float yDiff = Mathf.Abs (yDir);

		// go the larger differ axises
		if (xDiff > yDiff) {
			
			if (_collider.bounds.min.x <= 0 || _collider.bounds.max.x >= Grid.width - 1)
				return;

			transform.Translate (-Mathf.Sign (xDir) * Vector2.right);
		} else {
			if (_collider.bounds.min.y <= 0 || _collider.bounds.max.y >= Grid.height - 1)
				return;
			
			transform.Translate (-Mathf.Sign (yDir) * Vector2.up);
		}
	}

	void OnDrawGizmos ()
	{
		if (!Application.isPlaying)
			return;

		Gizmos.color = Color.red;

		Gizmos.DrawWireCube (transform.position, new Vector2 (Width, Height));
	}

	public void ResetRect ()
	{
		_rect = new Rect (Position.x, Position.y, _width, _height);
	}

	public void RemovePhysics ()
	{
		_rect = new Rect (Position.x, Position.y, _width, _height);
		DestroyImmediate (gameObject.GetComponent<Rigidbody2D> ());
		DestroyImmediate (gameObject.GetComponent<BoxCollider2D> ());
	}

	public void AddHallWay (HallWay hallway)
	{
		hallways.Add (hallway);
	}

	public void Fill ()
	{
		Vector2 pos = new Vector2 (rectMinX, rectMinY);
		//pos.x += 0.5f;
		//pos.y += 0.5f;
		Vector2 tempPos = pos;
		for (int i = 0; i < Width; i++) {
			tempPos.x = pos.x + i;
			for (int j = 0; j < Height; j++) {
				tempPos.y = pos.y + j;

				// left side
				if (i == 0) {
					// bottom left
					if (j == 0) {
						TileManager.instance.CreateTileOnGrid (Resources.Load<GameObject> ("Wall_SW"), tempPos);
					}
						// top left
						else if (j == Height - 1) {
						TileManager.instance.CreateTileOnGrid (Resources.Load<GameObject> ("Wall_NW"), tempPos);
					} else {
						TileManager.instance.CreateTileOnGrid (Resources.Load<GameObject> ("Wall_W"), tempPos);
					}
				}
					// right side
					else if (i == Width - 1) {
					// bottom left
					if (j == 0) {
						TileManager.instance.CreateTileOnGrid (Resources.Load<GameObject> ("Wall_SE"), tempPos);
					}
						// top left
						else if (j == Height - 1) {
						TileManager.instance.CreateTileOnGrid (Resources.Load<GameObject> ("Wall_NE"), tempPos);
					} else {
						TileManager.instance.CreateTileOnGrid (Resources.Load<GameObject> ("Wall_E"), tempPos);
					}
				}
					// top side
					else if (j == Height - 1) {
					TileManager.instance.CreateTileOnGrid (Resources.Load<GameObject> ("Wall_N"), tempPos);
				}
					// bottom
					else if (j == 0) {
					TileManager.instance.CreateTileOnGrid (Resources.Load<GameObject> ("Wall_S"), tempPos);
				} else
					TileManager.instance.CreateTileOnGrid (Resources.Load<GameObject> ("Floor"), tempPos);
			}		
		}

		foreach (var hallway in hallways) {
			TileManager.instance.CreateTileOnGrid (Resources.Load<GameObject> ("Door"), hallway.startPos, true);
			TileManager.instance.CreateTileOnGrid (Resources.Load<GameObject> ("Door"), hallway.endPos, true);

		}
	}
}
using UnityEngine;
using System.Collections.Generic;

public class PlayerLives : MonoBehaviour
{
	[SerializeField] List<GameObject> allLives;
	[SerializeField] int startingLives = 0;

	public static PlayerLives Instance = null;
	public static int heldLives = -1;

	void Awake()
	{
		// TODO Sam this is pretty bad, as this get remade every time a level start or resets
		Instance = this;
		if (heldLives < startingLives)
		{
			heldLives = startingLives;
		}
		ShowLives();
	}

	private void ShowLives()
	{
		for(int i = 0; i < allLives.Count; i++)
		{
			allLives[i].SetActive(i < heldLives);
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			heldLives = 1000;
			ShowLives();
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			heldLives = 0;
			ShowLives();
		}
	}

	public bool LoseLife()
	{
		if (heldLives > 0)
		{
			heldLives--;
			ShowLives();
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool GainLife()
	{
		if (heldLives < allLives.Count - 1)
		{
			heldLives++;
			ShowLives();
			return true;
		}
		else
		{
			return false;
		}
	}
}

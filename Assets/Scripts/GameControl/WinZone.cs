﻿using UnityEngine;
using System.Collections;

public class WinZone : MonoBehaviour {

	public void OnTriggerEnter(Collider other) {
		Player player = other.gameObject.GetComponent<Player>();
		if (player == null)
			return;
		GetComponent<AudioSource>().Play();
		FindObjectOfType<RandomMusic>().gameObject.SetActive(false);
		Menu.menu.win();
	}
}
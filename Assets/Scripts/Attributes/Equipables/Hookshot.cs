﻿using UnityEngine;
using System.Collections;

public class Hookshot : Item {

	public float reach = 20;
	public float reelSpeed = 10;

	protected Player player;
	protected Transform playerTransform;
	protected Rigidbody playerRigidbody;
	protected Vector3 attachPoint;

    public override void invoke(Inventory invoker) {
		breakConnection();

		player = invoker.getPlayer();
		RaycastHit[] hits = cast();
		if (hits.Length < 1) {
			breakConnection();
			print ("huh?");
			return;
		}
		attachPoint = hits[0].point;
		player.mover.lockMovement();
		playerRigidbody = player.GetComponent<Rigidbody>();
		playerTransform = player.transform;
		playerRigidbody.useGravity = false;
		Vector3 desiredVel = (attachPoint -player.transform.position).normalized *reelSpeed;
		playerRigidbody.velocity = desiredVel;

		StartCoroutine(reelIn());
    }

	public override void onEquip(Inventory equipper) {
		base.onEquip(equipper);
		gameObject.SetActive(true);
		Rigidbody rigidbody = GetComponent<Rigidbody>();
		if (rigidbody != null)
			rigidbody.useGravity = false;
		foreach(Collider col in GetComponents<Collider>())
			col.enabled = false;
	}

	public override void onUnequip(Inventory equipper) {
		base.onUnequip(equipper);
		breakConnection();
		gameObject.SetActive(false);
		Rigidbody rigidbody = GetComponent<Rigidbody>();
		if (rigidbody != null)
			rigidbody.useGravity = true;
		foreach(Collider col in GetComponents<Collider>())
			col.enabled = true;
	}

	protected IEnumerator reelIn() {
		while(true) {
			print("coroutine");
			Vector3 desiredVel = attachPoint -player.transform.position;
			if (desiredVel.sqrMagnitude < 1) {
				breakConnection();
				print ("Too close " +desiredVel);
				yield return null;
			}
			desiredVel = desiredVel.normalized *reelSpeed;
			if (Vector3.Dot(playerRigidbody.velocity, desiredVel) /Vector3.Dot(desiredVel, desiredVel) < 0.05f) {
				breakConnection();
				print ("Low velocity");
				yield return null;
			}

			playerRigidbody.velocity = desiredVel;

			yield return new WaitForFixedUpdate();
		}
	}

	protected void breakConnection() {
		if (player == null)
			return;
		player.mover.unlockMovement();
		if (playerRigidbody != null)
			playerRigidbody.useGravity = true;
		StopAllCoroutines();
		player = null;
	}

	protected RaycastHit[] cast() {
		Transform cam = player.cam.transform;
		RaycastHit[] hits = Physics.RaycastAll(cam.position, cam.forward, reach);
		if (hits.Length < 1)
			return new RaycastHit[0];
		RaycastHit finalHit = hits[0];
		foreach(RaycastHit hit in hits) {
			if (hit.distance < finalHit.distance)
				finalHit = hit;
		}
		return new RaycastHit[] {finalHit};
	}
}

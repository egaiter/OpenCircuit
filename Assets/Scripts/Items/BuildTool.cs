﻿using UnityEngine;
using UnityEngine.Networking;

[AddComponentMenu("Scripts/Items/BuildTool")]
public class BuildTool : ContextItem {

	public float range = 20;
	public GameObject structureBase;

	public override void beginInvoke(Inventory invoker) {
		Team team = holder.GetComponent<Team>();
		if (team != null && team.enabled && GlobalConfig.globalConfig.gamemode is Bases) {
			Transform cam = holder.getPlayer().cam.transform;

			RaycastHit hitInfo;
			if (Physics.Raycast(cam.position, cam.forward, out hitInfo, range)) {
				CmdSpawnTower(hitInfo.point);
				invoker.popContext(GetType());
			}
		}
	}

	[Command]
	private void CmdSpawnTower(Vector3 location) {
		Team team = holder.GetComponent<Team>();
		CentralRobotController crc = ((Bases) GlobalConfig.globalConfig.gamemode).getCRC(team.team.Id);
		Label towerBase =
			Instantiate(structureBase, location, Quaternion.identity).GetComponent<Label>();
		crc.sightingFound(towerBase.labelHandle, towerBase.transform.position,
			null);
		NetworkServer.Spawn(towerBase.gameObject);
	}

}

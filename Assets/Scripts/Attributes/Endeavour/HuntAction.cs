﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HuntAction : Endeavour {

	private Label target;

	public HuntAction(EndeavourFactory factory, RobotController controller, List<Goal> goals, Label target)
		: base(factory, controller, goals, target.labelHandle) {
		this.target = target;
		this.name = "hunt";
	}

	public override bool canExecute() {
		HoverJet jet = controller.getRobotComponent<HoverJet>();
		AbstractArms arms = controller.getRobotComponent<AbstractArms>();
        return arms != null && (!arms.hasTarget() || arms.getTarget() == target) && (!target.hasTag(TagEnum.Grabbed) || arms.hasTarget()) && jet != null && jet.canReach(target) && target.GetComponent<Player>() != null && !target.GetComponent<Player>().frozen;
	}

	protected override void onExecute() {
		HoverJet jet = controller.getRobotComponent<HoverJet>();
		if(jet != null && target != null) {
			jet.pursueTarget(target.labelHandle, false);
		}
	}

	public override bool isStale() {
		return target == null || !controller.knowsTarget(target.labelHandle);
	}

	public override void onMessage(RobotMessage message) {
		if(message.Type == RobotMessage.MessageType.ACTION && message.Message.Equals("target in reach")) {
			AbstractArms arms = controller.getRobotComponent<AbstractArms>();
			if(arms != null) {
                HoverJet jet = controller.getRobotComponent<HoverJet>();
                if (jet != null) {
                    jet.stop();
                }

				arms.attachTarget(target);
			}
		}
	}

	public override System.Type[] getRequiredComponents() {
		return new System.Type[] { typeof(HoverJet), typeof(AbstractArms) };
	}

	public override bool singleExecutor() {
		return false;
	}

	protected override float getCost() {
        if (target == null) {
            return float.PositiveInfinity;
        }
		HoverJet jet = controller.getRobotComponent<HoverJet>();
		if(jet != null) {
			return jet.calculatePathCost(target);
		}
		return 0;
	}
}

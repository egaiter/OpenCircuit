﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : Endeavour {

    private Tag target;

    public ShootAction(EndeavourFactory factory, RobotController controller, List<Goal> goals, Dictionary<TagEnum, Tag> tags)
        : base(factory, controller, goals, tags) {
        target = getTagOfType<Tag>(TagEnum.Team);
        name = "shoot";
    }

    public override bool isStale() {
        return !getController().knowsTarget(target.getLabelHandle()) || target.getLabelHandle().label == null || target.getLabelHandle().label.GetComponent<Team>().team.Id == controller.GetComponent<Team>().team.Id;
    }

    protected override void onExecute() {
        rifle.setTarget(target.getLabelHandle());
        jet.setTarget(target.getLabelHandle(), true);
    }

    public override Type[] getRequiredComponents() {
        return new[] {typeof(RoboRifle), typeof(HoverJet) };
    }

    public override bool canExecute() {
        return !target.getLabelHandle().hasTag(TagEnum.Frozen);
    }

    public override bool singleExecutor() {
        return false;
    }

    public override TagEnum getPrimaryTagType() {
        return TagEnum.Team;
    }

    protected override float getCost() {
        return jet.calculatePathCost(target.getLabelHandle().label);
    }
}
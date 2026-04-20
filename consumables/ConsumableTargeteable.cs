public abstract class ConsumableTargeteable : Consumable {
    public enum TargetType {
        BLOCKED_TILE, TOWER, }
        public object target {
            get;
            private set;
        }
        public override bool RequiresTarget() {
            return true;
        }
        public void Use(object selectedTarget) {
            target = selectedTarget;
            Action(target);
            EmitUsed();
        }
        public Tower GetTargetTower() {
            return target as Tower;
        }
        public abstract void Action(object target);
    }


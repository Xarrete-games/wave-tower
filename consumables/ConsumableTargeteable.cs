public abstract class ConsumableTargeteable : Consumable {
    public enum TargetType {
        BLOCKED_TILE, TOWER, }
        public object target {
            get;
            private set;
        }
        public override bool requires_target() {
            return true;
        }
        public void use(object selectedTarget) {
            target = selectedTarget;
            action(target);
            emit_used();
        }
        public Tower get_target_tower() {
            return target as Tower;
        }
        public abstract void action(object target);
    }


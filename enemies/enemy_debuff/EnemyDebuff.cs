using Godot;
public class EnemyDebuff : EnemyEffect {
    public enum Type {
        FROST, BURN, }
        public Type type;
        public EnemyDebuffData data;
        public Source source;
        public float value = 0.0f;
        public float duration = 0.0f;
        public float tick_interval = 0.0f;
        public int max_stacks = 99;
        public EnemyDebuff() {
        }
        public virtual void init(EnemyDebuffData debuffData, Source debuffSource) {
            data = debuffData;
            source = debuffSource;
            type = (Type)debuffData.debuff_type;
            value = debuffData.value;
            duration = debuffData.duration;
            tick_interval = debuffData.tick_interval;
            max_stacks = debuffData.max_stacks;
        }
        public static EnemyDebuff create_frost(Source source) {
            EnemyDebuffData data = DataLoader.Instance?.get_debuff_data((int)Type.FROST).As<EnemyDebuffData>();
            if (data == null) {
                return null;
            }
            EnemyDebuff debuff = data.create_debuff();
            debuff?.init(data, source);
            return debuff;
        }
        public static EnemyDebuff create_burn(Source source) {
            EnemyDebuffData data = DataLoader.Instance?.get_debuff_data((int)Type.BURN).As<EnemyDebuffData>();
            if (data == null) {
                return null;
            }
            EnemyDebuff debuff = data.create_debuff();
            debuff?.init(data, source);
            return debuff;
        }
        public static EnemyDebuff create_from_type(int type, Source source) {
            return type switch {
                (int)Type.FROST => create_frost(source), (int)Type.BURN => create_burn(source), _ => null, }
                ;
            }
            public virtual void on_apply(Enemy enemy) {
            }
            public virtual void on_tick(Enemy enemy) {
            }
            public virtual void on_expire(Enemy enemy) {
            }
        }


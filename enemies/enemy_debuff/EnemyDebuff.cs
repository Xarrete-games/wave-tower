using Godot;
public class EnemyDebuff : EnemyEffect {
    public enum Type {
        FROST, BURN, }
        public Type type;
        public EnemyDebuffData data;
        public Source source;
        public float value = 0.0f;
        public float duration = 0.0f;
        public float TickInterval = 0.0f;
        public int MaxStacks = 99;
        public EnemyDebuff() {
        }
        public virtual void init(EnemyDebuffData debuffData, Source debuffSource) {
            data = debuffData;
            source = debuffSource;
            type = (Type)debuffData.DebuffType;
            value = debuffData.value;
            duration = debuffData.duration;
            TickInterval = debuffData.TickInterval;
            MaxStacks = debuffData.MaxStacks;
        }
        public static EnemyDebuff create_frost(Source source) {
            EnemyDebuffData data = DataLoader.Instance?.GetDebuffData((int)Type.FROST);
            if (data == null) {
                return null;
            }
            EnemyDebuff debuff = data.CreateDebuff();
            debuff?.init(data, source);
            return debuff;
        }
        public static EnemyDebuff create_burn(Source source) {
            EnemyDebuffData data = DataLoader.Instance?.GetDebuffData((int)Type.BURN);
            if (data == null) {
                return null;
            }
            EnemyDebuff debuff = data.CreateDebuff();
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



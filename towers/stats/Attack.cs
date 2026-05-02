public class Attack
{
	public float Damage;
	public bool IsCritical = false;
	public bool IsExecution = false;
	public int Hits = 1;
	public int Bounces = 0;
	public Source Source;
	public float CritChance = 0.0f;

	public Attack()
	{
	}

	public Attack(
		float attackDamage,
		Source attackSource,
		bool isCritical = false,
		bool isExecution = false)
	{
		Damage = attackDamage;
		IsCritical = isCritical;
		IsExecution = isExecution;
		Source = attackSource;
	}
}
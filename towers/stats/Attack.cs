using Godot;
using System.Collections.Generic;

public class Attack
{
	public float Damage;
	public bool IsCritical = false;
	public bool IsExecution = false;
	public int Hits = 1;
	public int Bounces = 0;
	public List<Variant> Effects = new();
	public Source Source;
	public Dictionary<string, Variant> Tags = new();
	public float CritChance = 0.0f;

	public Attack()
	{
	}

	public Attack(
		float attackDamage,
		Source attackSource,
		bool isCritical = false,
		bool isExecution = false,
		Dictionary<string, Variant> attackTags = null)
	{
		Damage = attackDamage;
		IsCritical = isCritical;
		IsExecution = isExecution;
		Source = attackSource;
		Tags = attackTags ?? new Dictionary<string, Variant>();
	}
}
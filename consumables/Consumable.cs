using Godot;
using System;

public class Consumable
{
	public event Action<Consumable> Used;
	public event Action<Consumable> Clicked;

	public enum Type
	{
		OTHER,
		POTION,
	}

	public ConsumableData Data { get; set; }

	public Consumable()
	{
	}

	public Consumable(ConsumableData consumableData)
	{
		Init(consumableData);
	}

	public virtual void Init(ConsumableData consumableData)
	{
		Data = consumableData;
	}

	public virtual bool RequiresTarget()
	{
		return false;
	}

	public Source GetSource()
	{
		string id = Data?.Id ?? string.Empty;
		return new Source(Source.SourceType.CONSUMABLE, id);
	}

	public void EmitUsed()
	{
		Used?.Invoke(this);
	}

	public void EmitClicked()
	{
		Clicked?.Invoke(this);
	}

	protected Node GetSingleton(string name)
	{
		return (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>($"/root/{name}");
	}
}
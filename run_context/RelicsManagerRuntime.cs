using System.Collections.Generic;

public sealed class RelicsManagerRuntime
{
    private readonly List<AbstractModel> _relicListeners = new();

    public IReadOnlyList<AbstractModel> GetAllRelicListeners()
    {
        return this._relicListeners;
    }

    public void AddRelicListener(AbstractModel relic)
    {
        if (relic == null)
        {
            return;
        }

        if (!this._relicListeners.Contains(relic))
        {
            this._relicListeners.Add(relic);
        }
    }

    public void RemoveRelicListener(AbstractModel relic)
    {
        if (relic == null)
        {
            return;
        }

        this._relicListeners.Remove(relic);
    }

    public void Reset()
    {
        this._relicListeners.Clear();
    }
}

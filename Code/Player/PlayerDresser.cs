using Sandbox;

public sealed class PlayerDresser : Component
{
    [Property]
    Clothing.Slots slotFilter;

    [RequireComponent]
    Dresser Dresser { get; set; }

    protected override void OnStart()
    {
        var userClothingContainer = GetUserClothingContainer();
        var filteredClothingContainer = FilterClothingContainer(userClothingContainer, slotFilter);

        Dresser.Clothing = filteredClothingContainer.Clothing;
        Dresser.Apply();
    }

    ClothingContainer FilterClothingContainer(ClothingContainer original, Clothing.Slots filter)
    {
        var filtered = new ClothingContainer();

        foreach (var entry in original.Clothing)
        {
            if ((entry.Clothing.SlotsUnder & filter) != 0) continue;

            filtered.Add(entry);
        }

        return filtered;
    }
    
    ClothingContainer GetUserClothingContainer()
    {
        var container = new ClothingContainer();
        container.Deserialize(Network.Owner.GetUserData("avatar"));
        return container;
    }
}
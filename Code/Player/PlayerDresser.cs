using Sandbox;

public sealed class PlayerDresser : Component, IRoleEvent
{
    [Property]
    Clothing.Slots slotsFilter;

    [Property]
    Clothing hiderSuit;
    
    [Property]
    Clothing seekerSuit;

    [RequireComponent]
    Dresser Dresser { get; set; }

    void IRoleEvent.OnHider() => WearSuit(hiderSuit);
	void IRoleEvent.OnSeeker() => WearSuit(seekerSuit);

    void WearSuit(Clothing suit)
    {
        var userClothingContainer = GetUserClothingContainer();
        var newClothingContainer = FilterClothingContainer(userClothingContainer, slotsFilter);

        newClothingContainer.Add(suit);
        
        Dresser.Clothing = newClothingContainer.Clothing;
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
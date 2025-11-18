using Sandbox;
namespace HNS;

public class Clothes : Component
{
    [Property]
    Clothing.Slots Filter { get; set; }

    [RequireComponent]
    Dresser Dresser { get; set; }

    public void Equip(Clothing suit)
    {
        var userClothingContainer = GetUserClothingContainer();
        var newClothingContainer = FilterClothingContainer(userClothingContainer, Filter);

        newClothingContainer.Add(suit);

        Dresser.Clothing = newClothingContainer.Clothing;
        Dresser.Apply();
    }
    
    ClothingContainer GetUserClothingContainer()
    {
        var container = new ClothingContainer();
        container.Deserialize(Network.Owner.GetUserData("avatar"));
        return container;
    }

    ClothingContainer FilterClothingContainer(ClothingContainer original, Clothing.Slots filter)
    {
        var filtered = new ClothingContainer();

        foreach (var entry in original.Clothing)
        {
            if ((entry?.Clothing?.SlotsUnder & filter) != 0) continue;

            filtered.Add(entry);
        }

        return filtered;
    }
}

using UnityEngine;

/// <summary>
/// Monobehaviour to every item that is clasified as a Pokemon (Vuforia's pokemon object and pokemon cards)
/// </summary>
public class PokemonData : MonoBehaviour
{
    private Pokemon pokemonData;
    public Pokemon GetPokemonData() => pokemonData;
    public void SetPokemonData(Pokemon pokeData) => pokemonData = pokeData;
}

/// <summary>
/// All the data that a Pokemon haves
/// </summary>
[System.Serializable]
public class Pokemon
{
    public int id;
    public string name;
    public Texture2D image;
    public string[] elementTypes;

    public Pokemon(int id, string name, Texture2D image, string[] elementTypes)
    {
        this.id = id;
        this.name = name;
        this.image = image;
        this.elementTypes = elementTypes;
    }

    public override string ToString()
    {
        return $"ID: {id} Name: {name} ElementType : {elementTypes[0]} HasTexture: {image!=null}";
    }
}
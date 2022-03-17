using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PokedexHandler : MonoBehaviour
{
    [Header("Search for pokemon")]
    [SerializeField] private TMP_InputField pokemonNameInput;

    [Header("Pokemon list")]
    [SerializeField] private PokeCard[] pokeCards;
    [SerializeField] private GameObject loadingScreen;

    [Header("Select pokemon")]
    [SerializeField] private TMP_Text pokemonId;
    [SerializeField] private RawImage pokemonImage;
    [SerializeField] private TMP_Text pokemonName;
    [SerializeField] private TMP_Text pokemonTypes;

    private int actualPage = 0; //The page where the pokedex actually is

    public void GetPokemonData()
    {
        StartCoroutine(GetDataForPokedex());
         
    }

    private IEnumerator GetDataForPokedex()
    {
        Pokemon[] pokemonsRetrieved = new Pokemon[10];
        int offset = actualPage * 10;
        if(actualPage == 0)
        {
            offset = 1;
        }

        loadingScreen.SetActive(true);
        yield return StartCoroutine(PokeAPIHandler.Instance.GetPokemonArray(offset, pokemonsRetrieved));  
        loadingScreen.SetActive(false);

        for(int i = 0; i < pokemonsRetrieved.Length; i++)
        {
            if(pokemonsRetrieved[i] != null)
            {
                Pokemon actualPokemon = pokemonsRetrieved[i];
                pokeCards[i].ChangeCardData(actualPokemon.image,actualPokemon.id.ToString());
                pokeCards[i].GetParent().GetComponent<PokemonData>().SetPokemonData(actualPokemon);
            }
            else
            {
                pokeCards[i].ChangeCardData(Texture2D.blackTexture,"X");
            }
        }
    }

    public void NextPage()
    {
        actualPage++;
        GetPokemonData();
    }

    public void ReturnPage()
    {
        actualPage--;
        actualPage = Mathf.Clamp(actualPage, 0, 300);
        GetPokemonData();
    }

    public void ResetPokedexPage()
    {
        actualPage = 0;
    }

    public void SelectPokemon(PokemonData data)
    {
        pokemonId.text = data.GetPokemonData().id.ToString();
        pokemonName.text = data.GetPokemonData().name;
        pokemonImage.texture = data.GetPokemonData().image;

        pokemonTypes.text = string.Empty;
        foreach(string type in data.GetPokemonData().elementTypes)
        {
            pokemonTypes.text += type +"\n";
        }
    }

    public void SearchForPokemon()
    {
        Pokemon pokemonRetrieved = null;
        StartCoroutine(WaitForPokemonSearch(pokemonNameInput.text,pokemonRetrieved));
    }
    IEnumerator WaitForPokemonSearch(string name, Pokemon pokemonRetrieved)
    {
        loadingScreen.SetActive(true);
        yield return StartCoroutine(PokeAPIHandler.Instance.GetPokemon(name,result => pokemonRetrieved = result));
        loadingScreen.SetActive(false);
        
        if(pokemonRetrieved != null)
        {
            PokemonData data = new PokemonData();
            data.SetPokemonData(pokemonRetrieved);
            SelectPokemon(data);
        }
        else
        {
            pokemonId.text = "0";
            pokemonName.text = "DOESN'T EXIST";
            pokemonImage.texture = Texture2D.blackTexture;

            pokemonTypes.text = string.Empty;
        }
    }
}

[System.Serializable]
public class PokeCard
{
    public RawImage pokemonImage;
    public TMP_Text pokemonIdUGUI;

    private Pokemon pokemon;

    public void ChangeCardData(Texture2D image, string id)
    {
        pokemonImage.texture = image;
        pokemonIdUGUI.text = id;
    }

    public Transform GetParent()
    {
        return pokemonImage.transform.parent;
    }

}

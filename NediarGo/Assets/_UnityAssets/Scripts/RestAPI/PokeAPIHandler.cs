using System;
using System.Collections;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

public class PokeAPIHandler : MonoBehaviour
{
    public static PokeAPIHandler Instance {private set; get;}
    private readonly string pokeApiURL = "https://pokeapi.co/api/v2/pokemon/";
    private readonly string pokeApiPokemonListURL = "https://pokeapi.co/api/v2/pokemon?limit=1500";

    void Awake()
    {
        //Singleton
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    IEnumerator GetPokemon(int index, Action<Pokemon> pokemonRetrieved) //Overload for searching pokemons by id
    {
        string URL = pokeApiURL + index.ToString();

        UnityWebRequest apiRequest = UnityWebRequest.Get(URL);

        yield return apiRequest.SendWebRequest();

        if(apiRequest.result == UnityWebRequest.Result.ConnectionError || apiRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(apiRequest.error);
            yield break;
        }

        JSONNode pokeInfo = JSON.Parse(apiRequest.downloadHandler.text);

        string pokeName = pokeInfo["name"];
        pokeName = FirstLetterToUpperCase(pokeName);
        string pokeSpriteURL = pokeInfo["sprites"]["front_default"];

        JSONNode pokeTypes = pokeInfo["types"];
        string[] pokeTypeStrings = new string[pokeTypes.Count];

        for(int i = 0, j = pokeTypes.Count - 1; i < pokeTypes.Count; i++, j--)
        {
            pokeTypeStrings[j] = FirstLetterToUpperCase(pokeTypes[i]["type"]["name"]);
        }

        UnityWebRequest pokeTextureRequest = UnityWebRequestTexture.GetTexture(pokeSpriteURL);

        yield return pokeTextureRequest.SendWebRequest();

        if(pokeTextureRequest.result == UnityWebRequest.Result.ConnectionError || pokeTextureRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(pokeTextureRequest.error);
            yield break;
        }

        Texture2D pokeTexture = DownloadHandlerTexture.GetContent(pokeTextureRequest);
        pokeTexture.filterMode = FilterMode.Point;

        pokemonRetrieved(new Pokemon(index, pokeName, pokeTexture, pokeTypeStrings));
    }

    public IEnumerator GetPokemon(string pokemonName, Action<Pokemon> pokemonRetrieved) //Overload for searching pokemons by name
    {
        string URL = pokeApiPokemonListURL;

        UnityWebRequest apiRequest = UnityWebRequest.Get(URL);

        yield return apiRequest.SendWebRequest();

        if(apiRequest.result == UnityWebRequest.Result.ConnectionError || apiRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(apiRequest.error);
            yield break;
        }

        JSONNode pokeList = JSON.Parse(apiRequest.downloadHandler.text);

        bool pokemonFinded = false;
        int id = 0;

        for(int i = 0; i < pokeList["results"].Count; i++)
        {
            if(pokeList["results"][i]["name"] == pokemonName.ToLower().Trim())
            {
                pokemonFinded = true;
                id = i + 1;
                break;
            }
        }
        
        if(pokemonFinded)
        {
            yield return StartCoroutine(GetPokemon(id,pokemonRetrieved));
        }
        else
        {
            pokemonRetrieved = null;
        }
    }

    public IEnumerator GetPokemonArray(int offset, Pokemon[] pokemonArray)
    {
        int index = 0;

        for(int x = offset ; x < offset + 10 ; x++)
        {
            string URL = pokeApiURL + x.ToString();

            UnityWebRequest apiRequest = UnityWebRequest.Get(URL);

            yield return apiRequest.SendWebRequest();

            if(apiRequest.result == UnityWebRequest.Result.ConnectionError || apiRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(apiRequest.error);
                yield break;
            }

            JSONNode pokeInfo = JSON.Parse(apiRequest.downloadHandler.text);

            string pokeName = pokeInfo["name"];
            pokeName = FirstLetterToUpperCase(pokeName);
            string pokeSpriteURL = pokeInfo["sprites"]["front_default"];

            JSONNode pokeTypes = pokeInfo["types"];
            string[] pokeTypeStrings = new string[pokeTypes.Count];

            for(int i = 0, j = pokeTypes.Count - 1; i < pokeTypes.Count; i++, j--)
            {
                pokeTypeStrings[j] = FirstLetterToUpperCase(pokeTypes[i]["type"]["name"]);
            }

            UnityWebRequest pokeTextureRequest = UnityWebRequestTexture.GetTexture(pokeSpriteURL);

            yield return pokeTextureRequest.SendWebRequest();

            if(pokeTextureRequest.result == UnityWebRequest.Result.ConnectionError || pokeTextureRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(pokeTextureRequest.error);
                yield break;
            }

            Texture2D pokeTexture = DownloadHandlerTexture.GetContent(pokeTextureRequest);
            pokeTexture.filterMode = FilterMode.Point;

            Pokemon pokemonRetrieved = new Pokemon(x, pokeName, pokeTexture, pokeTypeStrings);

            pokemonArray[index] = pokemonRetrieved;
            index++;
        }
    }

    private string FirstLetterToUpperCase(string word)
    {
        return char.ToUpper(word[0]) + word.Substring(1);
    }
}

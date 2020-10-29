using System;
using System.Collections.Generic;

/*
    This class contains and deals with all the users data such as their history
*/
[Serializable]
public class UserData{
    public string homeUrl {get;set;}
    public int currentHistoryIndex {get;set;}
    public List<Favorite> favorites {get;set;}
    public List<History> history {get;set;}

    [Serializable]
    public struct Favorite{
        public string name {get;set;}
        public string url {get;set;}
    }

    [Serializable]
    public struct History{
        public string url {get;set;}
        public string title {get;set;}
        public DateTime time {get;set;}
    }

    public UserData(){
        favorites = new List<Favorite>();
        history = new List<History>();
        currentHistoryIndex = history.Count-1;
    }

    public void addHistory(string url, DateTime time, string title){
        History h = new History();
        h.time = time;
        h.url = url;
        h.title = title;
        history.Add(h);
        currentHistoryIndex = history.Count-1;
    }

    public void deleteHistory(){
        this.history = new List<History>();
        currentHistoryIndex = history.Count-1;
    }

    public void addFavorite(string url, string name){
        Favorite f = new Favorite();
        f.url = url;
        f.name = name;
        favorites.Add(f);
    }

    public void changeFavoriteName(string newName, string url){
        Favorite f = new Favorite();
        f.name = newName;
        f.url = url;
        for(int i=0 ; i<favorites.Count ; i++){
            if(favorites[i].url == url){
                favorites.RemoveAt(i);
                break;
            }
        }
        favorites.Add(f);
    }


    public string getHistory(int index){
        return history[index].url;
    }


    //janky
    public void setUpForSaving(){
        //weirdness if you dont have a homepage set
        if(homeUrl == null){
            currentHistoryIndex = history.Count;
        }else{
            currentHistoryIndex = history.Count-1;
        }
    }


    //just for testing
    public void print(){
        System.Console.WriteLine("Currenturlindex :{0}",currentHistoryIndex);
            System.Console.WriteLine("History:");
          foreach(History h in history){
            System.Console.WriteLine("url:{0}\ntime:{1}",h.url,h.time);
        }
        System.Console.WriteLine("\nFavorites:");
          foreach(Favorite f in favorites){
            System.Console.WriteLine("url:{0}\nname:{1}",f.url,f.name);
        }
        System.Console.WriteLine("\nHomepage:\n{0}",homeUrl);
      
    }
}


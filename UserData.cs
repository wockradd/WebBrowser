using System;
using System.Collections.Generic;

[Serializable]
public class UserData{
    public string homeUrl {get;set;}
    public string currentUrl {get;set;}
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
        public DateTime time {get;set;}
    }

    public UserData(){
        favorites = new List<Favorite>();
        history = new List<History>();
        homeUrl = "http://zetcode.com/gui/gtksharp/menus/";
        currentHistoryIndex = history.Count-1;
    }

    public void addHistory(string url, DateTime time){
        History h = new History();
        h.time = DateTime.Now;
        h.url = url;
        history.Add(h);
        currentHistoryIndex = history.Count-1;
    }

    public void addFavorite(string url){
        Favorite f = new Favorite();
        f.url = url;
        f.name = url;
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
        currentUrl = null;
    }

    //just for testing
    public void print(){
        System.Console.WriteLine("Currenturlindex :{0}",currentHistoryIndex);
            System.Console.WriteLine("History:");
          foreach(History h in history){
            System.Console.WriteLine("{0}\n{1}",h.url,h.time);
        }
        System.Console.WriteLine("\nFavorites:");
          foreach(Favorite f in favorites){
            System.Console.WriteLine("{0}\n{1}",f.url,f.name);
        }
        System.Console.WriteLine("\nHomepage:\n{0}",homeUrl);
      
    }
}


using System;
using System.Collections.Generic;

[Serializable]
public class UserData{
    public string homeUrl {get;set;}
    public int currentHistoryIndex {get;set;}
    private  List<Favorite> favorites;
    private List<History> history;

    [Serializable]
    private struct Favorite{
        public string name {get;set;}
        public string url {get;set;}
    }

    [Serializable]
    private struct History{
        public string url {get;set;}
        public DateTime time {get;set;}
    }

    public UserData(){
        favorites = new List<Favorite>();
        history = new List<History>();
        currentHistoryIndex = history.Count;
    }

    public void addHistory(string url, DateTime time){
        History h = new History();
        h.time = DateTime.Now;
        h.url = url;
        history.Add(h);
    }

    //just for testing
    public void print(){
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


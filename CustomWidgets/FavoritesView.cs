using Gtk;
using System.Collections.Generic;

public class FavoritesView:ScrolledWindow{
    private UserData userData;
    private VBox vBox;
    private List<FavoriteItem> items;

    public delegate void Func1(string s);
    public delegate void Func2(WebBrowser.States s);
    public delegate void Func3(string s, bool b);

    public FavoritesView(UserData data, Func1 updateStatus, Func2 changeView, Func3 makeRequest){
        userData = data;
        items = new List<FavoriteItem>();
        vBox = new VBox(false, 20);
        foreach(UserData.Favorite f in userData.favorites){
            items.Add(new FavoriteItem(f.url, f.name));
        }
        for(int i=0 ; i<items.Count ; i++){
            vBox.PackStart(items[i],false,false,0);
            items[i].save.Clicked += (obj,args) => updateName((FavoriteItem)((Button)obj).Parent, updateStatus);
            items[i].gotoUrl.Clicked += (obj,args) => gotoFav(changeView, makeRequest,((FavoriteItem)((Button)obj).Parent).urlEntry.Text);
        }
        this.Add(vBox);
    }


    public void updateName(FavoriteItem f, Func1 updateStatus){
        if(f.nameEntry.Text != ""){
            updateStatus("Saved");
            userData.changeFavoriteName(f.nameEntry.Text, f.urlEntry.Text);
        }
    }

    public void gotoFav(Func2 changeView, Func3 makeRequest, string url){
        changeView(WebBrowser.States.Main);
        makeRequest(url,true);
    }
}
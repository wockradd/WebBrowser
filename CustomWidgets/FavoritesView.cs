using Gtk;
using System.Collections.Generic;

public class FavoritesView:ScrolledWindow{
    UserData userData;
    private VBox vBox;
    List<FavoriteItem> items;

    public FavoritesView(UserData data){
        userData = data;
        items = new List<FavoriteItem>();
        vBox = new VBox();
        foreach(UserData.Favorite f in userData.favorites){
            items.Add(new FavoriteItem(f.url, f.name));
        }
        for(int i=0 ; i<items.Count ; i++){
            vBox.PackStart(items[i],false,false,0);
        }
        this.Add(vBox);
    }
}
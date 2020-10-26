using Gtk;
using System.Collections.Generic;

public class FavoritesView:ScrolledWindow{
    private UserData userData;
    private VBox vBox;
    private List<FavoriteItem> items;

    public delegate void Function(string s);

    public FavoritesView(UserData data, Function updateStatus){
        userData = data;
        items = new List<FavoriteItem>();
        vBox = new VBox(false, 20);
        foreach(UserData.Favorite f in userData.favorites){
            items.Add(new FavoriteItem(f.url, f.name));
        }
        for(int i=0 ; i<items.Count ; i++){
            vBox.PackStart(items[i],false,false,0);
            items[i].save.Clicked += (obj,args) => updateName((FavoriteItem)((Button)obj).Parent, updateStatus);
        }
        this.Add(vBox);
    }

    public void updateName(FavoriteItem f, Function func){
        if(f.nameEntry.Text != ""){
            func("Saved");
            userData.changeFavoriteName(f.nameEntry.Text, f.urlEntry.Text);
        }
    }
}
using Gtk;

/*
    Custom widget that contains one item in the users favorites
*/
public class FavoriteItem:Table{
    private Label urlLabel,nameLabel;

    //favoriteview needs to access these vars
    public Entry urlEntry {get;set;}
    public Entry nameEntry {get;set;}
    public Button gotoUrl {get;set;}
    public Button save {get;set;}

    public FavoriteItem(string url, string name)
    :base(2,3,false){
        //init widgets
        urlLabel = new Label("Url:");
        nameLabel = new Label("Name:");
        urlEntry = new Entry(url);
        urlEntry.IsEditable = false;
        nameEntry = new Entry(name);
        gotoUrl = new Button("Go to");
        save = new Button("Save name changes");

        //set up layout
        this.Attach(urlLabel,0,1,0,1);
        this.Attach(urlEntry,1,2,0,1);
        this.Attach(gotoUrl,2,3,0,1);
        this.Attach(nameLabel,0,1,1,2);
        this.Attach(nameEntry,1,2,1,2);
        this.Attach(save,2,3,1,2);
    }
}
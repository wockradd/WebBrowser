using Gtk;

public class FavoriteItem:Table{
    private Label urlLabel,nameLabel;
    private Entry urlEntry,nameEntry;
    private Button gotoUrl, save;

    public FavoriteItem(string url, string name)
    :base(2,3,false){
        urlLabel = new Label("Url:");
        nameLabel = new Label("Name:");
        urlEntry = new Entry(url);
        urlEntry.IsEditable = false;
        nameEntry = new Entry(name);
        gotoUrl = new Button("Go to");
        save = new Button("Save name changes");

        this.Attach(urlLabel,0,1,0,1);
        this.Attach(urlEntry,1,2,0,1);
        this.Attach(gotoUrl,2,3,0,1);
        this.Attach(nameLabel,0,1,1,2);
        this.Attach(nameEntry,1,2,1,2);
        this.Attach(save,2,3,1,2);
    }
}
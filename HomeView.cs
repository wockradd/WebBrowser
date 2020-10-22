using Gtk;

public class HomeView:VBox{
    private Label currentLabel, newLabel;
    private Entry currentEntry,newEntry;
    private Button update;
    private Alignment buttonAlign;

    private UserData userData;

    public HomeView(UserData data){
        userData = data;

        currentLabel = new Label("Current Homepage:");
        currentEntry = new Entry(userData.homeUrl);
        newLabel = new Label("New Homepage:");
        newEntry = new Entry();
        update = new Button("Update");
        buttonAlign = new Alignment(0.5f,0f,0f,0f);

        currentEntry.IsEditable = false;
        buttonAlign.Add(update);

        update.Clicked += (obj,args) => updateHomepage(newEntry.Text.Trim());

        this.PackStart(currentLabel,false,false,0);
        this.PackStart(currentEntry,false,false,0);
        this.PackStart(newLabel,false,false,0);
        this.PackStart(newEntry,false,false,0);
        this.PackStart(buttonAlign,false,false,0);
    }


    public void updateHomepage(string newHomeUrl){
        if(Requester.matchesUrl(newHomeUrl)){
            if(newHomeUrl == userData.homeUrl){
                newEntry.Text = "Thats already your homepage";
            }else{
                newEntry.Text = "Updated";
                userData.homeUrl = newHomeUrl;
                currentEntry.Text = userData.homeUrl;
            }
        }else{
            newEntry.Text = "Invalid url";
        }
    }
}
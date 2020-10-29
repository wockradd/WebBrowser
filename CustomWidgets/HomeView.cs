using Gtk;

/*
    Custom widget to deal with setting the users homepage
*/
public class HomeView:VBox{
    private Label currentLabel, newLabel;
    private Entry currentEntry,newEntry;
    private Button update;
    private Alignment buttonAlign;

    private UserData userData;

    public HomeView(UserData data){
        //init user data
        userData = data;

        //init gui
        currentLabel = new Label("Current Homepage:");
        currentEntry = new Entry();
        newLabel = new Label("New Homepage:");
        newEntry = new Entry();
        update = new Button("Update");
        buttonAlign = new Alignment(0.5f,0f,0f,0f);

        //set some gui properties
        currentEntry.IsEditable = false;
        buttonAlign.Add(update);

        //add event handlers
        update.Clicked += (obj,args) => updateHomepage(newEntry.Text.Trim());

        //finish layout
        this.PackStart(currentLabel,false,false,0);
        this.PackStart(currentEntry,false,false,0);
        this.PackStart(newLabel,false,false,0);
        this.PackStart(newEntry,false,false,0);
        this.PackStart(buttonAlign,false,false,0);

        setDefaultState();
    }
    

    public void setDefaultState(){
        currentEntry.Text = userData.homeUrl;
        newEntry.Text = "";
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
using Gtk;
using System;


/*
    Widget that contains the main default view with the search bar and stuff
*/
public class MainView:VBox{

    private string currentUrl = null;

    private TextView view;
    private TextBuffer buffer;
    private ScrolledWindow scroll;
    private HBox hBox;
    private Button backButton,forwardButton,reloadButton,homeButton,favoriteButton;
    private Entry searchBar;
    private Label statusText;
    
    private UserData userData;
    private Requester.Response response;

    public MainView(UserData data){
        view = new TextView ();
		buffer = view.Buffer;
        scroll = new ScrolledWindow();
        hBox = new HBox(false,0);
        backButton = new Button("<");
        forwardButton = new Button(">");
        reloadButton = new Button("\u27F3");
        homeButton = new Button("\u2302");
        favoriteButton = new Button();
        searchBar = new Entry();
        statusText = new Label();

        //set up main view
        view.Editable = false;
        scroll.Add(view);

        //set up serch bar layout
        hBox.PackStart(backButton,false,false,0);
        hBox.PackStart(forwardButton,false,false,0);
        hBox.PackStart(reloadButton,false,false,0);
        hBox.PackStart(searchBar,true,true,0);
        hBox.PackStart(homeButton,false,false,0);
        hBox.PackStart(favoriteButton,false,false,0);
        this.PackStart(hBox,false,false,0);
        this.PackStart(scroll,true,true,0);
        this.PackStart(statusText,false,false,0);


        //add event handlers
        favoriteButton.Clicked += (obj,args) => editFavorites();
        homeButton.Clicked += (obj,args) => loadHomepage();
        reloadButton.Clicked += (obj,args) => reloadCurrentUrl();
        searchBar.Activated += (obj,args) => makeRequest(searchBar.Text, true);
        backButton.Clicked += (obj,args) => goBack();
        forwardButton.Clicked += (obj,args) => goForward();


        //load in user data and set up
        userData = data;
        setButtonStates();
        loadHomepage();
    }


    public void setButtonStates(){
         if(userData.currentHistoryIndex >= 1){
            backButton.Sensitive = true;
        }else{
            backButton.Sensitive = false;
        }

        if(userData.currentHistoryIndex < userData.history.Count-1){
            forwardButton.Sensitive = true;
        }else{
            forwardButton.Sensitive = false;
        }

        favoriteButton.Label = "\u2606"; // default state, star outline
        foreach(UserData.Favorite f in userData.favorites){
            if(f.url == currentUrl){
                favoriteButton.Label = "\u2605";//filled star if in favorites
                break;
            }
        }
    }


    public void editFavorites(){
        bool alreadyFavorite = false;
        foreach(UserData.Favorite f in userData.favorites){
            if(f.url == currentUrl){
                alreadyFavorite = true;
                userData.favorites.Remove(f);
                statusText.Markup = "<span weight='bold' size='larger'>Removed from favorites</span>";
                break;
            }
        }
        if(!alreadyFavorite){
            statusText.Markup = "<span weight='bold' size='larger'>Added to favorites</span>";
            userData.addFavorite(currentUrl);
        }
        setButtonStates();
    }


    public void loadHomepage(){
        if(userData.homeUrl != null){ 
            searchBar.Text = userData.homeUrl;
            makeRequest(userData.homeUrl, true);
        }else{
            searchBar.Text = "";
            buffer.Text = "No homepage set.\nGo to View -> Homepage to set one.";
            favoriteButton.Sensitive = false;
        }
    }

    
    public void reloadCurrentUrl(){
        if(currentUrl != null){
            searchBar.Text = currentUrl; 
            makeRequest(currentUrl, false);
        }else{
            buffer.Text = "Nothing to reload";
        }
    }


    public void goBack(){
        makeRequest(userData.getHistory(--userData.currentHistoryIndex),false);
        searchBar.Text = userData.getHistory(userData.currentHistoryIndex);
    }


    public void goForward(){
        makeRequest(userData.getHistory(++userData.currentHistoryIndex),false);
        searchBar.Text = userData.getHistory(userData.currentHistoryIndex);
    }


    public async void makeRequest(string url, bool addToHistory){
        //before request
        hBox.Sensitive = false;
        buffer.Text = "Loading...";
        statusText.Text = "";

        response = await Requester.asyncRequest(url);

        //after request
        buffer.Text = response.res;
        statusText.Markup = "<span weight='bold' size='larger'>Status: " + response.status.ToString() + "</span>";
        hBox.Sensitive = true;
        currentUrl = url;
        if(addToHistory){userData.addHistory(url,DateTime.Now);}
        setButtonStates();
        searchBar.GrabFocus();
        searchBar.SelectRegion(searchBar.Text.Length,searchBar.Text.Length);
    }
}

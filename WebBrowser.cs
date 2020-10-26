using Gtk;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

/*
    http://zetcode.com/gui/gtksharp/menus/
    http://status.savanttools.com/?code=400%20Bad%20Request
    http://status.savanttools.com/?code=403%20Forbidden
    http://status.savanttools.com/?code=404%20Not%20Found
*/

/*
    Main GUI class containing all our widgets
*/
public class WebBrowser{
    enum States {Main,Home,Favorites,History}
    States currentState;

    //deafult widgets
    private Window win;
    private VBox vBox; 
    private MenuBar menuBar;
    private Menu menu;
    private MenuItem viewMenu,history,favorites,home, main;
    private TextView view;
    private TextBuffer buffer;
    private ScrolledWindow scroll;
    private HBox hBox;
    private Button backButton,forwardButton,reloadButton,homeButton,favoriteButton;
    private Entry searchBar;
    private Label statusText;

    private HomeView homeView;
    private FavoritesView favoritesView;
    private HistoryView historyView;

    private string currentUrl = null;
    private Requester.Response response;

    private UserData userData;
    private Stream fileStream;
    private BinaryFormatter formatter;


    public WebBrowser(){
        loadUserData();
        Application.Init();
        initGui();
        setButtonStates();
        loadHomepage();
	    Application.Run();
    }


    public void initGui(){
        //init widgets
        win = new Window ("Browser");
        vBox = new VBox(false,0);
        menuBar = new MenuBar();
        menu = new Menu();
        viewMenu = new MenuItem("View");
        history = new MenuItem("History");
        favorites = new MenuItem("Favorites");
        home = new MenuItem("Homepage");
        main = new MenuItem("Main");
        homeView = new HomeView(userData);
        favoritesView = new FavoritesView(userData);
        historyView = new HistoryView(userData);
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

         //set up menu
        viewMenu.Submenu = menu;
        menu.Add(home);
        menu.Add(favorites);
        menu.Add(history);
        menuBar.Append(viewMenu);


        //add event handlers
        win.DeleteEvent +=(obj,args) => closeAndSave();
        home.Activated += (obj,args) => setState(States.Home);
        main.Activated += (obj,args) => setState(States.Main);
        favorites.Activated += (obj,args) => setState(States.Favorites);
        history.Activated += (obj,args) => setState(States.History);
        favoriteButton.Clicked += (obj,args) => editFavorites();
        homeButton.Clicked += (obj,args) => loadHomepage();
        reloadButton.Clicked += (obj,args) => reloadCurrentUrl();
        searchBar.Activated += (obj,args) => makeRequest(searchBar.Text, true);
        backButton.Clicked += (obj,args) => goBack();
        forwardButton.Clicked += (obj,args) => goForward();

        
        //set up the default layout
        vBox.PackStart(menuBar,false,false,0);
        vBox.PackStart(hBox,false,false,0);
        vBox.PackStart(scroll,true,true,0);
        vBox.PackStart(statusText,false,false,0);

        

        //finish up 
        currentState = States.Main;
        win.SetDefaultSize (1000,600);
        win.Resizable = false;
        win.Add(vBox);
		win.ShowAll();
    }



    public void loadUserData(){
        try{
            fileStream = File.Open("data", FileMode.Open);
            formatter = new BinaryFormatter();
            userData = (UserData)formatter.Deserialize(fileStream);
            fileStream.Close();
        }catch(FileNotFoundException fnfe){
            userData = new UserData();
        }
        userData.print();
    }


    public void closeAndSave(){
        userData.setUpForSaving();
        fileStream = File.Open("data", FileMode.Create);
        formatter = new BinaryFormatter();
        formatter.Serialize(fileStream,userData);
        fileStream.Close();
        userData.print();
        Application.Quit();
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
        favoriteButton.Sensitive = true;
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
        setState(States.Main);
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
            makeRequest(currentUrl, true);
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
        if(url != ""){
            //before request
            if(currentState != States.Main){
                setState(States.Main);
            }
            hBox.Sensitive = false;
            buffer.Text = "Loading...\nWill timeout after a minute or two if no response";
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
    

    private void setState(States newState){
        //what to do to get ready for state change
        switch(currentState){
            case States.Main:
                vBox.Remove(scroll);
                vBox.Remove(statusText);
                menu.Add(main);
            break;
            case States.History:
                vBox.Remove(historyView);
                menu.Add(history);
            break;
            case States.Favorites:
                vBox.Remove(favoritesView);
                menu.Add(favorites);
            break;
            case States.Home:
                vBox.Remove(homeView);
                homeView.setDefaultState();
                menu.Add(home);
            break;
        }

        //what to do to finalise the state change
        switch(newState){
            case States.Main:
                vBox.Add(scroll);
                vBox.PackStart(statusText,false,false,0);
                menu.Remove(main);
                setButtonStates();
            break;
            case States.History:
                historyView.populate();
                vBox.Add(historyView);
                menu.Remove(history);
            break;
            case States.Favorites:
                favoritesView = new FavoritesView(userData);
                vBox.Add(favoritesView);
                menu.Remove(favorites);
            break;
            case States.Home:
                vBox.Add(homeView);
                menu.Remove(home);
            break;
        }
        currentState = newState;
        win.ShowAll();
    }
}
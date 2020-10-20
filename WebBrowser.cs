using System.Net;
using Gtk;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/*
    http://zetcode.com/gui/gtksharp/menus/
    http://status.savanttools.com/?code=400%20Bad%20Request
    http://status.savanttools.com/?code=403%20Forbidden
    http://status.savanttools.com/?code=404%20Not%20Found
*/


public class WebBrowser{
    private Window win;
    private TextView view;
    private TextBuffer buffer;
    private ScrolledWindow scroll;
    private HBox hBox;
    private VBox vBox; 
    private Button backButton,forwardButton,reloadButton,homeButton,favoriteButton;
    private Entry searchBar;
    private Label statusText;
    private MenuBar menuBar;
    private Menu menu;
    private MenuItem viewMenu,history,favorites,home;

    private UserData userData;
    private Stream fileStream;
    private BinaryFormatter formatter;


    public WebBrowser(){
        loadUserData();

        userData.print();

        Application.Init();
        initGui();
        loadHomepage();
	    Application.Run();
    }

    public void initGui(){
        //init widgets
        win = new Window ("Browser");
        view = new TextView ();
		buffer = view.Buffer;
        scroll = new ScrolledWindow();
        hBox = new HBox(false,0);
        vBox = new VBox(false,0);
        backButton = new Button("<");
        forwardButton = new Button(">");
        reloadButton = new Button("\u27F3");
        homeButton = new Button("\u2302");
        favoriteButton = new Button();
        searchBar = new Entry();
        statusText = new Label();
        menuBar = new MenuBar();
        menu = new Menu();
        viewMenu = new MenuItem("View");
        history = new MenuItem("History");
        favorites = new MenuItem("Favorites");
        home = new MenuItem("Homepage");

        setButtonStates();

        //add event handlers
        win.DeleteEvent +=(obj,args) => closeAndSave();
        searchBar.Activated += (obj,args) => asyncRequest(searchBar.Text, true);
        homeButton.Clicked += (obj,args) => loadHomepage();
        reloadButton.Clicked += (obj,args) => reloadCurrentUrl();
        backButton.Clicked += (obj,args) => goBack();
        forwardButton.Clicked += (obj,args) => goForward();
        favoriteButton.Clicked += (obj,args) => editFavorites();

        //set up menu
        viewMenu.Submenu = menu;
        menu.Append(history);
        menu.Append(favorites);
        menu.Append(home);
        menuBar.Append(viewMenu);

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

        //set up the final layout
        vBox.PackStart(menuBar,false,false,0);
        vBox.PackStart(hBox,false,false,0);
        vBox.PackStart(scroll,true,true,0);
        vBox.PackStart(statusText,false,false,0);

        //finish up 
		win.SetDefaultSize (1000,600);
        win.Add(vBox);
		win.ShowAll();
    }

    public void editFavorites(){
        bool alreadyFavorite = false;
        foreach(UserData.Favorite f in userData.favorites){
            if(f.url == userData.currentUrl){
                alreadyFavorite = true;
                userData.favorites.Remove(f);
                statusText.Markup = "<span weight='bold' size='larger'>Removed from favorites</span>";
                break;
            }
        }
        if(!alreadyFavorite){
            statusText.Markup = "<span weight='bold' size='larger'>Added to favorites</span>";
            userData.addFavorite(userData.currentUrl);
        }
        setButtonStates();
    }


    public void goBack(){
        asyncRequest(userData.getHistory(--userData.currentHistoryIndex),false);
        searchBar.Text = userData.getHistory(userData.currentHistoryIndex);
    }


    public void goForward(){
        asyncRequest(userData.getHistory(++userData.currentHistoryIndex),false);
        searchBar.Text = userData.getHistory(userData.currentHistoryIndex);
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
    }


    public void reloadCurrentUrl(){
        if(userData.currentUrl != null){
            searchBar.Text = userData.currentUrl; 
            asyncRequest(userData.currentUrl, false);
        }else{
            buffer.Text = "Nothing to reload";
        }
    }



    public void closeAndSave(){
        userData.setUpForSaving();
        fileStream = File.Open("data", FileMode.Create);
        formatter = new BinaryFormatter();
        formatter.Serialize(fileStream,userData);
        fileStream.Close();
        Application.Quit();
    }


    public void loadHomepage(){
        if(userData.homeUrl != null){
            searchBar.Text = userData.homeUrl; 
            asyncRequest(userData.homeUrl, true);
        }else{
            searchBar.Text = "";
            buffer.Text = "No homepage set.\nGo to View -> Homepage to set one.";
            favoriteButton.Sensitive = false;
        }
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
        favoriteButton.Label = "\u2606";
        foreach(UserData.Favorite f in userData.favorites){
            if(f.url == userData.currentUrl){
                favoriteButton.Label = "\u2605";
                break;
            }
        }
    }

    //janky, clean up way exceptions are handled
    //and move ui changing stuff to its own method
    public async void asyncRequest(string url, bool addToHistory){
        hBox.Sensitive = false;//stop user pressing buttons while we're loading
        buffer.Text = "Loading...";
        statusText.Text = "";

        WebRequest webReq;
        WebResponse webRes;
        Stream resStream;
        StreamReader streamReader;
        string resString;
        int statusCode;
        
        try{
            webReq = WebRequest.Create(url);
            webRes = await webReq.GetResponseAsync();
            resStream = webRes.GetResponseStream();
            streamReader= new StreamReader(resStream);
            resString = streamReader.ReadToEnd();
            buffer.Text = resString;
            statusCode = (int)((HttpWebResponse)webRes).StatusCode;
            webRes.Close();
            userData.currentUrl = url;
            if(addToHistory){userData.addHistory(url,DateTime.Now);}
            statusText.Markup = "<span weight='bold' size='larger'>Status: " + statusCode.ToString() + "</span>";
            favoriteButton.Sensitive = true;

        }catch(WebException we){
            buffer.Text = we.Message;

            if(we.Status == WebExceptionStatus.ProtocolError){
                webRes = we.Response;
                statusCode = (int)((HttpWebResponse)webRes).StatusCode;   
                webRes.Close();
                favoriteButton.Sensitive = true;
            }else{                        
              statusCode = -1;
            } 
            userData.currentUrl = url;
            if(addToHistory){userData.addHistory(url,DateTime.Now);}  
            statusText.Markup = "<span weight='bold' size='larger'>Status: " + statusCode.ToString() + "</span>";


        }catch(UriFormatException e){
            buffer.Text = e.Message;
            favoriteButton.Sensitive = false;
        }

        hBox.Sensitive = true;


       setButtonStates();
    }
}
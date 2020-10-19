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
        favoriteButton = new Button("\u2606");
        searchBar = new Entry();
        statusText = new Label();
        menuBar = new MenuBar();
        menu = new Menu();
        viewMenu = new MenuItem("View");
        history = new MenuItem("History");
        favorites = new MenuItem("Favorites");
        home = new MenuItem("Homepage");

        //add event handlers
        win.DeleteEvent +=(obj,args) => closeAndSave();
        searchBar.Activated += (obj,args) => asyncRequest(searchBar.Text, true);
        homeButton.Clicked += (obj,args) => loadHomepage();
        reloadButton.Clicked += (obj,args) => reloadCurrentUrl();

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
        userData.currentUrl = null;
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
            buffer.Text = "No homepage set";
        }
    }


    public async void asyncRequest(string url, bool addToHistory){
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

        }catch(WebException we){
            buffer.Text = we.Message;

            if(we.Status == WebExceptionStatus.ProtocolError){
                webRes = we.Response;
                statusCode = (int)((HttpWebResponse)webRes).StatusCode;   
                webRes.Close();
                userData.currentUrl = url;
                if(addToHistory){userData.addHistory(url,DateTime.Now);}  
            }else{                        
              statusCode = -1;
            } 

        }catch(UriFormatException e){
            statusCode = -1;
            buffer.Text = e.Message;
        }

        statusText.Markup = "<span weight='bold' size='larger'>Status: " + statusCode.ToString() + "</span>";
    }
}
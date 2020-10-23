using Gtk;
using System.IO;
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

    //custom widgets
    private MainView mainView;
    private HomeView homeView;
    private FavoritesView favoritesView;
    private HistoryView historyView;

    //file data vars
    private UserData userData;
    private Stream fileStream;
    private BinaryFormatter formatter;


    public WebBrowser(){
        loadUserData();
        Application.Init();
        initGui();
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
        mainView = new MainView(userData);
        homeView = new HomeView(userData);
        favoritesView = new FavoritesView(userData);
        historyView = new HistoryView(userData);


        //add event handlers
        win.DeleteEvent +=(obj,args) => closeAndSave();
        home.Activated += (obj,args) => setState(States.Home);
        main.Activated += (obj,args) => setState(States.Main);
        favorites.Activated += (obj,args) => setState(States.Favorites);
        history.Activated += (obj,args) => setState(States.History);


        //set up menu
        viewMenu.Submenu = menu;
        menu.Add(home);
        menu.Add(favorites);
        menu.Add(history);
        menuBar.Append(viewMenu);

        
        //set up the default layout
        vBox.PackStart(menuBar,false,false,0);
        vBox.PackStart(mainView,true,true,0);
        

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

    
    private void setState(States newState){
        //what to do to get ready for state change
        switch(currentState){
            case States.Main:
                vBox.Remove(mainView);
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
                vBox.Add(mainView);
                menu.Remove(main);
                mainView.setButtonStates();
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
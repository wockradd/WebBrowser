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
    //deafult widgets
    private Window win;
    private VBox vBox; 
    private MenuBar menuBar;
    private Menu menu;
    private MenuItem viewMenu,history,favorites,home;

    //custom widgets
    private MainView mainView;

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
        mainView = new MainView(userData);


        //add event handlers
        win.DeleteEvent +=(obj,args) => closeAndSave();
        

        //set up menu
        viewMenu.Submenu = menu;
        menu.Append(history);
        menu.Append(favorites);
        menu.Append(home);
        menuBar.Append(viewMenu);

        
        //set up the default layout
        vBox.PackStart(menuBar,false,false,0);
        vBox.PackStart(mainView,true,true,0);
        

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


    public void closeAndSave(){
        userData.setUpForSaving();
        fileStream = File.Open("data", FileMode.Create);
        formatter = new BinaryFormatter();
        formatter.Serialize(fileStream,userData);
        fileStream.Close();
        Application.Quit();
    }
}
using Gtk;
using System.Collections.Generic;

public class HistoryView:ScrolledWindow{
    private UserData userData;
    private List<HistoryItem> items;
    private Button delete;
    private VBox vBox;

    public delegate void Func1();
    public delegate void Func2(WebBrowser.States s);
    public delegate void Func3(string s, bool b);

    public HistoryView(UserData data, Func1 setButtons, Func2 changeView, Func3 makeRequest){
        //init user data
        userData = data;

        delete = new Button("Delete history");
        vBox = new VBox(false, 20);

        items = new List<HistoryItem>();
        foreach(UserData.History h in userData.history){
            items.Add(new HistoryItem(h.url, h.time.ToString(),h.title));
        }
        for(int i=items.Count-1 ; i>=0 ; i--){
            vBox.PackStart(items[i],false,false,0);
            items[i].gotoUrl.Clicked += (obj,args) => gotoHistory(changeView, makeRequest,((HistoryItem)((Button)obj).Parent.Parent).url);
        }
        

        //add event handlers
        delete.Clicked += (obj,args) => deleteHistory(setButtons);

        //finish layout
        vBox.PackStart(delete,false,false,0);
        this.Add(vBox);
    }


    

    public void deleteHistory(Func1 setButtons){
        userData.deleteHistory();
        for(int i=items.Count-1 ; i>=0 ; i--){
            vBox.Remove(items[i]);
        }
        setButtons();
    }

    public void gotoHistory(Func2 changeView, Func3 makeRequest, string url){
        changeView(WebBrowser.States.Main);
        makeRequest(url,true);
    }
}
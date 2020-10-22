using Gtk;
using System.Collections.Generic;

public class HistoryView:VBox{
    private UserData userData;
    private TextView view;
    private TextBuffer buffer;
    private Button delete;
    private ScrolledWindow scroll;

    public HistoryView(UserData data){
        userData = data;

        view = new TextView();
        buffer = view.Buffer;
        scroll = new ScrolledWindow();
        delete = new Button("Delete history");

        scroll.Add(view);

        delete.Clicked += (obj,args) => deleteHistory();

        this.PackStart(scroll,true,true,0);
        this.PackStart(delete,false,false,0);
    }

    public void populate(){
        string s = "";
        if(userData.history.Count == 0){
            buffer.Text = "No history";
        }else{
            foreach(UserData.History h in userData.history){
                s += "Url: " + h.url + "\nTime: " + h.time + "\n\n";
            }
            buffer.Text = s;
        }
    }


    public void deleteHistory(){
        userData.deleteHistory();
        populate();
    }
}
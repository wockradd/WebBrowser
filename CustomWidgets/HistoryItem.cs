using Gtk;

public class HistoryItem:Alignment{
    //historyview needs to access these vars
    public string url {get;set;}
    public Button gotoUrl {get;set;}

    private Label urlLabel,timeLabel,titleLabel;
    private VBox vBox;

    public HistoryItem(string url, string time, string title)
    :base(0.5f,0f,0f,0f){
        this.url = url;
        urlLabel = new Label("Url: " + url);
        timeLabel = new Label("Time: " + time);
        titleLabel = new Label("Title: " + title);
        gotoUrl = new Button("Go to");
        vBox = new VBox();

        vBox.PackStart(urlLabel,false,false,0);
        vBox.PackStart(titleLabel,false,false,0);
        vBox.PackStart(timeLabel,false,false,0);
        vBox.PackStart(gotoUrl,false,false,0);

        this.Add(vBox);
    }
}
//mcs -pkg:gtk-sharp-3.0 WebBrowser.cs Runner.cs UserData.cs -out:a.exe
//mono a.exe to run

/*
    TODO
    allow setting homepage
    allow deleting homepage
    allow changing homepage

    allow adding to favorites
    allow viewing favorites
    allow editing favorites

    allow viewing history
    allow deleting history
    allow moving back and forwards through history

*/

class Runner{
    public static void Main(string []args){
        WebBrowser wb = new WebBrowser();
    }
}
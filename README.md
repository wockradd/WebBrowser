# WebBrowser
A web browser written in c# for f21sc
## Compiling and running
mcs Runner.cs WebBrowser.cs UserData.cs -pkg:gtk-sharp-3.0 -out:a.exe

mono a.exe

### General
- [x] Timeout if request is taking too long
- [x] currentUrl should definetly be in webbrowser, not userdata
- [x] just need to do a lot of refactoring, only add more features once its nice and clean
- [x] check if messing with menus while getting requests breaks everything
- [x] better set state alg
- [x] look at changing requeter thing back to what it was like on the 20th
- [ ] resizing window not allowed currently, fix that 
- [x] history views a buggy mess
- [x] favorite button always disabled now?
- [x] display title of web page on main window
- [x] stop requests going through with empty urls
- [ ] check public/private status of favoriteitem and favoriteview vars
- [ ] status bar when in favorite view to let the user know the changes have been saved
- [ ] need to disable forward, back buttons after history is deleted

### View
- [x] history view
- [x] favorite view
- [x] homepage view
- [x] main view

### Homepage
- [x] Allow setting homepage
- [x] Allow changing homepage
- [x] Allow going to homepage

### Favorites
- [x] Allow adding to favorites
- [x] Allow removing to favorites
- [ ] Allow clicking on favorite to go to it
- [x] Allow viewing favorites
- [x] Allow editing favorites

### History
- [x] Allow viewing history
- [x] Allow deleting history
- [x] Allow moving back and forwards through history
- [ ] Allow clicking on history item to go to it

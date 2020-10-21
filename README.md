# WebBrowser
A web browser written in c# for f21sc
## Compiling and running
mcs Runner.cs WebBrowser.cs UserData.cs -pkg:gtk-sharp-3.0 -out:a.exe

mono a.exe

### General
- [x] Timeout if request is taking too long
- [x] currentUrl should definetly be in webbrowser, not userdata
- [ ] just need to do a lot of refactoring, only add more features once its nice and clean
- [ ] check if messing with menus while getting requests breaks everything

### View
- [ ] history view
- [ ] favorite view
- [ ] homepage view

### Homepage
- [ ] Allow setting homepage
- [ ] Allow changing homepage
- [x] Allow going to homepage

### Favorites
- [x] Allow adding to favorites
- [x] Allow removing to favorites
- [ ] Allow viewing favorites
- [ ] Allow editing favorites

### History
- [ ] Allow viewing history
- [ ] Allow deleting history
- [x] Allow moving back and forwards through history

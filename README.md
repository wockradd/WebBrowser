# WebBrowser
A web browser written in c# for f21sc
## Compiling and running
mcs Runner.cs WebBrowser.cs UserData.cs -pkg:gtk-sharp-3.0 -out:a.exe

mono a.exe

### General
- [] Timeout if request is taking too long

### View
- [] history view
- [] favorite view
- [] homepage view

### Homepage
- [ ] Allow setting homepage
- [ ] Allow deleting homepage
- [ ] Allow changing homepage

### Favorites
- [ ] Allow adding to favorites
- [ ] Allow viewing favorites
- [ ] Allow editing favorites

### History
- [ ] Allow viewing history
- [ ] Allow deleting history
- [x] Allow moving back and forwards through history

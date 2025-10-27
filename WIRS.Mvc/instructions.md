<!-- Purpose: so this is my project where I try to migrate webform to web razor with Telerik Controls. -->
<!-- You can see my old code in WIRS.Mvc/old-webforms/... -->
<!-- Note: You can see I have DataAccess which call to store procedure. It old logics so we not recommend to change my DataAccess.
You can see my old webform use BC (business component layer like usersBC it's is usersDataAcess now) which only call to its data access layer. I already move all of it to DataAccess so you can use it the same or can be a little difference for input and output but the main logics will be same. (We do not change DataAccess for any reason)

Note: since I cannot share the app settings. so all of this call method we need to implement the mock data logics when services to dataaccess. Include Appsetting Options...
We can make some thing we can reuse in unit test too. please create mock test data if needed from data access

Improvements:
- So move the own javascript for MVVM into separated js files.
- I don't like the current theme of Login page and Dashboard as well so make it better with bootsrap and telerik controls
- We will have menu to display and it will base on role in system. You can check my Data access for looking it (please create mock test data if needed from data access)

Make everything SOLD...
Note: Don't include any comment in my code -->
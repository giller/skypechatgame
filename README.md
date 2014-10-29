#Skype Chat Game
##Background
This is a project I created in about 7-10 days of free time before college started. The game is essentially just an endless quiz. A player will have to guess which member of the chat said a particular randomly pulled message from the chat. The person who holds the high score will be allowed to pick a youtube video that will loop for all people except for him who are currently playing the game.

I hosted it using AWS but won't post a link as you need an account to log in.

##How it works
This web application is written in C# using ASP.NET MVC. The database I used was an MSSQL database. SignalR is used to handle real-time communication between the client and the server. WebAPI is used to retrieve the video to play from the high score holder. The YouTube javascript API is used to control the video (volume and loading). I wrote some javascript for the client to interact with the SignalR code on the server, and also implemented a timer. 

##Issues
There are some issues with this project, my goal was to just get it working as quickly as possible. As a result, the database interaction is pretty horrible and wasteful, so much so that it encouraged me to read Code Complete. The other issue is the javascript. The client side javascript code is a mess and the timer is attached onto the window object. Also as I was working on this project alone, there is a little amount of comments.

Also for this github version I have removed all of the SkypeIDs, SQL commands and database connection info.
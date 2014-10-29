using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Diagnostics;
using SkypeChatGame.Models;
using WebMatrix.WebData;

namespace SkypeChatGame.Hubs
{
    //This class is a SingalR hub that is responsible for real time interaction with the game
    public class ChatHub : Hub
    {
        GameModel game;

        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            //Clients.All.addNewMessageToPage(name, message);
            Clients.Caller.addNewMessageToPage(name, message);
        }

        //Method called upon the beginning of a new game
        public void StartGame()
        {
            game = new GameModel();
            game.Score = 0;

            using (var db = new UsersContext())
            {
                var gamer = db.UserProfiles.Find(WebSecurity.CurrentUserId);
                game.Player = gamer;
                game.StartTime = DateTime.Now;
                gamer.Games.Add(game);
                db.SaveChanges();
                Debug.WriteLine("Added a new game for " + game.Player.UserName);
            }

            GetQuestion();
        }

        //Method called when a game ends
        public void CompletedGame()
        {
            using (var db = new UsersContext())
            {
                var gamer = db.UserProfiles.Find(WebSecurity.CurrentUserId);
                var games = gamer.Games;

                var RecentGame = games.Last();
                RecentGame.CompletionTime = DateTime.Now;

                if (RecentGame.Score > gamer.HighScore) gamer.HighScore = RecentGame.Score;

                db.SaveChanges();

                Debug.WriteLine("returning name of " + gamer.UserName);
                Clients.Caller.gameOver(RecentGame.Score);
            }
        }

        //Method called after the end of one question
        public void IncreaseScore()
        {
            using (var db = new UsersContext())
            {
                var game = db.UserProfiles.Find(WebSecurity.CurrentUserId).Games.Last();
                game.Score += 1;
                db.SaveChanges();
            }
        }

        public void GetQuestion()
        {
            var message = Skype.GetMessage(); 
            var People = Skype.AllowedPeople.ToList();
            People.Remove(message.Author);

            var Rand = new Random();

            var Num1 = Rand.Next(1, People.Count);
            var Name1 = People[Num1];
            People.Remove(Name1);

            var Num2 = Rand.Next(1, People.Count);
            var Name2 = People[Num2];

            var NameList = new List<string>();
            NameList.Add(message.Author);
            NameList.Add(Name1);
            NameList.Add(Name2);
            Shuffle(NameList);

            message.FakeAuthor1 = Name1;
            message.FakeAuthor2 = Name2;
            using (var db = new UsersContext())
            {
                var user = db.UserProfiles.Find(WebSecurity.CurrentUserId);
                user.Games.Last().Messages.Add(message);
                db.SaveChanges();
            }
            Clients.Caller.addQuestion(message.Contents, message.Timestamp, NameList[0], NameList[1], NameList[2]);
        }

        public void SubmitAnswer(string answer)
        {
            UserProfile UserProf;
            using (var db = new UsersContext())
            {
                UserProf = db.UserProfiles.Find(WebSecurity.CurrentUserId);
                if (UserProf.Games.Last().Messages.Last().Author == answer)
                {
                    UserProf.Games.Last().Score += 1;
                    db.SaveChanges();
                    Clients.Caller.loggy("correct");
                    GetQuestion();
                }
                else
                {
                    Clients.Caller.loggy("incorrect");
                    CompletedGame();
                }
            }
        }

        public void Shuffle(List<string> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}
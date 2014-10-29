//This spaghetti code is very messy, hackish and hastily put together

function Question() {
    this.go = true;
    this.countdown = false;
}

//Fix this to not use the global scope, http://ryanmorr.com/understanding-scope-and-context-in-javascript/ 
Question.prototype.stopTimer = function () {
    clearTimeout(window.timeout);
};

Question.prototype.startTimer = function (chat) {
    //this.Timer(5);
    timer(8);
    function timer(time) {
        if (time > 0) {
            this.timeout = setTimeout(function () {
                time -= 1;
                $('#time-left').text(time + " seconds left!");
                timer(time);
            }, 1000);
        } else if (false) {

        } else {
            chat.server.completedGame();
        }
    }
};

Question.prototype.Timer = function (time) {
    //console.log(this.countdown);
    console.log(time);

    if (time > 0) {
        this.time = time;
        var ayy = function () {
            console.log(this);
            this.Timer2();
        }
        this.timeout = setTimeout(ayy, 1000);
    }
    /* if (time > 0) {
         setTimeout(function () {
             time -= 1;
             console.log(this.time);
             $('#round').text("Chat " + time + " seconds left!");
             this.Timer(time)
         }, 1000);
     } else if (false) {

     } else {
         chat.server.completedGame(username);
     }*/
};

Question.prototype.Timer2 = function () {
    this.time -= 1;
    console.log(this.time);
    $('#round').text(time + " seconds left!");
    this.Timer(time);
};
// 2. This code loads the IFrame Player API code asynchronously.


// 3. This function creates an <iframe> (and YouTube player)
//    after the API code downloads.
var id;
var player;
function onYouTubeIframeAPIReady() {
    if (id != null) {
        player = new YT.Player('player', {
            height: '0',
            width: '0',
            videoId: id,
            events: {
                'onReady': onPlayerReady,
                'onStateChange': onPlayerStateChange
            }
        });
    }
    else {
        setTimeout(function () {
            onYouTubeIframeAPIReady();
        }, 1000);
    }
}

// 4. The API will call this function when the video player is ready.
function onPlayerReady(event) {
    if (id != null) {
        player.setVolume(10);
        event.target.playVideo();
    } else {
        setTimeout(function () {
            onPlayerReady(event);
        }, 2000);
    }
}

// 5. The API calls this function when the player's state changes.
//    The function indicates that when playing a video (state=1),
//    the player should play for six seconds and then stop.
var done = false;
function onPlayerStateChange(event) {
    if (event.data == YT.PlayerState.ENDED && !done) {
        player.loadVideoById(id);
    }
}

$("#slider-vertical").slider({
    orientation: "vertical",
    range: "min",
    min: 10,
    max: 100,
    value: 10,
    slide: function (event, ui) {
        $("#amount").val(ui.value);
        player.setVolume(ui.value);
    }
});


var muted = false;
$("#mute-button").click(function () {
    console.log("clicked" + muted);
    if (muted) {
        player.unMute();
        muted = false;
    } else {
        player.mute();
        muted = true;
    }
});
//player.setVolume($("#slider-vertical").val("value"));


$(function () {
    
    var name = "";
    (function () {
        var xhr = createXHR();

        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                name = xhr.responseText;
                name = name.replace(/["']/g, "");
                console.log(name);
                id = name;
                currentVideo(name);
                doWork(name);
            }
        }

        xhr.open("GET", "../../api/userinfo", true);
        xhr.send(null);
    })();

    function currentVideo(id) {
        /*var xhr = createXHR();

        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                console.log(JSON.parse(xhr.responseText));
                $('#video-meta h6').text(JSON.parse(xhr.responseText));
            }
        }

        xhr.responseType = 'json';
        xhr.open("GET", "https://gdata.youtube.com/feeds/api/videos/" + id + "?v=2", true);
        xhr.send(null);*/
        if (id != null && id != "null") {
            console.log(id);
            $('#video-meta h6').html("<a href=http://youtube.com/watch?v=" + id + ">Current meme: " + id + "</a>");
        }
    }

    function clearList() {
        $('#question-wrapper h2, #question-wrapper h4').remove();
        $('#discussion').text('');
    }

    function doWork(username) {
        // Reference the auto-generated proxy for the hub.  
        var chat = $.connection.chatHub;

        chat.client.loggy = function (logger) {
            console.log(logger);
            $('#time-left').text("Correct!");
        };

        chat.client.addQuestion = function (question, timestamp, name1, name2, name3) {
            setTimeout(function () {
                console.log("add question called");
                clearList();
                $('#question-wrapper').prepend('<h2 id="question">' + question + '</h2>');
                var date = new Date(timestamp * 1000);
                $('#question').append('<h4>' + date + '</h4>');
                $('#discussion').append('<input type="radio" id="radiogroup" name="radioGroup" value ="' + name1 + '"/>' + name1 + '<br />');
                $('#discussion').append('<input type="radio" id="radiogroup" name="radioGroup" value ="' + name2 + '"/>' + name2 + '<br />');
                $('#discussion').append('<input type="radio" id="radiogroup" name="radioGroup" value ="' + name3 + '"/>' + name3 + '<br />');
                $('#time-left').text("8 seconds left!");
                chat.question.startTimer(chat);
            }, 1000);
        };

        chat.client.gameOver = function (score) {
            clearList();
            $('#discussion').children().remove();
            $('#discussion').text('');
            $('#stoptimerbutton').addClass('hidden');
            $('#time-left').text("Game Over! You scored " + score);
            $('#sendmessage').removeClass('hidden');
            $('#sendmessage').val("Play Again!");
        }

        //THIS IS THE SOURCE OF THE PROBLEM
        //Double clicking potentiall causing problems
        // Start the connection.
        $.connection.hub.start().done(function () {
            $('#sendmessage').click(function () {
                $(this).addClass('hidden');
                $('#stoptimerbutton').removeClass('hidden');
                $('#round').addClass('hidden');

                chat.question = new Question();
                var question = chat.question;
                chat.server.startGame();

                //WHY IS THIS IN HERE
               
            });
            $('#stoptimerbutton').click(function () {
                chat.question.stopTimer();
                var Radio = $('input[name=radioGroup]:checked').val();
                if(Radio != null)
                    chat.server.submitAnswer(Radio);
                clearList();
            });

        });
    }
});
// This optional function html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}


function createXHR() {
    if (typeof XMLHttpRequest != 'undefined') {
        return new XMLHttpRequest();
    } else {
        try {
            return new ActiveXObject('Msxml2.XMLHTTP');
        } catch (e1) {
            try {
                return new ActiveXObject('Microsoft.XMLHTTP');
            } catch (e2) {
            }
        }
    }
    return null;
}
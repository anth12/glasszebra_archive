// Define the `phonecatApp` module
var app = angular.module('GlassZebraApp', []);
var baseUrl = '';
var skipSetup = false;

app.controller('MainController', function MainController($scope, $http) {

    $http.get('api/ping').then(r => {
        baseUrl = r.data + '/glasszebra';
    });

    $scope.NewGame = function () {
        $scope.started = true;
        $scope.watching = true;
    }

    $scope.WatchGame = function () {
        skipSetup = true;
        $scope.started = true;
        $scope.watching = true;
    }
    $scope.JoinGame = function () {
        $scope.started = true;
        $scope.playing = true;
    }

});


/* ===============================================================
            Play
   =============================================================== */
app.controller('PlayController', function PlayController($scope, $http) {

    updateGame();
    var ws = new WebSocket(`ws://${baseUrl}/ws`);

    var board = new DrawingBoard.Board('canvasBoard', {
        controls: [
            'Color',
            { Size: { type: 'dropdown' } },
            { DrawingMode: { filler: false } },
            'Navigation'
        ],
        size: 5,
        webStorage: false
    });
    
    var lastImg;
    function boardUpdated() {
        var img = board.getImg();
        if (img == lastImg)
            return;

        lastImg = img;
        ws.send(img);
    }

    board.ev.bind('board:reset', boardUpdated);
    board.ev.bind('board:stopDrawing', boardUpdated);

    $http.get('api/quiz/players').then(r => {
        $scope.players = r.data;
    })

    $scope.roundWon = function (player) {
        $scope.roundEnding = false;
        $http.post('api/quiz/score?player=' + player.name)
            .then(r => {
                $('.drawing-board-control-navigation-reset').click();
                $scope.nextQuestion(true);
        });
    }
    $scope.cancelEnd = function (player) {
        $scope.roundEnding = false;
    }

    $scope.nextQuestion = function (preventShow) {
        $http.get('api/quiz/question').then(r => {

            $scope.game = r.data;

            if (!preventShow) {
                $scope.questionVisible = true;
            }
        });
    }

    $scope.hideQuestion = function () {
        $scope.questionVisible = false;
    }
    $scope.showQuestion = function () {
        $scope.questionVisible = true;
    }

    function updateGame() {
        $http.get('api/quiz/game').then(r => {

            $scope.game = r.data;
        })
    }

});

/* ===============================================================
            Watch
   =============================================================== */
app.controller('WatchController', function PlayController($scope, $http) {

    $scope.baseUrl = baseUrl;
    $scope.categories = ['Easy', 'Moderate', 'Hard', 'VeryHard', 'Animal', 'Characters', 'Movies', 'TvShows', 'Books'];
    $scope.toggleSelection = function toggleSelection(item) {
        var idx = $scope.game.dificulties.indexOf(item);

        // Is currently selected
        if (idx > -1) {
            $scope.game.dificulties.splice(idx, 1);
        }

        // Is newly selected
        else {
            $scope.game.dificulties.push(item);
        }
        $http.post('api/quiz/dificulties', $scope.game.dificulties);
    };

    $scope.players = [{ name: '' }];

    $scope.addPlayer = function () {
        $scope.players.push({ name: '' });
    }

    $scope.startGame = function () {
        $http.post('api/quiz/players', $scope.players.map(p => p.name))
            .then(setup);
    }
    
    if (skipSetup) {
        setup();
    }

    var ws, canvas = null;

    function setup() {
        
        ws = new WebSocket(`ws://${baseUrl}/ws/listen`);
        $scope.setup = true;
        canvas = document.getElementById('watchCanvas');

        ws.onmessage = function (message) {

            if (message.data == 'update') {
                updateGame();
            } else {
                $scope.imageSrc = message.data;
                $scope.$apply();
            }
        }

        updateGame();
    }

    function updateGame() {
        $http.get('api/quiz/game').then(r => {

            $scope.game = r.data;
        })
    }

});
var Dashboard = function (ele) {
    var self = this;
    self.$elt = $(ele);
    self.RegisterObject = function () {
        if (self.$elt.data('object') == null)
            self.$elt.data('object', self);
    };
    self.RegisterObject();
    self.$board = self.$elt.find('.js-board');
    self.$upcount = self.$elt.find('.js-up-count');
    self.$downcount = self.$elt.find('.js-down-count');
    self.$winnerarea = self.$elt.find('.js-winner-area');
    self.$winner = self.$winnerarea.find('.js-winner');

    self.Init = function () {
        self.$elt.find('.js-new-game').click(self.NewGameClick);
    };

    self.NewGameClick = function (e) {
        $.ajax({
            url: '/Board/StartGame',
            data: {
                team: true
            },
            type: 'POST',
            success: function (data) { self.DisplayNewMove(data, false); }
        });
    };

    self.MakeComputerMove = function () {
        setTimeout(function () {
            self.$board.prepend('<div class="ui-spinner"></div>');
            $.ajax({
                url: '/Board/GetComputerMove',
                type: 'POST',
                success: function (data) {
                    self.DisplayNewMove(data, false);
                }
            });
        },
        2000);
    };

    self.DisplayNewMove = function (data, makeComputerMove) {
        var builder = new BoardBuilder();
        self.$upcount.text(data.board.UpPieces);
        self.$downcount.text(data.board.DownPieces);
        self.$board.html(builder.Build(data.board));
        if (data.board.Winner == true || data.board.Winner == false) {
            self.$winnerarea.show();
            var winnerText = data.board.Winner ? "Black" : "Red";
            winnerText += data.board.WinnerByElimination == true || data.board.WinnerByElimination == false ? " (by elimination) " : " (by gridlock)";
            self.$winner.text(winnerText);
        }
        else {
            self.$board.find('.js-playable').each(function (i) {
                (new Square($(this))).Init();
            });
            if (makeComputerMove)
                self.MakeComputerMove();
            else
                Move.HumanTurn = true;
        }
    };
};
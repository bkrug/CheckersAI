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
    self.moveCalcDelay = 1000;
    self.moveDisplayDelay = 1000;

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
            self.$board.prepend('<div class="js-loading ui-spinner"></div>');
            $.ajax({
                url: '/Board/GetComputerMove',
                type: 'POST',
                success: function (data) {
                    self.DisplayComputerMove(data);
                }
            });
        },
        self.moveCalcDelay);
    };

    self.DisplayComputerMove = function (data) {
        self.$board.find('.js-loading').remove();
        self.AnimateMove(data);
        setTimeout(function () {
            self.DisplayNewMove(data, false);
        },
        self.moveDisplayDelay);
    }

    self.AnimateMove = function (data) {
        var pos = {};
        pos.row = data.move.StartRow;
        pos.col = data.move.StartColumn;
        self.$board.find('.r' + pos.row + 'c' + pos.col).addClass('ui-high');
        for (var i = 0; i < data.move.Move.Steps.length; ++i) {
            pos = self.HighlightNextSquare(data.move.Move.Steps[i], pos);
            if (data.move.Move.Steps[i].Jump)
                pos = self.HighlightNextSquare(data.move.Move.Steps[i], pos);
        }
    };

    self.HighlightNextSquare = function (step, pos) {
        pos.row += Direction.DirectionY(step.Direction);
        pos.col += Direction.DirectionX(step.Direction);
        self.$board.find('.r' + pos.row + 'c' + pos.col).addClass('ui-high');
        return pos;
    }

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
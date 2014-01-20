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
            success: self.DisplayNewMove
        });
    };

    self.MakeComputerMove = function () {
    };

    self.DisplayNewMove = function (data, makeComputerMove) {
        var builder = new BoardBuilder();
        self.$upcount.text(data.board.UpPieces);
        self.$downcount.text(data.board.DownPieces);
        self.$board.html(builder.Build(data.board));
    };
};
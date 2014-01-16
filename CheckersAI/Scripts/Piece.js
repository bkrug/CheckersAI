var Piece = new function () {
    var self = this;

    self.Build = new function (callback) {
        $.ajax({
            url: '/Board/GetPieceEnum',
            success: new function (data) {
                SetValues(data, callback);
            }
        });
    };

    var SetValues = new function (data, callback) {
        var pieceNames = ["UP_TEAM", "UP_TEAM_KING", "DOWN_TEAM", "DOWN_TEAM_KING"];
        for (var i = 0; i < pieceNames.length; ++i) {
            if (data[pieceNames[i]] == null || data[pieceNames[i]] == undefined)
                alert('There is no value for piece type ' + pieceNames[i]);
            self[pieceNames[i]] = data[pieceNames[i]];
        }
        if (callback)
            callback();
    };

    self.UP_TEAM = null;
    self.UP_TEAM_KING = null;
    self.DOWN_TEAM = null;
    self.DOWN_TEAM_KING = null;
};
var Piece = function () {
    var self = this;

    self.Build = function (data, callback) {
        var pieceNames = ["UP_TEAM", "UP_TEAM_KING", "DOWN_TEAM", "DOWN_TEAM_KING"];
        for (var i = 0; i < pieceNames.length; ++i) {
            if (data[pieceNames[i]] == null || data[pieceNames[i]] == undefined)
                alert('There is no value for piece type ' + pieceNames[i]);
            Piece[pieceNames[i]] = data[pieceNames[i]];
        }
        if (callback)
            callback();
    };

    Piece.UP_TEAM = null;
    Piece.UP_TEAM_KING = null;
    Piece.DOWN_TEAM = null;
    Piece.DOWN_TEAM_KING = null;
};
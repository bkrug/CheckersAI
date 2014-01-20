var BoardBuilder = function () {
    var self = this;

    self.Build = function(board) {
        var results = '';
        for (var r = 0; r < 8; ++r) {
            results += '<div class="js-row ui-row">';
            for (var c = 0; c < 8; ++c) {
                var pieceClass = '';
                var kingData = "false";
                var downData = "null";
                switch (board.PieceLayout[r][c]) {
                    case Piece.UP_TEAM:
                        pieceClass = 'ui-up';
                        downData = "false";
                        break;
                    case Piece.UP_TEAM_KING:
                        pieceClass = 'ui-up-king';
                        kingData = "true";
                        downData = "false";
                        break;
                    case Piece.DOWN_TEAM:
                        pieceClass = 'ui-down';
                        downData = "true";
                        break;
                    case Piece.DOWN_TEAM_KING:
                        pieceClass = 'ui-down-king';
                        kingData = "true";
                        downData = "true";
                        break;
                }
                var playableClass = (r % 2 != c % 2) ? 'js-playable ui-playable' : 'ui-nonplayable';
                var positionClass = 'r' + r + 'c' + c;
                results += '<div class="js-square ui-square ' + positionClass + ' ' + playableClass + ' ' + pieceClass
                    + '" data-row="' + r + '" data-column="' + c + '" data-down="' + downData + '" data-king="' + kingData + '"></div>';
            }
            results += '</div>';
        }
        return results;
    }
};